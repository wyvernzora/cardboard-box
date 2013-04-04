using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using libDanbooru2.IO;

namespace CardboardBox
{
    /// <summary>
    /// Download helper that forces async download on a specific thread.
    /// </summary>
    class ImageDownloadHelper
    {
        private Boolean complete = false;

        public ImageDownloadHelper(String uri, String localFile)
        {
            WebClient wc = new WebClient();
            wc.OpenReadCompleted += (@s, e) =>
                {
                    if (e != null && !e.Cancelled)
                    {
                        try
                        {
                            if (!CardboardBoxSession.Instance.IsolatedStorage.DirectoryExists(Path.GetDirectoryName(localFile)))
                                CardboardBoxSession.Instance.IsolatedStorage.CreateDirectory(Path.GetDirectoryName(localFile));

                            ExStream exStream = new ExStream(e.Result);
                            using (
                                var fs = new IsolatedStorageFileStream(localFile, FileMode.Create,
                                                                       CardboardBoxSession.Instance.IsolatedStorage))
                            {
                                exStream.WriteTo(fs);
                            }
                            exStream.Close();
                        }
                        catch (Exception)
                        {
                            Error = true;
                        }
                    }
                    else
                    {
                        Error = true;
                    }

                    complete = true;
                };
            wc.OpenReadAsync(new Uri(uri));
        }


        public Boolean Error
        { get; private set; }

        public Boolean Complete
        { get { return complete; } }
    }
}
