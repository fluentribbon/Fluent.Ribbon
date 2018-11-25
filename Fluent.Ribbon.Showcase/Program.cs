namespace FluentTest
{
    using System;

    public static class Program
    {
        [STAThread]
        public static int Main(string[] args)
        {
            var app = new App();
            app.InitializeComponent();
            app.Run();

            return 0;
        }
    }
}