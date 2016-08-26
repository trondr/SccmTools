using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Common.Logging;
using Common.Logging.Simple;
using NUnit.Framework;
using SccmTools.Library;
using SccmTools.Library.Common.IO;
using SccmTools.Library.Infrastructure;
using SccmTools.Library.Infrastructure.LifeStyles;

namespace SccmTools.Tests.ManualTests
{
    [TestFixture(Category = "ManualTests")]
    public class PathOperationTests
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

        [Test]
        public void GetUncPathTest1_deta410()
        {
            Assert.AreEqual("deta410",Environment.UserDomainName.ToLower(),"This manual test must be run in user domain deta410. Please copy test for use in your user domain.");
            using(var testBooStrapper = new TestBootStrapper(GetType()))
            {
                var target = testBooStrapper.Container.Resolve<IPathOperation>();
                var path = @"p:\";
                var expected = @"\\edbwp004894-fs1\public";
                var actual = target.GetUncPath(path,false).ToLower();
                Assert.AreEqual(expected, actual,"Unc path was not expected");
            }
        }

        [Test]
        [ExpectedException(typeof(SccmToolsException))]
        public void GetUncPathTest2_deta410()
        {
            Assert.AreEqual("deta410",Environment.UserDomainName.ToLower(),"This manual test must be run in user domain deta410. Please copy test for use in your user domain.");
            using(var testBooStrapper = new TestBootStrapper(GetType()))
            {
                var target = testBooStrapper.Container.Resolve<IPathOperation>();
                var path = @"c:\Program Files";
                var expected = @"";
                var actual = target.GetUncPath(path, false).ToLower();
                Assert.AreEqual(expected, actual,"Unc path was not expected");
            }
        }

        [Test]        
        public void GetUncPathTest3_deta410()
        {
            Assert.AreEqual("deta410",Environment.UserDomainName.ToLower(),"This manual test must be run in user domain deta410. Please copy test for use in your user domain.");
            using(var testBooStrapper = new TestBootStrapper(GetType()))
            {
                var target = testBooStrapper.Container.Resolve<IPathOperation>();
                var path = @"c:\Program Files";
                var expected = @"\\edbwp004894-64\c$\program files";
                var actual = target.GetUncPath(path, true).ToLower();
                Assert.AreEqual(expected, actual,"Unc path was not expected");
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