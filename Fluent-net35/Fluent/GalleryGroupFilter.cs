#region Copyright and License Information
// Fluent Ribbon Control Suite
// http://fluent.codeplex.com/
// Copyright (c) Degtyarev Daniel, Rikker Serg. 2009-2010.  All rights reserved.
// 
// Distributed under the terms of the Microsoft Public License (Ms-PL). 
// The license is available online http://fluent.codeplex.com/license
#endregion

using System.Windows;

namespace Fluent
{
    /// <summary>
    /// Represents gallery group filter definition
    /// </summary>
    public class GalleryGroupFilter : DependencyObject
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
        /// Using a DependencyProperty as the backing store for Title.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), 
            typeof(GalleryGroupFilter), new UIPropertyMetadata("GalleryGroupFilter"));
               
        /// <summary>
        /// Gets or sets list pf groups splitted by comma
        /// </summary>
        public string Groups
        {
            get { return (string)GetValue(GroupsProperty); }
            set { SetValue(GroupsProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for ContextualGroups.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupsProperty =
            DependencyProperty.Register("ContextualGroups", typeof(string), 
            typeof(GalleryGroupFilter), new UIPropertyMetadata(""));

        #endregion
    }
}
