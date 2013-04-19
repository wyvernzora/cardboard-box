using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using CardboardBox.Model;

namespace CardboardBox.API
{
    /// <summary>
    /// Danbooru Client Class.
    /// </summary>
    public class Danbooru2Client
    {
        public const String PostIndex = "/posts.json?";
        public const String UserIndex = "/users.json?";
        public const String CommentIndex = "/comments.json?";

        public const Int32 MaxAttempts = 3;

        private readonly DanbooruCredentials credentials;
        private readonly String server;
        private Int32 pageSize;
        
        /// <summary>
        /// Constructor.
        /// Initializes a new instance.
        /// </summary>
        /// <param name="server">Danbooru 2.0 Server hostname (without 'http://').</param>
        /// <param name="credentials">Danbooru 1.0 API Credentials.</param>
        /// <param name="pagesize">Number of posts to retrieve per API call, defaults to 60.</param>
        public Danbooru2Client(String server, DanbooruCredentials credentials, Int32 pagesize = 60)
        {
            this.server = "http://" + server;
            this.credentials = credentials;
            pageSize = pagesize;
        }


        public Int32 PageSize
        { get { return pageSize; } }


        /// <summary>
        /// Gets the profile of a specific user.
        /// </summary>
        /// <param name="username">Name of the user.</param>
        /// <returns></returns>
        public User GetUser(String username)
        {
            Logging.D("DanbooruClient.GetUser(): Username={0}", username);

            var request = new DanbooruRequest<User[]>(credentials, server + UserIndex);
            request.AddArgument("search[name]", username);

            request.ExecuteRequest();
            while (request.Status == -1) Thread.Sleep(20);

            User[] result = request.Result;

            if (result == null)
                throw new Exception("Could not get user profile! HTTP Status " + request.Status);

            User user = result.FirstOrDefault(u => StringComparer.InvariantCultureIgnoreCase.Equals(username, u.Name));

            if (user == null)
                throw new Exception("User profile for " + username + " not found!");

            Logging.D("DanbooruClient.GetUser(): User profile retrieved!");

            return user;
        }

        /// <summary>
        /// Gets posts from Danbooru server that match a certain combination of tags.
        /// </summary>
        /// <param name="startPage">1-based index of the post page.</param>
        /// <param name="count">Number of pages to get.</param>
        /// <param name="tags">Tags to search for.</param>
        /// <returns></returns>
        public Post[] GetPosts(Int32 startPage, Int32 count, params String[] tags)
        {
            String query = String.Join("+", tags);

            Logging.D("DanbooruClient.GetPosts(): StartPage={0}; Count={1}; Query={2}", startPage, count, query);

            Int32 retries = 0;
            List<Post> temp = new List<Post>();
            for (int i = startPage; i < startPage + count; i++)
            {

                var request = new DanbooruRequest<Post[]>(credentials, server + PostIndex);
                request.AddArgument("limit", pageSize);
                request.AddArgument("tags", query);
                request.AddArgument("page", i);

                request.ExecuteRequest();
                while (request.Status == -1) Thread.Sleep(20);

                if (request.Status != 200)
                {
                    Logging.D("DanbooruClient.GetPosts(): Error getting posts (HTTP Status = {0}), {1} attempts left.",
                              request.Status, MaxAttempts - retries);
                    i--;
                    retries++;
                    if (retries >= MaxAttempts)
                    {
                        Logging.D("DanbooruClient.GetPosts(): Error getting posts but no attempts left, aborting.");
                        return temp.ToArray();
                    }
                    continue;
                }

                Post[] posts = request.Result;
                temp.AddRange(posts);

                if (posts.Length < pageSize) break;
            }

            return temp.ToArray();
        }
    }
}
