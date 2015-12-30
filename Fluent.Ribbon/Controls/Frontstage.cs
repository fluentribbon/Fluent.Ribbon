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
        public bool Shown
        {
            get { return (bool)GetValue( ShownProperty ); }
            set { SetValue( ShownProperty, value ); }
        }

        public static readonly DependencyProperty ShownProperty =
            DependencyProperty.Register( "Shown", typeof( bool ), typeof( Frontstage ), new PropertyMetadata( false ) );
        #endregion

        protected override bool Show()
        {
            //if( this.Shown ) return false;
            //return this.Shown = base.Show();

            return base.Show();
        }
    }
}
