using PostmarkDotNet.Model;
using System;
using System.Collections.Generic;

namespace PostmarkDotNet.Webhooks
{
    /// <summary>
    /// Representation of the payload of the click tracking webhook
    /// - https://postmarkapp.com/developer/webhooks/click-webhook
    /// </summary>
    public class PostmarkClickWebhookMessage : PostmarkClick
    {
        /// <summary>
        /// Where in the message the clicked link was located, e.g. "HTML" or "Text".
        /// </summary>
        public string ClickLocation { get; set; }

        /// <summary>
        ///   The time the click was received by the Postmark servers.
        /// </summary>
        /// <value>The time the click was received</value>
        public DateTime ReceivedAt { get; set; }

        /// <summary>
        /// The tags users add to emails
        /// </summary>
        /// <value>The specific tag string</value>
        public string Tag { get; set; }

        /// <summary>
        /// The email address of the recipient who clicked the link.
        /// </summary>
        /// <value>Email address of the recipient</value>
        public string Recipient { get; set; }

        /// <summary>
        /// The metadata for the clicked message.
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; }
    }
}
