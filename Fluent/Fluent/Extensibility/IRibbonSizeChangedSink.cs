namespace Fluent.Extensibility
{
    public interface IRibbonSizeChangedSink
    {
        void OnSizePropertyChanged(RibbonControlSize previous, RibbonControlSize current);
    }
}