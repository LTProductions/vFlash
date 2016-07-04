using System;
using System.ComponentModel;
using System.Linq;
using Template10.Common;
using Template10.Controls;
using Template10.Services.NavigationService;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace vFlash.Views
{
    public sealed partial class Shell : Page
    {
        public static Shell Instance { get; set; }
        public static HamburgerMenu HamburgerMenu => Instance.MyHamburgerMenu;
        

        public Shell()
        {
            Instance = this;
            InitializeComponent();
            if (App.MobileService.CurrentUser == null)
                LoginModal.IsModal = true;
        }

        public Shell(INavigationService navigationService) : this()
        {
            SetNavigationService(navigationService);
        }

        public void SetNavigationService(INavigationService navigationService)
        {
            MyHamburgerMenu.NavigationService = navigationService;
        }

        #region Login & User Info

        private void LoginLoggedIn(object sender, EventArgs e)
        {
           MyHamburgerMenu.NavigationService.Navigate(typeof(Views.MainPage));
           LoginModal.IsModal = false;
           
        }

        public static void SetLoginModal()
        {
            Instance.LoginModal.IsModal = true;
            Shell.Instance.MyHamburgerMenu.NavigationService.Navigate(typeof(Views.LoginRegisterMain));
        }

        #endregion
    }
}

