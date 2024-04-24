namespace BlazorApp2.Client
{
    public interface IPageTitleService
    {
        string PageTitle { get; }
        event Func<string, Task> OnPageTitleUpdated;
        Task UpdatePageTitleAsync();
    }

    public class PageTitleService : IPageTitleService
    {
        public string PageTitle { get; private set; }
        public event Func<string, Task> OnPageTitleUpdated;

        public async Task UpdatePageTitleAsync()
        {
            string title = "Главная";
            PageTitle = title;
            if (OnPageTitleUpdated != null)
            {
                await OnPageTitleUpdated.Invoke(title);
            }
        }
    }

}
