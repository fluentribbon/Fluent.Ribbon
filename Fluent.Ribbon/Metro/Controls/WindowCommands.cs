// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Media;
    using ControlzEx.Native;
    using Fluent.Helpers;

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
#pragma warning disable 618
        private SafeLibraryHandle user32;
#pragma warning restore 618
        private bool disposed;

        static WindowCommands()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(WindowCommands), new FrameworkPropertyMetadata(typeof(WindowCommands)));
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="WindowCommands"/> class.
        /// </summary>
        ~WindowCommands()
        {
            this.Dispose(false);
        }

        /// <inheritdoc />
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
            if (this.user32 != null)
            {
                this.user32.Close();
                this.user32 = null;
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
                {
                    minimize = this.GetCaption(900);
                }

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
                {
                    maximize = this.GetCaption(901);
                }

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
                {
                    closeText = this.GetCaption(905);
                }

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

        private string GetCaption(uint id)
        {
#pragma warning disable 618
            if (this.user32 == null)
            {
                this.user32 = UnsafeNativeMethods.LoadLibrary(Path.Combine(Environment.SystemDirectory, "User32.dll"));
            }

            var sb = new StringBuilder(256);
            if (UnsafeNativeMethods.LoadString(this.user32, id, sb, sb.Capacity) == 0)
            {
                sb.Clear();
                sb.AppendFormat("String with id '{0}' could not be found.", id);
            }
#pragma warning restore 618
            return sb.ToString().Replace("&", string.Empty);
        }

        /// <inheritdoc />
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.minimizeButton = this.Template.FindName("PART_Min", this) as System.Windows.Controls.Button;
            if (this.minimizeButton != null)
            {
                this.minimizeButton.Click += this.MinimizeClick;
            }

            this.maximizeButton = this.Template.FindName("PART_Max", this) as System.Windows.Controls.Button;
            if (this.maximizeButton != null)
            {
                this.maximizeButton.Click += this.MaximiseClick;
            }

            this.restoreButton = this.Template.FindName("PART_Restore", this) as System.Windows.Controls.Button;
            if (this.restoreButton != null)
            {
                this.restoreButton.Click += this.RestoreClick;
            }

            this.closeButton = this.GetTemplateChild("PART_Close") as System.Windows.Controls.Button;
            if (this.closeButton != null)
            {
                this.closeButton.Click += this.CloseClick;
            }
        }

        /// <inheritdoc />
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseRightButtonDown(e);

            WindowSteeringHelper.ShowSystemMenu(this, e);
        }

        private void MinimizeClick(object sender, RoutedEventArgs e)
        {
            var parentWindow = this.GetParentWindow();
            if (parentWindow != null)
            {
#pragma warning disable 618
                ControlzEx.Windows.Shell.SystemCommands.MinimizeWindow(parentWindow);
#pragma warning restore 618
            }
        }

        private void MaximiseClick(object sender, RoutedEventArgs e)
        {
            var parentWindow = this.GetParentWindow();
            if (parentWindow != null)
            {
#pragma warning disable 618
                ControlzEx.Windows.Shell.SystemCommands.MaximizeWindow(parentWindow);
#pragma warning restore 618
            }
        }

        private void RestoreClick(object sender, RoutedEventArgs e)
        {
            var parentWindow = this.GetParentWindow();
            if (parentWindow != null)
            {
#pragma warning disable 618
                ControlzEx.Windows.Shell.SystemCommands.RestoreWindow(parentWindow);
#pragma warning restore 618
            }
        }

        private void CloseClick(object sender, RoutedEventArgs e)
        {
            var parentWindow = this.GetParentWindow();

            if (parentWindow != null)
            {
#pragma warning disable 618
                ControlzEx.Windows.Shell.SystemCommands.CloseWindow(parentWindow);
#pragma warning restore 618
            }
        }

        private Window GetParentWindow()
        {
            var window = Window.GetWindow(this);

            if (window != null)
            {
                return window;
            }

            var parent = VisualTreeHelper.GetParent(this);
            Window parentWindow = null;

            while (parent != null
                && (parentWindow = parent as Window) == null)
            {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return parentWindow;
        }
    }
}