using System;
using System.Collections.Generic;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace vFlash.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class QuizPage : Page
    {
        public QuizPage()
        {
            this.InitializeComponent();

            // Set up the event.
            QuizViewModel.ShowScoreEvent += (s, e) =>
            {
                // Begin the storyboard.
                ShowScoreStoryboard.Begin();
            };

            QuizViewModel.FadeInQuizModelEvent += (s, e) =>
            {
                // Begin the storyboard.
                FadeQuesAnswStoryboard.Begin();
            };

        }

        private void SubmitAnswerButton_Click(object sender, RoutedEventArgs e)
        {
            answerA.IsChecked = false;
            answerB.IsChecked = false;
            answerC.IsChecked = false;
            answerD.IsChecked = false;
        }

    }
}
