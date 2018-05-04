namespace Fluent.Tests
{
    using System;
    using System.Windows;
    using NUnit.Framework;

    [SetUpFixture]
    public class AssemblySetup
    {
        [OneTimeSetUp]
        public void Setup()
        {
            var app = new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };

            app.Resources.MergedDictionaries.Add((ResourceDictionary)Application.LoadComponent(new Uri("/Fluent;component/Themes/Generic.xaml", UriKind.Relative)));
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            ////Application.Current.Shutdown();
        }
    }
}