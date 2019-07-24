using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;
using CoreGraphics;
using Foundation;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChattyMcChatApp.Models;
using ChattyMcChatApp.Pages;

[assembly: ExportRenderer(typeof(ChattyMcChatApp.Pages.ChatPage), typeof(ChattyMcChatApp.iOS.Pages.ChatPageRenderer))]

namespace ChattyMcChatApp.iOS.Pages
{
    public class ChatPageRenderer : PageRenderer
    {
        public ChatPageRenderer() : base()
        {

        }

        ChatTableViewController chatDisplay;
        UIView bottomSection;
        UIButton cameraButton;
        UITextField text;
        UIButton sendButton;

        ChatPage chat;

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            chat = this.Element as ChatPage;

            chatDisplay = new ChatTableViewController(this.Element as ChatPage);

            bottomSection = new UIView();

            cameraButton = new UIButton(UIButtonType.RoundedRect);
            cameraButton.SetImage(UIImage.FromBundle("Camera.png"), UIControlState.Normal);
            cameraButton.TouchUpInside += async (s, e) => await CameraButton_TouchUpInside(s, e);

            text = new UITextField();
            text.Font = BaseChatCell.textFont;
            text.Placeholder = "Enter Chat Message";
            text.BorderStyle = UITextBorderStyle.RoundedRect;
            text.Layer.BorderColor = UIColor.FromRGBA(222, 222, 222, 190).CGColor;
            text.Layer.BorderWidth = 0.5f;
            text.Layer.CornerRadius = 5f;

            sendButton = new UIButton(UIButtonType.Custom);
            sendButton.Font = BaseChatCell.textFont;
            sendButton.SetTitle("Send", UIControlState.Normal);
            sendButton.BackgroundColor = UIColor.Blue;

            bottomSection.AddSubviews(cameraButton, text, sendButton);

            View.AddSubviews(chatDisplay.View, bottomSection);

            sendButton.TouchUpInside += HandleSendPress;
            text.ShouldReturn += (tf) => {
                AddCurrentTextContent();
                return false;
            };
        }

        string GetGuidPrefix()
        {
            string result = "";
            foreach (char c in Guid.NewGuid().ToString())
                if (Char.IsLetterOrDigit(c))
                    result += c;
            return result;
        }

        async Task CameraButton_TouchUpInside(object sender, EventArgs e)
        {
            //			var optionCount = 0;
            //			if (CrossMedia.Current.IsCameraAvailable)
            //				optionCount++;
            //			if (CrossMedia.Current.IsPickPhotoSupported)
            //				optionCount++;
            //
            //			if (optionCount == 0)
            //			{
            //				//DisplayAlert("No Camera", ":( No camera avaialble.", "OK");
            //				return;
            //			}
            //
            //			MediaFile file = null;
            //
            //			if (CrossMedia.Current.IsCameraAvailable) {
            //				file = await CrossMedia.Current.TakePhotoAsync (new Media.Plugin.Abstractions.StoreCameraMediaOptions {
            //					Directory = System.Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments),
            //					Name = "ImageCapture_" + GetGuidPrefix () + ".jpg"
            //				});
            //			} else if (CrossMedia.Current.IsPickPhotoSupported) {
            //				file = await CrossMedia.Current.PickPhotoAsync ();
            //			}
            //
            //			if (file == null)
            //				return;
            //
            //			//DisplayAlert("File Location", file.Path, "OK");
            //			// Add the image to the list and refresh
            //
            //			chat.Chats.Add (new ImageMessage () {
            //				TimeOfMessage = DateTime.Now,
            //				ImageLocation = file.Path
            //			});
            //
            //			chatDisplay.TableView.ReloadData ();
        }

        void AddCurrentTextContent()
        {
            text.ResignFirstResponder();

            // Add the text to the system
            chat.Chats.Add(new TextMessage() { Text = text.Text, TimeOfMessage = DateTime.UtcNow });
            chatDisplay.TableView.ReloadData();
            //chatDisplay.TableView.SetContentOffset (new CGPoint (0, nfloat.MaxValue), true);

            // Add it to the output queue
            chat.AddMessageForDelivery(text.Text);

            text.Text = "";
        }

        void HandleSendPress(object sender, EventArgs e)
        {
            AddCurrentTextContent();
        }

        const float kOFFSET_FOR_KEYBOARD = 80.0f;

        void SetViewMovedUp(bool movedUp)
        {
            if (keyboardBounds.IsEmpty)
            {
                LayoutToFrame(View.Frame);
            }
            else
            {
                LayoutToFrame(new CGRect(0, 0, View.Frame.Width, View.Frame.Height - keyboardBounds.Height));
            }

            this.View.SetNeedsLayout();
        }

