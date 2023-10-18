using System;
using Windows.Storage.Pickers;
using Windows.UI.Text;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using static System.Net.Mime.MediaTypeNames;
using System.Linq;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Projects.Presentation.View.AppUserControl
{
    public sealed partial class DescriptionUserControl : UserControl
    {

        public string DesciptionText
        {
            get;
            set;
        } = "";
        public event Action<string> DescriptionRichEditBoxLostFocus;
        Color _CurrentColor = (Color)Windows.UI.Xaml.Application.Current.Resources["ComplementaryThemeColor"];
        bool _IsDescriptionChanged=false;
        public DescriptionUserControl()
        {
            this.InitializeComponent();
        }
        
        private void DescriptionUserControlPanel_Loaded(object sender, RoutedEventArgs e)
        {
            DescriptionRichEditBox.Document.SetText(TextSetOptions.FormatRtf, DesciptionText);
        }
        public void Refresh()
        {
            DescriptionRichEditBox.Document.SetText(TextSetOptions.FormatRtf,"");
        }
        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            DescriptionRichEditBox.Document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            DescriptionRichEditBox.Document.Selection.CharacterFormat.Italic = FormatEffect.Toggle;
        }
        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            Button clickedColor = (Button)sender;
            var rectangle = (Windows.UI.Xaml.Shapes.Rectangle)clickedColor.Content;
            var color = ((SolidColorBrush)rectangle.Fill).Color;

            DescriptionRichEditBox.Document.Selection.CharacterFormat.ForegroundColor = color;
            _CurrentColor = color;

            fontColorButton.Flyout.Hide();
            DescriptionRichEditBox.Focus(FocusState.Keyboard);
        }

        private void FindBoxHighlightMatches()
        {
            FindBoxRemoveHighlights();

            Color highlightBackgroundColor = (Color)App.Current.Resources["SystemColorHighlightColor"];
            Color highlightForegroundColor = (Color)App.Current.Resources["SystemColorHighlightTextColor"];

            string textToFind = findBox.Text;
            if (textToFind != null)
            {
                ITextRange searchRange = DescriptionRichEditBox.Document.GetRange(0, 0);
                while (searchRange.FindText(textToFind, TextConstants.MaxUnitCount, FindOptions.None) > 0)
                {
                    searchRange.CharacterFormat.BackgroundColor = highlightBackgroundColor;
                    searchRange.CharacterFormat.ForegroundColor = highlightForegroundColor;
                }
            }
        }

        private void FindBoxRemoveHighlights()
        {
            ITextRange documentRange = DescriptionRichEditBox.Document.GetRange(0, TextConstants.MaxUnitCount);
            SolidColorBrush defaultBackground = DescriptionRichEditBox.Background as SolidColorBrush;
            SolidColorBrush defaultForeground = DescriptionRichEditBox.Foreground as SolidColorBrush;

            documentRange.CharacterFormat.BackgroundColor = defaultBackground.Color;
            documentRange.CharacterFormat.ForegroundColor = defaultForeground.Color;
        }
       
        private void DescriptionRichEditBox_LostFocus(object sender, RoutedEventArgs e)
        {
            string text="";
            if (_IsDescriptionChanged)
            {
                DescriptionRichEditBox.Document.GetText(TextGetOptions.FormatRtf, out text);
                DescriptionRichEditBoxLostFocus?.Invoke(text);
                DesciptionText = text;
                _IsDescriptionChanged = false;
            }

        }
        private void DescriptionRichEditBox_GotFocus(object sender, RoutedEventArgs e)
        {
            
            ITextRange documentRange = DescriptionRichEditBox.Document.GetRange(0, TextConstants.MaxUnitCount);
            SolidColorBrush background = (SolidColorBrush)App.Current.Resources["TextControlBackgroundFocused"];

            if (background != null)
            {
                documentRange.CharacterFormat.BackgroundColor = background.Color;
            }
        }

        private void DescriptionRichEditBox_TextChanged(object sender, RoutedEventArgs e)
        {
            DescriptionRichEditBox.Document.Selection.CharacterFormat.ForegroundColor = _CurrentColor;
            _IsDescriptionChanged = true;

            DescriptionRichEditBox.Document.GetText(TextGetOptions.FormatRtf, out string text);
            if (text.Count() > 1000)
            {
                FindBoxStackPanel.Visibility = Visibility.Visible;
            }
            else
            {
                //FindBoxStackPanel.Visibility = Visibility.Collapsed;
            }
        }

       
    }
}
