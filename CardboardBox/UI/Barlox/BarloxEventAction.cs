// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// libWyvernzora.BarlogX/BarloxEventAction.cs
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
    ///     Action performed when an BarloxEvent occurs.
    /// </summary>
    public class BarloxEventAction
    {
        /// <summary>
        ///     Gets or sets an array of conditions.
        ///     The action is performed only when the current animation state
        ///     exists in the condition array.
        /// </summary>
        public Int32[] Condition { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether state changes
        ///     should be queued or switched.
        ///     If true, state changes will be enqueued and applied after
        ///     the current state finished. If false, state changes will be
        ///     applied immideately.
        /// </summary>
        public BarloxEventActionType Type { get; set; }

        /// <summary>
        ///     Gets or sets an array of parameters.
        ///     If more than one parameter is specified, the whole array
        ///     will be enqueued. Only the first element of the array is applied
        ///     immideately and all following elements are ignored.
        /// </summary>
        public Int32[] Parameters { get; set; }
    }
}