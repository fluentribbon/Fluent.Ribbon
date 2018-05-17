// ReSharper disable once CheckNamespace
namespace Fluent
{
    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using Fluent.Extensions;
    using Fluent.Internal;
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represents gallery item
    /// </summary>
    public class GalleryItem : ListBoxItem, IKeyTipedControl, ICommandSource
    {
        #region Properties

        #region KeyTip

        /// <inheritdoc />
        public string KeyTip
        {
            get { return (string)this.GetValue(KeyTipProperty); }
            set { this.SetValue(KeyTipProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Keys.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(typeof(GalleryItem));

        #endregion

        /// <summary>
        /// Gets a value that indicates whether a Button is currently activated.
        /// This is a dependency property.
        /// </summary>
        public bool IsPressed
        {
            get { return (bool)this.GetValue(IsPressedProperty); }
            private set { this.SetValue(IsPressedPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey IsPressedPropertyKey =
            DependencyProperty.RegisterReadOnly(nameof(IsPressed), typeof(bool),
            typeof(GalleryItem), new PropertyMetadata(BooleanBoxes.FalseBox));

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsPressed.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsPressedProperty = IsPressedPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets or sets GalleryItem group
        /// </summary>
        public string Group
        {
            get { return (string)this.GetValue(GroupProperty); }
            set { this.SetValue(GroupProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Group.
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupProperty =
            DependencyProperty.Register(nameof(Group), typeof(string),
            typeof(GalleryItem), new PropertyMetadata());

        /// <summary>
        /// Gets or sets whether ribbon control click must close backstage or popup.
        /// </summary>
        public bool IsDefinitive
        {
            get { return (bool)this.GetValue(IsDefinitiveProperty); }
            set { this.SetValue(IsDefinitiveProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsDefinitive.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsDefinitiveProperty =
            DependencyProperty.Register(nameof(IsDefinitive), typeof(bool), typeof(GalleryItem), new PropertyMetadata(BooleanBoxes.TrueBox));

        #region Command

        private bool currentCanExecute = true;

        /// <inheritdoc />
        [Category("Action")]
        [Localizability(LocalizationCategory.NeverLocalize)]
        [Bindable(true)]
        public ICommand Command
        {
            get
            {
                return (ICommand)this.GetValue(CommandProperty);
            }

            set
            {
                this.SetValue(CommandProperty, value);
            }
        }

        /// <inheritdoc />
        [Bindable(true)]
        [Localizability(LocalizationCategory.NeverLocalize)]
        [Category("Action")]
        public object CommandParameter
        {
            get
            {
                return this.GetValue(CommandParameterProperty);
            }

            set
            {
                this.SetValue(CommandParameterProperty, value);
            }
        }

        /// <inheritdoc />
        [Bindable(true)]
        [Category("Action")]
        public IInputElement CommandTarget
        {
            get
            {
                return (IInputElement)this.GetValue(CommandTargetProperty);
            }

            set
            {
                this.SetValue(CommandTargetProperty, value);
            }
        }

        /// <summary>
        /// Identifies the CommandParameter dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(GalleryItem), new PropertyMetadata());

        /// <summary>
        /// Identifies the routed Command dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(GalleryItem), new PropertyMetadata(OnCommandChanged));

        /// <summary>
        /// Identifies the CommandTarget dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register(nameof(CommandTarget), typeof(IInputElement), typeof(GalleryItem), new PropertyMetadata());

        /// <summary>
        /// Gets or sets the command to invoke when mouse enters or leaves this button. The commandparameter will be the <see cref="GalleryItem"/> instance.
        /// This is a dependency property.
        /// </summary>
        [Bindable(true)]
        [Category("Action")]
        public ICommand PreviewCommand
        {
            get { return (ICommand)this.GetValue(PreviewCommandProperty); }
            set { this.SetValue(PreviewCommandProperty, value); }
        }

        /// <summary>
        /// Identifies the PreviewCommand dependency property.
        /// </summary>
        public static readonly DependencyProperty PreviewCommandProperty =
            DependencyProperty.Register(nameof(PreviewCommand), typeof(ICommand), typeof(GalleryItem), new PropertyMetadata());

        /// <summary>
        /// Gets or sets the command to invoke when mouse enters or leaves this button. The commandparameter will be the <see cref="GalleryItem"/> instance.
        /// This is a dependency property.
        /// </summary>
        [Bindable(true)]
        [Category("Action")]
        public ICommand CancelPreviewCommand
        {
            get { return (ICommand)this.GetValue(CancelPreviewCommandProperty); }
            set { this.SetValue(CancelPreviewCommandProperty, value); }
        }

        /// <summary>
        /// Identifies the PreviewCommand dependency property.
        /// </summary>
        public static readonly DependencyProperty CancelPreviewCommandProperty =
            DependencyProperty.Register(nameof(CancelPreviewCommand), typeof(ICommand), typeof(GalleryItem), new PropertyMetadata());

        /// <summary>
        /// Handles Command changed
        /// </summary>
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as GalleryItem;
            if (control == null)
            {
                return;
            }

            if (e.OldValue is ICommand oldCommand)
            {
                oldCommand.CanExecuteChanged -= control.OnCommandCanExecuteChanged;
            }

            if (e.NewValue is ICommand newCommand)
            {
                newCommand.CanExecuteChanged += control.OnCommandCanExecuteChanged;
            }

            control.UpdateCanExecute();
        }

        /// <summary>
        /// Handles Command CanExecute changed
        /// </summary>
        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            this.UpdateCanExecute();
        }

        private void UpdateCanExecute()
        {
            var canExecute = this.Command != null
                && this.CanExecuteCommand();

            if (this.currentCanExecute != canExecute)
            {
                this.currentCanExecute = canExecute;
                this.CoerceValue(IsEnabledProperty);
            }
        }

        #endregion

        #region IsEnabled

        /// <inheritdoc />
        protected override bool IsEnabledCore => base.IsEnabledCore && (this.currentCanExecute || this.Command == null);

        #endregion

        #endregion

        #region Events

        #region Click

        /// <summary>
        /// Occurs when a RibbonControl is clicked.
        /// </summary>
        [Category("Behavior")]
        public event RoutedEventHandler Click
        {
            add
            {
                this.AddHandler(ClickEvent, value);
            }

            remove
            {
                this.RemoveHandler(ClickEvent, value);
            }
        }

        /// <summary>
        /// Identifies the RibbonControl.Click routed event.
        /// </summary>
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent(nameof(Click), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GalleryItem));

        /// <summary>
        /// Raises click event
        /// </summary>
        public void RaiseClick()
        {
            this.RaiseEvent(new RoutedEventArgs(ClickEvent, this));
        }

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static GalleryItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GalleryItem), new FrameworkPropertyMetadata(typeof(GalleryItem)));
            IsSelectedProperty.AddOwner(typeof(GalleryItem), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.None, OnIsSelectedChanged));
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                ((GalleryItem)d).BringIntoView();

                if (ItemsControl.ItemsControlFromItemContainer(d) is Selector parentSelector)
                {
                    var item = parentSelector.ItemContainerGenerator.ItemFromContainer(d);

                    if (ReferenceEquals(parentSelector.SelectedItem, item) == false)
                    {
                        parentSelector.SelectedItem = item;
                    }
                }
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public GalleryItem()
        {
            this.Click += this.OnClick;
        }

        #endregion

        #region Overrides

        /// <inheritdoc />
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this.IsPressed = true;
            Mouse.Capture(this);
            e.Handled = true;
        }

        /// <inheritdoc />
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            base.OnLostMouseCapture(e);

            this.IsPressed = false;
        }

        /// <inheritdoc />
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            this.IsPressed = false;
            if (ReferenceEquals(Mouse.Captured, this))
            {
                Mouse.Capture(null);
            }

            var position = Mouse.PrimaryDevice.GetPosition(this);

            if (position.X >= 0.0 && position.X <= this.ActualWidth && position.Y >= 0.0 && position.Y <= this.ActualHeight && e.ClickCount == 1)
            {
                this.RaiseClick();
                e.Handled = true;
            }

            e.Handled = true;
        }

        /// <inheritdoc />
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            CommandHelper.Execute(this.PreviewCommand, this, null);
        }

        /// <inheritdoc />
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            CommandHelper.Execute(this.CancelPreviewCommand, this, null);
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Handles click event
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">The event data</param>
        protected virtual void OnClick(object sender, RoutedEventArgs e)
        {
            // Close popup on click
            if (this.IsDefinitive)
            {
                PopupService.RaiseDismissPopupEvent(sender, DismissPopupMode.Always);
            }

            this.ExecuteCommand();
            this.IsSelected = true;
            e.Handled = true;
        }

        #endregion

        /// <inheritdoc />
        public KeyTipPressedResult OnKeyTipPressed()
        {
            this.RaiseClick();

            return KeyTipPressedResult.Empty;
        }

        /// <inheritdoc />
        public void OnKeyTipBack()
        {
        }
    }
}
