// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// Barlox.WP7/BarloxAnimation.cs
// --------------------------------------------------------------------------------
// Copyright (c) 2013, Jieni Luchijinzhou a.k.a Aragorn Wyvernzora
// 
// This file is a part of Barlog X Game Engine.
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy 
// of this software and associated documentation files (the "Software"), to deal 
// in the Software without restriction, including without limitation the rights 
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies 
// of the Software, and to permit persons to whom the Software is furnished to do 
// so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all 
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, 
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A 
// PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT 
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION 
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// --------------------------------------------------------------------------------
//     Barlog X Game Engine is an XNA/WPF reimplementation of the Barlog Engine
//     that I used for developing the Centipede: Deep Space Remix in CS180 course.
//     This file is in fact a modified version to suit Windows Phone 7, where it
//     is extremely expensive to dynamically crop bitmaps. Since we are expecting
//     to load precompiled BAX solutions, following advanced features have been removed:
//     - BAX script parser/compiler
//     - BAX solution loader
//     - BAX script debugger
//     - Advanced State Queue
//     - Dynamic transition weight
//     - Dynamic keyframe generation
//     - Keyframe cross-reference
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using libWyvernzora.IO;

namespace libWyvernzora.BarlogX.Animation
{
    /// <summary>
    ///     BarloxAnimation Finite State Machine.
    /// </summary>
    public class BarloxAnimation : IDisposable
    {
        private static readonly Random random = new Random();

        private readonly DispatcherTimer clock;

        private readonly BarloxFrame[] frames;

        private readonly StreamEx raw;
        private readonly BarloxState[] states;
        private Int32 cstate;
        private Int32 cframe;
        private Int32 fps;


        internal BarloxAnimation()
        {
            Height = 0;
            Width = 0;
            fps = 1;
            clock = new DispatcherTimer();
            clock.Tick += (@s, e) => NextFrame();
        }

        public BarloxAnimation(Stream data) : this()
        {
            raw = new StreamEx(data);

            // Verify Signature
            UInt64 magicNumber = raw.ReadUInt64();
            if (magicNumber != 24848203858985282)
                throw new Exception("BarloxAnimation.ctor() : Invalid magic number");

            // Load Metadata
            raw.Position = 0x20;
            Int32 imgDataAddress = raw.ReadInt32();
            Width = raw.ReadInt32();
            Height = raw.ReadInt32();
            FPS = raw.ReadInt32();

            // Load Frames
            Int32 framec = raw.ReadInt32();
            Int64 frameIndexAddress = raw.Position;

            frames = new BarloxFrame[framec];
            for (int i = 0; i < framec; i++)
            {
                raw.Position = frameIndexAddress + 0x10 * i; // Advance to specific frame index
                Int32 typeId = raw.ReadInt32();
                switch (typeId)
                {
                    case 0xD0:
                        {
                            Int32 address = raw.ReadInt32() + imgDataAddress;
                            Int32 length = raw.ReadInt32();
                            raw.ReadInt32(); // Alignment

                            BitmapImage image = new BitmapImage();
                            image.SetSource(new PartialStreamEx(raw, address, length));

                            frames[i] = new BarloxFrame {Source = image};
                        }
                        break;
                    case 0xD1:
                        throw new NotSupportedException(
                            "BarloxAnimation.ctor() : Cross-frame references not supported in Windows Phone version of Barlox!");
                    default:
                        throw new Exception("BarloxAnimation.ctor() : Unknown frame type!");
                }
            }

            // Load States
            raw.Position = frameIndexAddress + framec * 0x10;
            Int32 seqc = raw.ReadInt32();
            states = new BarloxState[seqc];

            for (int i = 0; i < seqc; i++)
            {
                List<Int32> sframes = new List<int>();
                List<BarloxState.Transition> strans = new List<BarloxState.Transition>();

                Int32 sframec = raw.ReadInt32();
                for (int s = 0; s < sframec; s++)
                    sframes.Add(raw.ReadInt32());

                Int32 stransc = raw.ReadInt32();
                for (int t = 0; t < stransc; t++)
                {
                    Int32 weight = raw.ReadInt32();
                    Int32 tgt = raw.ReadInt32();
                    strans.Add(new BarloxState.Transition {NextStateID = tgt, Weight = weight});
                }

                states[i] = new BarloxState(sframes.ToArray(), strans.ToArray());
            }
        }

        #region Threading & Events

        private EventHandler<FrameChangedEventArgs> frameChanged;

        public Boolean IsEnabled
        {
            get { return clock.IsEnabled; }
            set
            {
                if (value && FPS > 0)
                    clock.Start();
                else
                    clock.Stop();
            }
        }

        public event EventHandler<FrameChangedEventArgs> FrameChanged
        {
            add { frameChanged += value; }
            remove { frameChanged -= value; }
        }

        private void RaiseFrameChanged()
        {
            if (frameChanged != null)
                frameChanged(this, new FrameChangedEventArgs() { NewFrame = CurrentFrame});
        }

        public void Reset()
        {
            cstate = 0;
            cframe = 0;
            RaiseFrameChanged();
        }

        #endregion

        #region Metadata

        /// <summary>
        ///     Height of each and every frame in the animation.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        ///     Width of each end every frame in the animation.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        ///     Animation frames per second.
        /// </summary>
        public Int32 FPS
        {
            get { return fps; }
            set
            {
                fps = value;
                clock.Interval = TimeSpan.FromMilliseconds((Int32) (1000.0 / fps));
            }
        }

        #endregion

        #region States & Transitions

        /// <summary>
        ///     Gets the current animation frame.
        /// </summary>
        public BarloxFrame CurrentFrame
        {
            get { return frames[states[cstate].Frames[cframe]]; }
        }

        /// <summary>
        /// Gets the ID of the current animation state.
        /// Unvalidated, use with care!
        /// </summary>
        public int CurrentStateID
        {
            get { return cstate; }
            set
            {
                if (cstate != value && cstate >= 0 && cstate < states.Length)
                {
                    cstate = value;
                    cframe = 0;
                }
            }
        }

        /// <summary>
        ///     Advance animation to the next frame,
        ///     and to the next state if necessary.
        /// </summary>
        public void NextFrame()
        {
            BarloxState currentState = states[cstate];
            if (cframe + 1 < currentState.Frames.Length) cframe++;
            else
            {
                Int32 rand = random.Next(100);

                foreach (BarloxState.Transition t in currentState.Transitions)
                {
                    rand -= t.Weight;
                    if (rand <= 0)
                    {
                        cstate = t.NextStateID;
                        cframe = 0;
                        break;
                    }
                }
            }

            RaiseFrameChanged();
        }

        #endregion

        public void Dispose()
        {
            if (raw != null) raw.Close();
        }

        /// <summary>
        ///     Event args for FrameChanged event.
        /// </summary>
        public sealed class FrameChangedEventArgs : EventArgs
        {
            public BarloxFrame NewFrame { get; set; }
        }
    }
}