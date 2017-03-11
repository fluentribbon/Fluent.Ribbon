using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

// ReSharper disable once CheckNamespace
namespace Fluent
{
    using Fluent.Internal.KnownBoxes;

    /// <summary>
    /// Represent logical container for toolbar items
    /// </summary>
    [ContentProperty(nameof(Items))]
    public class RibbonToolBarControlGroup : ItemsControl
    {
        #region Properties

        /// <summary>
        /// Gets whether the group is the fisrt control in the row
        /// </summary>
        public bool IsFirstInRow
        {
            get { return (bool)this.GetValue(IsFirstInRowProperty); }
            set { this.SetValue(IsFirstInRowProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsFirstInRow.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsFirstInRowProperty =
            DependencyProperty.Register(nameof(IsFirstInRow), typeof(bool), typeof(RibbonToolBarControlGroup), new PropertyMetadata(BooleanBoxes.TrueBox));

        /// <summary>
        /// Gets whether the group is the last control in the row
        /// </summary>
        public bool IsLastInRow
        {
            get { return (bool)this.GetValue(IsLastInRowProperty); }
            set { this.SetValue(IsLastInRowProperty, value); }
        }

        /// <summary>
        /// Using a DependencyProperty as the backing store for IsFirstInRow.  
        /// This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty IsLastInRowProperty =
            DependencyProperty.Register(nameof(IsLastInRow), typeof(bool), typeof(RibbonToolBarControlGroup), new PropertyMetadata(BooleanBoxes.TrueBox));

        #endregion

        [SuppressMessage("Microsoft.Performance", "CA1810")]
        static RibbonToolBarControlGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonToolBarControlGroup), new FrameworkPropertyMetadata(typeof(RibbonToolBarControlGroup)));
        }
    }
}