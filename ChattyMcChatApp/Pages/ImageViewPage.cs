
using Xamarin.Forms;
using ChattyMcChatApp.Models;
using FFImageLoading.Forms;

namespace ChattyMcChatApp.Pages
{
    public class ImageViewPage : ContentPage
    {
        public ImageViewPage(ImageMessage img)
        {
            Title = "Image";

            Content = new CachedImage()
            {
                Source = img.ImageLocation
            };
        }
    }
}
