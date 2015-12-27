using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        /// <summary>
        /// Static constructor
        /// </summary>
        [SuppressMessage( "Microsoft.Performance", "CA1810" )]
        static FrontstageTabControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata( typeof( FrontstageTabControl ), new FrameworkPropertyMetadata( typeof( FrontstageTabControl ) ) );
            StyleProperty.OverrideMetadata( typeof( FrontstageTabControl ), new FrameworkPropertyMetadata( null, new CoerceValueCallback( OnCoerceStyle ) ) );
        }

        static object OnCoerceStyle( DependencyObject d, object basevalue )
        {
            if( basevalue == null )
            {
                basevalue = (d as FrameworkElement).TryFindResource( typeof( FrontstageTabControl ) );
            }

            return basevalue;
        }
    }
}
