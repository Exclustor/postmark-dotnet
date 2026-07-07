using System.Collections.Generic;

namespace PostmarkDotNet.Webhooks
{
    /// <summary>
    /// Representation of the payload of the spam complaint webhook
    /// - https://postmarkapp.com/developer/webhooks/spam-complaint-webhook
    /// </summary>
    public class PostmarkSpamComplaintWebhookMessage : PostmarkBounce
    {
        /// <summary>
        ///   The int based type code for this spam complaint.
        /// </summary>
        /// <value>The type code</value>
        public int TypeCode { get; set; }

        /// <summary>
        /// The full content of the spam complaint. Only included when IncludeContent is
        /// enabled on the SpamComplaint webhook trigger.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The metadata for the message that was complained about.
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; }
    }
}
