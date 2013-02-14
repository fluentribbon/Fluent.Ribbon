namespace Fluent
{
    /// <summary>
    /// Base interface for controls supports key tips
    /// </summary>
    public interface IKeyTipedControl
    {
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