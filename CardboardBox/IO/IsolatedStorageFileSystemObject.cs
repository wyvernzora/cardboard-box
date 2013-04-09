using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using libWyvernzora.IO.UnifiedFileSystem;

namespace CardboardBox.IO
{
    /// <summary>
    /// A UnifiedFileSystem wrapper for an IsolatedStorage File
    /// </summary>
    public class IsolatedStorageFileSystemObject : FileSystemObject
    {
        public IsolatedStorageFileSystemObject(string name, FileSystemObjectType type, long length, string dispName = "") : base(name, type, length, dispName)
        {
        }

        public override IList<FileSystemObject> GetChildren()
        {
            throw new NotImplementedException();
        }

        public override bool HasChild(string name)
        {
            throw new NotImplementedException();
        }

        public override FileSystemObject GetChild(string name)
        {
            throw new NotImplementedException();
        }
    }
}
