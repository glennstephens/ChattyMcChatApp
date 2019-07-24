using System;

namespace ChattyMcChatApp.Models
{
    public class ChatContent
    {

    }

    public class ChatPerson : ChatContent
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }

    public abstract class ChatMessage : ChatContent
    {
        public DateTime TimeOfMessage { get; set; }
    }

    public class TextMessage : ChatMessage
    {
        public string Text { get; set; }
    }

    public class ImageMessage : ChatMessage
    {
        public string URL { get; set; }
        public string ImageLocation { get; set; }
    }

    public class LinkMessage : ChatMessage
    {
        public string Title { get; set; }
        public string ExtractedContent { get; set; }
        public string URL { get; set; }
    }
}
