using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Fluent
{
    /// <summary>
    /// Represents states of ribbon group 
    /// </summary>
    public enum RibbonGroupBoxState
    {
        /// <summary>
        /// Large. All controls in the group will try to be large size
        /// </summary>
        Large = 0,
        /// <summary>
        /// Middle. All controls in the group will try to be middle size
        /// </summary>
        Middle,
        /// <summary>
        /// Small. All controls in the group will try to be small size
        /// </summary>
        Small,
        /// <summary>
        /// Collapsed. Group will collapse its content in a single button
        /// </summary>
        Collapsed
    }

    /// <summary>
    /// RibbonGroup represents a logical group of controls as they appear on
    /// a RibbonTab.  These groups can resize its content
    /// </summary>
    [TemplatePart(Name = "PART_DialogLauncherButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    public class RibbonGroupBox :GroupBox//ItemsControl
    {
        #region Fields

        private Button dialogLauncherButton = null;

        private Popup popup = null;

        #endregion

        #region Properties

        #region State

        /// <summary>
        /// Gets or sets the current state of the group
        /// </summary>
        public RibbonGroupBoxState State
        {
            get { return (RibbonGroupBoxState)GetValue(StateProperty); }
            set { SetValue(StateProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for State.  
        /// This enables animation, styling, binding, etc...
        /// </summary> 
        public static readonly DependencyProperty StateProperty =
            DependencyProperty.Register("State", typeof(RibbonGroupBoxState), typeof(RibbonGroupBox), new UIPropertyMetadata(RibbonGroupBoxState.Large, StatePropertyChanged));

        static void StatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGroupBox ribbonGroupBox = (RibbonGroupBox)d;
            RibbonGroupBoxState ribbonGroupBoxState = (RibbonGroupBoxState)e.NewValue;

            if (ribbonGroupBoxState == RibbonGroupBoxState.Collapsed)
            {
                // TODO: implement collapsed state
                //ribbonGroupBox.Width = double.NaN;
                /*for (int i = 0; i < ribbonGroupBox.Items.Count; i++)
                {
                    RibbonControl.SetSize((UIElement)ribbonGroupBox.Items[i], RibbonControlSize.Large);
                }*/
            }
            else
            {
                // TODO: implement it properly
                /*switch (ribbonGroupBoxState)
                {
                    case RibbonGroupBoxState.Large: (ribbonGroupBox.Content as Button).Width = 300; break;
                    case RibbonGroupBoxState.Middle: (ribbonGroupBox.Content as Button).Width = 150; break;
                    case RibbonGroupBoxState.Small: (ribbonGroupBox.Content as Button).Width = 50; break;
                }*/
                //(ribbonGroupBox.Parent as RibbonGroupsContainer).InvalidateMeasure();
                /*switch(ribbonGroupBoxState)
                {
                    case RibbonGroupBoxState.Large: ribbonGroupBox.Width = 300; break;
                    case RibbonGroupBoxState.Middle: ribbonGroupBox.Width = 150; break;
                    case RibbonGroupBoxState.Small: ribbonGroupBox.Width = 50; break;
                }*/
                UIElementCollection Items = (ribbonGroupBox.Content as WrapPanel).Children;
                for(int i=0;i<Items.Count;i++)
                {
                    //RibbonControl.SetSize((UIElement)ribbonGroupBox.Items[i], (RibbonControlSize)ribbonGroupBoxState);
                    if(ribbonGroupBoxState==RibbonGroupBoxState.Large)
                    {
                        (Items[i] as Button).Width = 100;
                        (Items[i] as Button).Height = 66;
                    }
                    else if (ribbonGroupBoxState == RibbonGroupBoxState.Middle)
                    {
                        (Items[i] as Button).Width = 50;
                        (Items[i] as Button).Height = 22;
                    }
                    else if (ribbonGroupBoxState == RibbonGroupBoxState.Small)
                    {
                        (Items[i] as Button).Width = 22;
                        (Items[i] as Button).Height = 22;
                    }
                }
                //(ribbonGroupBox.Parent as RibbonGroupsContainer).InvalidateMeasure();
                //(ribbonGroupBox.Content as WrapPanel).InvalidateArrange();
                //(VisualTreeHelper.GetParent(ribbonGroupBox.Items[0] as DependencyObject) as WrapPanel).InvalidateArrange();
                //(VisualTreeHelper.GetParent(ribbonGroupBox.Items[0] as DependencyObject) as WrapPanel).InvalidateMeasure();
                //ribbonGroupBox.InvalidateVisual();
                //ribbonGroupBox.UpdateLayout();
            }
        }

        #endregion

        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(RibbonGroupBox), new UIPropertyMetadata("RibbonGroupBox"));

        public bool IsDialogLauncherButtonVisible
        {
            get { return (bool)GetValue(IsDialogLauncherButtonVisibleProperty); }
            set { SetValue(IsDialogLauncherButtonVisibleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDialogLauncherButtonVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDialogLauncherButtonVisibleProperty =
            DependencyProperty.Register("IsDialogLauncherButtonVisible", typeof(bool), typeof(RibbonGroupBox), new UIPropertyMetadata(false));

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(bool), typeof(RibbonGroupBox), new UIPropertyMetadata(false, OnIsOpenChanged));



        public ImageSource Image
        {
            get { return (ImageSource)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Image.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ImageProperty =
            DependencyProperty.Register("Image", typeof(ImageSource), typeof(RibbonGroupBox), new UIPropertyMetadata(null));



        #endregion

        #region Events

        public event RoutedEventHandler DialogLauncherButtonClick;

        #endregion

        #region Initialize

        static RibbonGroupBox()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonGroupBox), new FrameworkPropertyMetadata(typeof(RibbonGroupBox)));

            EventManager.RegisterClassHandler(typeof(RibbonGroupBox), Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(OnClickThroughThunk));
            EventManager.RegisterClassHandler(typeof(RibbonGroupBox), Mouse.PreviewMouseUpOutsideCapturedElementEvent, new MouseButtonEventHandler(OnClickThroughThunk));
        }

        #endregion

        #region Overrides

        public override void OnApplyTemplate()
        {
            if (dialogLauncherButton != null) dialogLauncherButton.Click -= OnDialogLauncherButtonClick;
            dialogLauncherButton = GetTemplateChild("PART_DialogLauncherButton") as Button;
            if (dialogLauncherButton != null) dialogLauncherButton.Click += OnDialogLauncherButtonClick;

            popup = GetTemplateChild("PART_Popup") as Popup;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if (State == RibbonGroupBoxState.Collapsed) IsOpen = !IsOpen;
        }

        protected override void OnLostMouseCapture(MouseEventArgs e)
        {
            if (popup != null)
            {
                if (Mouse.Captured != this)
                {
                    UIElement selectedPopupChild = popup.Child;
                    if (e.OriginalSource == this)
                    {
                        // If Ribbon loses capture because something outside popup is clicked - close the popup
                        if (Mouse.Captured == null ||
                            !selectedPopupChild.IsAncestorOf(Mouse.Captured as DependencyObject))
                        {
                            this.IsOpen = false;
                        }
                    }
                    else
                    {
                        // If control inside Ribbon loses capture - restore capture to Ribbon
                        if (selectedPopupChild.IsAncestorOf(e.OriginalSource as DependencyObject))
                        {
                            if (this.IsOpen && Mouse.Captured == null)
                            {
                                Mouse.Capture(this, CaptureMode.SubTree);
                                e.Handled = true;
                            }
                        }
                        else
                        {
                            this.IsOpen = false;
                        }
                    }
                }
            }
            base.OnLostMouseCapture(e);
        }

        #endregion

        #region Event handling

        private void OnDialogLauncherButtonClick(object sender, RoutedEventArgs e)
        {
            if (DialogLauncherButtonClick != null) DialogLauncherButtonClick(this, e);
        }

        private static void OnClickThroughThunk(object sender, MouseButtonEventArgs e)
        {
            RibbonGroupBox ribbon = (RibbonGroupBox)sender;
            if (e.ChangedButton == MouseButton.Left || e.ChangedButton == MouseButton.Right)
            {
                if (Mouse.Captured == ribbon)
                {
                    ribbon.IsOpen = false;
                    Mouse.Capture(null);
                }
            }
        }

        private void OnRibbonGroupBoxPopupClosing()
        {
            if (Mouse.Captured == this)
            {
                Mouse.Capture(null);
            }
        }

        private void OnRibbonGroupBoxPopupOpening()
        {
            if (State == RibbonGroupBoxState.Collapsed)
            {
                Mouse.Capture(this, CaptureMode.SubTree);
            }
        }

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            RibbonGroupBox ribbon = (RibbonGroupBox)d;

            if (ribbon.IsOpen)
            {
                ribbon.OnRibbonGroupBoxPopupOpening();
            }
            else
            {
                ribbon.OnRibbonGroupBoxPopupClosing();
            }
        }

        #endregion
    }
}
