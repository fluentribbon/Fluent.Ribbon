using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Fluent
{
    public class GalleryGroupFilter:DependencyObject
    {
        #region Properties

        /// <summary>
        /// Gets or sets title of filter
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(GalleryGroupFilter), new UIPropertyMetadata("GalleryGroupFilter"));
               
        /// <summary>
        /// Gets or sets list pf groups splitted by comma
        /// </summary>
        public string Groups
        {
            get { return (string)GetValue(GroupsProperty); }
            set { SetValue(GroupsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Groups.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupsProperty =
            DependencyProperty.Register("Groups", typeof(string), typeof(GalleryGroupFilter), new UIPropertyMetadata(""));

        #endregion
    }
}
