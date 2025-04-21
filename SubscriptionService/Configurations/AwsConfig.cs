namespace WebApi.Subscription.Configurations;

public class AwsConfig
{
    public string Region { get; set; }
    public string AccessKey { get; set; }
    public string AccessSecret { get; set; }
    public string Queue { get; set; }
    public string Subscription { get; set; }
    public string Template { get; set; }
}

