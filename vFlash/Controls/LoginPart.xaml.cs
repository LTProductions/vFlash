using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Linq;
using Template10.Services.NavigationService;
using vFlash.Models;
using vFlash.Utils;
using vFlash.Views;
using Windows.Security.Credentials;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace vFlash.Controls
{
    public sealed partial class LoginPart : UserControl
    {
        public LoginPart()
        {
            this.InitializeComponent();
            MSLogin();
        }

        public event EventHandler LoggedIn;
        public static bool loginClicked = false;


        private async void MSLogin()
        {

            try
            {
                MobileServiceClient client;

                if (loginClicked)
                {
                    var authHandler = new AuthHandler(SaveUser);
                    client = new MobileServiceClient(App.MobileService.MobileAppUri, authHandler);
                    authHandler.Client = client;
                    MobileServiceUser user;
                    if (TryLoadUser(out user))
                    {
                        client.CurrentUser = user;
                        App.MobileService.CurrentUser = client.CurrentUser;
                    }
                }

                else
                {
                    client = new MobileServiceClient(App.MobileService.MobileAppUri);
                    MobileServiceUser user;
                    if (TryLoadUser(out user))
                    {
                        client.CurrentUser = user;
                        App.MobileService.CurrentUser = client.CurrentUser;
                    }
                }

                // Views.Busy.SetBusy(true, "Logging In...");
                var table = client.GetTable<ClassData>();
                var items = await table.Take(3).ToEnumerableAsync();
                LoggedIn?.Invoke(this, EventArgs.Empty);
                // Views.Busy.SetBusy(false);
            }
            catch (Exception ex)
            {
                //error, don't log in.
            }
        }

        private static bool TryLoadUser(out MobileServiceUser user)
        {
            object userId, authToken;
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue("userId", out userId) &&
                ApplicationData.Current.LocalSettings.Values.TryGetValue("authToken", out authToken))
            {
                user = new MobileServiceUser((string)userId)
                {
                    MobileServiceAuthenticationToken = (string)authToken
                };
                return true;
            }
            else
            {
                user = null;
                return false;
            }
        }

        private static void SaveUser(MobileServiceUser user)
        {
            ApplicationData.Current.LocalSettings.Values["userId"] = user.UserId;
            ApplicationData.Current.LocalSettings.Values["authToken"] = user.MobileServiceAuthenticationToken;
        }

        private void LoginMSButton_Click(object sender, RoutedEventArgs e)
        {
            loginClicked = true;
            MSLogin();
        }
    }
}