
using Xamarin.Forms;
using ChattyMcChatApp.Models;

namespace ChattyMcChatApp.Pages
{
    public class WebViewPage : ContentPage
    {
        public WebViewPage(LinkMessage link)
        {
            Title = link.Title;

            var view = new WebView()
            {
                Source = link.URL
            };
            Content = view;
        }
    }
}
