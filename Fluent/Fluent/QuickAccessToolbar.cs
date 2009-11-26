using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Fluent
{
    public class QuickAccessToolbar:ToolBar
    {
        #region Initialize

        static QuickAccessToolbar()
        {
            
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
        }

        #endregion
    }
}
