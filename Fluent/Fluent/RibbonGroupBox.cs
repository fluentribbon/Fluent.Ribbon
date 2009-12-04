using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
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
    [TemplatePart(Name = "PART_DownGrid", Type = typeof(Grid))]
    [TemplatePart(Name = "PART_UpPanel", Type = typeof(Panel))]
    public class RibbonGroupBox :ItemsControl
    {
        #region Fields

        private Button dialogLauncherButton = null;

        private Popup popup = null;

        private Grid downGrid = null;
        private Panel upPanel = null;

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

            SetChildSizes(ribbonGroupBoxState, ribbonGroupBox);

        }

        private static void SetChildSizes(RibbonGroupBoxState ribbonGroupBoxState, RibbonGroupBox ribbonGroupBox)
        {                            
            for (int i = 0; i < ribbonGroupBox.Items.Count; i++)
            {
                RibbonControl.SetAppropriateSize((UIElement)ribbonGroupBox.Items[i], ribbonGroupBoxState);
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


        protected override System.Collections.IEnumerator LogicalChildren
        {
            get
            {
                ArrayList array = new ArrayList();
                array.AddRange(Items);
                return array.GetEnumerator();
            }
        }

        #endregion

        #region Events

        public event RoutedEventHandler DialogLauncherButtonClick;

        #endregion

        #region Initialize

        static RibbonGroupBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonGroupBox), new FrameworkPropertyMetadata(typeof(RibbonGroupBox)));

            /*EventManager.RegisterClassHandler(typeof(RibbonGroupBox), Mouse.PreviewMouseDownOutsideCapturedElementEvent, new MouseButtonEventHandler(OnClickThroughThunk));
            EventManager.RegisterClassHandler(typeof(RibbonGroupBox), Mouse.PreviewMouseUpOutsideCapturedElementEvent, new MouseButtonEventHandler(OnClickThroughThunk));*/
        }

        public RibbonGroupBox()
        {
            AddHandler(Button.ClickEvent, new RoutedEventHandler(OnClick));
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            if(State==RibbonGroupBoxState.Collapsed)IsOpen = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets a panel with items
        /// </summary>
        /// <returns></returns>
        internal Panel GetPanel() { return upPanel; }

        #endregion

        #region Overrides

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Visual visual in e.NewItems)
                {
                    RibbonControl.SetAppropriateSize((UIElement) visual, State);
                }
            }
            base.OnItemsChanged(e);
        }

        public override void OnApplyTemplate()
        {
            if (dialogLauncherButton != null) dialogLauncherButton.Click -= OnDialogLauncherButtonClick;
            dialogLauncherButton = GetTemplateChild("PART_DialogLauncherButton") as Button;
            if (dialogLauncherButton != null) dialogLauncherButton.Click += OnDialogLauncherButtonClick;
            
            //if (popup != null) RemoveLogicalChild(popup);
            popup = GetTemplateChild("PART_Popup") as Popup;
            if(popup!=null)
            {
                //if (popup.Parent != null) (popup.Parent as Panel).Children.Remove(popup);
                //AddLogicalChild(popup);
                Binding binding = new Binding("IsOpen");
                binding.Mode = BindingMode.TwoWay;
                binding.Source = this;
                popup.SetBinding(Popup.IsOpenProperty, binding);
            }

            downGrid = GetTemplateChild("PART_DownGrid") as Grid;
            upPanel = GetTemplateChild("PART_UpPanel") as Panel;
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            if ((State == RibbonGroupBoxState.Collapsed)&&(popup!=null)&&(!IsOpen))
            {                
                IsOpen = true;
                Mouse.Capture(popup, CaptureMode.Element);
                e.Handled = true;
            }
        }

        /*protected override void OnLostMouseCapture(MouseEventArgs e)
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
        }*/

        protected override Size MeasureOverride(Size constraint)
        {
            if ((upPanel == null) || (downGrid == null)) return base.MeasureOverride(constraint);
            upPanel.Measure(new Size(double.PositiveInfinity, constraint.Height));
            double width = upPanel.DesiredSize.Width + upPanel.Margin.Left + upPanel.Margin.Right;
            Size size = new Size(width,constraint.Height);
            (upPanel.Parent as Grid).Measure(size);
            return size;
        }

        #endregion

        #region Event handling

        private void OnDialogLauncherButtonClick(object sender, RoutedEventArgs e)
        {
            if (DialogLauncherButtonClick != null) DialogLauncherButtonClick(this, e);
        }
        /*
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
        */
        private void OnRibbonGroupBoxPopupClosing()
        {
            /*if (Mouse.Captured == this)
            {
                Mouse.Capture(null);
            }*/
            IsHitTestVisible = true;
        }

        private void OnRibbonGroupBoxPopupOpening()
        {
            /*if (State == RibbonGroupBoxState.Collapsed)
            {
                Mouse.Capture(this, CaptureMode.SubTree);
            }*/
            IsHitTestVisible = false;
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
