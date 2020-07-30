namespace Fluent.Internal
{
    using System;
    using System.Windows;

    /// <summary>
    /// Executes action on framework element when it's loaded.
    /// </summary>
    /// <remarks>
    /// Action is execued only once, no matter how many times Loaded event is fired.
    /// </remarks>
    internal class WhenLoaded
    {
        public WhenLoaded(FrameworkElement target, Action<FrameworkElement> loadedAction)
        {
            this.target = target;
            this.loadedAction = loadedAction;

            this.DoAction();
        }

        private void DoAction()
        {
            if (this.target.IsLoaded)
            {
                this.loadedAction(this.target);
            }
            else
            {
                this.target.Loaded += this.TargetLoaded;
            }
        }

        private void TargetLoaded(object sender, RoutedEventArgs e)
        {
            this.target.Loaded -= this.TargetLoaded;
            this.loadedAction(this.target);
        }

        private readonly FrameworkElement target;
        private readonly Action<FrameworkElement> loadedAction;
    }

    /// <summary>
    /// <see cref="WhenLoaded"/> class wrapper to add clarity to the code.
    /// </summary>
    internal static class WhenLoadedExtension
    {
        /// <summary>
        /// Executes given action only if framework element is loaded. Otherwise waits until framework element
        /// is loaded and executes the action.
        /// </summary>
        /// <param name="frameworkElement">Target framework element which we want to be loaded.</param>
        /// <param name="loadedAction">Action to be executed when framework element is loaded.</param>
        /// <remarks>Action is executed only once.</remarks>
        public static void WhenLoaded(this FrameworkElement frameworkElement, Action<FrameworkElement> loadedAction)
        {
            // The following object will not be GCed before its time.
            // * if frameworkElement is already loaded the ctor immediately executes loadedAction and rests in piece
            // * otherwise framework element's Loaded event will keep the reference to event handler
            //   until it's loaded and and only then let it rest in piece.
            // ReSharper disable once ObjectCreationAsStatement
            new WhenLoaded(frameworkElement, loadedAction);
        }
    }
}
