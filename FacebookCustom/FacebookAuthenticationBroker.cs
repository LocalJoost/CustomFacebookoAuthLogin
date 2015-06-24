using System;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;

namespace FacebookCustom
{
  /// <summary>
  /// This class is a helper to replace the default WebAuthenticationBroker
  /// </summary>
  public static class FacebookAuthenticationBroker
  {
    public static Task<string> AuthenticateAsync(Uri uri)
    {
      var tcs = new TaskCompletionSource<string>();

      var w = new WebView
      {
        HorizontalAlignment = HorizontalAlignment.Stretch,
        VerticalAlignment = VerticalAlignment.Stretch,
        Margin = new Thickness(30.0),
      };

      var b = new Border
      {
        Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255)),
        Width = Window.Current.Bounds.Width,
        Height = Window.Current.Bounds.Height,
        Child = w
      };

      var p = new Popup
      {
        Width = Window.Current.Bounds.Width,
        Height = Window.Current.Bounds.Height,
        Child = b,
        HorizontalOffset = 0.0,
        VerticalOffset = 0.0
      };

      Window.Current.SizeChanged += (s, e) =>
          {
            p.Width = e.Size.Width;
            p.Height = e.Size.Height;
            b.Width = e.Size.Width;
            b.Height = e.Size.Height;
          };

      w.Source = uri;

      w.NavigationCompleted += (sender, args) =>
      {
        if (args.Uri != null)
        {
          if (args.Uri.OriginalString.Contains("access_token"))
          {
            tcs.SetResult(args.Uri.ToString());
            p.IsOpen = false;
          }
          if (args.Uri.OriginalString.Contains("error=access_denied"))
          {
            tcs.SetResult(null);
            p.IsOpen = false;
          }
        }
      };

      p.IsOpen = true;
      return tcs.Task;
    }
  }
}