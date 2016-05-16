using Microsoft.WindowsAzure.MobileServices;
using System;
using System.Linq;
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

        public event EventHandler LoggedIn;
        

        private async void LoginMSButton_Click(object sender, RoutedEventArgs e)
        {
            await SavedLogin.MSLogin(true);
            LoggedIn.Invoke(this, EventArgs.Empty);
        }
    }
}