        CGRect keyboardBounds;

        void HandleKeyboardState(NSNotification notification)
        {
            keyboardBounds = UIKeyboard.BoundsFromNotification(notification);
            View.Center = new CGPoint(View.Bounds.GetMidX(), View.Bounds.GetMidY() - keyboardBounds.Height);
            SetViewMovedUp(!keyboardBounds.IsEmpty);
        }

        void HandleKeyboardHideState(NSNotification notification)
        {
            keyboardBounds = new CGRect();
            View.Center = new CGPoint(View.Bounds.GetMidX(), View.Bounds.GetMidY());
            SetViewMovedUp(false);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillShowNotification, HandleKeyboardState);
            NSNotificationCenter.DefaultCenter.AddObserver(UIKeyboard.WillHideNotification, HandleKeyboardHideState);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            NSNotificationCenter.DefaultCenter.RemoveObserver(UIKeyboard.WillShowNotification);
            NSNotificationCenter.DefaultCenter.RemoveObserver(UIKeyboard.WillHideNotification);

            LayoutToFrame(View.Bounds);
        }

        const float bottomSectionHeight = 36f;
        const float gapSize = 4f;
        const float cameraButtonWidth = 30f;
        const float sendButtonWidth = 60f;

        private void LayoutToFrame(CGRect r)
        {
            var height = r.Height;

            chatDisplay.View.Frame = new CGRect(0, 0, r.Width, height - bottomSectionHeight);
            bottomSection.Frame = new CGRect(0, height - bottomSectionHeight, r.Width, bottomSectionHeight);

            cameraButton.Frame = new CGRect(gapSize, gapSize, cameraButtonWidth, bottomSectionHeight - gapSize * 2);
            text.Frame = new CGRect(gapSize * 2 + cameraButtonWidth, gapSize,
                r.Width - cameraButtonWidth - gapSize * 4 - sendButtonWidth, bottomSectionHeight - gapSize * 2);
            sendButton.Frame = new CGRect(r.Width - gapSize - sendButtonWidth, gapSize,
                sendButtonWidth, bottomSectionHeight - gapSize * 2);
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            LayoutToFrame(View.Bounds);
        }
    }

    public class BaseChatCell : UITableViewCell
    {
        public static nfloat ImageDisplaySize = 40f;
        public static nfloat ImageGap = 10f;

        public static string defaultFontName = "Avenir-Light";
        public static string headerFontName = "Avenir-Medium";

        public static UIFont textFont = UIFont.FromName(defaultFontName, 11f);
        public static UIFont headerFont = UIFont.FromName(headerFontName, 13f);
        public static UIFont timeStampFont = UIFont.FromName(defaultFontName, 10f);

        public static UIColor textColor = UIColor.FromRGBA(0, 0, 0, 222);
        public static UIColor headerColor = UIColor.Black;

        public BaseChatCell() : base()
        {
            SelectionStyle = UITableViewCellSelectionStyle.None;
        }

        public static nfloat HeightForFont(UIFont font, string s, nfloat width)
        {
            CGSize availableSize = new CGSize(width, float.MaxValue);
            NSString ns = new NSString(s);
            return ns.GetBoundingRect(
                availableSize,
                NSStringDrawingOptions.UsesLineFragmentOrigin,
                new UIStringAttributes
                {
                    ParagraphStyle = new NSMutableParagraphStyle { LineBreakMode = UILineBreakMode.WordWrap },
                    Font = font
                },
                context: null).Height;
        }
    }

    public class ChatHeaderCell : BaseChatCell
    {
        readonly UIImageView image;
        readonly UILabel name;

        public ChatHeaderCell() : base()
        {
            image = new UIImageView();
            image.ContentMode = UIViewContentMode.ScaleAspectFill;

            name = new UILabel()
            {
                Font = headerFont,
                TextColor = headerColor
            };

            AddSubviews(image, name);
        }

        public void UpdateData(ChatPerson person)
        {
            name.Text = person.Name;
            if (String.IsNullOrEmpty(person.ImageUrl))
                image.Image = GetImageForName(person.Name);
            else
                image.Image = UIImage.FromBundle("android-contact.png");
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var width = ContentView.Frame.Width;

            nfloat sizedWidth = width - ImageDisplaySize - ImageGap * 3;
            var controlHeight = HeightForFont(headerFont, name.Text, Convert.ToSingle(sizedWidth));

            var y = ((ImageDisplaySize + ImageGap * 2) - controlHeight) / 2f;
            name.Frame = new CGRect(ImageDisplaySize + ImageGap * 2, y,
                sizedWidth, controlHeight);

            image.Frame = new CGRect(ImageGap, ImageGap, ImageDisplaySize, ImageDisplaySize);
        }

        private readonly CGColor[] colors = {
            UIColor.Red.CGColor,
            UIColor.Blue.CGColor,
            UIColor.Brown.CGColor,
            UIColor.DarkGray.CGColor,
            UIColor.Magenta.CGColor,
            UIColor.Orange.CGColor,
            UIColor.Purple.CGColor,
        };

        static Random randomColor = new Random();
        static Dictionary<string, CGColor> nameToColors = new Dictionary<string, CGColor>();

        public UIImage GetImageForName(string name)
        {
            nfloat width = 32;
            nfloat height = 32;

            CGColor color;
            if (nameToColors.ContainsKey(name))
                color = nameToColors[name];
            else
            {
                color = colors[randomColor.Next(colors.Length)];
                nameToColors[name] = color;
            }

            UIFont font = UIFont.FromName(BaseChatCell.defaultFontName, 14);
            UIGraphics.BeginImageContextWithOptions(new CGSize(width, height), false, 0);

            var context = UIGraphics.GetCurrentContext();
            context.SetFillColor(color);
            context.AddArc(width / 2, height / 2, width / 2, 0, (nfloat)(2 * Math.PI), true);
            context.FillPath();

            var textAttributes = new UIStringAttributes
            {
                ForegroundColor = UIColor.White,
                BackgroundColor = UIColor.Clear,
                Font = font,
                ParagraphStyle = new NSMutableParagraphStyle { Alignment = UITextAlignment.Center },
            };

            string text;
            string[] splitFrom = name.Split(' ');
            if (splitFrom.Length > 1)
            {
                text = splitFrom[0][0].ToString() + splitFrom[1][0];
            }
            else if (splitFrom.Length > 0)
            {
                text = splitFrom[0][0].ToString();
            }
            else
            {
                text = "?";
            }

            NSString str = new NSString(text);

            var textSize = str.GetSizeUsingAttributes(textAttributes);
            str.DrawString(new CGRect(0, height / 2 - textSize.Height / 2,
                width, height), textAttributes);

            var image = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return image;

        }

        public static string CellId = "chatHeaderCell";
    }

    public class ChatTextCell : BaseChatCell
    {
        UILabel text;

        public ChatTextCell() : base()
        {
            text = new UILabel()
            {
                Font = textFont,
                TextColor = textColor,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };

            AddSubviews(text);
        }

        public void UpdateData(TextMessage message)
        {
            text.Text = message.Text;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var width = ContentView.Frame.Width;

            nfloat sizedWidth = width - ImageDisplaySize - ImageGap * 3;
            var controlHeight = HeightForFont(textFont, text.Text, Convert.ToSingle(sizedWidth));
            text.Frame = new CGRect(ImageDisplaySize + ImageGap * 2, 0,
                sizedWidth, controlHeight);
        }

        public static string CellId = "chatTextCell";
    }

    public class ChatImageCell : BaseChatCell
    {
        UIImageView image;

        public static nfloat ImageCellHeight = 240f;

        public ChatImageCell() : base()
        {
            image = new UIImageView();
            image.ContentMode = UIViewContentMode.ScaleAspectFit;

            AddSubviews(image);
        }

        public void UpdateData(ImageMessage message)
        {
            if (!String.IsNullOrEmpty(message.ImageLocation))
                image.Image = UIImage.FromFile(message.ImageLocation);
            else if (!String.IsNullOrEmpty(message.URL))
            {
                // Load the image from the URL and place it in the file system or find using a cache
                image.Image = null;
            }
            else
                image.Image = null;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var width = ContentView.Frame.Width;

            nfloat sizedWidth = width - ImageDisplaySize - ImageGap * 3;
            image.Frame = new CGRect(ImageDisplaySize + ImageGap * 2, 0,
                sizedWidth, ImageCellHeight);
        }

        public static string CellId = "chatImageCell";
    }

    public class ChatLinkCell : BaseChatCell
    {
        UILabel text;
        UILabel title;
        UIView quoteBlock;

        public static nfloat quoteBlockSize = 4f;
        public static nfloat quoteBlockGap = 10f;
        public static UIColor quoteBlockColor = UIColor.Blue;

        public void UpdateData(LinkMessage message)
        {
            title.Text = message.Title;
            text.Text = message.ExtractedContent;
        }

        public ChatLinkCell() : base()
        {
            text = new UILabel()
            {
                Font = textFont,
                TextColor = textColor,
                Lines = 0,
                LineBreakMode = UILineBreakMode.WordWrap
            };
            title = new UILabel()
            {
                Font = headerFont,
                TextColor = headerColor,
                LineBreakMode = UILineBreakMode.TailTruncation
            };
            quoteBlock = new UIView() { BackgroundColor = quoteBlockColor };

            AddSubviews(quoteBlock, title, text);

            Accessory = UITableViewCellAccessory.DisclosureIndicator;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var width = ContentView.Frame.Width;

            nfloat sizedWidth = width - ImageDisplaySize - ImageGap * 3 - quoteBlockSize - quoteBlockGap;

            var controlHeight = HeightForFont(textFont, text.Text, Convert.ToSingle(sizedWidth));
            title.Frame = new CGRect(ImageDisplaySize + ImageGap * 2 + quoteBlockSize + quoteBlockGap, 0,
                sizedWidth, 20f);

            text.Frame = new CGRect(ImageDisplaySize + ImageGap * 2 + quoteBlockSize + quoteBlockGap, 20f,
                sizedWidth, controlHeight);

            quoteBlock.Frame = new CGRect(ImageDisplaySize + ImageGap * 2, 0, quoteBlockSize, controlHeight + 20f);
        }

        public static string CellId = "chatLinkCell";
    }

    public class ChatTableViewController : UITableViewController
    {
        ChatPage _chat;

        public ChatTableViewController(ChatPage chat) : base()
        {
            _chat = chat;

            TableView.SeparatorStyle = UITableViewCellSeparatorStyle.None;
        }

        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            TableView.ContentInset = new UIEdgeInsets(this.TopLayoutGuide.Length, 0, 0, 0);
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell;

            var item = _chat.Chats[indexPath.Row];

            if (item is ChatPerson)
            {
                cell = tableView.DequeueReusableCell(ChatHeaderCell.CellId) as ChatHeaderCell;
                if (cell == null)
                    cell = new ChatHeaderCell();

                (cell as ChatHeaderCell).UpdateData(item as ChatPerson);
            }
            else if (item is TextMessage)
            {
                cell = tableView.DequeueReusableCell(ChatTextCell.CellId) as ChatTextCell;
                if (cell == null)
                    cell = new ChatTextCell();

                (cell as ChatTextCell).UpdateData(item as TextMessage);
            }
            else if (item is ImageMessage)
            {
                cell = tableView.DequeueReusableCell(ChatImageCell.CellId) as ChatImageCell;
                if (cell == null)
                    cell = new ChatImageCell();

                (cell as ChatImageCell).UpdateData(item as ImageMessage);
            }
            else if (item is LinkMessage)
            {
                cell = tableView.DequeueReusableCell(ChatLinkCell.CellId) as ChatLinkCell;
                if (cell == null)
                    cell = new ChatLinkCell();

                (cell as ChatLinkCell).UpdateData(item as LinkMessage);
            }
            else
                throw new Exception("The Chat Type is not supported");

            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return _chat.Chats.Count;
        }

        nfloat CalculateHeightFor(ChatContent item, UITableView tableView)
        {
            if (item is ChatPerson)
            {
                return BaseChatCell.ImageGap * 2 + BaseChatCell.ImageDisplaySize;
            }
            else if (item is TextMessage)
            {
                var tm = item as TextMessage;
                return BaseChatCell.ImageGap +
                BaseChatCell.HeightForFont(BaseChatCell.textFont,
                    tm.Text, tableView.Bounds.Width - BaseChatCell.ImageGap * 3f - BaseChatCell.ImageDisplaySize);
            }
            else if (item is ImageMessage)
            {
                return BaseChatCell.ImageGap + ChatImageCell.ImageCellHeight;
            }
            else if (item is LinkMessage)
            {
                var lm = item as LinkMessage;
                return BaseChatCell.ImageGap + 20f +
                    BaseChatCell.HeightForFont(BaseChatCell.textFont,
                        lm.ExtractedContent, tableView.Bounds.Width - BaseChatCell.ImageGap * 3f
                        - BaseChatCell.ImageDisplaySize - ChatLinkCell.quoteBlockSize - ChatLinkCell.quoteBlockGap);
            }

            throw new Exception("The Chat Type is not supported");
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            var item = _chat.Chats[indexPath.Row];
            return CalculateHeightFor(item, tableView);
        }

        public override nfloat EstimatedHeight(UITableView tableView, NSIndexPath indexPath)
        {
            var item = _chat.Chats[indexPath.Row];
            return CalculateHeightFor(item, tableView);
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            var item = _chat.Chats[indexPath.Row];
            if (item is ImageMessage)
            {
                _chat.PerformImageSelection(item as ImageMessage);
            }
            else if (item is LinkMessage)
            {
                _chat.PerformLinkSelection(item as LinkMessage);
            }
        }
    }
}
