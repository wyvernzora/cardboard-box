using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;

namespace libDanbooru2
{
    /// <summary>
    /// Danbooru image board credentials.
    /// </summary>
    /// <typeparam name="T">Type of deserialized response object.</typeparam>
    public sealed class DanbooruRequest<T> where T : class
    {
        private T queryResult;
        private String queryUrl;
        private Int32 queryStatus;
        private Int32 timeout;
        private Dictionary<String, String> args; 

        /// <summary>
        /// Constructor.
        /// Initializes a new instance.
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
        /// Gets arguments of the request.
        /// </summary>
        public IEnumerable<KeyValuePair<String, String>> Arguments
        { get { return args; } }      
        /// <summary>
        /// Gets status of the request.
        /// </summary>
        public Int32 Status
        { get { return queryStatus; } }
        /// <summary>
        /// Gets result of the request.
        /// </summary>
        public T Result
        { get { return queryResult; } }

        #endregion

        #region Arguments


        /// <summary>
        /// Adds an argument to the request and overwrites duplicates.
        /// </summary>
        /// <param name="name">Name of the argument.</param>
        /// <param name="value">Value of the argument.</param>
        public void AddArgument(String name, String value)
        {
            args[name] = value;
        }
        /// <summary>
        /// Adds an argument to the request and overwrites duplicates.
        /// </summary>
        /// <param name="name">Name of the argument.</param>
        /// <param name="value">Value of the argument.</param>
        public void AddArgument(String name, Object value)
        {
            args[name] = value.ToString();
        }
        /// <summary>
        /// Removes the specified argument from the request.
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
        /// Fires when DanbooruRequest receives response.
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
#if DEBUG
                    MemoryStream tmp = new MemoryStream();
                    response.GetResponseStream().CopyTo(tmp);

                    tmp.Position = 0;
                    StreamReader sr = new StreamReader(tmp);
                    String test = sr.ReadToEnd();

                    tmp.Position = 0;

                    var serializer = new DataContractJsonSerializer(typeof(T));
                    queryResult = serializer.ReadObject(tmp) as T;
#else

                    var serializer = new DataContractJsonSerializer(typeof(T));
                    queryResult = serializer.ReadObject(response.GetResponseStream()) as T;
#endif



                }

                queryStatus = (Int32)response.StatusCode;
            }
            catch (System.Net.WebException ex)
            {
                queryResult = null;
                //queryStatus = (Int32)HttpStatusCode.Forbidden;
                queryStatus = (Int32)ex.Status;
            }
            catch (Exception x)
            {
                queryResult = null;
                queryStatus = (Int32)HttpStatusCode.ExpectationFailed;
            }

            // Raise Event
            if (responseReceived != null)
                 responseReceived(this, new EventArgs());
        }

        #endregion

        /// <summary>
        /// Executes the Danbooru HTTP request.
        /// </summary>
        /// <returns></returns>
        public void ExecuteRequest(CookieContainer cookie)
        {
            String url = queryUrl + String.Join("&", from a in args select String.Format("{0}={1}", a.Key, a.Value));

            HttpWebRequest request = (HttpWebRequest) HttpWebRequest.Create(url);
            request.CookieContainer = cookie;
            request.UserAgent = Constants.UserAgentString;
            request.BeginGetResponse(OnResponseReceived, request);
        }
    }
}
