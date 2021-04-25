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
        /// <summary>
        /// Indicates whether the <see cref="StartScreen"/> has aleaady been shown or not.
        /// </summary>
        public bool Shown
        {
            get => (bool)this.GetValue(ShownProperty);
            set => this.SetValue(ShownProperty, value ? BooleanBoxes.TrueBox : BooleanBoxes.FalseBox);
        }

        /// <summary>Identifies the <see cref="Shown"/> dependency property.</summary>
        public static readonly DependencyProperty ShownProperty =
            DependencyProperty.Register(nameof(Shown), typeof(bool), typeof(StartScreen), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null));

        static StartScreen()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(StartScreen), new FrameworkPropertyMetadata(typeof(StartScreen)));

            VisibilityProperty.OverrideMetadata(typeof(StartScreen), new PropertyMetadata(OnVisibilityChanged));
        }

        private static void OnVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((StartScreen)d).UpdateIsTitleBarCollapsed();
        }

        // This is required for scenarios like in #445 where the start screen is shown, but never hidden through Hide() but made hidden by changing just it's visibility.
        private void UpdateIsTitleBarCollapsed()
        {
            if (this.IsOpen == false)
            {
                return;
            }

            var parentRibbon = GetParentRibbon(this);

            parentRibbon?.TitleBar?.SetCurrentValue(RibbonTitleBar.IsCollapsedProperty, this.Visibility == Visibility.Visible
                ? BooleanBoxes.TrueBox
                : BooleanBoxes.FalseBox);
        }

        /// <summary>
        /// Shows the <see cref="StartScreen"/>.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the <see cref="StartScreen"/> was made visible.
        /// <c>false</c> if the <see cref="StartScreen"/> was previously shown and was not made visible during this call.
        /// </returns>
        protected override bool Show()
        {
            if (this.Shown)
            {
                return false;
            }

            this.Shown = base.Show();

            if (this.Shown)
            {
                var parentRibbon = GetParentRibbon(this);

                parentRibbon?.TitleBar?.SetCurrentValue(RibbonTitleBar.IsCollapsedProperty, BooleanBoxes.TrueBox);
            }

            return this.Shown;
        }

        /// <summary>
        /// Hides the <see cref="StartScreen" />.
        /// </summary>
        protected override void Hide()
        {
            var wasShown = this.Shown;

            base.Hide();

            if (wasShown)
            {
                var parentRibbon = GetParentRibbon(this);

                parentRibbon?.TitleBar?.ClearValue(RibbonTitleBar.IsCollapsedProperty);
            }
        }
    }
}