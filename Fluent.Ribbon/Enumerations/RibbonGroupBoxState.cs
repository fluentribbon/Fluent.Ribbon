namespace Fluent
{
    /// <summary>
    /// Represents states of ribbon group 
    /// </summary>
    public enum RibbonGroupBoxState
    {
        /// <summary>
        /// Large. All controls in the group will try to be large size
        /// </summary>
        Large = 0,

        /// <summary>
        /// Middle. All controls in the group will try to be middle size
        /// </summary>
        Middle,

        /// <summary>
        /// Small. All controls in the group will try to be small size
        /// </summary>
        Small,

        /// <summary>
        /// Collapsed. Group will collapse its content in a single button
        /// </summary>
        Collapsed,

        /// <summary>
        /// QuickAccess. Group will collapse its content in a single button in quick access toolbar
        /// </summary>
        QuickAccess
    }
}