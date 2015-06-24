using System;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Facebook;

namespace FacebookNormal
{
  public sealed partial class MainPage : Page
  {

    private const string AppId = "406204362913207";
    private const string ExtendedPermissions = "publish_actions, user_managed_groups, user_groups";

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

        var redirectUri = WebAuthenticationBroker.GetCurrentApplicationCallbackUri().ToString();

        var loginUri = fb.GetLoginUrl(new
                                       {
                                         client_id = AppId,
                                         redirect_uri = redirectUri,
                                         scope = ExtendedPermissions,
                                         display = "popup",
                                         response_type = "token"
                                       });

        var callbackUri = new Uri(redirectUri, UriKind.Absolute);

        var authenticationResult =
          await
            WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, 
            loginUri, callbackUri);

        return ParseAuthenticationResult(fb, authenticationResult);
      }
      catch (Exception ex)
      {
        return ex.Message;
      }
    }

    public string ParseAuthenticationResult(FacebookClient fb, WebAuthenticationResult result)
    {
      switch (result.ResponseStatus)
      {
        case WebAuthenticationStatus.ErrorHttp:
          return "Error";
        case WebAuthenticationStatus.Success:

          var oAuthResult = fb.ParseOAuthCallbackUrl(new Uri(result.ResponseData));
          return oAuthResult.AccessToken;
        case WebAuthenticationStatus.UserCancel:
          return "Operation aborted";
      }
      return null;
    }
  }
}
