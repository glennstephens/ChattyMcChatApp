using System;

using Xamarin.Forms;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using Humanizer;
using System.Collections.Specialized;
using ChattyMcChatApp.Models;

namespace ChattyMcChatApp.Pages
{
    public class ChatPage : ContentPage
    {
        public ObservableCollection<ChatContent> Chats = new ObservableCollection<ChatContent>();

        public virtual void PerformImageSelection(ImageMessage image)
        {
            ImageViewPage page = new ImageViewPage(image);
            Navigation.PushAsync(page);
        }

        public virtual void PerformLinkSelection(LinkMessage link)
        {
            WebViewPage page = new WebViewPage(link);
            Navigation.PushAsync(page);
        }

        public ChatPage() : base()
        {
            Title = "Chat";

            Chats.Add(new ChatPerson() { ID = "1", Name = "Glenn Stephens" });

            // Lets setup some dummy data
            Chats.Add(
                new TextMessage() { Text = "Hello from Glenn. This is a message. Ideally it should word-wrap as well", TimeOfMessage = DateTime.Now.AddMinutes(-24) }
            );
            Chats.Add(
                new TextMessage() { Text = "Did you want to get together for coffee?", TimeOfMessage = DateTime.Now.AddMinutes(-21) }
            );
            Chats.Add(
                new ImageMessage() { ImageLocation = "tesla1.jpg" }
            );
            Chats.Add(new ChatPerson() { ID = "2", Name = "David Murray" });
            Chats.Add(
                new TextMessage() { Text = "Sure", TimeOfMessage = DateTime.Now.AddMinutes(-15) }
            );
            Chats.Add(new TextMessage()
            {
                Text =
                    "This is a really long piece of content and should wrap several lines of code that we should be " +
                    "ok with. It will also let us test out the core mechanisms of how something like this would work. Again," +
                    "This is a really long piece of content and should wrap several lines of code that we should be " +
                    "ok with. It will also let us test out the core mechanisms of how something like this would work"
            });
            Chats.Add(new ChatPerson() { ID = "3", Name = "Automated System" });
            Chats.Add(new LinkMessage()
            {
                Title = "Check on Google",
                URL = "https://www.google.com",
                ExtractedContent = "Check this value on Google to see the true result. You may find that there is more information available to you and you're really just not aware of it"
            });
        }

        public void AddMessageForDelivery(string content)
        {

        }
    }
}
