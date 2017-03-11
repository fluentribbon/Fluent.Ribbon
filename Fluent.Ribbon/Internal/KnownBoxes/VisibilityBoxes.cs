namespace Fluent.Internal.KnownBoxes
{
    using System;
    using System.Windows;

    internal static class VisibilityBoxes
    {
        internal static object Visible = Visibility.Visible;

        internal static object Hidden = Visibility.Hidden;

        internal static object Collapsed = Visibility.Collapsed;

        internal static object Box(Visibility value)
        {
            switch (value)
            {
                case Visibility.Visible:
                    return Visible;

                case Visibility.Hidden:
                    return Hidden;

                case Visibility.Collapsed:
                    return Collapsed;

                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }
}