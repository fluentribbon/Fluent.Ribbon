﻿namespace Fluent
{
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Represents control that have drop down popup
    /// </summary>
    public interface IDropDownControl
    {
        /// <summary>
        /// Gets drop down popup
        /// </summary>
        Popup DropDownPopup { get; }

        /// <summary>
        /// Gets a value indicating whether control context menu is opened
        /// </summary>
        bool IsContextMenuOpened { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether drop down is opened
        /// </summary>
        bool IsDropDownOpen { get; set; }
    }
}