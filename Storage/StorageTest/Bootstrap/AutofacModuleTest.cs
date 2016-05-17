using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using Autofac;
using Autofac.Core;
using Autofac.Core.Activators.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Storage;
using Storage.Bootstrap;
using Storage.IO;
using Storage.Service;
using Storage.Service.FileSystem;
using Storage.Util;
using Storage.Util.Interface;
using Storage.View;
using Storage.ViewModel;

namespace StorageTest.Bootstrap
{
    [TestClass]
    public class AutofacModuleTest
    {
        private readonly ISet<Type> counter = new HashSet<Type>();
        private IContainer container;

        [TestInitialize]
        public void TestInitialize()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new AutofacModule());
            container = builder.Build();

            counter.Clear();
            counter.Add(typeof (IComponentContext)); // Exclude Autofac Services
        }

        [TestMethod]
        public void TestRegistrations()
        {
            // System Types
            // TODO: Testing of Delegate Registration
            //AssertRegistered<App, IContainerProperty>();
            //AssertRegistered<App, IDispatcherProperty>();
            //AssertRegistered<App, IExitable>();
            counter.Add(typeof (App));
            AssertRegistered<MainWindow, IMainWindow>();

            // Third-Party Libraries
            AssertRegistered<FileSystem, IFileSystem>();
            AssertRegistered<FileSystemWatcherWrapper, FileSystemWatcherBase>();

            // Bootstrap
            AssertRegistered<Bootstrapper, IBootstrapper>();

            // IO
            AssertRegistered<XmlExporter, IExporter>();
            AssertRegistered<XmlImporter, IImporter>();

            // Service
            AssertRegistered<Song, ISong>();
            AssertRegistered<Directories, IDirectories>();

            // Service.FileSystem
            AssertRegistered<Checker, IChecker>();
            AssertRegistered<Watcher, IWatcher>();

            // Util
            AssertRegistered<Dispatcher, IDispatcher>();
            AssertRegistered<Formatter, IFormatter>();
            AssertRegistered<MessageBox, IMessageBox>();
            AssertRegistered<Settings, ISettings>();

            // View
            AssertRegistered<DefaultView, IUserControl>(ViewType.DEFAULT);
            AssertRegistered<DirectoryView, IUserControl>(ViewType.DIRECTORY);
            AssertRegistered<SongView, IUserControl>(ViewType.SONG);
            AssertRegistered<TaskView, IUserControl>(ViewType.TASK);

            // ViewModel
            AssertRegistered<MainViewModel, IMainViewModel>();
            AssertRegistered<MainViewModel, IDirectoryCollectionProperty>();
            AssertRegistered<DirectoryViewModel, IDirectoryViewModel>();
            AssertRegistered<SongViewModel, ISongViewModel>();
            AssertRegistered<TaskViewModel, ITaskViewModel>();

            // Test Count
            Assert.AreEqual(counter.Count, container.ComponentRegistry.Registrations.Count());
        }

        private Type GetImplementationType<T>(object key = null)
        {
            Autofac.Core.Service service;
            if (key == null) {
                service = new TypedService(typeof (T));
            } else {
                service = new KeyedService(key, typeof (T));
            }

            var types = container.ComponentRegistry.RegistrationsFor(service)
                .Select(x => x.Activator)
                .OfType<ReflectionActivator>()
                .Select(x => x.LimitType)
                .ToList();

            Assert.AreEqual(1, types.Count);

            return types.First();
        }

        private void AssertRegistered<TClass, TRegistration>(object key = null)
        {
            counter.Add(typeof (TClass));
            Assert.AreEqual(typeof (TClass), GetImplementationType<TRegistration>(key));
        }
    }
}
