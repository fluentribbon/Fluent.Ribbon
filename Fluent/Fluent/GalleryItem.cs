#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright (c) Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Fluent
{
    /// <summary>
    /// Represents gallery item
    /// </summary>
    public class GalleryItem : ListBoxItem, IKeyTipedControl
    {
        #region Fields


        #endregion

        #region Properties

        /// <summary>
        /// Gets a value that indicates whether a Button is currently activated. 
        /// This is a dependency property.
        /// </summary>
        public bool IsPressed
        {
            get { return (bool)GetValue(IsPressedProperty); }
            private set { SetValue(IsPressedPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey IsPressedPropertyKey =
            DependencyProperty.RegisterReadOnly("IsPressed", typeof(bool),
            typeof(GalleryItem), new UIPropertyMetadata(false));

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
            get { return (string)GetValue(GroupProperty); }
            set { SetValue(GroupProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Group.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupProperty =
            DependencyProperty.Register("Group", typeof(string),
            typeof(GalleryItem), new UIPropertyMetadata(null));


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
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
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
                return GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
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
                return (IInputElement)GetValue(CommandTargetProperty);
            }
            set
            {
                SetValue(CommandTargetProperty, value);
            }
        }

        /// <summary>
        /// Identifies the CommandParameter dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(GalleryItem), new FrameworkPropertyMetadata(null));
        /// <summary>
        /// Identifies the routed Command dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(GalleryItem), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCommandChanged)));

        /// <summary>
        /// Identifies the CommandTarget dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(GalleryItem), new FrameworkPropertyMetadata(null));

        // Keep a copy of the handler so it doesn't get garbage collected.
        [SuppressMessage("Microsoft.Performance", "CA1823")]
        EventHandler canExecuteChangedHandler;

        /// <summary>
        /// Handles Command changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            GalleryItem control = d as GalleryItem;
            EventHandler handler = control.OnCommandCanExecuteChanged;
            if (e.OldValue != null)
            {
                (e.OldValue as ICommand).CanExecuteChanged -= handler;
            }
            if (e.NewValue != null)
            {
                handler = new EventHandler(control.OnCommandCanExecuteChanged);
                control.canExecuteChangedHandler = handler;
                (e.NewValue as ICommand).CanExecuteChanged += handler;

                //RoutedUICommand cmd = e.NewValue as RoutedUICommand;
                //if ((cmd != null) && (control.Content==null)) control.Content = cmd.Text;
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
            UpdateCanExecute();
        }

        private void UpdateCanExecute()
        {
            bool canExecute = Command != null && CanExecuteCommand();
            if (currentCanExecute != canExecute)
            {
                currentCanExecute = canExecute;
                CoerceValue(IsEnabledProperty);
            }
        }

        /// <summary>
        /// Execute command
        /// </summary>
        protected void ExecuteCommand()
        {
            ICommand command = Command;
            if (command != null)
            {
                object commandParameter = CommandParameter;
                RoutedCommand routedCommand = command as RoutedCommand;
                if (routedCommand != null)
                {
                    if (routedCommand.CanExecute(commandParameter, CommandTarget))
                    {
                        routedCommand.Execute(commandParameter, CommandTarget);
                    }
                }
                else if (command.CanExecute(commandParameter))
                {
                    command.Execute(commandParameter);
                }
            }
        }

        /// <summary>
        /// Determines whether the Command can be executed
        /// </summary>
        /// <returns>Returns Command CanExecute</returns>
        protected bool CanExecuteCommand()
        {
            ICommand command = Command;
            if (command == null)
            {
                return false;
            }
            object commandParameter = CommandParameter;
            RoutedCommand routedCommand = command as RoutedCommand;
            if (routedCommand == null)
            {
                return command.CanExecute(commandParameter);
            }
            return routedCommand.CanExecute(commandParameter, CommandTarget);
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
                return (base.IsEnabledCore && (currentCanExecute || Command == null));
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
                AddHandler(ClickEvent, value);
            }
            remove
            {
                RemoveHandler(ClickEvent, value);
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
            RaiseEvent(new RoutedEventArgs(ClickEvent, this));
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
            IsSelectedProperty.AddOwner(typeof(GalleryItem), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.None, OnIsSelectedPropertyChanged));
            StyleProperty.OverrideMetadata(typeof(GalleryItem), new FrameworkPropertyMetadata(null, new CoerceValueCallback(OnCoerceStyle)));
        }

        // Coerce object style
        static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                basevalue = (d as FrameworkElement).TryFindResource(typeof(GalleryItem));
            }

            return basevalue;
        }

        static void OnIsSelectedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                ((GalleryItem)d).BringIntoView();
                Selector parentSelector = ItemsControl.ItemsControlFromItemContainer(((GalleryItem)d)) as Selector;
                if (parentSelector != null) parentSelector.SelectedItem = parentSelector.ItemContainerGenerator.ItemFromContainer(((GalleryItem)d));
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public GalleryItem()
        {
            Click += OnClick;
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
            if (Mouse.Captured == this)
            {
                Mouse.Capture(null);
            }

            var position = Mouse.PrimaryDevice.GetPosition(this);

            if ((position.X >= 0.0 && position.X <= this.ActualWidth)
                && (position.Y >= 0.0 && position.Y <= this.ActualHeight)
                && e.ClickCount == 1)
            {
                this.RaiseClick();
                e.Handled = true;
            }

            e.Handled = true;
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
            ExecuteCommand();
            IsSelected = true;
            PopupService.RaiseDismissPopupEvent(sender, DismissPopupMode.Always);
            e.Handled = true;
        }

        #endregion

        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        public void OnKeyTipPressed()
        {
            RaiseClick();
        }

        /// <summary>
        /// Handles back navigation with KeyTips
        /// </summary>
        public void OnKeyTipBack()
        {
        }
    }
}
