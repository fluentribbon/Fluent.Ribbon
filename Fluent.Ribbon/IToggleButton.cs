namespace Fluent
{
    /// <summary>
    /// Interface for controls that support <see cref="ToggleButton"/>-Behavior
    /// </summary>
    public interface IToggleButton
    {
        /// <summary>
        /// Gets or sets the name of the group that the toggle button belongs to. 
        /// Use the GroupName property to specify a grouping of toggle buttons to 
        /// create a mutually exclusive set of controls. You can use the GroupName 
        /// property when only one selection is possible from a list of available 
        /// options. When this property is set, only one ToggleButton in the specified
        /// group can be selected at a time.
        /// </summary>
        string GroupName { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether SplitButton is checked
        /// </summary>
        bool? IsChecked { get; set; }

        /// <summary>
        /// Gets a value that indicates whether the ToggleButton is fully loaded
        /// </summary>
        bool IsLoaded { get; }
    }
}