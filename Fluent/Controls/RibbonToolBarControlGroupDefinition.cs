﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;

namespace Fluent
{
    /// <summary>
    /// Represent logical container for toolbar items
    /// </summary>
    [ContentProperty("Children")]
    public class RibbonToolBarControlGroupDefinition : DependencyObject
    {
        #region Events

        /// <summary>
        /// Occures when children has been changed
        /// </summary>
        public event NotifyCollectionChangedEventHandler ChildrenChanged;

        #endregion

        #region Fields

        // User defined rows
        readonly ObservableCollection<RibbonToolBarControlDefinition> children = new ObservableCollection<RibbonToolBarControlDefinition>();

        #endregion

        #region Children Property

        /// <summary>
        /// Gets rows
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ObservableCollection<RibbonToolBarControlDefinition> Children
        {
            get { return this.children; }
        }

        #endregion

        #region Initialization

        /// <summary>
        /// Default constructor
        /// </summary>
        public RibbonToolBarControlGroupDefinition()
        {
            this.children.CollectionChanged += this.OnChildrenCollectionChanged;
        }

        void OnChildrenCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (this.ChildrenChanged != null)
                this.ChildrenChanged(sender, e);
        }

        #endregion
    }
}