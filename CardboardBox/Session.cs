﻿// =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-
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

// ReSharper disable InconsistentNaming

        public const String SiteUrl = "http://danbooru.donmai.us";

        public const String UserIndexUrl = "/user/index.json?";

        public const String PostIndexUrl = "/posts.json?";

        public const String CommentIndexUrl = "/comments.json?";
        
        public const String PreviewDir = "/ssd/data/preview/";


        public const Int32 PostRequestPageSize = 60;

        public const Int32 PostRequestTupleSize = 20;

        public const String UserAgentString =
            "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.31 (KHTML, like Gecko) Chrome/26.0.1410.43 Safari/537.31";

        public const String SaltString = "choujin-steiner--{0}--";
// ReSharper restore InconsistentNaming

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

        #region Posts

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

        /// <summary>
        /// Navigates to the specified Uri.
        /// </summary>
        /// <param name="uri"></param>
        public void Navigate(Uri uri)
        {
            var phoneApplicationFrame = Application.Current.RootVisual as PhoneApplicationFrame;
            if (phoneApplicationFrame != null)
                phoneApplicationFrame.Navigate(uri);
            else
                throw new Exception("Navigation Error!");
        }

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

            // Nullify Stored Credentials
            settings.Remove("username");
            settings.Remove("hash");

            // Go Back to Login
            Navigate(new Uri("/Modules/LoginPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Clears and reloads the session.
        /// </summary>
        internal void ReloadSession()
        {
            ClearBackStack();
            instance = null;
            // Go Back to Login
            Navigate(new Uri("/Modules/LoginPage.xaml", UriKind.Relative));
        }

        /// <summary>
        /// Loads more New Posts on the caller thread.
        /// May block for a long time.
        /// </summary>
        /// <param name="pages"></param>
        internal PostTuple[] GetMoreNewPosts(Int32 pages)
        {
            Logging.D("Loading more new posts!");
            List<PostTuple> tuples = new List<PostTuple>();
            Int32 retry = 0;
            for (int i = 0; i < pages; i++)
            {
                var newRequest = new DanbooruRequest<Post[]>(Credentials, SiteUrl + PostIndexUrl);
                newRequest.AddArgument("limit", PostRequestPageSize);

                String rating = MaxComplement;
                if (MaxComplement != String.Empty)
                    newRequest.AddArgument("tags", rating);

                Int32 page = ((NewPosts.Count + tuples.Count) * 3) / PostRequestPageSize + 1;

                newRequest.AddArgument("page", page);

                newRequest.ExecuteRequest(Cookie);
                while (newRequest.Status == -1) ;
                Post[] newPosts = newRequest.Result;

                if (newRequest.Status != 200)
                {
                    Logging.D("ERROR: Failed to load more new posts, HTTP Status = {0}, {1} retries left", newRequest.Status, retry);
                    i--;
                    retry++;
                    if (retry >= 3)
                    {
                        Logging.D("ERROR: Failed to load more posts and retries have been used up!");
                        return tuples.ToArray();
                    }

                    continue;
                }

                newPosts = newPosts.PadEnd(NumericOps.Align(newPosts.Length, 3), null);

                foreach (var p in newPosts)
                    if (p != null) p.PreviewUrl = new Uri(SiteUrl + PreviewDir + p.MD5 + ".jpg");

                for (int j = 0; j < newPosts.Length - 2; j += 3)
                    tuples.Add(new PostTuple { First = newPosts[j], Second = newPosts[j + 1], Third = newPosts[j + 2] });

                if (newPosts.Length < PostRequestPageSize) break;
            }


            Logging.D("More new posts loaded!");
            return tuples.ToArray();
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
            Int32 tagCount = tags.Length;
            if (MaxComplement != String.Empty) tagCount++;

            if (tagCount > userLevels[User.Level].TagLimit)
                throw new UnauthorizedAccessException(String.Format("Attempt to search for {0} tags while the user level only permits {1}.", tagCount, userLevels[User.Level].TagLimit));
            
            // Get Stuff
            Logging.D("Executing a query: page={0}, count={1}, tag_count={2}", startPage, pages, tags.Length);
            List<PostTuple> tuples = new List<PostTuple>();
            Int32 retry = 0;
            for (int i = startPage; i < startPage + pages; i++)
            {
                var request = new DanbooruRequest<Post[]>(Credentials, SiteUrl + PostIndexUrl);
                request.AddArgument("limit", PostRequestPageSize);

                String rating = MaxComplement;
                if (tags.Any(t => t.StartsWith("rating:", StringComparison.CurrentCultureIgnoreCase)))
                    rating = String.Empty;
                String tag = rating;
                if (tags.Length != 0)  tag += "+" + String.Join("+", tags);

                request.AddArgument("tags", tag);

                Logging.D("Query: {0}", tag);

                request.AddArgument("page", i);
                request.ExecuteRequest(Cookie);
                while (request.Status == -1) Thread.Sleep(20);  // wait

                Post[] posts = request.Result;

                if (request.Status != 200)
                {
                    Logging.D("ERROR: Failed to execute query, HTTP Status = {0}, {1} retries left", request.Status, retry);
                    i--;
                    retry++;
                    if (retry >= 3)
                    {
                        Logging.D("ERROR: Failed to execute query and retries have been used up, abort!");
                        return tuples.ToArray();
                    }
                    continue;
                }

                posts = posts.PadEnd(NumericOps.Align(posts.Length, 3), null);

                foreach (var p in posts)
                    if (p != null) p.PreviewUrl = new Uri(SiteUrl + PreviewDir + p.MD5 + ".jpg");

                for (int j = 0; j < posts.Length - 2; j += 3)
                    tuples.Add(new PostTuple { First = posts[j], Second = posts[j + 1], Third = posts[j + 2] });

                if (posts.Length < PostRequestPageSize) break;
            }

            return tuples.ToArray();
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
                    Logging.D("Starting Session Initialization");

                    // Get User Profile
                    DanbooruRequest<User[]> userRequest = new DanbooruRequest<User[]>(Instance.Credentials,
                                                                                      SiteUrl + UserIndexUrl);
                    userRequest.AddArgument("name", Instance.Credentials.Username);

                    userRequest.ExecuteRequest(Instance.Cookie);
                    while (userRequest.Status == -1) ; // Wait for request to complete
                    User[] result = userRequest.Result;
                    User =
                        result.First(
                            u => StringComparer.InvariantCultureIgnoreCase.Equals(instance.Credentials.Username, u.Name));

                    Logging.D("User profile retrieved");

#if DEBUG
                    if (User == null)
                        Debugger.Break(); // Since user is logged in, the profile MUST be there.
#endif
                    Logging.D("Loading posts");

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