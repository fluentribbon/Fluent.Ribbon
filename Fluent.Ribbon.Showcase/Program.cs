namespace FluentTest
{
    using System;

    public static class Program
    {
        [STAThread]
#pragma warning disable CA1801 // Review unused parameters
        public static int Main(string[] args)
#pragma warning restore CA1801 // Review unused parameters
        {
            var app = new App();
            app.InitializeComponent();
            app.Run();

            return 0;
        }
    }
}