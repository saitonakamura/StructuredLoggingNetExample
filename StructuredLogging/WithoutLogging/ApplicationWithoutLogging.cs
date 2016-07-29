using System.Threading;
using Autofac;
using NUnit.Framework;

namespace StructuredLogging.WithoutLogging
{
    [TestFixture]
    public class TestsWithoutLogging
    {
        [Test]
        public void Test1()
        {
            var classWithVirtualMethodsToLog = IocContainerWithoutLogging.Container.Resolve<ClassWithVirtualMethodsToLogWithoutLogging>();

            classWithVirtualMethodsToLog.SomeMethodToLog();
        }
    }

    public static class IocContainerWithoutLogging
    {
        static IocContainerWithoutLogging()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<ClassWithVirtualMethodsToLogWithoutLogging>();

            Container = containerBuilder.Build();
        }

        public static IContainer Container { get; set; }
    }

    public class ClassWithVirtualMethodsToLogWithoutLogging
    {
        public void SomeMethodToLog()
        {
            Thread.Sleep(1234);
        }
    }
}