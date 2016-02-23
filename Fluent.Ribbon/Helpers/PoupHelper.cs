namespace Fluent.Helpers
{
    using System.Windows;
    using System.Windows.Controls.Primitives;

    /// <summary>
    /// Helper class to position <see cref="Popup"/>.
    /// </summary>
    public static class PopupHelper
    {
        /// <summary>
        /// Positions <see cref="Popup"/> like <see cref="PlacementMode.Relative"/> would but ignores the value of <see cref="SystemParameters.MenuDropAlignment"/>.
        /// </summary>
        public static CustomPopupPlacementCallback SimplePlacementCallback => GetSimplePlacement;

        /// <summary>
        /// Gets the <see cref="CustomPopupPlacement"/> values for a <see cref="Popup"/> like <see cref="PlacementMode.Relative"/> would but ignores the value of <see cref="SystemParameters.MenuDropAlignment"/>.
        /// </summary>
        public static CustomPopupPlacement[] GetSimplePlacement(Size popupSize, Size targetSize, Point offset)
        {
            return new[]
                   {
                       new CustomPopupPlacement
                       {
                           Point = new Point(0, 0),
                           PrimaryAxis = PopupPrimaryAxis.None
                       }
                   };
        }
    }
}