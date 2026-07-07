using Xunit;
using PostmarkDotNet;
using PostmarkDotNet.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Postmark.Tests
{
    public class ClientBulkSendingTests : ClientBaseFixture
    {
        public ClientBulkSendingTests()
        {
            Client = new PostmarkClient(WriteTestServerToken, BaseUrl);
        }

        private PostmarkBulkMessage ConstructBulkMessage(int recipientCount = 3)
        {
            var message = new PostmarkBulkMessage
            {
                From = WriteTestSenderEmailAddress,
                Subject = $"Bulk Integration Test - {TestingDate} - Hi {{{{FirstName}}}}",
                HtmlBody = "<html><body>Hi, {{FirstName}}!</body></html>",
                TextBody = "Hi, {{FirstName}}!",
                MessageStream = "broadcast",
                Tag = "bulk-integration-testing",
                TrackOpens = true,
                TrackLinks = LinkTrackingOptions.HtmlAndText,
                Metadata = new Dictionary<string, string> { { "campaign", "integration" } },
                Headers = new HeaderCollection
                {
                    new MailHeader("X-Integration-Testing-Postmark-Type-Message", TestingDate.ToString("o"))
                },
                Messages = Enumerable.Range(0, recipientCount)
                    .Select(k => new PostmarkBulkMessageRecipient
                    {
                        To = WriteTestEmailRecipientAddress,
                        TemplateModel = new Dictionary<string, object> { { "FirstName", $"Recipient{k}" } }
                    })
                    .ToList()
            };

            return message;
        }

        [Fact]
        public async void Client_CanSendABulkEmail()
        {
            var result = await Client.SendBulkEmailAsync(ConstructBulkMessage(3));

            Assert.Equal("Accepted", result.Status);
            Assert.False(string.IsNullOrWhiteSpace(result.Id));
            Assert.NotEqual(default, result.SubmittedAt);
            Assert.Equal(3, result.TotalMessages);
        }

        [Fact]
        public async void Client_CanRetrieveBulkEmailStatus()
        {
            var sendResult = await Client.SendBulkEmailAsync(ConstructBulkMessage());

            var status = await Client.GetBulkEmailStatusAsync(sendResult.Id);

            Assert.Equal(sendResult.Id, status.Id);
            Assert.False(string.IsNullOrWhiteSpace(status.Status));
            Assert.True(status.TotalMessages > 0);
            Assert.True(status.PercentageCompleted >= 0 && status.PercentageCompleted <= 100);
        }

        [Fact]
        public async void Client_CanSendABulkEmailWithoutTemplateModel()
        {
            var message = new PostmarkBulkMessage
            {
                From = WriteTestSenderEmailAddress,
                Subject = $"Bulk Integration Test (no model) - {TestingDate}",
                HtmlBody = $"<html><body>Testing the Postmark .net bulk client, <b>{TestingDate}</b></body></html>",
                TextBody = "This is plain text.",
                MessageStream = "broadcast",
                Messages = new List<PostmarkBulkMessageRecipient>
                {
                    new PostmarkBulkMessageRecipient { To = WriteTestEmailRecipientAddress }
                }
            };

            var result = await Client.SendBulkEmailAsync(message);

            Assert.Equal("Accepted", result.Status);
            Assert.False(string.IsNullOrWhiteSpace(result.Id));
        }
    }
}
