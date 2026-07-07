using PostmarkDotNet.Model;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PostmarkDotNet
{
    /// <summary>
    /// A single recipient entry in a <see cref="PostmarkBulkMessage"/>.
    /// The shared message content (From, Subject, Body/Template, etc.) is defined once on the
    /// parent <see cref="PostmarkBulkMessage"/>; each recipient supplies its own addressing and
    /// per-recipient template values.
    /// </summary>
    /// <remarks>
    /// The Bulk API validates recipient objects strictly and rejects fields that are present but null,
    /// so optional properties are omitted from the serialized payload when not set.
    /// </remarks>
    public class PostmarkBulkMessageRecipient
    {
        /// <summary>
        ///   Any recipients. Separate multiple recipients with a comma (up to 50 addresses).
        /// </summary>
        public string To { get; set; }

        /// <summary>
        ///   Any CC recipients. Separate multiple recipients with a comma (up to 50 addresses).
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Cc { get; set; }

        /// <summary>
        ///   Any BCC recipients. Separate multiple recipients with a comma (up to 50 addresses).
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Bcc { get; set; }

        /// <summary>
        ///   The values to merge with the template (or inline body) for this specific recipient.
        ///   Must be a JSON object (e.g. a Dictionary&lt;string, object&gt;, POCO, or anonymous type).
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object TemplateModel { get; set; }

        /// <summary>
        ///   A dictionary of optional metadata for this recipient. If provided, this overrides
        ///   any metadata specified on the parent <see cref="PostmarkBulkMessage"/>.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IDictionary<string, string> Metadata { get; set; }

        /// <summary>
        ///   A collection of optional message headers for this recipient.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public HeaderCollection Headers { get; set; }
    }
}
