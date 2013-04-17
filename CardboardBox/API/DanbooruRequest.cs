// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// CardboardBox/DanbooruRequest.cs
// --------------------------------------------------------------------------------
// Copyright (c) 2013, Jieni Luchijinzhou a.k.a Aragorn Wyvernzora
// 
// This file is a part of CardboardBox.
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
using System.Net;
using System.Runtime.Serialization.Json;
using libDanbooru2;

namespace CardboardBox.API
{
    /// <summary>
    ///     Danbooru image board credentials.
    /// </summary>
    /// <typeparam name="T">Type of deserialized response object.</typeparam>
    public sealed class DanbooruRequest<T> where T : class
    {
        private readonly Dictionary<String, String> args;
        private readonly String queryUrl;
        private T queryResult;
        private Int32 queryStatus;
        private Int32 timeout;

        /// <summary>
        ///     Constructor.
        ///     Initializes a new instance.
        /// </summary>
        /// <param name="credentials">User credentials, null if anonymous.</param>
        /// <param name="baseUrl">Base API url of the image board.</param>
        public DanbooruRequest(DanbooruCredentials credentials, String baseUrl)
        {
            queryResult = null;
            queryStatus = -1;
            timeout = 0;
            args = new Dictionary<String, String>();

            queryUrl = baseUrl;
            if (credentials != null)
            {
                args.Add("login", credentials.Username);
                args.Add("password_hash", credentials.Hash);
            }
        }

        #region Properties

        /// <summary>
        ///     Gets arguments of the request.
        /// </summary>
        public IEnumerable<KeyValuePair<String, String>> Arguments
        {
            get { return args; }
        }

        /// <summary>
        ///     Gets status of the request.
        /// </summary>
        public Int32 Status
        {
            get { return queryStatus; }
        }

        /// <summary>
        ///     Gets result of the request.
        /// </summary>
        public T Result
        {
            get { return queryResult; }
        }

        #endregion

        #region Arguments

        /// <summary>
        ///     Adds an argument to the request and overwrites duplicates.
        /// </summary>
        /// <param name="name">Name of the argument.</param>
        /// <param name="value">Value of the argument.</param>
        public void AddArgument(String name, String value)
        {
            args[name] = value;
        }

        /// <summary>
        ///     Adds an argument to the request and overwrites duplicates.
        /// </summary>
        /// <param name="name">Name of the argument.</param>
        /// <param name="value">Value of the argument.</param>
        public void AddArgument(String name, Object value)
        {
            args[name] = value.ToString();
        }

        /// <summary>
        ///     Removes the specified argument from the request.
        /// </summary>
        /// <param name="name">Name of the argument to remove.</param>
        public void RemoveArgument(String name)
        {
            args.Remove(name);
        }

        #endregion

        #region Events

        private EventHandler responseReceived;

        /// <summary>
        ///     Fires when DanbooruRequest receives response.
        /// </summary>
        public event EventHandler ResponseReceived
        {
            add { responseReceived += value; }
            remove { responseReceived -= value; }
        }

        private void OnResponseReceived(IAsyncResult result)
        {
            // Get Response
            if (result == null) throw new Exception();
            HttpWebRequest request = result.AsyncState as HttpWebRequest;
            if (request == null) throw new Exception();

            try
            {
                HttpWebResponse response = (HttpWebResponse) request.EndGetResponse(result);

                // Get Response Object
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var serializer = new DataContractJsonSerializer(typeof(T));
                    queryResult = serializer.ReadObject(response.GetResponseStream()) as T;
                }

                queryStatus = (Int32) response.StatusCode;
            }
            catch (WebException ex)
            {
                queryResult = null;
                //queryStatus = (Int32)HttpStatusCode.Forbidden;
                queryStatus = (Int32) ex.Status;
            }
            catch (Exception x)
            {
                queryResult = null;
                queryStatus = (Int32) HttpStatusCode.ExpectationFailed;
            }

            // Raise Event
            if (responseReceived != null)
                responseReceived(this, new EventArgs());
        }

        #endregion

        /// <summary>
        ///     Executes the Danbooru HTTP request.
        /// </summary>
        /// <returns></returns>
        public void ExecuteRequest(CookieContainer cookie)
        {
            String url = queryUrl + String.Join("&", from a in args select String.Format("{0}={1}", a.Key, a.Value));

            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(url);
            request.CookieContainer = cookie;
            request.UserAgent = Session.UserAgentString;
            request.BeginGetResponse(OnResponseReceived, request);
        }
    }
}