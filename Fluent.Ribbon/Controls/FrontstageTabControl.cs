using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Fluent
{
    public class FrontstageTabControl : BackstageTabControl
    {
        #region LeftContent
        public object LeftContent
        {
            get { return (object)GetValue( LeftContentProperty ); }
            set { SetValue( LeftContentProperty, value ); }
        }

        public static readonly DependencyProperty LeftContentProperty =
            DependencyProperty.Register( "LeftContent", typeof( object ), typeof( FrontstageTabControl ) );
        #endregion

        #region RightContent
        public object RightContent
        {
            get { return (object)GetValue( RightContentProperty ); }
            set { SetValue( RightContentProperty, value ); }
        }

        public static readonly DependencyProperty RightContentProperty =
            DependencyProperty.Register( "RightContent", typeof( object ), typeof( FrontstageTabControl ) );

        #endregion

        public FrontstageTabControl()
        {
            this.Loaded += FrontStage_Loaded;
        }

        void FrontStage_Loaded( object sender, RoutedEventArgs e )
        {
            var template = Application.Current.
                FindResource( "FrontstageControlTemplate" ) as ControlTemplate;

            if( template != null )
                this.Template = template;
        }
    }
}
