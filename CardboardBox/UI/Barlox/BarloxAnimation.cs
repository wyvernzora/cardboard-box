#define WP7

// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// libWyvernzora.BarlogX/BarloxAnimation.cs
// --------------------------------------------------------------------------------
// Copyright (c) 2013, Jieni Luchijinzhou a.k.a Aragorn Wyvernzora
// 
// This file is a part of libWyvernzora.BarlogX.
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
// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using libWyvernzora.IO;
using libWyvernzora.Utilities;

//Uncomment the following line if using this class in WP7

//#define WP7

namespace libWyvernzora.BarlogX.Animation
{
    /// <summary>
    ///     BarloxAnimation Finite State Machine.
    /// </summary>
    public class BarloxAnimation : IDisposable
    {
        protected static readonly Random random = new Random();

        protected readonly DispatcherTimer clock;

        protected readonly StreamEx raw;
        protected readonly Queue<Int32> stateQueue;
        protected Int32 cframe;
        protected Int32 cstate;
        protected Dictionary<String, BarloxEvent> events;
        protected Int32 fps;
        protected BarloxFrame[] frames;
        protected BarloxState[] states;


        internal BarloxAnimation()
        {
            Height = 0;
            Width = 0;
            fps = 1;
            clock = new DispatcherTimer();
            clock.Tick += (@s, e) => NextFrame();

            stateQueue = new Queue<int>();
            events = new Dictionary<string, BarloxEvent>(StringComparer.CurrentCultureIgnoreCase);
        }

        public BarloxAnimation(Stream data) : this()
        {
            raw = new StreamEx(data);

            // Verify Signature
            UInt64 magicNumber = raw.ReadUInt64();
            if (magicNumber == 24848203858985282)
                LoadVersion1(data);     // Version 1 BXA file
            else if (magicNumber == 3328571540640383561)
                LoadVersion2(data);     // Version 2 IBXA file
            else
                throw new Exception("BarloxAnimation.ctor() : Invalid magic number");
        }

