// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System.Windows;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents the container for the <see cref="StartScreenTabControl"/>.
    /// </summary>
    public class StartScreen : Backstage
    {
        private bool previousTitleBarIsCollapsed;

        /// <summary>
        /// Indicates whether the <see cref="StartScreen"/> has aleaady been shown or not.
        /// </summary>
        public bool Shown
        {
            get { return (bool)this.GetValue(ShownProperty); }
            set { this.SetValue(ShownProperty, value); }
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for <see cref="Shown"/>.  
        /// </summary>
        public static readonly DependencyProperty ShownProperty =
            DependencyProperty.Register(nameof(Shown), typeof(bool), typeof(StartScreen), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null));

        static StartScreen()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StartScreen), new FrameworkPropertyMetadata(typeof(StartScreen)));
        }

        /// <summary>
        /// Shows the <see cref="StartScreen"/>.
        /// </summary>
        protected override bool Show()
        {
            var parentRibbon = GetParentRibbon(this);

            if (parentRibbon?.TitleBar != null)
            {
                this.previousTitleBarIsCollapsed = parentRibbon.TitleBar.IsCollapsed;
                parentRibbon.TitleBar.IsCollapsed = true;
            }

            if (this.Shown)
            {
                return false;
            }

            return this.Shown = base.Show();
        }

        /// <summary>
        /// Hides the <see cref="StartScreen"/>.
        /// </summary>
        protected override void Hide()
        {
            base.Hide();

            var parentRibbon = GetParentRibbon(this);
            if (parentRibbon?.TitleBar != null)
            {
                parentRibbon.TitleBar.IsCollapsed = this.previousTitleBarIsCollapsed;
            }
        }
    }
}