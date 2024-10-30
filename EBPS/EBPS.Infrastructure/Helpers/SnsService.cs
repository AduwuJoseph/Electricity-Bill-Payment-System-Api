using Amazon.SimpleNotificationService.Model;
using Amazon.SimpleNotificationService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Amazon;
using Amazon.SQS.Model;
using Amazon.SQS;
using System.Text.Json;
using EBPS.Contract.Dtos;

namespace EBPS.Infrastructure.Helpers
{
    public class SnsService
    {
        private static readonly string TopicArn = "<PUT TOPIC ARN HERE>";

        public static async Task<string> RemoveSub(IAmazonSimpleNotificationService client, string subArn)
        {
            var request = new UnsubscribeRequest();
            request.SubscriptionArn = subArn;
            await client.UnsubscribeAsync(request);

            return string.Empty;
        }

        public static async Task<SendMessageResponse> SendMessage(IAmazonSQS sqsClient, SendMessageRequest request)
        {
            // Consume the message from SQS queue
            var response = await sqsClient.SendMessageAsync(request);           
            return response;
        }

        public static async Task<string> CreateQueue(IAmazonSQS sqsClient, string SqsName)
        {
            var request = new CreateQueueRequest
            {
                QueueName = SqsName
            };
            var response = await sqsClient.CreateQueueAsync(request);
            return response.QueueUrl;
        }
        
        public static async Task<Message> ReceiveMessage(IAmazonSQS sqsClient, ReceiveMessageRequest request)
        {
            // Consume the message from SQS queue
            var receiveMessageResponse = await sqsClient.ReceiveMessageAsync(request);

            if (receiveMessageResponse.Messages.Count > 0)
            {
                var sqsMessage = receiveMessageResponse.Messages[0];
                return sqsMessage;
            }
            return null;
        }

        public static async Task<string> GetSubArn(IAmazonSimpleNotificationService client, string email)
        {
            var request = new ListSubscriptionsByTopicRequest();
            request.TopicArn = TopicArn;
            var subArn = string.Empty;

            var response = await client.ListSubscriptionsByTopicAsync(request);
            List<Subscription> allSubs = response.Subscriptions;

            // Get the ARN Value for this subscription.
            foreach (Subscription sub in allSubs)
            {
                if (sub.Endpoint.Equals(email))
                {
                    subArn = sub.SubscriptionArn;
                    return subArn;
                }
            }

            return string.Empty;
        }

        public static async Task<string> PublishMessage(IAmazonSimpleNotificationService client, string body)
        {
            var request = new PublishRequest();
            request.Message = body;
            request.TopicArn = TopicArn;

            var response = await client.PublishAsync(request);

            return response.MessageId;
        }

        public static async Task<string> SubscribeEmail(IAmazonSimpleNotificationService client, string email)
        {
            var request = new SubscribeRequest();
            request.Protocol = "email";
            request.Endpoint = email;
            request.TopicArn = TopicArn;
            request.ReturnSubscriptionArn = true;

            var response = await client.SubscribeAsync(request);

            return response.SubscriptionArn;
        }

        public static async Task<List<Subscription>> GetSubscriptionsListAsync(IAmazonSimpleNotificationService client)
        {
            var request = new ListSubscriptionsByTopicRequest
            {
                TopicArn = TopicArn,
            };
            var response = await client.ListSubscriptionsByTopicAsync(request);
            return response.Subscriptions;
        }

        public async Task<string> UnSubEmail(string email)
        {
            var client = new AmazonSimpleNotificationServiceClient(RegionEndpoint.USEast2);
            var arnValue = await GetSubArn(client, email);
            await RemoveSub(client, arnValue);
            return $"{email} was successfully deleted!";
        }

        public async Task<string> PubTopic(string body, string lang)
        {
            var client = new AmazonSimpleNotificationServiceClient(RegionEndpoint.USEast2);
            var msgId = await PublishMessage(client, body);
            return msgId;
        }

        public async Task<string> SubEmail(string email)
        {
            var client = new AmazonSimpleNotificationServiceClient(RegionEndpoint.USEast2);
            var subArn = await SubscribeEmail(client, email);
            return subArn;
        }

        public async Task<string> GetSubs()
        {
            var client = new AmazonSimpleNotificationServiceClient(RegionEndpoint.USEast2);
            var subscriptions = await GetSubscriptionsListAsync(client);
            var val = this.DisplaySubscriptionList(subscriptions);
            return val;
        }

        public string DisplaySubscriptionList(List<Subscription> subscriptionList)
        {
            var email = string.Empty;
            List<string> emailList = new List<string>();
            foreach (var subscription in subscriptionList)
            {
                emailList.Add(subscription.Endpoint);
                email = subscription.Endpoint;
            }

            var xml = this.GenerateXML(emailList);
            return xml;
        }

        // Convert the list to XML to pass back to the view.
        private string GenerateXML(List<string> subsList)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
            doc.AppendChild(docNode);

            XmlNode subsNode = doc.CreateElement("Subs");
            doc.AppendChild(subsNode);

            // Iterate through the collection.
            foreach (string sub in subsList)
            {
                XmlNode subNode = doc.CreateElement("Sub");
                subsNode.AppendChild(subNode);

                XmlNode email = doc.CreateElement("email");
                email.AppendChild(doc.CreateTextNode(sub));
                subNode.AppendChild(email);
            }

            return doc.OuterXml;
        }

    }
}
