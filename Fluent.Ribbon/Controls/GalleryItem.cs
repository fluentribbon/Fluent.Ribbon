using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

// ReSharper disable once CheckNamespace
namespace Fluent
{
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

        /// <summary>
        /// Gets or sets KeyTip for element.
        /// </summary>
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


        #region Command

        private bool currentCanExecute = true;

        /// <summary>
        /// Gets or sets the command to invoke when this button is pressed. This is a dependency property.
        /// </summary>
        [Category("Action"), Localizability(LocalizationCategory.NeverLocalize), Bindable(true)]
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

        /// <summary>
        /// Gets or sets the parameter to pass to the System.Windows.Controls.Primitives.ButtonBase.Command property. This is a dependency property.
        /// </summary>
        [Bindable(true), Localizability(LocalizationCategory.NeverLocalize), Category("Action")]
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

        /// <summary>
        /// Gets or sets the element on which to raise the specified command. This is a dependency property.
        /// </summary>
        [Bindable(true), Category("Action")]
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
        [Bindable(true), Category("Action")]
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
        [Bindable(true), Category("Action")]
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
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as GalleryItem;
            if (control == null)
            {
                return;
            }

            var oldCommand = e.OldValue as ICommand;
            if (oldCommand != null)
            {
                oldCommand.CanExecuteChanged -= control.OnCommandCanExecuteChanged;
            }

            var newCommand = e.NewValue as ICommand;
            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += control.OnCommandCanExecuteChanged;
            }

            control.UpdateCanExecute();
        }
        /// <summary>
        /// Handles Command CanExecute changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Gets a value that becomes the return 
        /// value of IsEnabled in derived classes. 
        /// </summary>
        /// <returns>
        /// true if the element is enabled; otherwise, false.
        /// </returns>
        protected override bool IsEnabledCore
        {
            get
            {
                return base.IsEnabledCore && (this.currentCanExecute || this.Command == null);
            }
        }

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
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GalleryItem));

        /// <summary>
        /// Raises click event
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1030")]
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
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static GalleryItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GalleryItem), new FrameworkPropertyMetadata(typeof(GalleryItem)));
            IsSelectedProperty.AddOwner(typeof(GalleryItem), new FrameworkPropertyMetadata(BooleanBoxes.FalseBox, FrameworkPropertyMetadataOptions.None, OnIsSelectedPropertyChanged));
        }

        private static void OnIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                ((GalleryItem)d).BringIntoView();

                var parentSelector = ItemsControl.ItemsControlFromItemContainer(d) as Selector;

                if (parentSelector != null)
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

        /// <summary>
        /// Provides class handling for the System.Windows.UIElement.MouseLeftButtonDown routed event that occurs 
        /// when the left mouse button is pressed while the mouse pointer is over this control.
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            this.IsPressed = true;
            Mouse.Capture(this);
            e.Handled = true;
        }

        /// <summary>
        /// Invoked when an unhandled <see cref="E:System.Windows.Input.Mouse.LostMouseCapture"/> attached event reaches an element in its route that is derived from this class. Implement this method to add class handling for this event. 
        /// </summary>
        /// <param name="e">The <see cref="T:System.Windows.Input.MouseEventArgs"/> that contains event data.</param>
        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            base.OnLostMouseCapture(e);

            this.IsPressed = false;
        }

        /// <summary>
        /// Provides class handling for the System.Windows.UIElement.MouseLeftButtonUp routed event that occurs 
        /// when the left mouse button is released while the mouse pointer is over this control.
        /// </summary>
        /// <param name="e">The event data.</param>
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

        /// <summary>
        /// Called when the mouse enters a <see cref="T:System.Windows.Controls.ListBoxItem"/>. 
        /// </summary>
        /// <param name="e">The event data.</param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            CommandHelper.Execute(this.PreviewCommand, this, null);
        }

        /// <summary>
        /// Called when the mouse leaves a <see cref="T:System.Windows.Controls.ListBoxItem"/>. 
        /// </summary>
        /// <param name="e">The event data.</param>
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
            PopupService.RaiseDismissPopupEvent(sender, DismissPopupMode.Always);

            this.ExecuteCommand();
            this.IsSelected = true;
            e.Handled = true;
        }

        #endregion

        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        public void OnKeyTipPressed()
        {
            this.RaiseClick();
        }

        /// <summary>
        /// Handles back navigation with KeyTips
        /// </summary>
        public void OnKeyTipBack()
        {
        }
    }
}