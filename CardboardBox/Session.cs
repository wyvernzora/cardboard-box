// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
// CardboardBox/Session.cs
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
using System.ComponentModel;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using CardboardBox.API;
using CardboardBox.Model;
using CardboardBox.UI;
using CardboardBox.Utilities;
using Microsoft.Phone.Controls;
using libDanbooru2;
using libWyvernzora.Core;

namespace CardboardBox
{
    /// <summary>
    ///     Session data storage and interface for cache management.
    ///     Can only be initialized if user credentials have been verified.
    /// </summary>
    internal class Session : INotifyPropertyChanged
    {
        #region Singleton

        private static Session instance;

        /// <summary>
        ///     Gets the global instance of the Session.
        /// </summary>
        public static Session Instance
        {
            get { return instance ?? (instance = new Session()); }
        }

        #endregion

        #region Constants



        #endregion

        /// <summary>
        ///     Constructor.
        ///     Initializes a new instance.
        /// </summary>
        public Session()
        {
            Logging.D("Session Constructor");

            // Get Isolated Storage Instance
            IsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();

            // Try to restore user credentials
            settings = IsolatedStorageSettings.ApplicationSettings;
            if (settings.Contains("username") && settings.Contains("hash"))
            {
                Credentials = DanbooruCredentials.FromCredentials(settings["username"].ToString(),
                                                                  settings["hash"].ToString());
                Logging.D("Found credentials for {0}", Credentials.Username);
            }
            // For the testing purposes, enforce maximum rating.
            MaxRating = Rating.Safe;

            // Supported User Levels
            userLevels = new Dictionary<int, UserLevel>()
                {
                    {-1, new UserLevel(-1, "Unknown", 0)},
                    {0, new UserLevel(0, "Anonymous", 0)},
                    {20, new UserLevel(20, "Member", 2)},
                    {30, new UserLevel(30, "Gold (Paid)", 6)},
                    {31, new UserLevel(31, "Platinum (Paid)", 12)},
                    {32, new UserLevel(32, "Builder", Int32.MaxValue)},
                    {33, new UserLevel(33, "Contributor", Int32.MaxValue)},
                    {35, new UserLevel(35, "Janitor", Int32.MaxValue)},
                    {40, new UserLevel(40, "Moderator", Int32.MaxValue)},
                    {50, new UserLevel(50, "Administrator", Int32.MaxValue)}
                };
            
        }

        private readonly IsolatedStorageSettings settings;
        private readonly Dictionary<int, UserLevel> userLevels;


        /// <summary>
        /// Gets the IsolatedStorage instance for the application.
        /// </summary>
        public IsolatedStorageFile IsolatedStorage { get; private set; }

        /// <summary>
        ///     Gets or sets credentials for current session.
        /// </summary>
        public DanbooruCredentials Credentials { get; set; }

        /// <summary>
        /// Gets the Danbooru 2.0 API Client for the session.
        /// </summary>
        public Danbooru2Client Client { get; private set; }

        /// <summary>
        ///     Gets the current user info.
        /// </summary>
        public User User { get; private set; }

        /// <summary>
        ///     Gets the current user level and limits.
        /// </summary>
        public UserLevel Level { get { return userLevels[User.Level]; }}

        /// <summary>
        /// Gets the post viewer template generator for the session.
        /// </summary>
        public PostViewerTemplate PostViewerTemplate { get; private set; }

        /// <summary>
        /// Gets or sets the cookie for this session.
        /// </summary>
        public CookieContainer Cookie { get; set; }

        #region Session Variables

        /// <summary>
        /// Gets or sets a list of newest posts.
        /// </summary>
        public PostTupleCollection NewPosts { get; set; }

        /// <summary>
        /// Gets or sets the number of pages loaded for new posts.
        /// </summary>
        public Int32 NewPostPagesLoaded { get; set; }

        /// <summary>
        /// Gets or sets currently selected post.
        /// </summary>
        public Post Selected { get; set; }

        /// <summary>
        /// Current tag query.
        /// </summary>
        public String[] Query { get; set; }

        /// <summary>
        /// Gets or sets arguments for navigation request.
        /// </summary>
        public String NavigationArguments { get; set; }
        
        #endregion

        #region User Preferences

        private Rating maxRating;

        /// <summary>
        ///     Gets or sets maximum post rating acceptable for the user.
        ///     TODO Parental Control Integration
        /// </summary>
        public Rating MaxRating
        {
            get { return maxRating; }
            set
            {
                if (maxRating != value)
                {
                    maxRating = value;
                    OnPropertyChanged("MaxRating");
                }
            }
        }

