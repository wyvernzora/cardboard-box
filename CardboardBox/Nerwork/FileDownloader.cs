// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// CardboardBox/FileDownloader.cs
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
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Threading;
using System.Windows;
using libDanbooru2;

namespace CardboardBox.Nerwork
{
    internal class FileDownloader
    {
        private readonly Session session;
        private Boolean busy;

        /// <summary>
        ///     Constructor.
        ///     Initializes a new instance.
        /// </summary>
        /// <param name="session">Session that owns the downloader.</param>
        /// <param name="forceEventUi">If true, events will be invoked on the UI thread.</param>
        public FileDownloader(Session session)
        {
            this.session = session;
        }

        /// <summary>
        ///     Gets a value indicating whether the downloader is downloading a file.
        /// </summary>
        public Boolean Busy
        {
            get { return busy; }
        }

        #region Events

        private EventHandler<FileDownloadedEventArgs> downloadFinished;

        private void OnDownloadFinished(FileDownloadedEventArgs e)
        {
            // Forces the event onto the UI thread so that UI can take advantage of downloaded data.
            if (downloadFinished != null)
            {
                    downloadFinished(this, e);
            }
        }

        public event EventHandler<FileDownloadedEventArgs> DownloadFinished
        {
            add { downloadFinished += value; }
            remove { downloadFinished -= value; }
        }

        /// <summary>
        ///     Event arguments for the file download finished event.
        /// </summary>
        public class FileDownloadedEventArgs : EventArgs
        {
            /// <summary>
            ///     Constructor.
            ///     Initializes a new instance.
            /// </summary>
            /// <param name="id">
            ///     <see cref="ID" />
            /// </param>
            /// <param name="request">
            ///     <see cref="RequestUri" />
            /// </param>
            /// <param name="local">
            ///     <see cref="LocalPath" />
            /// </param>
            public FileDownloadedEventArgs(Int32 id, Uri request, String local)
            {
                ID = id;
                LocalPath = local;
                RequestUri = request;
            }

            /// <summary>
            ///     Constructor.
            ///     Initializes a new instance.
            /// </summary>
            /// <param name="ex">Error that occured during the download.</param>
            public FileDownloadedEventArgs(Exception ex)
            {
                Error = true;
                ErrorDetails = ex;
            }

            /// <summary>
            ///     Gets requested file Uri.
            /// </summary>
            public Uri RequestUri { get; private set; }

            /// <summary>
            ///     Gets local path of the downloaded file.
            /// </summary>
            public String LocalPath { get; private set; }

            /// <summary>
            ///     Gets ID of the download task.
            /// </summary>
            public Int32 ID { get; private set; }

            /// <summary>
            ///     Gets a value indicating whether there was an error.
            /// </summary>
            public Boolean Error { get; private set; }

            /// <summary>
            ///     Gets the exception that was thrown when the error occured.
            /// </summary>
            public Exception ErrorDetails { get; private set; }
        }

        #endregion

        /// <summary>
        ///     Begins downloading the file on a background thread.
        ///     Returns immideately.
        /// </summary>
        /// <param name="taskid">ID of the task assigned by caller.</param>
        /// <param name="remote">URI of the file to download.</param>
        /// <param name="local">Local path for the downloaded file; if file already exists it is overwritten.</param>
        public void DownloadAsync(Int32 taskid, Uri remote, String local)
        {
            busy = true;

            HttpWebRequest request = WebRequest.CreateHttp(remote);
            request.AllowAutoRedirect = true;
            request.UserAgent = "Mozilla/5.0 (Windows; U; MSIE 9.0; WIndows NT 9.0; en-US))";
            request.CookieContainer = session.Cookie;

            request.BeginGetResponse(@cb =>
                {
                    try
                    {
                        HttpWebResponse response = request.EndGetResponse(cb) as HttpWebResponse;

                        if (response == null) throw new Exception("No response received for the task #" + taskid);

                        using (Stream data = response.GetResponseStream())
                        {
                            // Make Sure Directory Exists
                            String dir = Path.GetDirectoryName(local);
                            if (dir != null && !session.IsolatedStorage.DirectoryExists(dir))
                                session.IsolatedStorage.CreateDirectory(dir);

                            using (IsolatedStorageFileStream file = session.IsolatedStorage.OpenFile(local, FileMode.Create))
                            {
                                data.CopyTo(file);
                            }
                        }

                        OnDownloadFinished(new FileDownloadedEventArgs(taskid, remote, local));
                    }
                    catch (Exception x)
                    {
                        OnDownloadFinished(new FileDownloadedEventArgs(x));
                    }

                    busy = false;
                }, local);

        }

        /// <summary>
        ///     Begins downloading the file on a background thread.
        ///     Blocks the calling thread until the download finishes.
        /// </summary>
        /// <param name="taskid">ID of the task assigned by caller.</param>
        /// <param name="remote">URI of the file to download.</param>
        /// <param name="local">Local path for the downloaded file; if file already exists it is overwritten.</param>
        public void Download(Int32 taskid, Uri remote, String local)
        {
        //    DownloadAsync(taskid, remote, local);
         //   while (busy) Thread.Sleep(10); // Block the calling thread until download finishes.
            
        }
    }
}