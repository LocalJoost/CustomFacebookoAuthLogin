using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Facebook;

namespace FacebookCustom
{
  public sealed partial class MainPage : Page
  {

    private const string AppId = "your app id here";
    private const string ExtendedPermissions = "publish_actions, user_managed_groups, user_groups";

    private const string FbSuccess = "https://www.facebook.com/connect/login_success.html";

    public MainPage()
    {
      this.InitializeComponent();
    }

    private async void ButtonBase_OnClick(object sender, RoutedEventArgs e)
    {
      var result = await AuthenticateFacebookAsync();
      var md = new MessageDialog("your token is: " + result);
      await md.ShowAsync();
    }

    private async Task<string> AuthenticateFacebookAsync()
    {
      try
      {
        var fb = new FacebookClient();

        var loginUri = fb.GetLoginUrl(new
        {
          client_id = AppId,
          redirect_uri = FbSuccess,
          scope = ExtendedPermissions,
          display = "popup",
          response_type = "token"
        });

        var authenticationResult =
          await
            FacebookAuthenticationBroker.AuthenticateAsync(loginUri);

        return ParseAuthenticationResult(authenticationResult);
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }

    private static string ParseAuthenticationResult(string authResult)
    {
      var pattern = 
        string.Format("{0}#access_token={1}&expires_in={2}", 
                      FbSuccess,"(?<access_token>.+)", "(?<expires_in>.+)");
      var match = Regex.Match(authResult, pattern);
      return match.Groups["access_token"].Value;
    }
  }
}
