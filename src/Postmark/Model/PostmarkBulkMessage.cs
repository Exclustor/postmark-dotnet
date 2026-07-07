using PostmarkDotNet.Model;
using System;
using System.Collections.Generic;
using System.IO;

namespace PostmarkDotNet
{
    /// <summary>
    /// A message destined for the Postmark Bulk Email API (<c>POST /email/bulk</c>).
    /// </summary>
    /// <remarks>
    /// The Bulk Email API is intended for broadcast/marketing sends (newsletters, announcements).
    /// This is distinct from the transactional batch endpoints (<c>/email/batch</c>,
    /// <c>/email/batchWithTemplates</c>): a single message definition is supplied here, and each entry in
    /// <see cref="Messages"/> provides its own recipient addressing and per-recipient template values.
    /// The request is accepted for asynchronous processing; use
    /// <see cref="PostmarkClient.GetBulkEmailStatusAsync"/> to track progress.
    /// </remarks>
    public class PostmarkBulkMessage
    {
        public PostmarkBulkMessage()
        {
            Attachments = new List<PostmarkMessageAttachment>(0);
            Messages = new List<PostmarkBulkMessageRecipient>();
            TrackLinks = LinkTrackingOptions.None;
        }

        /// <summary>
        ///   The message stream used to send this message. Defaults to the server's broadcast stream.
        /// </summary>
        public string MessageStream { get; set; }

        /// <summary>
        ///   The sender's email address. Must be a valid sender signature. Required.
        /// </summary>
        public string From { get; set; }

        /// <summary>
        ///   The email address to reply to. This is optional.
        /// </summary>
        public string ReplyTo { get; set; }

        /// <summary>
        ///   The message subject line. May contain template placeholders when using inline content.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        ///   The HTML body of the message. May be null if <see cref="TextBody"/> or a template is used.
        /// </summary>
        public string HtmlBody { get; set; }

        /// <summary>
        ///   The plain text body of the message. May be null if <see cref="HtmlBody"/> or a template is used.
        /// </summary>
        public string TextBody { get; set; }

        /// <summary>
        ///   The id of a hosted template to use when sending this message.
        ///   Either this or <see cref="TemplateAlias"/> may be provided; TemplateId takes precedence when both are set.
        /// </summary>
        public long? TemplateId { get; set; }

        /// <summary>
        ///   The alias of a hosted template to use when sending this message.
        ///   Either this or <see cref="TemplateId"/> may be provided; TemplateId takes precedence when both are set.
        /// </summary>
        public string TemplateAlias { get; set; }

        /// <summary>
        ///   Should the CSS in the HtmlBody (or template) be inlined? Defaults to true.
        /// </summary>
        public bool InlineCss { get; set; } = true;

        /// <summary>
        ///   An optional message tag, used for breaking down statistics in the Postmark UI.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        ///   A dictionary of optional metadata applied to every recipient.
        ///   Individual recipients may override this via <see cref="PostmarkBulkMessageRecipient.Metadata"/>.
        /// </summary>
        public IDictionary<string, string> Metadata { get; set; }

        /// <summary>
        ///   Track these messages using Postmark's OpenTracking feature.
        /// </summary>
        public bool? TrackOpens { get; set; }

        /// <summary>
        ///   Track these messages using Postmark's LinkTracking feature.
        /// </summary>
        public LinkTrackingOptions? TrackLinks { get; set; }

        /// <summary>
        ///   A collection of optional message headers applied to every recipient.
        /// </summary>
        public HeaderCollection Headers { get; set; } = new HeaderCollection();

        /// <summary>
        ///   A collection of optional file attachments applied to every recipient.
        /// </summary>
        public ICollection<PostmarkMessageAttachment> Attachments { get; set; }

        /// <summary>
        ///   The per-recipient messages to send. Postmark accepts an unlimited number of recipients per
        ///   call, subject to the 50 MB total payload limit (including attachments). Required.
        /// </summary>
        public ICollection<PostmarkBulkMessageRecipient> Messages { get; set; }

        private static byte[] ReadStream(Stream input, int bufferSize)
        {
            var buffer = new byte[bufferSize];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        ///  Adds a file attachment stream with inline support.
        /// </summary>
        /// <param name = "contentStream">An opened stream of the file attachment.</param>
        /// <param name="attachmentName">The file name associated with the attachment.</param>
        /// <param name = "contentType">The content type.</param>
        /// <param name="contentId">The ContentId for inlined images.</param>
        public void AddAttachment(Stream contentStream, string attachmentName, string contentType = "application/octet-stream", string contentId = null)
        {
            var content = ReadStream(contentStream, 8067);
            var payload = Convert.ToBase64String(content);

            var attachment = new PostmarkMessageAttachment
            {
                Name = attachmentName,
                ContentType = contentType,
                Content = payload
            };

            if ((contentId?.Trim() ?? "") != null)
            {
                attachment.ContentId = contentId;
            }

            Attachments.Add(attachment);
        }

        /// <summary>
        ///  Adds a file attachment using a byte[] array with inline support.
        /// </summary>
        /// <param name="content"> The file contents.</param>
        /// <param name="attachmentName">The file name of the attachment.</param>
        /// <param name = "contentType">The content type.</param>
        /// <param name = "contentId">The ContentId for inline images.</param>
        public void AddAttachment(byte[] content, string attachmentName, string contentType = "application/octet-stream", string contentId = null)
        {
            using (var ms = new MemoryStream(content))
            {
                this.AddAttachment(ms, attachmentName, contentType, contentId);
            }
        }
    }
}
