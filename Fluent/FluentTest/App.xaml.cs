using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using Fluent;

namespace FluentTest
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("fa");
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru");
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("de");
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("hu");
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("cs");
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("fr");
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("pl");
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("ja");
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("nl");
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("pt");
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("es");
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh");
            //Thread.CurrentThread.CurrentUICulture = new CultureInfo("sv");
        }

        public App()
        {
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("ru-RU"); ;
        }

        private void OnApplicationStartup(object sender, StartupEventArgs e)
        {
            
        }
    }
}
