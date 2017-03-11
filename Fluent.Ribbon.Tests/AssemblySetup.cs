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
            var app = new Application();
            app.Resources.MergedDictionaries.Add((ResourceDictionary)Application.LoadComponent(new Uri("/Fluent;Component/Themes/Generic.xaml", UriKind.Relative)));
        }
    }
}