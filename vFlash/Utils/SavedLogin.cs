using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vFlash.Models;
using Windows.Storage;

namespace vFlash.Utils
{
    public static class SavedLogin
    {

        public static async Task MSLogin(Boolean loginClicked)
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




    }
}
