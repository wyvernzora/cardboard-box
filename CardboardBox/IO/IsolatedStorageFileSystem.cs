using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libWyvernzora.IO.UnifiedFileSystem;

namespace CardboardBox.IO
{
    /// <summary>
    /// UnifiedFileSystem wrapper for WP7 IsolatedStorage.
    /// </summary>
    public class IsolatedStorageFileSystem : IFileSystem<IsolatedStorageFileSystemObject>
    {

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public IEnumerable<IsolatedStorageFileSystemObject> Files
        {
            get { throw new NotImplementedException(); }
        }

        public IsolatedStorageFileSystemObject Root
        {
            get { throw new NotImplementedException(); }
        }

        public IsolatedStorageFileSystemObject GetFileSystemObject(string path)
        {
            throw new NotImplementedException();
        }

        public libWyvernzora.IO.StreamEx OpenFileSystemObject(IsolatedStorageFileSystemObject obj, System.IO.FileAccess mode)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
