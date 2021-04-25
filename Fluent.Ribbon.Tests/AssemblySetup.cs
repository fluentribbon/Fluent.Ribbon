namespace Fluent.Tests
{
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Threading;
    using NUnit.Framework;

    [SetUpFixture]
    public class AssemblySetup
    {
        [OneTimeSetUp]
        public void Setup()
        {
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext());

            var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };

            app.Resources.MergedDictionaries.Add((ResourceDictionary)Application.LoadComponent(new Uri("/Fluent;component/Themes/Generic.xaml", UriKind.Relative)));
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }
    }
}