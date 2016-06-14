using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace vFlash.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FCStackAddPage : Page
    {
        public FCStackAddPage()
        {
            this.InitializeComponent();
        }

        private void FCStackName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (FCStackName.Text != CheckBoxText(FCStackName.Text))
            {
                int CaretPosition = FCStackName.SelectionStart - 1;
                FCStackName.Text = CheckBoxText(FCStackName.Text);
                FCStackName.SelectionStart = CaretPosition;
            }
        }

        public string CheckBoxText(string val)
        {
            var r = new Regex("[^a-zA-Z0-9-_]+");
            return r.Replace(val, "");
        }
    }
}
