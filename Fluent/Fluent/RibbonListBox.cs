#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright (c) Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Fluent
{
    /// <summary>
    /// Represents custom listbox
    /// </summary>
    [TemplatePart(Name = "PART_ItemsPresenter", Type = typeof(ItemsPresenter))]
    public class RibbonListBox : ListBox
    {
        #region Field

        ItemsPresenter presenter;

        #endregion

        #region Properties

        /// <summary>
        /// Gets width of the inner panel of the list box
        /// </summary>
        public double InnerPanelWidth
        {
            get { return presenter != null ? presenter.DesiredSize.Width : 0; }
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

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application 
        /// code or internal processes call ApplyTemplate
        /// </summary>
        public override void OnApplyTemplate()
        {
            presenter = GetTemplateChild("PART_ItemsPresenter") as ItemsPresenter;
        }

        #endregion
    }
}
