using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Fluent
{
    /// <summary>
    /// Represents logical sizes of a ribbon control 
    /// </summary>
    public enum RibbonControlSize
    {
        /// <summary>
        /// Large size of a control
        /// </summary>
        Large = 0,
        /// <summary>
        /// Middle size of a control
        /// </summary>
        Middle,
        /// <summary>
        /// Small size of a control
        /// </summary>
        Small
    }

    /// <summary>
    /// Represent base class for Fluent controls
    /// </summary>
    public abstract class RibbonControl:Control, ICommandSource
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

        ///     When the ControlSizeDefinition property changes we need to invalidate the parent chain measure so that
        ///     the RibbonGroupsContainer can calculate the new size within the same MeasureOverride call.  This property
        ///     usually changes from RibbonGroupsContainer.MeasureOverride.
        private static void OnSizePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Visual visual = d as Visual;
            while (visual != null)
            {
                UIElement uiElement = visual as UIElement;
                if (uiElement != null)
                {
                    if (uiElement is RibbonGroupsContainer)
                    {
                        break;
                    }

                    uiElement.InvalidateMeasure();
                }

                visual = VisualTreeHelper.GetParent(visual) as Visual;
            }
        }

        /// <summary>
        /// Gets sor sets Size for the element
        /// </summary>
        public RibbonControlSize Size
        {
            get { return (RibbonControlSize)GetValue(SizeProperty); }
            set { SetValue(SizeProperty,value);}
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
          typeof(RibbonControl),
          new FrameworkPropertyMetadata("Large, Middle, Small",
              FrameworkPropertyMetadataOptions.AffectsArrange |
              FrameworkPropertyMetadataOptions.AffectsMeasure |
              FrameworkPropertyMetadataOptions.AffectsRender |
              FrameworkPropertyMetadataOptions.AffectsParentArrange |
              FrameworkPropertyMetadataOptions.AffectsParentMeasure,
              OnSizeDefinitionPropertyChanged)
        );

        // Handles SizeDefinitionProperty changes
        static void OnSizeDefinitionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {            
            // Find parent group box
            RibbonGroupBox groupBox = FindParentRibbonGroupBox(d);
            UIElement element = (UIElement) d;
            if (groupBox != null)
            {
                SetAppropriateSize(element, groupBox.State);
            }
            else SetAppropriateSize(element, RibbonGroupBoxState.Large);
        }

        // Finds parent group box
        [SuppressMessage("Microsoft.Performance", "CA1800")]
        static RibbonGroupBox FindParentRibbonGroupBox(DependencyObject o)
        {
            while (!(o is RibbonGroupBox)) { o = VisualTreeHelper.GetParent(o); if (o == null) break; }
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
            int index = (int)state;
            if (state == RibbonGroupBoxState.Collapsed) index = 0;
            RibbonControl control = (element as RibbonControl);
            if (control!=null) control.Size = GetThreeSizeDefinition(element)[index];
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
            string[] splitted = ((element as RibbonControl).SizeDefinition).Split(new char[] { ' ', ',', ';', '-', '>' }, StringSplitOptions.RemoveEmptyEntries);
            
            int count = Math.Min(splitted.Length, 3);
            if (count == 0) return new RibbonControlSize[] { RibbonControlSize.Large, RibbonControlSize.Large, RibbonControlSize.Large };

            RibbonControlSize[] sizes = new RibbonControlSize[3];
            for (int i = 0; i < count; i++)
            {
                switch(splitted[i])
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

        #region Text

        /// <summary>
        /// Gets or sets element Text
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Text.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(RibbonControl), new UIPropertyMetadata(""));

        #endregion

        #region Icon

        /// <summary>
        /// Gets or sets Icon for the element
        /// </summary>
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(RibbonControl), new UIPropertyMetadata(null));            
        
        #endregion

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
        /// Identifies the RibbonControl.Click routed event.
        /// </summary>
        public static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(RibbonControl));

        #endregion

        #region Command

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
        /// Identifies the System.Windows.Controls.Primitives.ButtonBase.CommandParameter dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(RibbonControl), new FrameworkPropertyMetadata(null));
        /// <summary>
        /// Identifies the routed System.Windows.Controls.Primitives.ButtonBase.Command dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(RibbonControl), new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnCommandChanged)));

        /// <summary>
        /// Identifies the System.Windows.Controls.Primitives.ButtonBase.CommandTarget dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(RibbonControl), new FrameworkPropertyMetadata(null));

        // Keep a copy of the handler so it doesn't get garbage collected.
        [SuppressMessage("Microsoft.Performance", "CA1823")]
        private static EventHandler canExecuteChangedHandler;

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
                canExecuteChangedHandler = handler;
                (e.NewValue as ICommand).CanExecuteChanged += handler;                
            }
            control.CoerceValue(IsEnabledProperty);
        }
        /// <summary>
        /// Handles Command CanExecute changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCommandCanExecuteChanged(object sender, EventArgs e)
        {
            CoerceValue(IsEnabledProperty);
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
                IInputElement commandTarget = CommandTarget;
                RoutedCommand routedCommand = command as RoutedCommand;
                if (routedCommand != null)
                {
                    if (commandTarget == null)
                    {
                        commandTarget = this as IInputElement;
                    }
                    if (routedCommand.CanExecute(commandParameter, commandTarget))
                    {
                        routedCommand.Execute(commandParameter, commandTarget);
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
            IInputElement commandTarget = CommandTarget;
            RoutedCommand routedCommand = command as RoutedCommand;
            if (routedCommand == null)
            {
                return command.CanExecute(commandParameter);
            }
            if (commandTarget == null)
            {
                commandTarget = this as IInputElement;
            }
            return routedCommand.CanExecute(commandParameter, commandTarget);
        }

        #endregion

        #region IsEnabled

        /// <summary>
        /// Handles IsEnabled changes
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e">The event data.</param>
        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Coerces IsEnabled 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="basevalue"></param>
        /// <returns></returns>
        private static object CoerceIsEnabled(DependencyObject d, object basevalue)
        {
            RibbonControl control = (d as RibbonControl);
            if (control!=null)
            {
                
                if (control.Command != null)
                {
                    return ((bool) basevalue) && control.CanExecuteCommand();
                }
            }
            return basevalue;
        }

        #endregion        

        #region Focusable

        /// <summary>
        /// Handles IsEnabled changes
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e">The event data.</param>
        private static void OnFocusableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Coerces IsEnabled 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="basevalue"></param>
        /// <returns></returns>
        private static object CoerceFocusable(DependencyObject d, object basevalue)
        {
            RibbonControl control = (d as RibbonControl);
            if (control!=null)
            {                
                Ribbon ribbon = control.FindParentRibbon();
                if (ribbon != null)
                {
                    return ((bool)basevalue) && ribbon.Focusable;
                }
            }
            return basevalue;
        }

        // Find parent ribbon
        private Ribbon FindParentRibbon()
        {
            DependencyObject element = this.Parent;
            while (element != null)
            {
                Ribbon ribbon = element as Ribbon;
                if (ribbon != null) return ribbon;
                element = VisualTreeHelper.GetParent(element);
            }
            return null;
        }

        #endregion        

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static RibbonControl()
        {
            IsEnabledProperty.AddOwner(typeof (RibbonControl), new FrameworkPropertyMetadata(OnIsEnabledChanged, CoerceIsEnabled));
            FocusableProperty.AddOwner(typeof(RibbonControl), new FrameworkPropertyMetadata(OnFocusableChanged, CoerceFocusable));

            ToolTipService.ShowOnDisabledProperty.OverrideMetadata(typeof(RibbonControl), new FrameworkPropertyMetadata(true));
            ToolTipService.InitialShowDelayProperty.OverrideMetadata(typeof(RibbonControl), new FrameworkPropertyMetadata(900));
            ToolTipService.BetweenShowDelayProperty.OverrideMetadata(typeof(RibbonControl), new FrameworkPropertyMetadata(0));
            ToolTipService.ShowDurationProperty.OverrideMetadata(typeof(RibbonControl), new FrameworkPropertyMetadata(20000));            
        }

        #endregion

        #region Overrides
        
        /// <summary>
        /// Invoked when an unhandled System.Windows.Input.Keyboard.KeyUp attached event reaches 
        /// an element in its route that is derived from this class. Implement this method to add class handling for this event.
        /// </summary>
        /// <param name="e">The System.Windows.Input.KeyEventArgs that contains the event data.</param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            if((e.Key==Key.Return)||(e.Key==Key.Space))
            {
                RaiseEvent(new RoutedEventArgs(RibbonControl.ClickEvent, this));
                e.Handled = true;
            }
            base.OnKeyUp(e);
        }

        #endregion
    }
}


