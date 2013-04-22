using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using CardboardBox.Model;
using Community.CsharpSqlite.SQLiteClient;
using libWyvernzora.Utilities;

namespace CardboardBox
{
    /// <summary>
    /// Singleton entry point for all local database operations.
    /// </summary>
    public class Database
    {
        #region Constants

        // Database Parameters
        public const String DatabaseFileName = "opix.db";

        public const String ConnectionString = "Version=3,uri=file:opix.db";

        public const Int32 PostPageSize = 60;

        // Create Table Commands
        private const String CreateUserTable = 
            @"CREATE TABLE user (
                [id] INTEGER PRIMARY KEY, 
                [name] TEXT, 
                [rating] TEXT, 
                [feed] TEXT)";
        
        private const String CreateFavoriteTable = 
            @"CREATE TABLE favorites (
                [id] INTEGER, 
                [userid] INTEGER, 
                [parent_id] INTEGER, 
                [has_children] INTEGER, 
                [rating] TEXT, 
                [uploader] TEXT, 
                [uploader_id] INTEGER,
                [md5] TEXT,
                [image_height] INTEGER,
                [image_width] INTEGER,
                [file_size] INTEGER,
                [file_ext] TEXT,
                [tag_string_artist] TEXT,
                [tag_string_character] TEXT,
                [tag_string_copyright] TEXT,
                [tag_string_general] TEXT,
                [has_large] INTEGER,
                [add_time] INTEGER)";

        private const String CreateBookmarkTable =
            @"CREATE TABLE bookmarks (
                [id] INTEGER PRIMARY KEY,
                [userid] INTEGER,
                [query] TEXT
                )";
        

        #endregion

        #region Singleton Pattern

        private static Database instance;
        public static Database Instance
        { get { return instance ?? (instance = new Database()); } }

        #endregion

        #region Database Connection Maintenance

        private SqliteConnection dbConnection;

        public void OpenConnection()
        {
            try
            {
                dbConnection = new SqliteConnection(ConnectionString);
                dbConnection.Open();
                if (dbConnection == null) throw new Exception("Database Connection object is null.");
            }
            catch (Exception x)
            {
                throw new ApplicationException("Failed to open Database Connection.", x);
            }
        }

        public void CloseConnection()
        {
            if (dbConnection != null)
            {
                dbConnection.Close();
                dbConnection.Dispose();
                dbConnection = null;
            }
        }

        public Boolean HasConnection
        { get { return dbConnection != null; } }

        #endregion

        #region Database Creation

        public Boolean DatabaseExists
        {
            get
            {
                var istore = IsolatedStorageFile.GetUserStoreForApplication();
                return istore.FileExists(DatabaseFileName);
            }
        }

        public void CreateDatabase()
        {
            if (dbConnection != null) CloseConnection();

            var istore = IsolatedStorageFile.GetUserStoreForApplication();
            if (istore.FileExists(DatabaseFileName)) 
                istore.DeleteFile(DatabaseFileName);

            OpenConnection();

            using (var cmd = dbConnection.CreateCommand())
            {
                cmd.Transaction = dbConnection.BeginTransaction();

                // Create User Table
                cmd.CommandText = CreateUserTable;
                cmd.ExecuteNonQuery();

                // Create Favorites table
                cmd.CommandText = CreateFavoriteTable;
                cmd.ExecuteNonQuery();

                // Create Bookmarks Table
                cmd.CommandText = CreateBookmarkTable;
                cmd.ExecuteNonQuery();

                cmd.Transaction.Commit();
            }
        }

        #endregion

        #region User Profiles

        public void AddUser(User user, String rating, String feed)
        {
            if (dbConnection == null)
                throw new InvalidOperationException("Database Connection is null!");

            using (var cmd = dbConnection.CreateCommand())
            {
                cmd.CommandText = "INSERT OR REPLACE INTO user(id, name, rating, feed) VALUES(@id, @name, @rating, @feed);SELECT last_insert_rowid();";
                cmd.Parameters.Add("@id", user.ID);
                cmd.Parameters.Add("@name", user.Name);
                cmd.Parameters.Add("@rating", rating);
                cmd.Parameters.Add("@feed", feed);
                cmd.ExecuteScalar();
            }
        }

        public void RemoveUser(User user)
        {
            if (dbConnection == null)
                throw new InvalidOperationException("Database Connection is null!");

            using (var cmd = dbConnection.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM user WHERE id = @id";
                cmd.Parameters.Add("@id", user.ID);
                cmd.ExecuteScalar();
            }
        }

        public Boolean HasUser(User user)
        {
            if (dbConnection == null)
                throw new InvalidOperationException("Database Connection is null!");

            Int32 result;

            using (var cmd = dbConnection.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(id) FROM user WHERE id=@id";
                cmd.Parameters.Add("@id", user.ID);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    result = reader.GetInt32(0);
                }
            }

            return result > 0;
        }

        public UserConfig GetUserConfig(User user)
        {
            if (dbConnection == null)
                throw new InvalidOperationException("Database Connection is null!");

            UserConfig config = new UserConfig();

            using (var cmd = dbConnection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM user WHERE id=@id";
                cmd.Parameters.Add("@id", user.ID);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    config.ID = reader.GetInt32(0);
                    config.Username = reader.GetString(1);
                    config.Rating = reader.GetString(2);
                    config.Feed = reader.GetString(3);
                }
            }

            return config;
        }

        #endregion

        #region Favorites

        public void AddFavorite(User user, Post post)
        {
            if (dbConnection == null)
                throw new InvalidOperationException("Database Connection is null!");

            using (var cmd = dbConnection.CreateCommand())
            {
                cmd.CommandText = "INSERT OR IGNORE INTO favorites(id, userid, parent_id, has_children, rating, uploader, uploader_id, md5, image_height, image_width, file_size, file_ext, tag_string_artist, tag_string_character, tag_string_copyright, tag_string_general, has_large, add_time) " +
                    "VALUES(@id, @userid, @parent_id, @has_children, @rating, @uploader, @uploader_id, @md5, @image_height, @image_width, @file_size, @file_ext, @tag_string_artist, @tag_string_character, @tag_string_copyright, @tag_string_general, @has_large, @add_time);SELECT last_insert_rowid();";
                cmd.Parameters.Add("@id", post.ID);
                cmd.Parameters.Add("@userid", user.ID);
                cmd.Parameters.Add("@parent_id", post.ParentIdString);
                cmd.Parameters.Add("@has_children", post.HasChildren ? 1 : 0);
                cmd.Parameters.Add("@rating", post.RatingString);
                cmd.Parameters.Add("@uploader", post.Author);
                cmd.Parameters.Add("@uploader_id", post.AuthorID);
                cmd.Parameters.Add("@md5", post.MD5);
                cmd.Parameters.Add("@image_height", post.Height);
                cmd.Parameters.Add("@image_width", post.Width);
                cmd.Parameters.Add("@file_size", post.FileSize);
                cmd.Parameters.Add("@file_ext", post.FileExtension);
                cmd.Parameters.Add("@tag_string_artist", post.ArtistTagsString);
                cmd.Parameters.Add("@tag_string_character", post.CharacterTagsString);
                cmd.Parameters.Add("@tag_string_copyright", post.CopyrightTagsString);
                cmd.Parameters.Add("@tag_string_general", post.GeneralTagsString);
                cmd.Parameters.Add("@has_large", post.HasLarge ? 1 : 0);
                cmd.Parameters.Add("@add_time", DateTime.Now.Ticks / 10000);
                cmd.ExecuteScalar();
            }

        }

        public void RemoveFavorite(User user, Post post)
        {
            if (dbConnection == null)
                throw new InvalidOperationException("Database Connection is null!");

            using (var cmd = dbConnection.CreateCommand())
            {
                cmd.CommandText = "DELETE FROM favorites WHERE userid = @userid AND id = @id";
                cmd.Parameters.Add("@id", post.ID);
                cmd.Parameters.Add("@userid", user.ID);
                cmd.ExecuteScalar();
            }
        }

        public Boolean HasFavorite(User user, Post post)
        {
            if (dbConnection == null)
                throw new InvalidOperationException("Database Connection is null!");

            Int32 result;

            using (var cmd = dbConnection.CreateCommand())
            {
                cmd.CommandText = "SELECT COUNT(id) FROM favorites WHERE id = @id AND userid = @userid";
                cmd.Parameters.Add("@id", post.ID);
                cmd.Parameters.Add("@userid", user.ID);
                using (var reader = cmd.ExecuteReader())
                {
                    reader.Read();
                    result = reader.GetInt32(0);
                }
            }

            return result > 0;
        }

        public Post[] GetFavorites(User user, Int64 lastAddTime)
        {
            if (dbConnection == null)
                throw new InvalidOperationException("Database Connection is null!");

            List<Post> result = new List<Post>();

            using (var cmd = dbConnection.CreateCommand())
            {
                cmd.CommandText = "SELECT * FROM favorites WHERE userid = @userid AND add_time < @last_time ORDER BY add_time DESC LIMIT @page_size";
                cmd.Parameters.Add("@userid", user.ID);
                cmd.Parameters.Add("@last_time", lastAddTime);
                cmd.Parameters.Add("@page_size", PostPageSize);
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Post p = new Post();

                        p.ID = reader.GetInt32(0);

                        if (reader.IsDBNull(2)) p.ParentIdString = null;
                        else p.ParentIdString = reader.GetString(2);

                        p.HasChildren = ReadNullableInt32(reader, 3, 0) != 0;
                        p.RatingString = ReadNullableString(reader, 4, "s");
                        p.Author = ReadNullableString(reader, 5, "Anonymous");
                        p.AuthorID = ReadNullableInt32(reader, 6, 0);
                        p.MD5 = ReadNullableString(reader, 7, "");
                        p.Height = ReadNullableInt32(reader, 8, 0);
                        p.Width = ReadNullableInt32(reader, 9, 0);
                        p.FileSize = ReadNullableInt32(reader, 10, 0);
                        p.FileExtension = ReadNullableString(reader, 11, ".jpg");
                        p.ArtistTagsString = ReadNullableString(reader, 12, "");
                        p.CharacterTagsString = ReadNullableString(reader, 13, "");
                        p.CopyrightTagsString = ReadNullableString(reader, 14, "");
                        p.GeneralTagsString = ReadNullableString(reader, 15, "");
                        p.HasLarge = ReadNullableInt32(reader, 16, 0) != 0;
                        p.LastAddTime = reader.GetInt64(17);
                            
                        result.Add(p);
                    }
                }
            }

            return result.ToArray();
        }

        #endregion

        #region Utility Methods

        private Int32 ReadNullableInt32(SqliteDataReader reader, Int32 column, Int32 value)
        {
            if (reader.IsDBNull(column))
                return value;
            return reader.GetInt32(column);
        }

        private String ReadNullableString(SqliteDataReader reader, Int32 column, String value)
        {
            if (reader.IsDBNull(column))
                return value;
            return reader.GetString(column);
        }

        #endregion

        #region Nested Data Types

        public class UserConfig
        {
            public Int32 ID { get; set; }
            public String Username { get; set; }
            public String Rating { get; set; }
            public String Feed { get; set; }
        }

        #endregion



    }
}
