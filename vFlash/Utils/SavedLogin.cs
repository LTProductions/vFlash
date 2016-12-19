using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using vFlash.Models;
using Windows.Storage;

namespace vFlash.Utils
{
    /// <summary>
    /// Holds the method for logging a usser in with saved data.
    /// </summary>
    public static class SavedLogin
    {
        /// <summary>
        /// Login to a Microsoft account. If loginClicked is true, don't load user from storage.
        /// </summary>
        /// <param name="loginClicked"></param>
        /// <returns></returns>
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

                var table = client.GetTable<ClassData>();
                var items = await table.Take(3).ToEnumerableAsync();
            }
            catch(Exception e)
            {
                App.MobileService.CurrentUser = new MobileServiceUser("");
                throw new System.Net.WebException();
            }
        }

        /// <summary>
        /// Out the user from storage.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
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
