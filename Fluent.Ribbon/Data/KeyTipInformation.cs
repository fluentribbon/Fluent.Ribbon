// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.Windows;
    using System.Windows.Data;

    /// <summary>
    /// Container class for KeyTip informations
    /// </summary>
    public class KeyTipInformation
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="keys">The keys to be used for <see cref="KeyTip"/>.</param>
        /// <param name="associatedElement">The element to which this instance belongs to.</param>
        /// <param name="hide">Defines if the created <see cref="KeyTip"/> should be hidden or not.</param>
        public KeyTipInformation(string keys, FrameworkElement associatedElement, bool hide)
        {
            if (string.IsNullOrEmpty(keys))
            {
                throw new ArgumentNullException(nameof(keys));
            }

            if (associatedElement == null)
            {
                throw new ArgumentNullException(nameof(associatedElement));
            }

            this.Keys = keys;
            this.AssociatedElement = associatedElement;
            this.VisualTarget = this.AssociatedElement;

            this.DefaultVisibility = hide
                                         ? Visibility.Collapsed
                                         : Visibility.Visible;

            this.KeyTip = new KeyTip
                          {
                              Content = keys,
                              Visibility = this.DefaultVisibility
                          };

            // Bind IsEnabled property
            var binding = new Binding(nameof(this.AssociatedElement.IsEnabled))
                          {
                              Source = this.AssociatedElement,
                              Mode = BindingMode.OneWay
                          };
            this.KeyTip.SetBinding(UIElement.IsEnabledProperty, binding);
        }

        /// <summary>
        /// Gets <see cref="Fluent.KeyTip.KeysProperty"/>
        /// </summary>
        public string Keys { get; }

        /// <summary>
        /// Gets the element this instance belongs to.
        /// </summary>
        public FrameworkElement AssociatedElement { get; }

        /// <summary>
        /// Gets or sets the element which acts as the visual target.
        /// </summary>
        public FrameworkElement VisualTarget { get; set; }

        /// <summary>
        /// Gets the initial visibility.
        /// </summary>
        public Visibility DefaultVisibility { get; }

        /// <summary>
        /// Gets the <see cref="Fluent.KeyTip"/> for <see cref="AssociatedElement"/>.
        /// </summary>
        public KeyTip KeyTip { get; }

        /// <summary>
        /// Gets or sets the position of <see cref="KeyTip"/>.
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// Gets or sets the backed up value of <see cref="System.Windows.Visibility"/> of <see cref="KeyTip"/>
        /// </summary>
        public Visibility BackupVisibility { get; set; }

        /// <summary>
        /// Gets <see cref="UIElement.IsVisible" /> from <see cref="KeyTip"/>.
        /// </summary>
        public bool IsVisible => this.KeyTip.IsVisible;

        /// <summary>
        /// Gets or sets <see cref="UIElement.Visibility" /> from <see cref="KeyTip"/>.
        /// </summary>
        public Visibility Visibility
        {
            get { return this.KeyTip.Visibility; }
            set { this.KeyTip.Visibility = value; }
        }

        /// <summary>
        /// Gets <see cref="UIElement.IsEnabled" /> from <see cref="KeyTip"/>
        /// </summary>
        public bool IsEnabled => this.KeyTip.IsEnabled;
    }
}