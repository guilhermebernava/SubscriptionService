using Amazon.SQS;
using Amazon.SQS.Model;

namespace Services.Services;

public class SqsClient
{
    private readonly IAmazonSQS _sqsClient;

    public SqsClient(IAmazonSQS sqsClient)
    {
        
        _sqsClient = sqsClient;
    }

    public SqsClient(string accessKey, string secretKey, string region)
    {
        var config = new AmazonSQSConfig
        {
            RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region)
        };
        _sqsClient = new AmazonSQSClient(accessKey, secretKey, config);
    }

    public async Task<string> SendMessageAsync(string queueUrl, string messageBody)
    {
        var request = new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageBody = messageBody
        };
        var response = await _sqsClient.SendMessageAsync(request);
        return response.MessageId;
    }

    public async Task<List<Message>> ReceiveMessagesAsync(string queueUrl, int maxMessages = 1)
    {
        var request = new ReceiveMessageRequest
        {
            QueueUrl = queueUrl,
            MaxNumberOfMessages = maxMessages,
            WaitTimeSeconds = 10
        };
        var response = await _sqsClient.ReceiveMessageAsync(request);
        return response.Messages;
    }
}
