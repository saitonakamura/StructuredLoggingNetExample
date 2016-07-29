using System;
using System.Diagnostics;
using System.Threading;
using Autofac;
using Autofac.Extras.DynamicProxy2;
using Castle.DynamicProxy;
using NUnit.Framework;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace StructuredLogging.WithLogging
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Test1()
        {
            var classWithVirtualMethodsToLog = IocContainer.Container.Resolve<ClassWithVirtualMethodsToLog>();

            classWithVirtualMethodsToLog.SomeMethodToLog();
        }
    }

    public static class IocContainer
    {
        static IocContainer()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<LoggerInterceptor>();
            containerBuilder.RegisterType<ClassWithVirtualMethodsToLog>()
                .EnableClassInterceptors().InterceptedBy(typeof(LoggerInterceptor));

            var logger = new LoggerConfiguration()
                .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://docker.local:9200")) { AutoRegisterTemplate = true })
                .CreateLogger();

            Log.Logger = logger;

            Container = containerBuilder.Build();
        }

        public static IContainer Container { get; set; }
    }

    public class LoggerInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            var methodExecutionTimer = new Stopwatch();

            methodExecutionTimer.Start();

            invocation.Proceed();

            methodExecutionTimer.Stop();

            var methodName = invocation.Method.Name;
            var methodExecutionTime = methodExecutionTimer.Elapsed;

            Log.Information("Method '{methodName}' is called, {methodExecutionTime} elapsed", methodName, methodExecutionTime);
        }
    }

    public class ClassWithVirtualMethodsToLog
    {
        public virtual void SomeMethodToLog()
        {
            Thread.Sleep(1234);
        }
    }
}