#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright � Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

namespace Fluent
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;

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
        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static ScreenTip()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ScreenTip), new FrameworkPropertyMetadata(typeof(ScreenTip)));
            StyleProperty.OverrideMetadata(typeof(ScreenTip), new FrameworkPropertyMetadata(null, OnCoerceStyle));
        }

        private static object OnCoerceStyle(DependencyObject d, object basevalue)
        {
            if (basevalue == null)
            {
                var frameworkElement = d as FrameworkElement;
                if (frameworkElement != null)
                {
                    basevalue = frameworkElement.TryFindResource(typeof(ScreenTip));
                }
            }

            return basevalue;
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public ScreenTip()
        {
            this.Opened += this.OnToolTipOpened;
            this.Closed += this.OnToolTipClosed;
            this.CustomPopupPlacementCallback = this.CustomPopupPlacementMethod;
            this.Placement = PlacementMode.Custom;
            this.HelpLabelVisibility = Visibility.Visible;
        }

        #endregion

        #region Popup Custom Placement

        // Calculate two variants: below and upper ribbon
        CustomPopupPlacement[] CustomPopupPlacementMethod(Size popupSize, Size targetSize, Point offset)
        {
            if (this.PlacementTarget == null)
            {
                return new CustomPopupPlacement[] { };
            }

            Ribbon ribbon = null;
            UIElement topLevelElement = null;
            FindControls(this.PlacementTarget, ref ribbon, ref topLevelElement);

            // Exclude QAT items
            var notQuickAccessItem = !IsQuickAccessItem(this.PlacementTarget);
            var notContextMenuChild = !IsContextMenuChild(this.PlacementTarget);
            var rightToLeftOffset = this.FlowDirection == FlowDirection.RightToLeft
                                           ? -popupSize.Width
                                           : 0;

            var decoratorChild = GetDecoratorChild(topLevelElement);

            if (notQuickAccessItem
                && this.IsRibbonAligned
                && ribbon != null)
            {
                var belowY = ribbon.TranslatePoint(new Point(0, ribbon.ActualHeight), this.PlacementTarget).Y;
                var aboveY = ribbon.TranslatePoint(new Point(0, 0), this.PlacementTarget).Y - popupSize.Height;
                var below = new CustomPopupPlacement(new Point(rightToLeftOffset, belowY + 1), PopupPrimaryAxis.Horizontal);
                var above = new CustomPopupPlacement(new Point(rightToLeftOffset, aboveY - 1), PopupPrimaryAxis.Horizontal);
                return new[] { below, above };
            }

            if (notQuickAccessItem
                && this.IsRibbonAligned
                && notContextMenuChild
                && topLevelElement is Window == false
                && decoratorChild != null)
            {
                // Placed on Popup?                
                var belowY = decoratorChild.TranslatePoint(new Point(0, ((FrameworkElement)decoratorChild).ActualHeight), this.PlacementTarget).Y;
                var aboveY = decoratorChild.TranslatePoint(new Point(0, 0), this.PlacementTarget).Y - popupSize.Height;
                var below = new CustomPopupPlacement(new Point(rightToLeftOffset, belowY + 1), PopupPrimaryAxis.Horizontal);
                var above = new CustomPopupPlacement(new Point(rightToLeftOffset, aboveY - 1), PopupPrimaryAxis.Horizontal);
                return new[] { below, above };
            }

            return new[] { 
                new CustomPopupPlacement(new Point(rightToLeftOffset, this.PlacementTarget.RenderSize.Height + 1), PopupPrimaryAxis.Horizontal),
                new CustomPopupPlacement(new Point(rightToLeftOffset, -popupSize.Height - 1), PopupPrimaryAxis.Horizontal)};
        }

        private static bool IsContextMenuChild(UIElement element)
        {
            do
            {
                var parent = VisualTreeHelper.GetParent(element) as UIElement;
                //if (parent is ContextMenuBar) return true;
                element = parent;
            } while (element != null);

            return false;
        }

        private static bool IsQuickAccessItem(UIElement element)
        {
            do
            {
                var parent = VisualTreeHelper.GetParent(element) as UIElement;
                if (parent is QuickAccessToolBar)
                {
                    return true;
                }

                element = parent;
            }
            while (element != null);

            return false;
        }

        private static UIElement GetDecoratorChild(UIElement popupRoot)
        {
            if (popupRoot == null)
            {
                return null;
            }

            var decorator = popupRoot as AdornerDecorator;
            if (decorator != null)
            {
                return decorator.Child;
            }

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(popupRoot); i++)
            {
                var element = GetDecoratorChild(VisualTreeHelper.GetChild(popupRoot, i) as UIElement);
                if (element != null)
                {
                    return element;
                }
            }

            return null;
        }

        private static void FindControls(UIElement obj, ref Ribbon ribbon, ref UIElement topLevelElement)
        {
            if (obj == null)
            {
                return;
            }

            var objRibbon = obj as Ribbon;
            if (objRibbon != null)
            {
                ribbon = objRibbon;
            }

            var parentVisual = VisualTreeHelper.GetParent(obj) as UIElement;
            if (parentVisual == null)
            {
                topLevelElement = obj;
            }
            else
            {
                FindControls(parentVisual, ref ribbon, ref topLevelElement);
            }
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
            get { return (object)this.GetValue(HelpTopicProperty); }
            set { this.SetValue(HelpTopicProperty, value); }
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

        #region ShowHelp Property
        /// <summary>
        /// Shows or hides the Help Label
        /// </summary>
        [System.ComponentModel.DisplayName("HelpLabelVisibility"),
        System.ComponentModel.Category("Screen Tip"),
        System.ComponentModel.Description("Sets the visibility of the F1 Help Label")]
        public Visibility HelpLabelVisibility
        {
            get { return (Visibility)GetValue(HelpLabelVisibilityProperty); }
            set { SetValue(HelpLabelVisibilityProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store the boolean.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HelpLabelVisibilityProperty =
            DependencyProperty.Register("HelpLabelVisibility", typeof(Visibility), typeof(ScreenTip), new UIPropertyMetadata(Visibility.Visible));
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
        private IInputElement focusedElement;

        private void OnToolTipClosed(object sender, RoutedEventArgs e)
        {
            if (this.focusedElement == null)
            {
                return;
            }

            this.focusedElement.PreviewKeyDown -= this.OnFocusedElementPreviewKeyDown;
            this.focusedElement = null;
        }

        private void OnToolTipOpened(object sender, RoutedEventArgs e)
        {
            if (this.HelpTopic == null)
            {
                return;
            }

            this.focusedElement = Keyboard.FocusedElement;
            if (this.focusedElement != null)
            {
                this.focusedElement.PreviewKeyDown += this.OnFocusedElementPreviewKeyDown;
            }
        }

        void OnFocusedElementPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.F1)
            {
                return;
            }

            e.Handled = true;

            if (HelpPressed != null)
            {
                HelpPressed(null, new ScreenTipHelpEventArgs(this.HelpTopic));
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
            this.HelpTopic = helpTopic;
        }
    }
}