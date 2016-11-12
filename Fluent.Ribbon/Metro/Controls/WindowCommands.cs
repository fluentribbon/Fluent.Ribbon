// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    /// Contains commands for <see cref="RibbonWindow"/>
    /// </summary>
    [TemplatePart(Name = "PART_Min", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Max", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Restore", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Close", Type = typeof(Button))]
    public class WindowCommands : ItemsControl, IDisposable
    {
        private static string minimize;
        private static string maximize;
        private static string closeText;
        private static string restore;
        private System.Windows.Controls.Button minimizeButton;        
        private System.Windows.Controls.Button maximizeButton;
        private System.Windows.Controls.Button restoreButton;
        private System.Windows.Controls.Button closeButton;
        private IntPtr user32 = IntPtr.Zero;
        private bool disposed;

        static WindowCommands()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowCommands), new FrameworkPropertyMetadata(typeof(WindowCommands)));
        }

        /// <summary>
        /// Finalizer
        /// </summary>
        ~WindowCommands()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios.
        /// If disposing equals true, the method has been called directly
        /// or indirectly by a user's code. Managed and unmanaged resources
        /// can be disposed.
        /// If disposing equals false, the method has been called by the
        /// runtime from inside the finalizer and you should not reference
        /// other objects. Only unmanaged resources can be disposed.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (this.disposed)
            {
                return;
            }

            // If disposing equals true, dispose all managed
            // and unmanaged resources.
            if (disposing)
            {
                // Dispose managed resources.
            }

            // Call the appropriate methods to clean up
            // unmanaged resources here.
            // If disposing is false,
            // only the following code is executed.
            if (this.user32 != IntPtr.Zero)
            {
                NativeMethods.FreeLibrary(this.user32);
                this.user32 = IntPtr.Zero;
            }

            // Note disposing has been done.
            this.disposed = true;
        }

        /// <summary>
        /// Retrieves the translated string for Minimize
        /// </summary>
        public string Minimize
        {
            get
            {
                if (string.IsNullOrEmpty(minimize))
                    minimize = this.GetCaption(900);
                return minimize;
            }
        }

        /// <summary>
        /// Retrieves the translated string for Maximize
        /// </summary>
        public string Maximize
        {
            get
            {
                if (string.IsNullOrEmpty(maximize))
                    maximize = this.GetCaption(901);
                return maximize;
            }
        }

        /// <summary>
        /// Retrieves the translated string for Restore
        /// </summary>
        public string Restore
        {
            get
            {
                if (string.IsNullOrEmpty(restore))
                {
                    restore = this.GetCaption(903);
                }

                return restore;
            }
        }

        /// <summary>
        /// Retrieves the translated string for Close
        /// </summary>
        public string Close
        {
            get
            {
                if (string.IsNullOrEmpty(closeText))
                    closeText = this.GetCaption(905);
                return closeText;
            }
        }

        /// <summary>
        /// Gets or sets the button brush
        /// </summary>
        public Brush ButtonBrush
        {
            get { return (Brush)this.GetValue(ButtonBrushProperty); }
            set { this.SetValue(ButtonBrushProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ButtonBrush.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ButtonBrushProperty = DependencyProperty.Register(nameof(ButtonBrush), typeof(Brush), typeof(WindowCommands), new PropertyMetadata(Brushes.Black));        

        private string GetCaption(int id)
        {
            if (this.user32 == IntPtr.Zero)
            {
                this.user32 = NativeMethods.LoadLibrary(Environment.SystemDirectory + "\\User32.dll");
            }

            var sb = new StringBuilder(256);
            NativeMethods.LoadString(this.user32, (uint)id, sb, sb.Capacity);
            return sb.ToString().Replace("&", "");
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate"/>.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.minimizeButton = this.Template.FindName("PART_Min", this) as System.Windows.Controls.Button;
            if (this.minimizeButton != null)
                this.minimizeButton.Click += this.MinimizeClick;

            this.maximizeButton = this.Template.FindName("PART_Max", this) as System.Windows.Controls.Button;
            if (this.maximizeButton != null)
                this.maximizeButton.Click += this.MaximiseClick;

            this.restoreButton = this.Template.FindName("PART_Restore", this) as System.Windows.Controls.Button;
            if (this.restoreButton != null)
                this.restoreButton.Click += this.RestoreClick;

            this.closeButton = this.GetTemplateChild("PART_Close") as System.Windows.Controls.Button;
            if (this.closeButton != null)
                this.closeButton.Click += this.CloseClick;
        }

        private void MinimizeClick(object sender, RoutedEventArgs e)
        {
            var parentWindow = this.GetParentWindow();
            if (parentWindow != null)
                parentWindow.WindowState = WindowState.Minimized;
        }

        private void MaximiseClick(object sender, RoutedEventArgs e)
        {
            var parentWindow = this.GetParentWindow();
            if (parentWindow == null)
                return;

            parentWindow.WindowState = WindowState.Maximized;
        }

        private void RestoreClick(object sender, RoutedEventArgs e)
        {
            var parentWindow = this.GetParentWindow();
            if (parentWindow == null)
                return;

            parentWindow.WindowState = WindowState.Normal;
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            var parentWindow = this.GetParentWindow();
            parentWindow?.Close();
        }

        private Window GetParentWindow()
        {
            var parent = VisualTreeHelper.GetParent(this);

            while (parent != null && !(parent is Window))
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            var parentWindow = parent as Window;
            return parentWindow;
        }
    }
}