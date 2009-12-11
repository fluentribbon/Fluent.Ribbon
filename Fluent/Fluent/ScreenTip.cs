using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Controls.Primitives;
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
            CustomPopupPlacementCallback = CustomPopupPlacementMethod;
            Placement = PlacementMode.Custom;
        }

        #endregion

        #region Popup Custom Placement

        // Calculate two variants: below and upper ribbon
        CustomPopupPlacement[] CustomPopupPlacementMethod(Size popupSize, Size targetSize, Point offset)
        {
            if (PlacementTarget == null) return new CustomPopupPlacement[] {};

            Ribbon ribbon = null;
            UIElement topLevelElement = null;
            FindControls(PlacementTarget, ref ribbon, ref topLevelElement);
            
            
            if (IsRibbonAligned && (ribbon != null))
            {
                double belowY = ribbon.TranslatePoint(new Point(0, ribbon.ActualHeight), PlacementTarget).Y;
                double aboveY = ribbon.TranslatePoint(new Point(0, 0), PlacementTarget).Y - popupSize.Height;
                CustomPopupPlacement below = new CustomPopupPlacement(new Point(0, belowY + 1), PopupPrimaryAxis.Horizontal);
                CustomPopupPlacement above = new CustomPopupPlacement(new Point(0, aboveY - 1), PopupPrimaryAxis.Horizontal);
                return new CustomPopupPlacement[] { below, above };
            }
            else if (IsRibbonAligned && (!(topLevelElement is Window)))
            {
                // Placed on Popup?                
                UIElement decoratorChild = GetDecoratorChild(topLevelElement);
                double belowY = decoratorChild.TranslatePoint(new Point(0, ((FrameworkElement)decoratorChild).ActualHeight), PlacementTarget).Y;
                double aboveY = decoratorChild.TranslatePoint(new Point(0, 0), PlacementTarget).Y - popupSize.Height;
                CustomPopupPlacement below = new CustomPopupPlacement(new Point(0, belowY + 1), PopupPrimaryAxis.Horizontal);
                CustomPopupPlacement above = new CustomPopupPlacement(new Point(0, aboveY - 1), PopupPrimaryAxis.Horizontal);
                return new CustomPopupPlacement[] { below, above };
            }
            else
            {
                double x = Mouse.GetPosition(PlacementTarget).X;
                return new CustomPopupPlacement[] { 
                    new CustomPopupPlacement(new Point(x, PlacementTarget.RenderSize.Height + 1), PopupPrimaryAxis.Horizontal),
                    new CustomPopupPlacement(new Point(x, -popupSize.Height - 1), PopupPrimaryAxis.Horizontal)};
            }
        }

        UIElement GetDecoratorChild(UIElement popupRoot)
        {
            if (popupRoot == null) return null;
            if (popupRoot is AdornerDecorator) return ((AdornerDecorator)popupRoot).Child;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(popupRoot); i++)
            {
                UIElement element = GetDecoratorChild(VisualTreeHelper.GetChild(popupRoot, i) as UIElement);
                if (element != null) return element;
            }
            return null;
        }

        void FindControls(UIElement obj, ref Ribbon ribbon, ref UIElement topLevelElement)
        {
            if (obj == null) return;
            if (obj is Ribbon) ribbon = (Ribbon)obj;

            UIElement parentVisual = VisualTreeHelper.GetParent(obj) as UIElement;
            if (parentVisual == null) topLevelElement = obj;
            else FindControls(parentVisual, ref ribbon, ref topLevelElement);            
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

        #region IsRibbonAligned
        
        /// <summary>
        /// Gets or set whether ScreenTip should positioned below Ribbon
        /// </summary>
        public bool IsRibbonAligned
        {
            get { return (bool)GetValue(IsRibbonAlignedProperty); }
            set { SetValue(IsRibbonAlignedProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for BelowRibbon.  
        /// This enables animation, styling, binding, etc...
        /// </summary> 
        public static readonly DependencyProperty IsRibbonAlignedProperty = 
            DependencyProperty.Register("BelowRibbon", typeof(bool), typeof(ScreenTip), 
            new UIPropertyMetadata(true));
        

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
}
