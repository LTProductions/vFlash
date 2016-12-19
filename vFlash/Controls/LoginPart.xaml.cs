using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
        }

        /// <summary>
        /// Event handler; listened to by Shell.xaml.cs
        /// </summary>
        public event EventHandler LoggedIn;
        
        /// <summary>
        /// Fires when the user clicks "Login with Microsoft Account" button.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void LoginMSButton_Click(object sender, RoutedEventArgs e)
        {
            // Pass true so the method knows the user initiated the login and the login data is not coming from storage.

            try
            {
                await SavedLogin.MSLogin(true);
                // Fire the event!
                LoggedIn.Invoke(this, EventArgs.Empty);
            }
            catch(WebException)
            {
                var msgDialog = new MessageDialog("Please check your internet connection.");
                await msgDialog.ShowAsync();
            }
        }
    }
}