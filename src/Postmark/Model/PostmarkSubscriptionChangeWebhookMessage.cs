using System;
using System.Collections.Generic;

namespace PostmarkDotNet.Webhooks
{
    /// <summary>
    /// Representation of the payload of the subscription change webhook
    /// - https://postmarkapp.com/developer/webhooks/subscription-change-webhook
    /// </summary>
    public class PostmarkSubscriptionChangeWebhookMessage
    {
        /// <summary>
        /// The type of webhook record. Always "SubscriptionChange".
        /// </summary>
        public string RecordType { get; set; }

        /// <summary>
        /// The ID of the message associated with the subscription change.
        /// Can be null for manual suppressions and reactivations.
        /// </summary>
        public string MessageID { get; set; }

        /// <summary>
        /// The ID of the Server that sent the original message.
        /// </summary>
        public int ServerID { get; set; }

        /// <summary>
        /// The message stream on which the recipient's subscription changed.
        /// </summary>
        public string MessageStream { get; set; }

        /// <summary>
        /// The time the subscription change occurred.
        /// </summary>
        public DateTime ChangedAt { get; set; }

        /// <summary>
        /// The email address of the recipient whose subscription changed.
        /// </summary>
        public string Recipient { get; set; }

        /// <summary>
        /// Where the subscription change originated, e.g. "Recipient", "Customer", or "Admin".
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// Whether sending to this recipient is currently suppressed.
        /// </summary>
        public bool SuppressSending { get; set; }

        /// <summary>
        /// The reason sending is suppressed, e.g. "HardBounce", "SpamComplaint", or
        /// "ManualSuppression". Null during reactivations (when SuppressSending is false).
        /// </summary>
        public string SuppressionReason { get; set; }

        /// <summary>
        /// The tag associated with the message, if any.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// The metadata for the message. Empty on reactivations.
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; }
    }
}
