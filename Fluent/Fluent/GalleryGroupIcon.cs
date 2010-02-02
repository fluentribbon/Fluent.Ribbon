using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace Fluent
{
    public class GalleryGroupIcon : DependencyObject
    {

        /// <summary>
        /// Gets or sets group name
        /// </summary>
        public string GroupName
        {
            get { return (string)GetValue(GroupNameProperty); }
            set { SetValue(GroupNameProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for GroupName.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register("GroupName", typeof(string), typeof(GalleryGroupIcon), new UIPropertyMetadata(null));


        /// <summary>
        /// Gets or sets group icon
        /// </summary>
        public ImageSource Icon
        {
            get { return (ImageSource)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for Icon.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(ImageSource), typeof(GalleryGroupIcon),
                                        new UIPropertyMetadata(null));
    }
}
