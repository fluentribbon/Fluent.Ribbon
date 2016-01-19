
namespace Fluent.AttachedProperties
{
    using System;
    using System.Windows;

    public class Margins
    {
        #region Right Property
        public static readonly DependencyProperty RightProperty = DependencyProperty.RegisterAttached(
         "Right",
         typeof( int ),
         typeof( Margins ),
         new UIPropertyMetadata( OnRightPropertyChanged ) );

        public static int GetRight( FrameworkElement element )
        {
            return (int)element.GetValue( RightProperty );
        }

        public static void SetRight( FrameworkElement element, string value )
        {
            element.SetValue( RightProperty, value );
        }

        private static void OnRightPropertyChanged( DependencyObject obj, DependencyPropertyChangedEventArgs args )
        {
            var element = obj as FrameworkElement;

            if( element != null )
            {
                var margin = element.Margin;
                margin.Right = (int)args.NewValue;
                element.Margin = margin;
            }
        }

        #endregion

        #region Left Property
        public static readonly DependencyProperty LeftProperty = DependencyProperty.RegisterAttached(
           "Left",
           typeof( int ),
           typeof( Margins ),
           new UIPropertyMetadata( OnLeftPropertyChanged ) );

        public static int GetLeft( FrameworkElement element )
        {
            return (int)element.GetValue( LeftProperty );
        }

        public static void SetLeft( FrameworkElement element, string value )
        {
            element.SetValue( LeftProperty, value );
        }

        private static void OnLeftPropertyChanged( DependencyObject obj, DependencyPropertyChangedEventArgs args )
        {
            var element = obj as FrameworkElement;

            if( element != null )
            {
                var margin = element.Margin;
                margin.Left = (int)args.NewValue;
                element.Margin = margin;
            }
        }
        #endregion

        #region Top Property
        public static readonly DependencyProperty TopProperty = DependencyProperty.RegisterAttached(
           "Top",
           typeof( int ),
           typeof( Margins ),
           new UIPropertyMetadata( OnTopPropertyChanged ) );

        public static int GetTop( FrameworkElement element )
        {
            return (int)element.GetValue( TopProperty );
        }

        public static void SetTop( FrameworkElement element, string value )
        {
            element.SetValue( TopProperty, value );
        }

        private static void OnTopPropertyChanged( DependencyObject obj, DependencyPropertyChangedEventArgs args )
        {
            var element = obj as FrameworkElement;

            if( element != null )
            {
                var margin = element.Margin;
                margin.Top = (int)args.NewValue;
                element.Margin = margin;
            }
        }
        #endregion

        #region Bottom Property
        public static readonly DependencyProperty BottomProperty = DependencyProperty.RegisterAttached(
           "Bottom",
           typeof( int ),
           typeof( Margins ),
           new UIPropertyMetadata( OnBottomPropertyChanged ) );

        public static int GetBottom( FrameworkElement element )
        {
            return (int)element.GetValue( BottomProperty );
        }

        public static void SetBottom( FrameworkElement element, string value )
        {
            element.SetValue( BottomProperty, value );
        }

        private static void OnBottomPropertyChanged( DependencyObject obj, DependencyPropertyChangedEventArgs args )
        {
            var element = obj as FrameworkElement;

            if( element != null )
            {
                var margin = element.Margin;
                margin.Bottom = (int)args.NewValue;
                element.Margin = margin;
            }
        }
        #endregion
    }
}