        /// <summary>
        /// Gets the rating for query that satisfies the MaxRating setting.
        /// </summary>
        public String MaxComplement
        {
            get
            {
                if (maxRating == Rating.Safe) return "rating:s";
                return maxRating == Rating.Questionable ? "-rating:e" : "";
            }
        }

        #endregion

        #region Methods
        
        public void ClearBackStack()
        {
            var phoneApplicationFrame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (phoneApplicationFrame != null)
                while (phoneApplicationFrame.BackStack.Any())
                    phoneApplicationFrame.RemoveBackEntry();
            else
                throw new Exception("Error!");
        }

        /// <summary>
        /// Confirms that the user successfully logged in.
        /// </summary>
        internal void ConfirmUserLogin()
        {
            settings["username"] = Credentials.Username;
            settings["hash"] = Credentials.Hash;
            settings.Save();
        }

        /// <summary>
        /// Logs user out.
        /// Saves settings, discards session and goes back
        /// to the login page.
        /// </summary>
        internal void LogOut()
        {
            // Save Settings

            // Discard Session
            instance = null;
            Credentials = null;

            // Nullify Stored Credentials
            settings.Remove("username");
            settings.Remove("hash");
            settings.Save();

            // Go Back to Login
            NavigationHelper.Navigate(new Uri(Constants.LoginView, UriKind.Relative));
        }
        
        /// <summary>
        /// Loads more New Posts on the caller thread.
        /// May block for a long time.
        /// </summary>
        /// <param name="pages"></param>
        internal PostTuple[] GetMoreNewPosts(Int32 pages)
        {
            Int32 page = (NewPosts.Count * 3) / Client.PageSize + 1;
            return ExecutePostQuery(page, pages);
        }

        /// <summary>
        /// Searches for the specified tag combination.
        /// </summary>
        /// <param name="startPage"></param>
        /// <param name="pages"></param>
        /// <param name="tags"></param>
        /// <returns></returns>
        internal PostTuple[] ExecutePostQuery(Int32 startPage, Int32 pages, params String[] tags)
        {
            // Check for user level enforcement
            Int32 levelLimit = Level.TagLimit;
            String[] query = ResolveQueryString(String.Join(" ", tags));

            if (query.Length > levelLimit)
                throw new UnauthorizedAccessException(
                    String.Format("Attempt to search {0} tags while user level only allows {1}.", tags.Length,
                                  levelLimit));

            List<PostTuple> tuples = new List<PostTuple>();
            Post[] posts = Client.GetPosts(startPage, pages, query);

            posts = posts.PadEnd(posts.Length.Align(3), null);

            for (int j = 0; j < posts.Length - 2; j += 3)
                tuples.Add(new PostTuple {First = posts[j], Second = posts[j + 1], Third = posts[j + 2]});
            
            return tuples.ToArray();
        }

        internal String[] ResolveQueryString(String query)
        {
            // Get User Settings
            String rating = MaxComplement;
            Boolean hasRating = !String.IsNullOrEmpty(rating);
            var tags = new List<String>(query.Split(new[] {' '}, StringSplitOptions.RemoveEmptyEntries));

            // If there is no rating tag in query and rating filter is enabled, add a rating tag
            if (!tags.Any(t => t.StartsWith("rating:", StringComparison.CurrentCultureIgnoreCase)) && hasRating) tags.Add(rating);

            return tags.ToArray();
        }
        

        #endregion

        #region Preference Settings Notification

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(String propName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }

        #endregion

        public UserLevel GetUserLevel(Int32 level)
        {
            return userLevels.ContainsKey(level) ? userLevels[level] : userLevels[-1];
        }

        public void InitializeAsync(Action callback)
        {
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += (@s, e) =>
                {
                    Logging.D("Session.InitializeAsync(): Starting Session Initialization");

                    // Create the client
                    Client = new Danbooru2Client(Constants.SiteHostname, Credentials);

                    // Get User Profile
                    User = Client.GetUser(Credentials.Username);

                    Logging.D("Session.InitializeAsync(): Loading posts");

                    // Load Template generator
                    PostViewerTemplate = new PostViewerTemplate("Assets/template.html");

                    // Load first 3 pages
                    NewPosts = new PostTupleCollection();
                    PostTuple[] tuples = GetMoreNewPosts(3);
                    foreach (var t in tuples) NewPosts.Add(t);
                };
            bw.RunWorkerCompleted += (@s, e) => callback();
            bw.RunWorkerAsync();
        }
    }
}