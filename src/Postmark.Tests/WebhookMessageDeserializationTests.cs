using System;
using System.Globalization;
using System.Text.Json;
using PostmarkDotNet;
using PostmarkDotNet.Webhooks;
using Xunit;

namespace Postmark.Tests
{
    /// <summary>
    /// Verifies that the webhook payload models deserialize the documented Postmark
    /// webhook payloads. Payloads are copied from the public docs at
    /// https://postmarkapp.com/developer/webhooks so the property casing matches the
    /// case-sensitive System.Text.Json defaults consumers use.
    /// </summary>
    public class WebhookMessageDeserializationTests
    {
        [Fact]
        public void CanDeserializeClickWebhook()
        {
            // https://postmarkapp.com/developer/webhooks/click-webhook
            var json = @"{
              ""RecordType"": ""Click"",
              ""MessageStream"": ""outbound"",
              ""ClickLocation"": ""HTML"",
              ""Client"": { ""Name"": ""Chrome 35.0.1916.153"", ""Company"": ""Google"", ""Family"": ""Chrome"" },
              ""OS"": { ""Name"": ""OS X 10.7 Lion"", ""Company"": ""Apple Computer, Inc."", ""Family"": ""OS X 10"" },
              ""Platform"": ""Desktop"",
              ""UserAgent"": ""Mozilla/5.0"",
              ""OriginalLink"": ""https://example.com"",
              ""Geo"": { ""CountryISOCode"": ""RS"", ""Country"": ""Serbia"", ""City"": ""Novi Sad"", ""IP"": ""8.8.8.8"" },
              ""MessageID"": ""00000000-0000-0000-0000-000000000000"",
              ""Metadata"": { ""a_key"": ""a_value"", ""b_key"": ""b_value"" },
              ""ReceivedAt"": ""2017-10-25T15:21:11.9065619Z"",
              ""Tag"": ""welcome-email"",
              ""Recipient"": ""john@example.com""
            }";

            var message = JsonSerializer.Deserialize<PostmarkClickWebhookMessage>(json);

            Assert.NotNull(message);
            Assert.Equal("HTML", message.ClickLocation);
            Assert.Equal("Desktop", message.Platform);
            Assert.Equal("https://example.com", message.OriginalLink);
            Assert.Equal("00000000-0000-0000-0000-000000000000", message.MessageID);
            Assert.Equal("welcome-email", message.Tag);
            Assert.Equal("john@example.com", message.Recipient);
            var expectedReceivedAt = DateTime.Parse(
                "2017-10-25T15:21:11.9065619Z", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            Assert.Equal(expectedReceivedAt.ToUniversalTime(), message.ReceivedAt.ToUniversalTime());
            Assert.Equal("Chrome", message.Client.Family);
            Assert.Equal("OS X 10", message.OS.Family);
            Assert.Equal("Serbia", message.Geo.Country);
            Assert.Equal("a_value", message.Metadata["a_key"]);
        }

        [Fact]
        public void CanDeserializeSpamComplaintWebhook()
        {
            // https://postmarkapp.com/developer/webhooks/spam-complaint-webhook
            var json = @"{
              ""RecordType"": ""SpamComplaint"",
              ""MessageStream"": ""outbound"",
              ""ID"": 42,
              ""Type"": ""SpamComplaint"",
              ""TypeCode"": 100001,
              ""Name"": ""Spam Complaint"",
              ""Tag"": ""my-tag"",
              ""MessageID"": ""00000000-0000-0000-0000-000000000000"",
              ""ServerID"": 1234,
              ""Description"": ""The subscriber explicitly marked this message as spam."",
              ""Details"": ""Test spam complaint details"",
              ""Email"": ""jim@example.com"",
              ""From"": ""sender@example.com"",
              ""BouncedAt"": ""2019-11-05T16:33:54.9070259Z"",
              ""DumpAvailable"": true,
              ""Inactive"": true,
              ""CanActivate"": false,
              ""Subject"": ""Test subject"",
              ""Content"": ""Abuse report content"",
              ""Metadata"": { ""a_key"": ""a_value"", ""b_key"": ""b_value"" }
            }";

            var message = JsonSerializer.Deserialize<PostmarkSpamComplaintWebhookMessage>(json);

            Assert.NotNull(message);
            Assert.Equal(42, message.ID);
            Assert.Equal(PostmarkBounceType.SpamComplaint, message.Type);
            Assert.Equal(100001, message.TypeCode);
            Assert.Equal("jim@example.com", message.Email);
            Assert.Equal("sender@example.com", message.From);
            Assert.Equal(1234, message.ServerID);
            Assert.True(message.Inactive);
            Assert.False(message.CanActivate);
            Assert.Equal("Test subject", message.Subject);
            Assert.Equal("Abuse report content", message.Content);
            Assert.Equal("b_value", message.Metadata["b_key"]);
        }

        [Fact]
        public void CanDeserializeSubscriptionChangeWebhook()
        {
            // https://postmarkapp.com/developer/webhooks/subscription-change-webhook
            var json = @"{
              ""RecordType"": ""SubscriptionChange"",
              ""MessageID"": ""883953f4-6105-42a2-a16a-77a8eac79483"",
              ""ServerID"": 123456,
              ""MessageStream"": ""outbound"",
              ""ChangedAt"": ""2020-02-01T10:53:34.416071Z"",
              ""Recipient"": ""bounced-address@wildbit.com"",
              ""Origin"": ""Recipient"",
              ""SuppressSending"": true,
              ""SuppressionReason"": ""HardBounce"",
              ""Tag"": ""my-tag"",
              ""Metadata"": { ""example"": ""value"", ""example_2"": ""value"" }
            }";

            var message = JsonSerializer.Deserialize<PostmarkSubscriptionChangeWebhookMessage>(json);

            Assert.NotNull(message);
            Assert.Equal("SubscriptionChange", message.RecordType);
            Assert.Equal("883953f4-6105-42a2-a16a-77a8eac79483", message.MessageID);
            Assert.Equal(123456, message.ServerID);
            Assert.Equal("outbound", message.MessageStream);
            Assert.Equal("bounced-address@wildbit.com", message.Recipient);
            Assert.Equal("Recipient", message.Origin);
            Assert.True(message.SuppressSending);
            Assert.Equal("HardBounce", message.SuppressionReason);
            Assert.Equal("my-tag", message.Tag);
            Assert.Equal("value", message.Metadata["example"]);
        }

        [Fact]
        public void SubscriptionChangeReactivationHasNullSuppressionReason()
        {
            // Reactivations (SuppressSending = false) omit SuppressionReason/Tag and carry empty Metadata.
            var json = @"{
              ""RecordType"": ""SubscriptionChange"",
              ""MessageID"": ""883953f4-6105-42a2-a16a-77a8eac79483"",
              ""ServerID"": 123456,
              ""MessageStream"": ""outbound"",
              ""ChangedAt"": ""2020-02-01T10:53:34.416071Z"",
              ""Recipient"": ""reactivated@example.com"",
              ""Origin"": ""Recipient"",
              ""SuppressSending"": false,
              ""SuppressionReason"": null,
              ""Tag"": null,
              ""Metadata"": {}
            }";

            var message = JsonSerializer.Deserialize<PostmarkSubscriptionChangeWebhookMessage>(json);

            Assert.NotNull(message);
            Assert.False(message.SuppressSending);
            Assert.Null(message.SuppressionReason);
            Assert.Null(message.Tag);
            Assert.Empty(message.Metadata);
        }
    }
}
