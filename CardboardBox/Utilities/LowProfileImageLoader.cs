// Copyright (C) Microsoft Corporation. All Rights Reserved.
// This code released under the terms of the Microsoft Public License
// (Ms-PL, http://opensource.org/licenses/ms-pl.html).

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Resources;

namespace CardboardBox.Utilities
{
    /// <summary>
    ///     Provides access to the Image.UriSource attached property which allows
    ///     Images to be loaded by Windows Phone with less impact to the UI thread.
    /// </summary>
    public static class LowProfileImageLoader
    {
        private const int WorkItemQuantum = 5;
        private static readonly Thread Thread = new Thread(WorkerThreadProc);
        private static readonly Queue<PendingRequest> PendingRequests = new Queue<PendingRequest>();
        private static readonly Queue<IAsyncResult> PendingResponses = new Queue<IAsyncResult>();
        private static readonly object SyncBlock = new object();
        private static bool exiting;

        /// <summary>
        ///     Identifies the UriSource attached DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty UriSourceProperty = DependencyProperty.RegisterAttached(
            "UriSource", typeof(Uri), typeof(LowProfileImageLoader), new PropertyMetadata(OnUriSourceChanged));

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline",
            Justification = "Static constructor performs additional tasks.")]
        static LowProfileImageLoader()
        {
            // Start worker thread
            Thread.Start();
            Application.Current.Exit += HandleApplicationExit;
            IsEnabled = true;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether low-profile image loading is enabled.
        /// </summary>
        public static bool IsEnabled { get; set; }

        /// <summary>
        ///     Gets the value of the Uri to use for providing the contents of the Image's Source property.
        /// </summary>
        /// <param name="obj">Image needing its Source property set.</param>
        /// <returns>Uri to use for providing the contents of the Source property.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "UriSource is applicable only to Image elements.")]
        public static Uri GetUriSource(Image obj)
        {
            if (null == obj)
            {
                throw new ArgumentNullException("obj");
            }
            return (Uri) obj.GetValue(UriSourceProperty);
        }

        /// <summary>
        ///     Sets the value of the Uri to use for providing the contents of the Image's Source property.
        /// </summary>
        /// <param name="obj">Image needing its Source property set.</param>
        /// <param name="value">Uri to use for providing the contents of the Source property.</param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "UriSource is applicable only to Image elements.")]
        public static void SetUriSource(Image obj, Uri value)
        {
            if (null == obj)
            {
                throw new ArgumentNullException("obj");
            }
            obj.SetValue(UriSourceProperty, value);
        }

        private static void HandleApplicationExit(object sender, EventArgs e)
        {
            // Tell worker thread to exit
            exiting = true;
            if (Monitor.TryEnter(SyncBlock, 100))
            {
                Monitor.Pulse(SyncBlock);
                Monitor.Exit(SyncBlock);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Relevant exceptions don't have a common base class.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity",
            Justification = "Linear flow is easy to understand.")]
        private static void WorkerThreadProc(object unused)
        {
            var rand = new Random();
            var pendingRequests = new List<PendingRequest>();
            var pendingResponses = new Queue<IAsyncResult>();
            while (!exiting)
            {
                lock (SyncBlock)
                {
                    // Wait for more work if there's nothing left to do
                    if ((0 == PendingRequests.Count) && (0 == PendingResponses.Count) && (0 == pendingRequests.Count) &&
                        (0 == pendingResponses.Count))
                    {
                        Monitor.Wait(SyncBlock);
                        if (exiting)
                        {
                            return;
                        }
                    }
                    // Copy work items to private collections
                    while (0 < PendingRequests.Count)
                    {
                        PendingRequest pendingRequest = PendingRequests.Dequeue();
                        // Search for another pending request for the same Image element
                        for (int i = 0; i < pendingRequests.Count; i++)
                        {
                            if (pendingRequests[i].Image == pendingRequest.Image)
                            {
                                // Found one; replace it
                                pendingRequests[i] = pendingRequest;
                                pendingRequest = null;
                                break;
                            }
                        }
                        if (null != pendingRequest)
                        {
                            // Unique request; add it
                            pendingRequests.Add(pendingRequest);
                        }
                    }
                    while (0 < PendingResponses.Count)
                    {
                        pendingResponses.Enqueue(PendingResponses.Dequeue());
                    }
                }
                var pendingCompletions = new Queue<PendingCompletion>();
                // Process pending requests
                int count = pendingRequests.Count;
                for (int i = 0; (0 < count) && (i < WorkItemQuantum); i++)
                {
                    // Choose a random item to behave reasonably at both extremes (FIFO/FILO)
                    int index = rand.Next(count);
                    PendingRequest pendingRequest = pendingRequests[index];
                    pendingRequests[index] = pendingRequests[count - 1];
                    pendingRequests.RemoveAt(count - 1);
                    count--;
                    if (pendingRequest.Uri.IsAbsoluteUri)
                    {
                        // Download from network
                        HttpWebRequest webRequest = WebRequest.CreateHttp(pendingRequest.Uri);
                        webRequest.AllowReadStreamBuffering = true;
                            // Don't want to block this thread or the UI thread on network access
                        webRequest.BeginGetResponse(HandleGetResponseResult,
                                                    new ResponseState(webRequest, pendingRequest.Image,
                                                                      pendingRequest.Uri));
                    }
                    else
                    {
                        // Load from application (must have "Build Action"="Content")
                        string originalUriString = pendingRequest.Uri.OriginalString;
                        // Trim leading '/' to avoid problems
                        Uri resourceStreamUri = originalUriString.StartsWith("/", StringComparison.Ordinal)
                                                    ? new Uri(originalUriString.TrimStart('/'), UriKind.Relative)
                                                    : pendingRequest.Uri;
                        // Enqueue resource stream for completion
                        StreamResourceInfo streamResourceInfo = Application.GetResourceStream(resourceStreamUri);
                        if (null != streamResourceInfo)
                        {
                            pendingCompletions.Enqueue(new PendingCompletion(pendingRequest.Image, pendingRequest.Uri,
                                                                             streamResourceInfo.Stream));
                        }
                    }
                    // Yield to UI thread
                    Thread.Sleep(1);
                }
                // Process pending responses
                for (int i = 0; (0 < pendingResponses.Count) && (i < WorkItemQuantum); i++)
                {
                    IAsyncResult pendingResponse = pendingResponses.Dequeue();
                    var responseState = (ResponseState) pendingResponse.AsyncState;
                    try
                    {
                        WebResponse response = responseState.WebRequest.EndGetResponse(pendingResponse);
                        pendingCompletions.Enqueue(new PendingCompletion(responseState.Image, responseState.Uri,
                                                                         response.GetResponseStream()));
                    }
                    catch (WebException)
                    {
                        // Ignore web exceptions (ex: not found)
                    }
                    // Yield to UI thread
                    Thread.Sleep(1);
                }
                // Process pending completions
                if (0 < pendingCompletions.Count)
                {
                    // Get the Dispatcher and process everything that needs to happen on the UI thread in one batch
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                        {
                            while (0 < pendingCompletions.Count)
                            {
                                // Decode the image and set the source
                                PendingCompletion pendingCompletion = pendingCompletions.Dequeue();
                                if (GetUriSource(pendingCompletion.Image) == pendingCompletion.Uri)
                                {
                                    var bitmap = new BitmapImage();
                                    try
                                    {
                                        bitmap.SetSource(pendingCompletion.Stream);
                                    }
                                    catch
                                    {
                                        // Ignore image decode exceptions (ex: invalid image)
                                    }
                                    pendingCompletion.Image.Source = bitmap;
                                }
                                else
                                {
                                    // Uri mis-match; do nothing
                                }
                                // Dispose of response stream
                                pendingCompletion.Stream.Dispose();
                            }
                        });
                }
            }
        }

        private static void OnUriSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var image = (Image) o;
            var uri = (Uri) e.NewValue;

            if (!IsEnabled || DesignerProperties.IsInDesignTool)
            {
                // Avoid handing off to the worker thread (can cause problems for design tools)
                image.Source = new BitmapImage(uri);
            }
            else
            {
                // Clear-out the current image because it's now stale (helps when used with virtualization)
                image.Source = null;
                lock (SyncBlock)
                {
                    // Enqueue the request
                    PendingRequests.Enqueue(new PendingRequest(image, uri));
                    Monitor.Pulse(SyncBlock);
                }
            }
        }

        private static void HandleGetResponseResult(IAsyncResult result)
        {
            lock (SyncBlock)
            {
                // Enqueue the response
                PendingResponses.Enqueue(result);
                Monitor.Pulse(SyncBlock);
            }
        }

        private class PendingCompletion
        {
            public PendingCompletion(Image image, Uri uri, Stream stream)
            {
                Image = image;
                Uri = uri;
                Stream = stream;
            }

            public Image Image { get; private set; }
            public Uri Uri { get; private set; }
            public Stream Stream { get; private set; }
        }

        private class PendingRequest
        {
            public PendingRequest(Image image, Uri uri)
            {
                Image = image;
                Uri = uri;
            }

            public Image Image { get; private set; }
            public Uri Uri { get; private set; }
        }

        private class ResponseState
        {
            public ResponseState(WebRequest webRequest, Image image, Uri uri)
            {
                WebRequest = webRequest;
                Image = image;
                Uri = uri;
            }

            public WebRequest WebRequest { get; private set; }
            public Image Image { get; private set; }
            public Uri Uri { get; private set; }
        }
    }
}