namespace Fluent.Tests.Helper
{
    using System;
    using System.Windows.Threading;

    public static class UIHelper
    {
        public static void DoEvents()
        {
            Dispatcher.CurrentDispatcher.DoEvents();
        }

        public static void DoEvents(this Dispatcher dispatcher)
        {
            dispatcher.Invoke(DispatcherPriority.Background, new Action(delegate { }));
        }
    }
}