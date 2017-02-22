namespace Fluent
{
    public interface IRibbonWindow
    {
        double TitleBarHeight { get; }

        RibbonTitleBar TitleBar { get; }
    }
}