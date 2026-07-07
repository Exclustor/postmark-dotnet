using System;
using System.Text.Json.Serialization;

namespace PostmarkDotNet
{
    /// <summary>
    /// The status/progress of a bulk email request (<c>GET /email/bulk/{bulk-request-id}</c>).
    /// </summary>
    public class PostmarkBulkEmailStatus
    {
        /// <summary>
        ///   The identifier of the bulk request.
        /// </summary>
        [JsonPropertyName("Id")]
        public string Id { get; set; }

        /// <summary>
        ///   The time the request was received by Postmark.
        /// </summary>
        public DateTime SubmittedAt { get; set; }

        /// <summary>
        ///   The total number of messages contained in the request.
        /// </summary>
        public int TotalMessages { get; set; }

        /// <summary>
        ///   The percentage of the request that has been processed, from 0 to 100.
        /// </summary>
        public double PercentageCompleted { get; set; }

        /// <summary>
        ///   The processing status of the request.
        ///   One of "Accepted", "Processing", "Completed", or "Cancelled".
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        ///   The subject line of the messages in the request.
        /// </summary>
        public string Subject { get; set; }
    }
}
