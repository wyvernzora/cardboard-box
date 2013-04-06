using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Media.Imaging;
using libDanbooru2;

namespace CardboardBox
{
    /// <summary>
    /// Session data storage and interface for cache management.
    /// Can only be initialized if user credentials have been verified.
    /// </summary>
    class Session
    {
        #region Singleton

        private static Session instance;

        public static Session Instance
        {
            get { return instance ?? (instance = new Session()); }
        }



        #endregion

        /// <summary>
        /// Constructor.
        /// Initializes a new instance.
        /// </summary>
        public Session()
        {
            RatingLock = true; // Force rating lock

        }

        public void InitializeAsync(Action callback)
        {


            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (@s, e) =>
                {
                    // Initialization work here
                    Cache = new LocalCacheManager();

                    // Get User Profile
                    DanbooruRequest<User[]> userRequest = new DanbooruRequest<User[]>(App.Credentials, Constants.DanbooruUserIndexUrl);
                    userRequest.AddArgument("name", App.Credentials.Username);

                    userRequest.ExecuteRequest();
                    while (userRequest.Status == -1) ; // Wait for request to complete
                    User[] result = userRequest.Result;
                    User =
                        result.First(
                            u => StringComparer.InvariantCultureIgnoreCase.Equals(App.Credentials.Username, u.Name));

                    // Get New For Today
                    DanbooruRequest<Post[]> whatNewRequest = new DanbooruRequest<Post[]>(App.Credentials, Constants.DanbooruPostIndexUrl);
                    whatNewRequest.AddArgument("limit", 100);
                    whatNewRequest.AddArgument("page", 1);
                    if (RatingLock) whatNewRequest.AddArgument("tags", "rating:s");
                    whatNewRequest.ExecuteRequest();
                    while (whatNewRequest.Status == -1) ; // Wait for request to complete
                    WhatsNewPosts = whatNewRequest.Result;

                    // Update Cache
                    Cache.EnsureTileCache(WhatsNewPosts);

                };
            bw.RunWorkerCompleted += (@s, e) =>
                                     callback();
            bw.RunWorkerAsync();
        }


        public User User { get; set; }

        public Boolean RatingLock { get; set; }

        public LocalCacheManager Cache { get; private set; }

        public Post SelectedPost { get; set; }


        public Post[] WhatsNewPosts { get; set; }
    }
}
