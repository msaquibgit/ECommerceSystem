namespace NotificationService.Application.Interfaces
{
    public interface ITemplateRenderer
    {
        // Renders tokenized subject/content with data e.g., {OrderNumber}
        
            string Render(string template, IReadOnlyDictionary<string, object>? data);
        

    }
}
