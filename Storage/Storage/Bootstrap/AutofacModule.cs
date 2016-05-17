using System;
using System.IO.Abstractions;
using System.Windows;
using Autofac;
using Storage.IO;
using Storage.Service;
using Storage.Service.FileSystem;
using Storage.Util;
using Storage.Util.Interface;
using Storage.View;
using Storage.ViewModel;
using MessageBox = Storage.Util.MessageBox;

namespace Storage.Bootstrap
{
    public class AutofacModule : Module
    {
        #region Fields

        private ContainerBuilder builder;

        #endregion

        #region Overridden Methods

        protected override void Load(ContainerBuilder containerBuilder)
        {
            builder = containerBuilder;
            Load();
        }

        #endregion

        private void Load()
        {
            // System Types
            Register<App, IDispatcherProperty, IExitable>(c => Application.Current as App);
            Register<MainWindow, IMainWindow>();

            // Third-Party Libraries
            Register<FileSystem, IFileSystem>();
            Register<FileSystemWatcherWrapper, FileSystemWatcherBase>();

            // Bootstrap
            Register<Bootstrapper, IBootstrapper>();

            // IO
            Register<XmlExporter, IExporter>();
            Register<XmlImporter, IImporter>();

            // Service
            Register<Song, ISong>();
            Register<Directories, IDirectories>();

            // Service.FileSystem
            Register<Checker, IChecker>();
            Register<Watcher, IWatcher>();

            // Util
            Register<Dispatcher, IDispatcher>();
            Register<Formatter, IFormatter>();
            Register<MessageBox, IMessageBox>();
            Register<Settings, ISettings>();

            // View
            Register<DefaultView, IUserControl>(ViewType.DEFAULT);
            Register<DirectoryView, IUserControl>(ViewType.DIRECTORY);
            Register<SongView, IUserControl>(ViewType.SONG);
            Register<TaskView, IUserControl>(ViewType.TASK);

            // ViewModel
            Register<MainViewModel, IMainViewModel, IDirectoryCollectionProperty>();
            Register<DirectoryViewModel, IDirectoryViewModel>(false);
            Register<SongViewModel, ISongViewModel>(false);
            Register<TaskViewModel, ITaskViewModel>(false);
        }

        #region Wrapper Methods

        private void Register<TClass, TRegistration>(bool singleInstance = true)
        {
            var register = builder
                .RegisterType<TClass>()
                .As<TRegistration>();
            if (singleInstance) {
                register.SingleInstance();
            }
        }

        private void Register<TClass, TRegistration>(object key)
        {
            builder
                .RegisterType<TClass>()
                .As<TClass>()
                .Keyed<TRegistration>(key)
                .SingleInstance();
        }

        private void Register<TClass, TRegistration1, TRegistration2>()
        {
            builder
                .RegisterType<TClass>()
                .As<TClass>()
                .As<TRegistration1>()
                .As<TRegistration2>()
                .SingleInstance();
        }

        private void Register<TClass, TRegistration1, TRegistration2>(Func<IComponentContext, TClass> function)
        {
            builder
                .Register(function)
                .As<TRegistration1>()
                .As<TRegistration2>()
                .SingleInstance();
        }

        #endregion
    }
}
