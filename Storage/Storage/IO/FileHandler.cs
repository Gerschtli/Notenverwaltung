using System;
using System.IO;
using System.IO.Abstractions;

namespace Storage.IO
{
    public abstract class FileHandler
    {
        #region Fields

        protected readonly IFileSystem fileSystem;

        #endregion

        #region Constructor

        protected FileHandler(IFileSystem fileSystem)
        {
            this.fileSystem = fileSystem;
        }

        #endregion

        #region Abstracted Methods

        protected void SetVisible(string path)
        {
            IgnoreFileNotFound(
                () => fileSystem.File.SetAttributes(path, GetFileAttributes(path) & ~FileAttributes.Hidden));
        }

        protected void SetHidden(string path)
        {
            IgnoreFileNotFound(
                () => fileSystem.File.SetAttributes(path, GetFileAttributes(path) | FileAttributes.Hidden));
        }

        #endregion

        #region Private Methods

        private FileAttributes GetFileAttributes(string path)
        {
            return fileSystem.File.GetAttributes(path);
        }

        private void IgnoreFileNotFound(Action action)
        {
            try {
                action();
            } catch (Exception ex) {
                if (!(ex is FileNotFoundException) && !(ex is DirectoryNotFoundException)) {
                    throw;
                }
            }
        }

        #endregion
    }
}
