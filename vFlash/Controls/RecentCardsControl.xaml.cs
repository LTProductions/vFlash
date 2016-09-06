using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace vFlash.Controls
{
    public sealed partial class RecentCardsControl : UserControl
    {
        public RecentCardsControl()
        {
            this.InitializeComponent();
            //(this.Content as FrameworkElement).DataContext = this;
        }

        //public DelegateCommand Command
        //{
        //    get { return (DelegateCommand)GetValue(CommandProperty); }
        //    set { SetValueDp(CommandProperty, value); }
        //}
        //public static readonly DependencyProperty CommandProperty =
        //    DependencyProperty.Register("Command", typeof(DelegateCommand), typeof(RecentCardsControl), null);

        //public event PropertyChangedEventHandler PropertyChanged;
        //void SetValueDp(DependencyProperty property, object value,
        //    [System.Runtime.CompilerServices.CallerMemberName] String p = null)
        //{
        //    SetValue(property, value);
        //    if (PropertyChanged != null)
        //        PropertyChanged(this, new PropertyChangedEventArgs(p));
        //}
    }
}
