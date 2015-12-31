using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Fluent;

namespace Fluent
{
    public class Frontstage : Backstage
    {
        #region Shown
        /// <summary>
        /// Indicates whether the Frontstage has aleaady been shown or not
        /// </summary>
        public bool Shown
        {
            get { return (bool)GetValue( ShownProperty ); }
            set { SetValue( ShownProperty, value ); }
        }

        public static readonly DependencyProperty ShownProperty =
            DependencyProperty.Register( "Shown", typeof( bool ), typeof( Frontstage ), new FrameworkPropertyMetadata( false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, null ) );
        #endregion

        protected override bool Show()
        {
            var ribbon = base.FindRibbon();
            ribbon.TitleBar.IsCollapsed = true;

            if( this.Shown ) return false;
            return this.Shown = base.Show();
        }
    }
}
