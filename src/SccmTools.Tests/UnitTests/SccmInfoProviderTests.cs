﻿using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Common.Logging;
using Common.Logging.Simple;
using NUnit.Framework;
using SccmTools.Library.Infrastructure;
using SccmTools.Library.Infrastructure.LifeStyles;
using SccmTools.Library.Module.Services;

namespace SccmTools.Tests.UnitTests
{
    [TestFixture(Category = "UnitTests")]
    public class SccmInfoProviderTests
    {
        private ConsoleOutLogger _logger;

        [SetUp]
        public void SetUp()
        {
            _logger = new ConsoleOutLogger(this.GetType().Name, LogLevel.All, true, false, false, "yyyy-MM-dd hh:mm:ss");            
        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test()]
        public void SccmInfoProviderGetSiteCodeTest()
        {            
            using(var testBooStrapper = new TestBootStrapper(GetType()))
            {
               var target = testBooStrapper.Container.Resolve<ISccmInfoProvider>();               
               var actual = target.GetSiteCode();
               _logger.Info("SiteCode: " + actual);
               Assert.IsNotNullOrEmpty(actual);
            }
        }

        [Test]
        public void SccmInfoProviderGetSiteServerTest()
        {            
            using(var testBooStrapper = new TestBootStrapper(GetType()))
            {
               var target = testBooStrapper.Container.Resolve<ISccmInfoProvider>();               
               var actual = target.GetSiteServer();
               _logger.Info("SiteServer: " + actual);
               Assert.IsNotNullOrEmpty(actual);
            }
        }

        [Test(Description = "Note! This test requires that current user has read access to the SCCM server. Minimum role: Read-only Analyst")]
        public void SccmInfoProviderGetSiteIdTest()
        {            
            using(var testBooStrapper = new TestBootStrapper(GetType()))
            {
               var target = testBooStrapper.Container.Resolve<ISccmInfoProvider>();               
               var actual = target.GetSiteId();
               _logger.Info("SiteId: " + actual);
               Assert.IsNotNullOrEmpty(actual);
            }
        }

        [Test(Description = "Note! This test requires that current user has read access to the SCCM server. Minimum role: Read-only Analyst")]
        public void SccmInfoProviderGetAuthoringScopeIdTest()
        {            
            using(var testBooStrapper = new TestBootStrapper(GetType()))
            {
               var target = testBooStrapper.Container.Resolve<ISccmInfoProvider>();               
               var actual = target.GetAuthoringScopeId();
               _logger.Info("AuthoringScopeId: " + actual);
               Assert.IsNotNullOrEmpty(actual);
            }
        }

        internal class TestBootStrapper: IDisposable
        {
            readonly ILog _logger;
            private IWindsorContainer _container;

            public TestBootStrapper(Type type)
            {
                _logger = new ConsoleOutLogger(type.Name,LogLevel.Info, true, false,false,"yyyy-MM-dd HH:mm:ss");
            }

            public IWindsorContainer Container
            {
                get
                {
                    if(_container == null)
                    {
                        _container = new WindsorContainer();
                        _container.Register(Component.For<IWindsorContainer>().Instance(_container));
            
                        //Configure logging
                        _container.Register(Component.For<ILog>().Instance(_logger));
            
                        //Manual override registrations for interfaces that the interface under test is dependent on
                        //_container.Register(Component.For<ISomeInterface>().Instance(MockRepository.GenerateStub<ISomeInterface>()));

                        //Factory registrations example:

                        //container.Register(Component.For<ITeamProviderFactory>().AsFactory());
                        //container.Register(
                        //    Component.For<ITeamProvider>()
                        //        .ImplementedBy<CsvTeamProvider>()
                        //        .Named("CsvTeamProvider")
                        //        .LifeStyle.Transient);
                        //container.Register(
                        //    Component.For<ITeamProvider>()
                        //        .ImplementedBy<SqlTeamProvider>()
                        //        .Named("SqlTeamProvider")
                        //        .LifeStyle.Transient);

                        ///////////////////////////////////////////////////////////////////
                        //Automatic registrations
                        ///////////////////////////////////////////////////////////////////
                        //
                        //   Register all command providers and attach logging interceptor
                        //
                        const string libraryRootNameSpace = "SccmTools.Library";
                        
                        //
                        //   Register all singletons found in the library
                        //
                        _container.Register(Classes.FromAssemblyContaining<CommandDefinition>()
                            .InNamespace(libraryRootNameSpace, true)
                            .If(type => Attribute.IsDefined(type, typeof(SingletonAttribute)))
                            .WithService.DefaultInterfaces().LifestyleSingleton());
                        
                        //
                        //   Register all transients found in the library
                        //
                        _container.Register(Classes.FromAssemblyContaining<CommandDefinition>()
                            .InNamespace(libraryRootNameSpace, true)
                            .WithService.DefaultInterfaces().LifestyleTransient());

                    }
                    return _container;
                }

            }

            ~TestBootStrapper()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
            }

            protected virtual void Dispose(bool disposing)
            {
                if(disposing)
                {
                    if(_container != null)
                    {
                        _container.Dispose();
                        _container = null;
                    }
                }
            }
        }
    }
}