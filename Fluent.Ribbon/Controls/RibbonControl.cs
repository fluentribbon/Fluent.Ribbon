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

// ReSharper disable once CheckNamespace
namespace Fluent
{
    using Fluent.Extensions;
    using Fluent.Internal;
    using Fluent.Internal.KnownBoxes;
    using Fluent.Metro.Native;

    /// <summary>
    /// Represent base class for Fluent controls
    /// </summary>
    public abstract class RibbonControl : Control, ICommandSource, IQuickAccessItemProvider, IRibbonControl
    {

    #region WhiteIcon
    public static readonly DependencyProperty WhiteIconProperty = DependencyProperty.RegisterAttached(
    "WhiteIcon",
    typeof(object),
    typeof(RibbonControl),
    new FrameworkPropertyMetadata(null, IconChanged)
  );

    private static void IconChanged(DependencyObject targetObject, DependencyPropertyChangedEventArgs e)
    {

    }

    public static void SetWhiteIcon(UIElement element, object value)
    {
      element.SetValue(WhiteIconProperty, value);
    }
    public static object GetWhiteIcon(UIElement element)
    {
      return element.GetValue(WhiteIconProperty);
    }
    #endregion

    #region IsQuickAccessItem
    public static readonly DependencyProperty IsQuickAccessItemProperty = DependencyProperty.RegisterAttached(
    "IsQuickAccessItem",
    typeof(bool),
    typeof(RibbonControl),
    new FrameworkPropertyMetadata(false)
  );


    public static void SetIsQuickAccessItem(UIElement element, bool value)
    {
      element.SetValue(IsQuickAccessItemProperty, value);
    }
    public static bool GetIsQuickAccessItem(UIElement element)
    {
      return (bool)element.GetValue(IsQuickAccessItemProperty);
    }
    #endregion

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
            DependencyProperty.Register(nameof(Header), typeof(object), typeof(RibbonControl), new PropertyMetadata());

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
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(object), typeof(RibbonControl), new PropertyMetadata(OnIconChanged));

        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var element = (RibbonControl)d;

            var oldElement = e.OldValue as FrameworkElement;
            if (oldElement != null)
            {
                element.RemoveLogicalChild(oldElement);
            }

            var newElement = e.NewValue as FrameworkElement;
            if (newElement != null)
            {
                element.AddLogicalChild(newElement);
            }
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
        public static readonly DependencyProperty CommandParameterProperty = ButtonBase.CommandParameterProperty.AddOwner(typeof(RibbonControl), new PropertyMetadata());
        /// <summary>
        /// Identifies the routed Command dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = ButtonBase.CommandProperty.AddOwner(typeof(RibbonControl), new PropertyMetadata(OnCommandChanged));

        /// <summary>
        /// Identifies the CommandTarget dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandTargetProperty = ButtonBase.CommandTargetProperty.AddOwner(typeof(RibbonControl), new PropertyMetadata());

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
            var type = typeof(RibbonControl);
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
            Bind(source, element, nameof(source.DataContext), DataContextProperty, BindingMode.OneWay);
            RibbonControl.SetIsQuickAccessItem(element, true);
            
            if (source is ICommandSource)
            {
                if (source is MenuItem)
                {
                    Bind(source, element, nameof(ICommandSource.CommandParameter), System.Windows.Controls.MenuItem.CommandParameterProperty, BindingMode.OneWay);
                    Bind(source, element, nameof(ICommandSource.CommandTarget), System.Windows.Controls.MenuItem.CommandTargetProperty, BindingMode.OneWay);
                    Bind(source, element, nameof(ICommandSource.Command), System.Windows.Controls.MenuItem.CommandProperty, BindingMode.OneWay);
                }
                else
                {
                    Bind(source, element, nameof(ICommandSource.CommandParameter), ButtonBase.CommandParameterProperty, BindingMode.OneWay);
                    Bind(source, element, nameof(ICommandSource.CommandTarget), ButtonBase.CommandTargetProperty, BindingMode.OneWay);
                    Bind(source, element, nameof(ICommandSource.Command), ButtonBase.CommandProperty, BindingMode.OneWay);
                }
            }

            Bind(source, element, nameof(FontFamily), FontFamilyProperty, BindingMode.OneWay);
            Bind(source, element, nameof(FontSize), FontSizeProperty, BindingMode.OneWay);
            Bind(source, element, nameof(FontStretch), FontStretchProperty, BindingMode.OneWay);
            Bind(source, element, nameof(FontStyle), FontStyleProperty, BindingMode.OneWay);
            Bind(source, element, nameof(FontWeight), FontWeightProperty, BindingMode.OneWay);

