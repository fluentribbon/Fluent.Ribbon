namespace FluentTest.ViewModels
{
    using FluentTest.ViewModels.IssueRepros;

    public class IssueReprosViewModel
    {
        public IssueReprosViewModel()
        {
            this.ThemeManagerFromThread = new ThemeManagerFromThread();
        }

        public ThemeManagerFromThread ThemeManagerFromThread { get; }
    }
}