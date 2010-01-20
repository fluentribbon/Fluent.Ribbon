using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Fluent
{
    [StyleTypedProperty(Property = "ItemContainerStyle", StyleTargetType = typeof(GalleryItem))]
    public class GalleryGroup:HeaderedItemsControl
    {
        #region Properties

        /// <summary>
        /// Gets or sets whether group header is visible
        /// </summary>
        public bool IsHeaderVisible
        {
            get { return (bool)GetValue(IsHeaderVisibleProperty); }
            set { SetValue(IsHeaderVisibleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsHeaderVisible.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsHeaderVisibleProperty =
            DependencyProperty.Register("IsHeaderVisible", typeof(bool), typeof(GalleryGroup), new UIPropertyMetadata(false));

        /// <summary>
        /// Gets or sets gallery group icon
        /// </summary>
        public object Icon
        {
            get { return (object)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(object), typeof(GalleryGroup), new UIPropertyMetadata(null, OnIconChanged));

        // Handles icon changed
        private static void OnIconChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if(e.NewValue!=null)(d as GalleryGroup).HasIcon = true;
            else (d as GalleryGroup).HasIcon = false;
        }

        /// <summary>
        /// Gets whether group has icon
        /// </summary>
        public bool HasIcon
        {
            get { return (bool)GetValue(HasIconProperty); }
            private set { SetValue(HasIconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for HasIcon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty HasIconProperty =
            DependencyProperty.Register("HasIcon", typeof(bool), typeof(GalleryGroup), new UIPropertyMetadata(false));

        #endregion

        #region Constructors

        /// <summary>
        /// Static constructor
        /// </summary>
        static GalleryGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GalleryGroup), new FrameworkPropertyMetadata(typeof(GalleryGroup)));
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public GalleryGroup()
        {
            
        }
        
        #endregion

        #region Overrides

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
        /// Creates or identifies the element that is used to display the given item.
        /// </summary>
        /// <returns>The element that is used to display the given item.</returns>
        protected override DependencyObject GetContainerForItemOverride()
        {
            return new GalleryItem();
        }

        #endregion
    }
}
