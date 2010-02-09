using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Fluent
{
    public class RibbonListBox:ListBox
    {
        #region Field

        private ItemsPresenter presenter;

        #endregion

        #region Properties

        public double InnerPanelWidth
        {
            get { if (presenter == null) return 0; else return presenter.DesiredSize.Width; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static RibbonListBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonListBox), new FrameworkPropertyMetadata(typeof(RibbonListBox)));            
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonListBox()
        {
            Focusable = false;
            FocusManager.SetIsFocusScope(this, false);
        }

        #endregion

        #region Overrides               

        /// <summary>
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new GalleryItem();
        }

        /// <summary>
        /// Determines if the specified item is (or is eligible to be) its own container.
        /// </summary>
        /// <param name="item">The item to check.</param>
        /// <returns>true if the item is (or is eligible to be) its own container; otherwise, false.</returns>
        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return (item is GalleryItem);
        }

        public override void OnApplyTemplate()
        {
            presenter = GetTemplateChild("PART_ItemsPresenter") as ItemsPresenter;
        }

        #endregion
    }
}
