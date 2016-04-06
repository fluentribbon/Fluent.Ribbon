namespace Fluent
{
    /// <summary>
    /// Base interface for controls supports key tips
    /// </summary>
    public interface IKeyTipedControl
    {
        /// <summary>
        /// Get and sets KeyTip for element.
        /// </summary>
        string KeyTip { get; set; }

        /// <summary>
        /// Handles key tip pressed
        /// </summary>
        void OnKeyTipPressed();

        /// <summary>
        /// Handles back navigation with KeyTips
        /// </summary>
        void OnKeyTipBack();
    }
}