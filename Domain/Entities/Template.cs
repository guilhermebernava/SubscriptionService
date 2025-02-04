namespace Domain.Entities;
public class Template
{
    public Template()
    {
        
    }
    public Template(string templateHtml)
    {
        Id = Guid.NewGuid().ToString();
        TemplateHtml = templateHtml;
    }

    public string Id { get; set; }
    public string TemplateHtml { get; set; }
}
