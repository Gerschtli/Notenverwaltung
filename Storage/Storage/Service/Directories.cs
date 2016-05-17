using System;
using System.Collections.Generic;
using System.Linq;
using Storage.Model;
using Storage.Util;
using Storage.Util.Interface;
using Storage.ViewModel;
using DirectoryModel = Storage.Model.Directory;

namespace Storage.Service
{
    public class Directories : IDirectories
    {
        #region Constructor

        public Directories(
            IDispatcher dispatcher, IDirectoryCollectionProperty model, Func<IDirectoryViewModel> directoryFactory,
            Func<ISongViewModel> songFactory, Func<ITaskViewModel> taskFactory)
        {
            this.dispatcher = dispatcher;
            this.model = model;
            this.directoryFactory = directoryFactory;
            this.songFactory = songFactory;
            this.taskFactory = taskFactory;
        }

        #endregion

        #region Fields

        private readonly Func<IDirectoryViewModel> directoryFactory;
        private readonly IDispatcher dispatcher;
        private readonly IDirectoryCollectionProperty model;
        private readonly Func<ISongViewModel> songFactory;
        private readonly Func<ITaskViewModel> taskFactory;

        #endregion

        #region Public Methods

        public void AddDirectory(DirectoryModel directory)
        {
            var directoryViewModel = directoryFactory();
            directoryViewModel.Directory = directory;
            AddItem(directoryViewModel);
        }

        public void AddSong(Model.Song song)
        {
            var songViewModel = songFactory();
            songViewModel.Song = song;
            AddItem(songViewModel);
        }

        public void AddTask(Task task)
        {
            var taskViewModel = taskFactory();
            taskViewModel.Task = task;
            AddItem(taskViewModel);
        }

        public void UpdatePath(string oldPath, string newPath)
        {
            dispatcher.Invoke(
                () =>
                {
                    Iterate(
                        model.Directories,
                        item => item.Path.StartsWith(oldPath),
                        item => item.Path = string.Concat(newPath, item.Path.Substring(oldPath.Length)));
                });
        }

        public void DeleteByPath(string path)
        {
            dispatcher.Invoke(
                () =>
                {
                    var list = new List<IDirectoryListItem>();
                    Iterate(
                        model.Directories,
                        item => item.Path.StartsWith(path),
                        item => list.Add(item));
                    list.ForEach(item => model.Directories.Remove(item));
                });
        }

        #endregion

        #region Private Methods

        private void AddItem(IDirectoryListItem viewModel)
        {
            dispatcher.Invoke(() => { model.Directories.Add(viewModel); });
        }

        private void Iterate<T>(IEnumerable<T> collection, Func<T, bool> predicate, Action<T> action)
        {
            foreach (var item in collection.Where(predicate)) {
                action(item);
            }
        }

        #endregion
    }
}
