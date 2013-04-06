using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using libDanbooru2;

namespace CardboardBox
{
    /// <summary>
    /// Manages Locally cached images
    /// </summary>
    class LocalCacheManager
    {
        /// <summary>
        /// Local Cache Entry
        /// </summary>
        public class CacheEntry
        {
            public Int32 ID { get; set; }

            public String TileCache { get; set; }

            public String SampleCache { get; set; }
        }

        private IsolatedStorageFile istore;
        private Dictionary<Int32, CacheEntry> cache; 

        /// <summary>
        /// Constructor.
        /// Initializes a new instance.
        /// </summary>
        public LocalCacheManager()
        {
            // Initialize fields
            istore = IsolatedStorageFile.GetUserStoreForApplication();
            cache = new Dictionary<int, CacheEntry>();

            // Load Cache Index
            using (var sr = new StreamReader(istore.OpenFile("cache.index", FileMode.OpenOrCreate)))
            {
                for (String line = sr.ReadLine(); line != null; line = sr.ReadLine())
                {
                    if (line.StartsWith("//")) continue; // Skip comments

                    String[] sub = line.Split('|');
                    CacheEntry entry = new CacheEntry()
                        {
                            ID = Int32.Parse(sub[0]),
                            TileCache = sub[1],
                            SampleCache = sub[2]
                        };
                    cache.Add(entry.ID, entry);
                }
            }
        }
        
        public IsolatedStorageFile IsolatedStorage
        { get { return istore; } }

        #region Methods

        /// <summary>
        /// Makes sure that tile cache for the specified post is there.
        /// If it is not, attempts to download.
        /// Only returns if the cache is there.
        /// </summary>
        /// <param name="post"></param>
        public void EnsureTileCache(Post post)
        {
            if (cache.ContainsKey(post.ID) && cache[post.ID].TileCache != null)
                return;

            String url = Constants.DanbooruPreviewUrl + post.MD5 + ".jpg";
            String local = Constants.TileCachePath + post.ID.ToString(CultureInfo.InvariantCulture) + ".jpg";

            // Download Tile
            var idh = new ImageDownloadHelper(url, local, this);
            while (!idh.Complete) ; // Wait

            if (cache.ContainsKey(post.ID)) cache[post.ID].TileCache = local;
            else
                cache.Add(post.ID, new CacheEntry() {ID = post.ID, TileCache = local, SampleCache = null});
        }

        public void EnsureTileCache(IEnumerable<Post> posts)
        {
            foreach (var p in posts) EnsureTileCache(p);
        }

        public BitmapSource GetTile(Post p)
        {
            EnsureTileCache(p);

            BitmapImage img = new BitmapImage();
            using (var stream = istore.OpenFile(cache[p.ID].TileCache, FileMode.Open))
            {
                img.SetSource(stream);
            }
            return img;
        }


        /// <summary>
        /// Makes sure that tile cache for the specified post is there.
        /// If it is not, attempts to download.
        /// Only returns if the cache is there.
        /// </summary>
        /// <param name="post"></param>
        public void EnsureSampleCache(Post post)
        {
            if (cache.ContainsKey(post.ID) && cache[post.ID].SampleCache != null)
                return;

            String url = "http://danbooru.donmai.us" + Session.Instance.SelectedPost.FileUrl.ToString();
            String local = Constants.SampleCachePath + post.ID.ToString(CultureInfo.InvariantCulture) + ".png";

            // Download Tile
            var idh = new ImageDownloadHelper(url, local, this);
            while (!idh.Complete) ; // Wait

            if (cache.ContainsKey(post.ID)) 
                cache[post.ID].SampleCache = local;
            else
                cache.Add(post.ID, new CacheEntry() { ID = post.ID, TileCache = null, SampleCache = local });
        }

        public BitmapSource GetSample(Post p)
        {
            EnsureSampleCache(p);

            BitmapImage img = new BitmapImage();
            using (var stream = istore.OpenFile(cache[p.ID].SampleCache, FileMode.Open))
            {
                img.SetSource(stream);
            }
            return img;
        }

        public Stream GetSampleStream(Post p)
        {
            if (!istore.FileExists(cache[p.ID].SampleCache))
                System.Diagnostics.Debugger.Break();
            return istore.OpenFile(cache[p.ID].SampleCache, FileMode.Open, FileAccess.Read, FileShare.Read);
        }



        public void SaveCacheIndex()
        {
            using (var sw = new StreamWriter(istore.OpenFile("cache.index", FileMode.Truncate)))
            {
                foreach (var v in cache.Values)
                    sw.WriteLine("{0}|{1}|{2}", v.ID, v.TileCache, v.SampleCache);
            }
        }

        #endregion
    }
}
