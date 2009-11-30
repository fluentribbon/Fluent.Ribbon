using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Fluent
{
    [TemplatePart(Name = "PART_MenuDownButton", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_MenuDownPopupButton", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_ToolbarDownButton", Type = typeof(ToggleButton))]
    [TemplatePart(Name = "PART_Menu", Type = typeof(Menu))]
    [TemplatePart(Name = "PART_MenuPopup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_ToolbarPopup", Type = typeof(Popup))]
    public class QuickAccessToolbar:ToolBar
    {
        #region Fields

        private ToggleButton menuDownButton = null;
        private ToggleButton menuDownPopupButton = null;
        private ToggleButton toolbarDownButton = null;
        private Menu menu = null;
        private Popup menuPopup = null;
        private Popup toolbarPopup = null;
        
        #endregion

        #region Initialize

        static QuickAccessToolbar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(QuickAccessToolbar), new FrameworkPropertyMetadata(typeof(QuickAccessToolbar)));            
        }

        public QuickAccessToolbar()
        {

        }

        #endregion

        #region Override

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (this.Parent is RibbonTitleBar) (this.Parent as RibbonTitleBar).InvalidateMeasure();
            base.OnItemsChanged(e);
            UpdateKeyTips();
        }

        public override void OnApplyTemplate()
        {
            /*if (menuDownButton != null) menuDownButton.PreviewMouseLeftButtonDown -= OnMenuDownButtonClick;
            if (menuDownPopupButton != null) menuDownPopupButton.PreviewMouseLeftButtonDown -= OnMenuDownButtonClick;
            if (toolbarDownButton != null) toolbarDownButton.PreviewMouseLeftButtonDown -= OnToolbatDownButtonClick;
            */
            menuDownButton = GetTemplateChild("PART_MenuDownButton") as ToggleButton;
            menuDownPopupButton = GetTemplateChild("PART_MenuDownButtonPopup") as ToggleButton;
            toolbarDownButton = GetTemplateChild("PART_ToolbarDownButton") as ToggleButton;
            /*
            if (menuDownButton != null) menuDownButton.PreviewMouseLeftButtonDown += OnMenuDownButtonClick;
            if (menuDownPopupButton != null) menuDownPopupButton.PreviewMouseLeftButtonDown += OnMenuDownButtonClick;
            if (toolbarDownButton != null) toolbarDownButton.PreviewMouseLeftButtonDown += OnToolbatDownButtonClick;
            */
            menu = GetTemplateChild("PART_Menu") as Menu;
            menuPopup = GetTemplateChild("PART_MenuPopup") as Popup;
            toolbarPopup = GetTemplateChild("PART_ToolbarPopup") as Popup;
        }


        #endregion

        #region Methods

        void UpdateKeyTips()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                // TODO: generate keys for quick access items properly
                if (Items[i] is UIElement) KeyTip.SetKeys((UIElement)Items[i], (i + 1).ToString());
            }
        }

        #endregion

        #region Event handling

        #endregion
    }
}
