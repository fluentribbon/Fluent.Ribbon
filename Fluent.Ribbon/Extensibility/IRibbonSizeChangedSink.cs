namespace Fluent.Extensibility
{
    /// <summary>
    /// Interface which is used to signal size changes
    /// </summary>
    public interface IRibbonSizeChangedSink
    {
        /// <summary>
        /// Called when the size is changed
        /// </summary>
        /// <param name="previous">Size before change</param>
        /// <param name="current">Size after change</param>
        void OnSizePropertyChanged(RibbonControlSize previous, RibbonControlSize current);
    }
}