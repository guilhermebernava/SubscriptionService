namespace Domain.Entities;
public class Template
{
    public Template()
    {
        
    }
    public Template(string templateHtml,string userId )
    {
        Id = Guid.NewGuid().ToString();
        TemplateHtml = templateHtml;
        UserId = userId;    
    }

    public string Id { get; set; }
    public string TemplateHtml { get; set; }
    public string UserId { get; set; }
}
