using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using System.Text;

namespace Fluent
{
    /// <summary>
    /// ScreenTips display the name of the control, 
    /// the keyboard shortcut for the control, and a brief description 
    /// of how to use the control. ScreenTips also can provide F1 support, 
    /// which opens help and takes the user directly to the related 
    /// help topic for the control whose ScreenTip was 
    /// displayed when the F1 button was pressed
    /// </summary>
    public class ScreenTip : ToolTip
    {
        #region Initialization

        /// <summary>
        /// Static constructor
        /// </summary>
        static ScreenTip() { DefaultStyleKeyProperty.OverrideMetadata(typeof(ScreenTip), new FrameworkPropertyMetadata(typeof(ScreenTip))); }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ScreenTip()
        {
            Opened += OnToolTipOpened;
            Closed += OnToolTipClosed; 
        }

        #endregion

        #region Title Property

        /// <summary>
        /// Gets or sets title of the screen tip
        /// </summary>
        [System.ComponentModel.DisplayName("Title"),
        System.ComponentModel.Category("Screen Tip"),
        System.ComponentModel.Description("Title of the screen tip")]
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Title. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ScreenTip), new UIPropertyMetadata(""));

        #endregion

        #region Text Property
        
        /// <summary>
        /// Gets or sets text of the screen tip
        /// </summary>
        [System.ComponentModel.DisplayName("Text"),
        System.ComponentModel.Category("Screen Tip"),
        System.ComponentModel.Description("Main text of the screen tip")]
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
            DependencyProperty.Register("Text", typeof(string), typeof(ScreenTip), new UIPropertyMetadata(""));

        #endregion

        #region DisableReason Property
        
        /// <summary>
        /// Gets or sets disable reason of the associated screen tip's control
        /// </summary>
        [System.ComponentModel.DisplayName("Disable Reason"),
        System.ComponentModel.Category("Screen Tip"),
        System.ComponentModel.Description("Describe here what would cause disable of the control")]
        public string DisableReason
        {
            get { return (string)GetValue(DisableReasonProperty); }
            set { SetValue(DisableReasonProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for DisableReason. 
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DisableReasonProperty =
            DependencyProperty.Register("DisableReason", typeof(string), typeof(ScreenTip), new UIPropertyMetadata(""));

        #endregion

        #region HelpTopic Property
        
        /// <summary>
        /// Gets or sets help topic of the ScreenTip
        /// </summary>
        [System.ComponentModel.DisplayName("Help Topic"),
        System.ComponentModel.Category("Screen Tip"),
        System.ComponentModel.Description("Help topic (it will be used to execute help)")]
        public object HelpTopic
        {
            get { return (object)GetValue(HelpTopicProperty); }
            set { SetValue(HelpTopicProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HelpTopic.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HelpTopicProperty =
            DependencyProperty.Register("HelpTopic", typeof(object), typeof(ScreenTip), new UIPropertyMetadata(null));

        #endregion

        #region Image Property

        /// <summary>
        /// Gets or sets image of the screen tip
        /// </summary>
        [System.ComponentModel.DisplayName("Image"),
        System.ComponentModel.Category("Screen Tip"),
        System.ComponentModel.Description("Image of the screen tip")]
        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Image.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(ScreenTip), new UIPropertyMetadata(null));

        #endregion
 
        #region Help Invocation

        /// <summary>
        /// Occurs when user press F1 on ScreenTip with HelpTopic filled
        /// </summary>
        public static event EventHandler<ScreenTipHelpEventArgs> HelpPressed;

        #endregion

        #region F1 Help Handling

        // Currently focused element
        IInputElement focusedElement = null;

        void OnToolTipClosed(object sender, RoutedEventArgs e)
        {        
            if (focusedElement != null)
            {
                focusedElement.PreviewKeyDown -= OnFocusedElementPreviewKeyDown;
                focusedElement = null;
            }
        }

        void OnToolTipOpened(object sender, RoutedEventArgs e)
        {
            if (HelpTopic != null)
            {
                focusedElement = Keyboard.FocusedElement;
                if (focusedElement != null)
                {
                    focusedElement.PreviewKeyDown += new KeyEventHandler(OnFocusedElementPreviewKeyDown);
                }
            }
        }

        void OnFocusedElementPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F1)
            {
                e.Handled = true;
                if (HelpPressed != null) HelpPressed(null, new ScreenTipHelpEventArgs(HelpTopic));
            }
        }

        #endregion
    }

    /// <summary>
    /// Event args for HelpPressed event handler
    /// </summary>
    public class ScreenTipHelpEventArgs : EventArgs
    {
        /// <summary>
        /// Gets help topic associated with screen tip
        /// </summary>
        public object HelpTopic { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="helpTopic">Help topic</param>
        public ScreenTipHelpEventArgs(object helpTopic)
        {
            HelpTopic = helpTopic;
        }
    }

    /*// TODO: change default values of ScreenTip properties to null and fix control template triggers

    /// <summary>
    /// Includes attached properties for controls 
    /// that want to be in ribbon group
    /// </summary>
    public class ScreenTip
    {
        #region Title Attached Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for Title.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.RegisterAttached(
          "Title",
          typeof(string),
          typeof(ScreenTip),
          new FrameworkPropertyMetadata("", TitlePropertyChanged)
        );

        static void TitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ToolTip toolTip = ((FrameworkElement)d).ToolTip as ToolTip;
            if (toolTip == null)
            {
                toolTip = new ToolTip();
                (d as FrameworkElement).ToolTip = toolTip;
                toolTip.Opened += OnToolTipOpened;
                toolTip.Closed += OnToolTipClosed;
            }
            ToolTipService.SetShowOnDisabled(d, true);
            ToolTipService.SetShowDuration(d, 20000);
            ToolTipService.SetInitialShowDelay(d, 900);
        }

        /// <summary>
        /// Sets value of attached property Title for the given element
        /// </summary>
        /// <param name="element">The given element</param>
        /// <param name="value">Value</param>
        public static void SetTitle(UIElement element, string value)
        {            
            element.SetValue(TitleProperty, value);
        }

        /// <summary>
        /// Gets value of the attached property Title of the given element
        /// </summary>
        /// <param name="element">The given element</param>
        [System.ComponentModel.DisplayName("Title"),
        AttachedPropertyBrowsableForChildren(IncludeDescendants = true),
        System.ComponentModel.Category("Screen Tip"),
        System.ComponentModel.Description("Title of the screen tip")]
        public static string GetTitle(UIElement element)
        {
            return (string)element.GetValue(TitleProperty);
        }

        #endregion

        #region F1 Help Handling

        // Currently opened tooltip & focused element
        static ToolTip openedToolTip = null;
        static IInputElement focusedElement = null;

        static void OnToolTipClosed(object sender, RoutedEventArgs e)
        {
            openedToolTip = null;
            if (focusedElement != null)
            {
                focusedElement.PreviewKeyDown -= OnFocusedElementPreviewKeyDown;
                focusedElement = null;
            }
        }

        static void OnToolTipOpened(object sender, RoutedEventArgs e)
        {
            ToolTip toolTip = (ToolTip)sender;
            openedToolTip = toolTip;
            if (toolTip.PlacementTarget != null && GetHelpTopic(toolTip.PlacementTarget) != null)
            {
                focusedElement = Keyboard.FocusedElement;
                if (focusedElement != null)
                {
                    focusedElement.PreviewKeyDown += new KeyEventHandler(OnFocusedElementPreviewKeyDown);
                }
            }
        }

        static void OnFocusedElementPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if ((openedToolTip == null) && (openedToolTip.PlacementTarget != null)) return;
            if (e.Key == Key.F1)
            {
                e.Handled = true;
                if (HelpPressed != null) HelpPressed(null, new ScreenTipHelpEventArgs(GetHelpTopic(openedToolTip.PlacementTarget)));
            }
        }

        #endregion

        #region Text Attached Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for Text.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
          "Text",
          typeof(string),
          typeof(ScreenTip),
          new FrameworkPropertyMetadata("",
              FrameworkPropertyMetadataOptions.None)
        );

        /// <summary>
        /// Sets value of attached property Text for the given element
        /// </summary>
        /// <param name="element">The given element</param>
        /// <param name="value">Value</param>
        public static void SetText(UIElement element, string value)
        {
            element.SetValue(TextProperty, value);
        }

        /// <summary>
        /// Gets value of the attached property Text of the given element
        /// </summary>
        /// <param name="element">The given element</param>
        [System.ComponentModel.DisplayName("Text"),
        AttachedPropertyBrowsableForChildren(IncludeDescendants = true),
        System.ComponentModel.Category("Screen Tip"),
        System.ComponentModel.Description("Main text of the screen tip")]
        public static string GetText(UIElement element)
        {
            return (string)element.GetValue(TextProperty);
        }

        #endregion

        #region DisableReason Attached Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for DisableReason.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty DisableReasonProperty = DependencyProperty.RegisterAttached(
          "DisableReason",
          typeof(string),
          typeof(ScreenTip),
          new FrameworkPropertyMetadata("",
              FrameworkPropertyMetadataOptions.None)
        );

        /// <summary>
        /// Sets value of attached property DisableReason for the given element
        /// </summary>
        /// <param name="element">The given element</param>
        /// <param name="value">Value</param>
        public static void SetDisableReason(UIElement element, string value)
        {
            element.SetValue(DisableReasonProperty, value);
        }

        /// <summary>
        /// Gets value of the attached property Text of the given element
        /// </summary>
        /// <param name="element">The given element</param>
        [System.ComponentModel.DisplayName("Disable Reason"),
        AttachedPropertyBrowsableForChildren(IncludeDescendants = true),
        System.ComponentModel.Category("Screen Tip"),
        System.ComponentModel.Description("Describe here what would cause disable of the control")]
        public static string GetDisableReason(UIElement element)
        {
            return (string)element.GetValue(DisableReasonProperty);
        }

        #endregion

        #region Help Attached Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for Help.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HelpTopicProperty = DependencyProperty.RegisterAttached(
          "HelpTopic",
          typeof(object),
          typeof(ScreenTip),
          new FrameworkPropertyMetadata(null,
              FrameworkPropertyMetadataOptions.None)
        );

        /// <summary>
        /// Sets value of attached property Help for the given element
        /// </summary>
        /// <param name="element">The given element</param>
        /// <param name="value">Value</param>
        public static void SetHelpTopic(UIElement element, object value)
        {
            element.SetValue(HelpTopicProperty, value);
        }

        /// <summary>
        /// Gets value of the attached property Help of the given element
        /// </summary>
        /// <param name="element">The given element</param>
        [System.ComponentModel.DisplayName("Help Topic"),
        AttachedPropertyBrowsableForChildren(IncludeDescendants = true),
        System.ComponentModel.Category("Screen Tip"),
        System.ComponentModel.Description("Help topic (it will be used to execute help)")]
        public static string GetHelpTopic(UIElement element)
        {
            return (string)element.GetValue(HelpTopicProperty);
        }

        #endregion

        #region Image Attached Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for Image.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty ImageProperty = DependencyProperty.RegisterAttached(
          "Image",
          typeof(ImageSource),
          typeof(ScreenTip),
          new FrameworkPropertyMetadata(null,
              FrameworkPropertyMetadataOptions.None)
        );

        /// <summary>
        /// Sets value of attached property Image for the given element
        /// </summary>
        /// <param name="element">The given element</param>
        /// <param name="value">Value</param>
        public static void SetImage(UIElement element, ImageSource value)
        {
            element.SetValue(ImageProperty, value);
        }

        /// <summary>
        /// Gets value of the attached property Image of the given element
        /// </summary>
        /// <param name="element">The given element</param>
        [System.ComponentModel.DisplayName("Image"),
        AttachedPropertyBrowsableForChildren(IncludeDescendants = true),
        System.ComponentModel.Category("Screen Tip"),
        System.ComponentModel.Description("Image of the screen tip")]
        public static ImageSource GetImage(UIElement element)
        {
            return (ImageSource)element.GetValue(ImageProperty);
        }

        #endregion

        #region Width Attached Property

        /// <summary>
        /// Using a DependencyProperty as the backing store for Width.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty WidthProperty = DependencyProperty.RegisterAttached(
          "Width",
          typeof(double),
          typeof(ScreenTip),
          new FrameworkPropertyMetadata((double)260.0,
              FrameworkPropertyMetadataOptions.None)
        );

        /// <summary>
        /// Sets value of attached property Width for the given element
        /// </summary>
        /// <param name="element">The given element</param>
        /// <param name="value">Value</param>
        public static void SetWidth(UIElement element, double value)
        {
            element.SetValue(WidthProperty, value);
        }

        /// <summary>
        /// Gets value of the attached property Width of the given element
        /// </summary>
        /// <param name="element">The given element</param>
        [System.ComponentModel.DisplayName("Width"),
        AttachedPropertyBrowsableForChildren(IncludeDescendants = true),
        System.ComponentModel.Category("Screen Tip"),
        System.ComponentModel.Description("Width of the screen tip")]
        public static double GetWidth(UIElement element)
        {
            return (double)element.GetValue(WidthProperty);
        }

        #endregion

        #region Help Invocation

        /// <summary>
        /// Occurs when user press F1 on ScreenTip with HelpTopic filled
        /// </summary>
        public static event EventHandler<ScreenTipHelpEventArgs> HelpPressed;
       

        #endregion
    }

    /// <summary>
    /// Event args for HelpPressed event handler
    /// </summary>
    public class ScreenTipHelpEventArgs : EventArgs
    {
        /// <summary>
        /// Gets help topic associated with screen tip
        /// </summary>
        public object HelpTopic { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="helpTopic">Help topic</param>
        public ScreenTipHelpEventArgs(object helpTopic)
        {
            HelpTopic = helpTopic;
        }
    }*/
}
