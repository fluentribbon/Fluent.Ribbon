namespace Fluent.Tests.TestClasses
{
    using System;
    using System.Diagnostics;

    public class TestRibbonWindow : RibbonWindow, IDisposable
    {
        public TestRibbonWindow()
            : this(null)
        {
        }

        public TestRibbonWindow(object content)
        {
            this.Content = content;

            this.ShowActivated = false;
            this.ShowInTaskbar = false;

            if (Debugger.IsAttached == false)
            {
                this.Left = int.MinValue;
                this.Top = int.MinValue;
            }

            this.Show();
        }

        public void Dispose()
        {
            this.Close();
        }
    }
}