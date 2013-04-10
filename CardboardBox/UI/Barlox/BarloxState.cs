// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// libWyvernzora.BarlogX/BarloxSequence.cs
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

namespace libWyvernzora.BarlogX.Animation
{
    /// <summary>
    ///     A sequence of BarloxFrames.
    ///     Also a state in BarloxAnimation FSM.
    /// </summary>
    /// <remarks>
    ///     For a description of BarloxAnimation framework, see
    ///     http://www.wyvernzora.com/2012/05/simple-application-of-finite-state.html
    /// </remarks>
    public class BarloxState
    {

        /// <summary>
        ///     Constructor.
        ///     Initializes a new instance.
        /// </summary>
        internal BarloxState()
        {
            
        }


        /// <summary>
        ///     Constructor.
        ///     Initializes a new instance.
        /// </summary>
        /// <param name="frames">Frames in this BarloxState.</param>
        /// <param name="transitions">Transitions from this BarloxState.</param>
        public BarloxState(Int32[] frames, Transition[] transitions)
        {
            Frames = frames;
            Transitions = transitions;
        }

        /// <summary>
        ///     Frames in this BarloxState.
        /// </summary>
        public Int32[] Frames { get; internal set; }

        /// <summary>
        ///     Transitions from this BarloxState.
        /// </summary>
        public Transition[] Transitions { get; internal set; }

        /// <summary>
        ///     Transition from one state to another within BarloxAnimation FSM.
        /// </summary>
        public class Transition : IComparable<Transition>
        {
            /// <summary>
            ///     ID of the next state.
            /// </summary>
            public Int32 NextStateID { get; set; }

            /// <summary>
            ///     Weight of the transition.
            /// </summary>
            public Int32 Weight { get; set; }

            public int CompareTo(Transition other)
            {
                return Weight.CompareTo(other.Weight);
            }
        }
    }
}