            Bind(source, element, nameof(Foreground), ForegroundProperty, BindingMode.OneWay);
            Bind(source, element, nameof(IsEnabled), IsEnabledProperty, BindingMode.OneWay);
            Bind(source, element, nameof(Opacity), OpacityProperty, BindingMode.OneWay);
            Bind(source, element, nameof(SnapsToDevicePixels), SnapsToDevicePixelsProperty, BindingMode.OneWay);

            Bind(source, element, new PropertyPath(FocusManager.IsFocusScopeProperty), FocusManager.IsFocusScopeProperty, BindingMode.OneWay);

            var headeredControl = source as IHeaderedControl;

            if (headeredControl != null)
            {
                Bind(source, element, nameof(IHeaderedControl.Header), HeaderProperty, BindingMode.OneWay);

                if (source.ToolTip != null
                    || BindingOperations.IsDataBound(source, ToolTipProperty))
                {
                    Bind(source, element, nameof(ToolTip), ToolTipProperty, BindingMode.OneWay);
                }
                else
                {
                    Bind(source, element, nameof(IHeaderedControl.Header), ToolTipProperty, BindingMode.OneWay);
                }
            }

            var ribbonControl = source as IRibbonControl;
            if (ribbonControl?.Icon != null)
            {
                var iconVisual = ribbonControl.Icon as Visual;
                if (iconVisual != null)
                {
                    var rect = new Rectangle
                    {
                        Width = 16,
                        Height = 16,
                        Fill = new VisualBrush(iconVisual)
                    };
                    ((IRibbonControl)element).Icon = rect;
                }
                else
                {
                    Bind(source, element, nameof(IRibbonControl.Icon), IconProperty, BindingMode.OneWay);
                }
            }
              
            Bind(source, element, new PropertyPath(RibbonControl.WhiteIconProperty), RibbonControl.WhiteIconProperty, BindingMode.OneWay);

            var attachedProperties = DependencyObjectHelper.GetAttachedProperties(source);
            foreach (var attachedProperty in attachedProperties)
              RibbonControl.Bind(source, element, new PropertyPath(attachedProperty), attachedProperty, BindingMode.OneWay);
            
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
            DependencyProperty.Register(nameof(CanAddToQuickAccessToolBar), typeof(bool), typeof(RibbonControl), new PropertyMetadata(BooleanBoxes.TrueBox, OnCanAddToQuickAccessToolbarChanged));

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

        internal static void Bind(object source, FrameworkElement target, string path, DependencyProperty property, BindingMode mode, UpdateSourceTrigger updateSourceTrigger)
        {
            Bind(source, target, new PropertyPath(path), property, mode, updateSourceTrigger);
        }

        internal static void Bind(object source, FrameworkElement target, PropertyPath path, DependencyProperty property, BindingMode mode)
        {
            Bind(source, target, path, property, mode, UpdateSourceTrigger.Default);
        }

        internal static void Bind(object source, FrameworkElement target, PropertyPath path, DependencyProperty property, BindingMode mode, UpdateSourceTrigger updateSourceTrigger)
        {
            var binding = new Binding
            {
                Path = path,
                Source = source,
                Mode = mode,
                UpdateSourceTrigger = updateSourceTrigger
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
            var monitor = NativeMethods.MonitorFromRect(ref tabItemRect, MONITORINFO.MonitorOptions.MONITOR_DEFAULTTONEAREST);
            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MONITORINFO();
                monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
                NativeMethods.GetMonitorInfo(monitor, monitorInfo);
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

            var monitor = NativeMethods.MonitorFromRect(ref tabItemRect, MONITORINFO.MonitorOptions.MONITOR_DEFAULTTONEAREST);
            if (monitor != IntPtr.Zero)
            {
                var monitorInfo = new MONITORINFO();
                monitorInfo.cbSize = Marshal.SizeOf(monitorInfo);
                NativeMethods.GetMonitorInfo(monitor, monitorInfo);
                return new Rect(monitorInfo.rcMonitor.left, monitorInfo.rcMonitor.top, monitorInfo.rcMonitor.right - monitorInfo.rcMonitor.left, monitorInfo.rcMonitor.bottom - monitorInfo.rcMonitor.top);
            }
            return new Rect();
        }

        /// <summary>
        /// Get the parent <see cref="Ribbon"/>.
        /// </summary>
        /// <returns>The found <see cref="Ribbon"/> or <c>null</c> of no parent <see cref="Ribbon"/> could be found.</returns>
        public static Ribbon GetParentRibbon(DependencyObject obj)
        {
            return UIHelper.GetParent<Ribbon>(obj);
        }

        #endregion
    }
}