namespace Fluent
{
    /// <summary>
    /// Base interface for controls requiring simplified state
    /// </summary>
    public interface ISimplifiedStateControl
    {
        /// <summary>
        /// Update simplified state.
        /// </summary>
        void UpdateSimplifiedState(bool isSimplified);
    }
}