        private void LoadVersion1(Stream data)
        {
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

#if WP7
                            image.SetSource(new PartialStreamEx(raw, address, length));
#else
                            using (new ActionLock(image.BeginInit, image.EndInit))
                            {
                                image.StreamSource = new PartialStreamEx(raw, address, length);
                            }
#endif

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

        private void LoadVersion2(Stream data)
        {
            raw.Position = 0x20;    // Image Data Start
            Int32 dataStart = raw.ReadVInt();

            // Isolate Index
            PartialStreamEx index = new PartialStreamEx(raw, 0, dataStart, FileAccess.Read) {Position = 0x20};
            index.ReadVInt();

            // Load Metadata
            Height = index.ReadVInt();
            Width = index.ReadVInt();
            FPS = index.ReadVInt();

            // Load Resources
            Int32 framec = index.ReadVInt();

            frames = new BarloxFrame[framec];
            for (int i = 0; i < framec; i++)
            {
                Byte type = (Byte) index.ReadByte();
                switch (type)
                {
                    case 0xD0:
                        {
                            Int32 address = index.ReadVInt();
                            Int32 length = index.ReadVInt();

                            BitmapImage image = new BitmapImage();

#if WP7
                            image.SetSource(new PartialStreamEx(raw, address + dataStart, length));
#else
                            using (new ActionLock(image.BeginInit, image.EndInit))
                            {
                                image.StreamSource = new PartialStreamEx(raw, address + dataStart, length);
                            }
#endif
                            frames[i] = new BarloxFrame { Type = BarloxFrameType.Image, Source = image };
                        }
                        break;
                    case 0xD1:
                        {
#if WP7
                            throw new NotSupportedException("BarloxAnimation.ctor() : Cross-frame references not supported in Windows Phone version of Barlox!");
#else
                            Int32 refid = index.ReadVInt();
                            Int32 x = index.ReadVInt();
                            Int32 y = index.ReadVInt();

                            frames[i] = new BarloxFrame
                                {
                                    Type = BarloxFrameType.Reference,
                                    ReferenceID = refid,
                                    X = x,
                                    Y = y
                                };
#endif
                        }
                        break;
                    default:
                        throw new Exception("BarloxAnimation.ctor() : Unknown frame type!");
                }
            }

            // Load States
            Int32 statec = index.ReadVInt();
            states = new BarloxState[statec];
            for (int i = 0; i < statec; i++)
            {
                List<Int32> sframes = new List<int>();
                List<BarloxState.Transition> strans = new List<BarloxState.Transition>();

                Int32 sframec = index.ReadVInt();
                for (int s = 0; s < sframec; s++)
                    sframes.Add(index.ReadVInt());

                Int32 stransc = index.ReadVInt();
                for (int t = 0; t < stransc; t++)
                {
                    Int32 weight = index.ReadVInt();
                    Int32 tgt = index.ReadVInt();
                    strans.Add(new BarloxState.Transition { NextStateID = tgt, Weight = weight });
                }

                states[i] = new BarloxState(sframes.ToArray(), strans.ToArray());
            }

            // Load Events
            Int32 eventc = index.ReadVInt();
            for (int x = 0; x < eventc; x++)
            {
                BarloxEvent ev = new BarloxEvent {Name = index.ReadString()};

                // Load Actions
                Int32 acc = index.ReadVInt();
                ev.Actions = new BarloxEventAction[acc];
                for (int i = 0; i < acc; i++)
                {
                    BarloxEventAction action = new BarloxEventAction();

                    Byte type = (Byte) index.ReadByte();
                    if (type == 0xA0) action.Type = BarloxEventActionType.Switch;
                    else if (type == 0xA1) action.Type = BarloxEventActionType.Queue;
                    else throw new Exception("Cannot recognize action type: " + index.Position);

                    // Load Conditions
                    Int32 cc = index.ReadVInt();
                    action.Condition = new int[cc];
                    for (int k = 0; k < cc; k++) action.Condition[k] = index.ReadVInt();

                    // Load Parameters
                    Int32 pc = index.ReadVInt();
                    action.Parameters = new int[pc];
                    for (int k = 0; k < pc; k++) action.Parameters[k] = index.ReadVInt();

                    ev.Actions[i] = action;
                }

                events.Add(ev.Name, ev);
            }

            // Create Crop Frames
#if !WP7
            foreach (var f in frames)
            {
                if (f.Type == BarloxFrameType.Reference)
                {
                    BarloxFrame refd = frames[f.ReferenceID];
                    f.Source = new CroppedBitmap(refd.Source, new Int32Rect(f.X, f.Y, Width, Height));
                }
            }
#endif
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
                frameChanged(this, new FrameChangedEventArgs {NewFrame = CurrentFrame});
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
        public int Height { get; protected set; }

        /// <summary>
        ///     Width of each end every frame in the animation.
        /// </summary>
        public int Width { get; protected set; }

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
        /// Gets all events defined for the animation.
        /// </summary>
        public IEnumerable<BarloxEvent> Events
        { get { return events.Values; } }

        /// <summary>
        ///     Gets the current animation frame.
        /// </summary>
        public BarloxFrame CurrentFrame
        {
            get { return frames[states[cstate].Frames[cframe]]; }
        }



        /// <summary>
        ///     Triggers an event and causes associated
        ///     BarloxEventAction to be applied.
        /// </summary>
        /// <param name="eventName">Name of the event, case insensitive.</param>
        public void TriggerEvent(String eventName)
        {
            if (!events.ContainsKey(eventName))
                throw new Exception("Cannot find the event: " + eventName);

            BarloxEvent e = events[eventName];
            BarloxEventAction action = e.Actions.FirstOrDefault(a => a.Condition.Contains(cstate));

            if (action == null) return;

            switch (action.Type)
            {
                case BarloxEventActionType.Switch:
                    cstate = action.Parameters[0];
                    cframe = 0;
                    break;
                case BarloxEventActionType.Queue:
                    foreach (int i in action.Parameters) stateQueue.Enqueue(i);
                    break;
            }
        }

        /// <summary>
        ///     Puts a state ID into the state queue so that when the
        ///     next transition is requested the animation will force-transition
        ///     to the enqueued state.
        /// </summary>
        /// <param name="id">ID of the state to enqueue.</param>
        public void EnqueueState(Int32 id)
        {
            if (id >= 0 && id < states.Length)
                stateQueue.Enqueue(id);
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
                if (stateQueue.Count == 0)
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
                else
                {
                    cstate = stateQueue.Dequeue();
                    cframe = 0;
                }
            }

            RaiseFrameChanged();
        }

        #endregion

        public void Dispose()
        {
            if (raw != null) raw.Close();
#if WP7
            foreach (var f in frames)
            {
                f.Source.SetSource(null);
            }
#endif
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