#region Copyright and License Information
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
using System.Globalization;
using System.Net.Cache;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Fluent
{
    /// <summary>
    /// Converts string or ImageSource to Image control
    /// </summary>
    public class ObjectToImageConverter : IValueConverter
    {
        #region Implementation of IValueConverter

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value produced by the binding source.</param><param name="targetType">The type of the binding target property.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                var image = new Image
                    {
                        Source = new BitmapImage(new Uri(value as string, UriKind.RelativeOrAbsolute), new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore))
                    };
                return image;
            }

            var imageSource = value as ImageSource;

            if (imageSource != null)
            {
                var image = new Image
                    {
                        Source = imageSource
                    };
                return image;
            }
            return value;
        }

        /// <summary>
        /// Converts a value. 
        /// </summary>
        /// <returns>
        /// A converted value. If the method returns null, the valid null value is used.
        /// </returns>
        /// <param name="value">The value that is produced by the binding target.</param><param name="targetType">The type to convert to.</param><param name="parameter">The converter parameter to use.</param><param name="culture">The culture to use in the converter.</param>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
    /// <summary>
    /// Represent base class for Fluent controls
    /// </summary>
    public abstract class RibbonControl : Control, ICommandSource, IQuickAccessItemProvider, IRibbonControl
    {
        #region Size Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for Size.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeProperty = DependencyProperty.Register(
          "Size",
          typeof(RibbonControlSize),
          typeof(RibbonControl),
          new FrameworkPropertyMetadata(RibbonControlSize.Large,
              FrameworkPropertyMetadataOptions.AffectsArrange |
              FrameworkPropertyMetadataOptions.AffectsMeasure |
              FrameworkPropertyMetadataOptions.AffectsRender |
              FrameworkPropertyMetadataOptions.AffectsParentArrange |
              FrameworkPropertyMetadataOptions.AffectsParentMeasure,
              OnSizePropertyChanged)
        );

        // When the ControlSizeDefinition property changes we need to invalidate 
        // the parent chain measure so that the RibbonGroupsContainer can calculate 
        // the new size within the same MeasureOverride call.  This property
        // usually changes from RibbonGroupsContainer.MeasureOverride.
        private static void OnSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonControl ribbonControl = (RibbonControl)d;
            ribbonControl.OnSizePropertyChanged(
                (RibbonControlSize)e.OldValue,
                (RibbonControlSize)e.NewValue);
        }

        /// <summary>
        /// Gets or sets Size for the element
        /// </summary>
        public RibbonControlSize Size
        {
            get { return (RibbonControlSize)GetValue(SizeProperty); }
            set { SetValue(SizeProperty, value); }
        }

        #endregion

        #region SizeDefinition Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for SizeDefinition.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty SizeDefinitionProperty = DependencyProperty.Register(
          "SizeDefinition",
          typeof(string),
          typeof(RibbonControl), new FrameworkPropertyMetadata("Large, Middle, Small",
                                          FrameworkPropertyMetadataOptions.AffectsArrange |
                                          FrameworkPropertyMetadataOptions.AffectsMeasure |
                                          FrameworkPropertyMetadataOptions.AffectsRender |
                                          FrameworkPropertyMetadataOptions.AffectsParentArrange |
                                          FrameworkPropertyMetadataOptions.AffectsParentMeasure,
                                          OnSizeDefinitionPropertyChanged));

        /// <summary>
        /// Adds <paramref name="type"/> as owner to <see cref="SizeDefinitionProperty"/>
        /// </summary>
        /// <param name="type">The type to add as owner</param>
        /// <returns>The <see cref="DependencyProperty"/> returned from SizeDefinitionProperty.AddOwner</returns>
        public static DependencyProperty AttachSizeDefinition(Type type)
        {
            return SizeDefinitionProperty.AddOwner(type,
                                                    new FrameworkPropertyMetadata("Large, Middle, Small",
                                                                                FrameworkPropertyMetadataOptions
                                                                                    .AffectsArrange |
                                                                                FrameworkPropertyMetadataOptions
                                                                                    .AffectsMeasure |
                                                                                FrameworkPropertyMetadataOptions
                                                                                    .AffectsRender |
                                                                                FrameworkPropertyMetadataOptions
                                                                                    .AffectsParentArrange |
                                                                                FrameworkPropertyMetadataOptions
                                                                                    .AffectsParentMeasure,
                                                                                OnSizeDefinitionPropertyChanged));
        }

        // Handles SizeDefinitionProperty changes
        internal static void OnSizeDefinitionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Find parent group box
            var groupBox = FindParentRibbonGroupBox(d);
            var element = (UIElement)d;

            if (groupBox != null)
            {
                SetAppropriateSize(element, groupBox.State);
            }
            else
            {
                SetAppropriateSize(element, RibbonGroupBoxState.Large);
            }
        }

        // Finds parent group box
        [SuppressMessage("Microsoft.Performance", "CA1800")]
        internal static RibbonGroupBox FindParentRibbonGroupBox(DependencyObject o)
        {
            while (!(o is RibbonGroupBox))
            {
                o = VisualTreeHelper.GetParent(o) ?? LogicalTreeHelper.GetParent(o);
                if (o == null) break;
            }
            return (RibbonGroupBox)o;
        }

        /// <summary>
        /// Sets appropriate size of the control according to the 
        /// given group box state and control's size definition
        /// </summary>
        /// <param name="element">UI Element</param>
        /// <param name="state">Group box state</param>
        public static void SetAppropriateSize(UIElement element, RibbonGroupBoxState state)
        {
            var index = (int)state;
            if (state == RibbonGroupBoxState.Collapsed)
            {
                index = 0;
            }

            var control = element as IRibbonControl;
            if (control != null)
            {
                control.Size = GetThreeSizeDefinition(element)[index];
            }
        }


        /// <summary>
        /// Gets or sets SizeDefinition for element
        /// </summary>
        public string SizeDefinition
        {
            get { return (string)GetValue(SizeDefinitionProperty); }
            set { SetValue(SizeDefinitionProperty, value); }
        }

        /// <summary>
        /// Gets value of the attached property SizeDefinition of the given element
        /// </summary>
        /// <param name="element">The given element</param>
        public static RibbonControlSize[] GetThreeSizeDefinition(UIElement element)
        {
            string[] splitted = ((element as IRibbonControl).SizeDefinition).Split(new char[] { ' ', ',', ';', '-', '>' }, StringSplitOptions.RemoveEmptyEntries);

            int count = Math.Min(splitted.Length, 3);
            if (count == 0) return new RibbonControlSize[] { RibbonControlSize.Large, RibbonControlSize.Large, RibbonControlSize.Large };

            RibbonControlSize[] sizes = new RibbonControlSize[3];
            for (int i = 0; i < count; i++)
            {
                switch (splitted[i])
                {
                    case "Large": sizes[i] = RibbonControlSize.Large; break;
                    case "Middle": sizes[i] = RibbonControlSize.Middle; break;
                    case "Small": sizes[i] = RibbonControlSize.Small; break;
                    default: sizes[i] = RibbonControlSize.Large; break;
                }
            }
            for (int i = count; i < 3; i++)
            {
                sizes[i] = sizes[count - 1];
            }
            return sizes;
        }

        #endregion

        #region Header

        /// <summary>
        /// Gets or sets element header
        /// </summary>
        public object Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
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
            get { return GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
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
        public static readonly DependencyProperty CommandParameterProperty = ButtonBase.CommandParameterProperty.AddOwner(typeof(RibbonControl), new FrameworkPropertyMetadata(null));
        /// <summary>
        /// Identifies the routed Command dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = ButtonBase.CommandProperty.AddOwner(typeof(RibbonControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCommandChanged)));

        /// <summary>
        /// Identifies the CommandTarget dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandTargetProperty = ButtonBase.CommandTargetProperty.AddOwner(typeof(RibbonControl), new FrameworkPropertyMetadata(null));

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
            RibbonControl control = d as RibbonControl;
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

                RoutedUICommand cmd = e.NewValue as RoutedUICommand;
                if ((cmd != null) && (control.Header == null)) control.Header = cmd.Text;
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


        /// <summary>
        /// Coerces IsEnabled 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="basevalue"></param>
        /// <returns></returns>
        private static object CoerceIsEnabled(DependencyObject d, object basevalue)
        {
            RibbonControl control = (RibbonControl)d;
            UIElement parent = LogicalTreeHelper.GetParent(control) as UIElement;
            bool parentIsEnabled = parent == null || parent.IsEnabled;
            bool commandIsEnabled = control.Command == null || control.currentCanExecute;

            // We force disable if parent is disabled or command cannot be executed
            return (bool)basevalue && parentIsEnabled && commandIsEnabled;
        }

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
            if (source is ICommandSource)
            {
                if (source is MenuItem)
                {
                    Bind(source, element, "CommandParameter", ButtonBase.CommandParameterProperty, BindingMode.OneWay);
                    Bind(source, element, "CommandTarget", MenuItem.CommandTargetProperty, BindingMode.OneWay);
                    Bind(source, element, "Command", MenuItem.CommandProperty, BindingMode.OneWay);
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

            IRibbonControl sourceControl = source as IRibbonControl;
            if (sourceControl.Icon != null)
            {
                Visual iconVisual = sourceControl.Icon as Visual;
                if (iconVisual != null)
                {
                    Rectangle rect = new Rectangle();
                    rect.Width = 16;
                    rect.Height = 16;
                    rect.Fill = new VisualBrush(iconVisual);
                    (element as IRibbonControl).Icon = rect;
                }
                else Bind(source, element, "Icon", RibbonControl.IconProperty, BindingMode.OneWay);
            }
            if (sourceControl.Header != null) Bind(source, element, "Header", RibbonControl.HeaderProperty, BindingMode.OneWay);

            (element as IRibbonControl).Size = RibbonControlSize.Small;
        }

        /// <summary>
        /// Gets or sets whether control can be added to quick access toolbar
        /// </summary>
        public bool CanAddToQuickAccessToolBar
        {
            get { return (bool)GetValue(CanAddToQuickAccessToolBarProperty); }
            set { SetValue(CanAddToQuickAccessToolBarProperty, value); }
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
            d.CoerceValue(FrameworkElement.ContextMenuProperty);
        }

        #endregion

        #region Binding

        /// <summary>
        /// Binds elements property
        /// </summary>
        /// <param name="source">Source element</param>
        /// <param name="target">Target element</param>
        /// <param name="path">Property path</param>
        /// <param name="property">Property to bind</param>
        /// <param name="mode">Binding mode</param>
        static internal void Bind(object source, FrameworkElement target, string path, DependencyProperty property, BindingMode mode)
        {
            Binding binding = new Binding();
            binding.Path = new PropertyPath(path);
            binding.Source = source;
            binding.Mode = mode;
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

        #region Protected

        /// <summary>
        /// Handles size property changing
        /// </summary>
        /// <param name="previous">Previous value</param>
        /// <param name="current">Current value</param>
        protected virtual void OnSizePropertyChanged(RibbonControlSize previous, RibbonControlSize current)
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
            Point tabItemPos = control.PointToScreen(new Point(0, 0));
            NativeMethods.Rect tabItemRect = new NativeMethods.Rect();
            tabItemRect.Left = (int)tabItemPos.X;
            tabItemRect.Top = (int)tabItemPos.Y;
            tabItemRect.Right = (int)tabItemPos.X + (int)control.ActualWidth;
            tabItemRect.Bottom = (int)tabItemPos.Y + (int)control.ActualHeight;
            uint MONITOR_DEFAULTTONEAREST = 0x00000002;
            System.IntPtr monitor = NativeMethods.MonitorFromRect(ref tabItemRect, MONITOR_DEFAULTTONEAREST);
            if (monitor != System.IntPtr.Zero)
            {
                NativeMethods.MonitorInfo monitorInfo = new NativeMethods.MonitorInfo();
                monitorInfo.Size = Marshal.SizeOf(monitorInfo);
                NativeMethods.GetMonitorInfo(monitor, monitorInfo);
                return new Rect(monitorInfo.Work.Left, monitorInfo.Work.Top, monitorInfo.Work.Right - monitorInfo.Work.Left, monitorInfo.Work.Bottom - monitorInfo.Work.Top);
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
            Point tabItemPos = control.PointToScreen(new Point(0, 0));
            NativeMethods.Rect tabItemRect = new NativeMethods.Rect();
            tabItemRect.Left = (int)tabItemPos.X;
            tabItemRect.Top = (int)tabItemPos.Y;
            tabItemRect.Right = (int)tabItemPos.X + (int)control.ActualWidth;
            tabItemRect.Bottom = (int)tabItemPos.Y + (int)control.ActualHeight;
            uint MONITOR_DEFAULTTONEAREST = 0x00000002;
            System.IntPtr monitor = NativeMethods.MonitorFromRect(ref tabItemRect, MONITOR_DEFAULTTONEAREST);
            if (monitor != System.IntPtr.Zero)
            {
                NativeMethods.MonitorInfo monitorInfo = new NativeMethods.MonitorInfo();
                monitorInfo.Size = Marshal.SizeOf(monitorInfo);
                NativeMethods.GetMonitorInfo(monitor, monitorInfo);
                return new Rect(monitorInfo.Monitor.Left, monitorInfo.Monitor.Top, monitorInfo.Monitor.Right - monitorInfo.Monitor.Left, monitorInfo.Monitor.Bottom - monitorInfo.Monitor.Top);
            }
            return new Rect();
        }

        #endregion
    }
}


