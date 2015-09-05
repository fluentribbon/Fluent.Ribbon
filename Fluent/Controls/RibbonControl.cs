﻿#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright � Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Fluent
{
    using Fluent.Internal;
    using Fluent.Metro.Native;

    /// <summary>
    /// Represent base class for Fluent controls
    /// </summary>
    public abstract class RibbonControl : Control, ICommandSource, IQuickAccessItemProvider, IRibbonControl
    {
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
        public static readonly DependencyProperty KeyTipProperty = Fluent.KeyTip.KeysProperty.AddOwner(typeof(RibbonControl));

        #endregion

        #region Header

        /// <summary>
        /// Gets or sets element header
        /// </summary>
        public object Header
        {
            get { return (string)this.GetValue(HeaderProperty); }
            set { this.SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Header.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(RibbonControl), new UIPropertyMetadata(null));

        #endregion

        #region Icon

        /// <summary>
        /// Gets or sets Icon for the element
        /// </summary>
        public object Icon
        {
            get { return this.GetValue(IconProperty); }
            set { this.SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(object), typeof(RibbonControl), new UIPropertyMetadata(null, OnIconChanged));

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonControl element = d as RibbonControl;
            FrameworkElement oldElement = e.OldValue as FrameworkElement;
            if (oldElement != null) element.RemoveLogicalChild(oldElement);
            FrameworkElement newElement = e.NewValue as FrameworkElement;
            if (newElement != null) element.AddLogicalChild(newElement);
        }

        #endregion

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
        public static readonly DependencyProperty CommandParameterProperty = ButtonBase.CommandParameterProperty.AddOwner(typeof(RibbonControl), new FrameworkPropertyMetadata(null));
        /// <summary>
        /// Identifies the routed Command dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = ButtonBase.CommandProperty.AddOwner(typeof(RibbonControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCommandChanged)));

        /// <summary>
        /// Identifies the CommandTarget dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandTargetProperty = ButtonBase.CommandTargetProperty.AddOwner(typeof(RibbonControl), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Handles Command changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RibbonControl;

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

                var routedUiCommand = e.NewValue as RoutedUICommand;
                if (routedUiCommand != null 
                    && control.Header == null)
                {
                    control.Header = routedUiCommand.Text;
                }
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

        /// <summary>
        /// Execute command
        /// </summary>
        protected void ExecuteCommand()
        {
            CommandHelper.Execute(this.Command, this.CommandParameter, this.CommandTarget);
        }

        /// <summary>
        /// Determines whether the Command can be executed
        /// </summary>
        /// <returns>Returns Command CanExecute</returns>
        protected bool CanExecuteCommand()
        {
            return CommandHelper.CanExecute(this.Command, this.CommandParameter, this.CommandTarget);
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
                return (base.IsEnabledCore && (this.currentCanExecute || this.Command == null));
            }
        }

        #endregion

        #region Size

        /// <summary>
        /// Gets or sets Size for the element.
        /// </summary>
        public RibbonControlSize Size
        {
            get { return (RibbonControlSize)this.GetValue(SizeProperty); }
            set { this.SetValue(SizeProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Size.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeProperty = RibbonProperties.SizeProperty.AddOwner(typeof(RibbonControl));

        #endregion

        #region SizeDefinition

        /// <summary>
        /// Gets or sets SizeDefinition for element.
        /// </summary>
        public RibbonControlSizeDefinition SizeDefinition
        {
            get { return (RibbonControlSizeDefinition)this.GetValue(SizeDefinitionProperty); }
            set { this.SetValue(SizeDefinitionProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for SizeDefinition.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeDefinitionProperty = RibbonProperties.SizeDefinitionProperty.AddOwner(typeof(RibbonControl));

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static RibbonControl()
        {
            Type type = typeof(RibbonControl);
            ContextMenuService.Attach(type);
            ToolTipService.Attach(type);
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        protected RibbonControl()
        {
            ContextMenuService.Coerce(this);
        }

        #endregion

        #region QuickAccess

        /// <summary>
        /// Gets control which represents shortcut item.
        /// This item MUST be syncronized with the original 
        /// and send command to original one control.
        /// </summary>
        /// <returns>Control which represents shortcut item</returns>
        public abstract FrameworkElement CreateQuickAccessItem();

        /// <summary>
        /// Binds default properties of control to quick access element
        /// </summary>
        /// <param name="element">Toolbar item</param>
        /// <param name="source">Source item</param>
        public static void BindQuickAccessItem(FrameworkElement source, FrameworkElement element)
        {
            Bind(source, element, "DataContext", DataContextProperty, BindingMode.OneWay);

            if (source is ICommandSource)
            {
                if (source is MenuItem)
                {
                    Bind(source, element, "CommandParameter", ButtonBase.CommandParameterProperty, BindingMode.OneWay);
                    Bind(source, element, "CommandTarget", System.Windows.Controls.MenuItem.CommandTargetProperty, BindingMode.OneWay);
                    Bind(source, element, "Command", System.Windows.Controls.MenuItem.CommandProperty, BindingMode.OneWay);
                }
                else
                {
                    Bind(source, element, "CommandParameter", ButtonBase.CommandParameterProperty, BindingMode.OneWay);
                    Bind(source, element, "CommandTarget", ButtonBase.CommandTargetProperty, BindingMode.OneWay);
                    Bind(source, element, "Command", ButtonBase.CommandProperty, BindingMode.OneWay);
                }
            }

            Bind(source, element, "ToolTip", ToolTipProperty, BindingMode.OneWay);

            Bind(source, element, "FontFamily", FontFamilyProperty, BindingMode.OneWay);
            Bind(source, element, "FontSize", FontSizeProperty, BindingMode.OneWay);
            Bind(source, element, "FontStretch", FontStretchProperty, BindingMode.OneWay);
            Bind(source, element, "FontStyle", FontStyleProperty, BindingMode.OneWay);
            Bind(source, element, "FontWeight", FontWeightProperty, BindingMode.OneWay);

            Bind(source, element, "Foreground", ForegroundProperty, BindingMode.OneWay);
            Bind(source, element, "IsEnabled", IsEnabledProperty, BindingMode.OneWay);
            Bind(source, element, "Opacity", OpacityProperty, BindingMode.OneWay);
            Bind(source, element, "SnapsToDevicePixels", SnapsToDevicePixelsProperty, BindingMode.OneWay);

            Bind(source, element, new PropertyPath(FocusManager.IsFocusScopeProperty), FocusManager.IsFocusScopeProperty, BindingMode.OneWay);

            var sourceControl = source as IRibbonControl;
            if (sourceControl != null)
            {
                if (sourceControl.Icon != null)
                {
                    var iconVisual = sourceControl.Icon as Visual;
                    if (iconVisual != null)
                    {
                        var rect = new Rectangle();
                        rect.Width = 16;
                        rect.Height = 16;
                        rect.Fill = new VisualBrush(iconVisual);
                        ((IRibbonControl) element).Icon = rect;
                    }
                    else
                    {
                        Bind(source, element, "Icon", IconProperty, BindingMode.OneWay);
                    }
                }

                if (sourceControl.Header != null)
                {
                    Bind(source, element, "Header", HeaderProperty, BindingMode.OneWay);
                }
            }

            RibbonProperties.SetSize(element, RibbonControlSize.Small);
        }

        /// <summary>
        /// Gets or sets whether control can be added to quick access toolbar
        /// </summary>
        public bool CanAddToQuickAccessToolBar
        {
            get { return (bool)this.GetValue(CanAddToQuickAccessToolBarProperty); }
            set { this.SetValue(CanAddToQuickAccessToolBarProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for CanAddToQuickAccessToolBar.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty CanAddToQuickAccessToolBarProperty =
            DependencyProperty.Register("CanAddToQuickAccessToolBar", typeof(bool), typeof(RibbonControl), new UIPropertyMetadata(true, OnCanAddToQuickAccessToolbarChanged));

        /// <summary>
        /// Occurs then CanAddToQuickAccessToolBar property changed
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        public static void OnCanAddToQuickAccessToolbarChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.CoerceValue(ContextMenuProperty);
        }

        #endregion

        #region Binding

        internal static void Bind(object source, FrameworkElement target, string path, DependencyProperty property, BindingMode mode)
        {
            Bind(source, target, new PropertyPath(path), property, mode);
        }

        internal static void Bind(object source, FrameworkElement target, PropertyPath path, DependencyProperty property, BindingMode mode)
        {
            var binding = new Binding
            {
                Path = path,
                Source = source,
                Mode = mode
            };
            target.SetBinding(property, binding);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        public virtual void OnKeyTipPressed()
        {
        }

        /// <summary>
        /// Handles back navigation with KeyTips
        /// </summary>
        public virtual void OnKeyTipBack()
        {
        }

        #endregion

        #region StaticMethods

        /// <summary>
        /// Returns screen workarea in witch control is placed
        /// </summary>
        /// <param name="control">Control</param>
        /// <returns>Workarea in witch control is placed</returns>
        public static Rect GetControlWorkArea(FrameworkElement control)
        {
            var tabItemPos = control.PointToScreen(new Point(0, 0));
            var tabItemRect = new RECT();
            tabItemRect.left = (int)tabItemPos.X;
            tabItemRect.top = (int)tabItemPos.Y;
            tabItemRect.right = (int)tabItemPos.X + (int)control.ActualWidth;
            tabItemRect.bottom = (int)tabItemPos.Y + (int)control.ActualHeight;
            const uint MONITOR_DEFAULTTONEAREST = 0x00000002;
            var monitor = NativeMethods.MonitorFromRect(ref tabItemRect, MONITOR_DEFAULTTONEAREST);
            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MONITORINFO();
                monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
                UnsafeNativeMethods.GetMonitorInfo(monitor, monitorInfo);
                return new Rect(monitorInfo.rcWork.left, monitorInfo.rcWork.top, monitorInfo.rcWork.right - monitorInfo.rcWork.left, monitorInfo.rcWork.bottom - monitorInfo.rcWork.top);
            }
            return new Rect();
        }

        /// <summary>
        /// Returns monitor in witch control is placed
        /// </summary>
        /// <param name="control">Control</param>
        /// <returns>Workarea in witch control is placed</returns>
        public static Rect GetControlMonitor(FrameworkElement control)
        {
            var tabItemPos = control.PointToScreen(new Point(0, 0));
            var tabItemRect = new RECT();
            tabItemRect.left = (int)tabItemPos.X;
            tabItemRect.top = (int)tabItemPos.Y;
            tabItemRect.right = (int)tabItemPos.X + (int)control.ActualWidth;
            tabItemRect.bottom = (int)tabItemPos.Y + (int)control.ActualHeight;
            const uint MONITOR_DEFAULTTONEAREST = 0x00000002;
            var monitor = NativeMethods.MonitorFromRect(ref tabItemRect, MONITOR_DEFAULTTONEAREST);
            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MONITORINFO();
                monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
                UnsafeNativeMethods.GetMonitorInfo(monitor, monitorInfo);
                return new Rect(monitorInfo.rcMonitor.left, monitorInfo.rcMonitor.top, monitorInfo.rcMonitor.right - monitorInfo.rcMonitor.left, monitorInfo.rcMonitor.bottom - monitorInfo.rcMonitor.top);
            }
            return new Rect();
        }

        #endregion
    }
}