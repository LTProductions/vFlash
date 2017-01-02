using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using vFlash.Models;
using vFlash.Utils;
using Windows.Foundation;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Navigation;
using System.IO;
using Prism.Commands;
using System.Net;
using System.Diagnostics;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Media;

namespace vFlash.ViewModels
{
    /// <summary>
    /// ViewModel used for an Interactive Voice Study Session.
    /// Corresponding View: InteractiveVoicePage.xaml
    /// </summary>
    public class InteractiveVoiceViewModel : BaseDataPage
    {

        #region Fields/Properties

        // Holds the flashcards for this group.
        private List<FlashcardData> flashCards;

        // StudySessionData object; one study session per.
        private StudySessionData studySession;

        // List of ScoreData that will be used for keeping track of user's score and inserting into the database.
        private List<ScoreData> scoreList = new List<ScoreData>();

        #region Speech

        // Fields used for Speech Recognition
        IAsyncOperation<SpeechRecognitionResult> recognitionOperation;
        SpeechRecognizer speechRecognizer;

        // Required field for speech.
        private static uint HResultPrivacyStatementDeclined = 0x80045509;

        #region MediaElements (breaking MVVM)

        private MediaElement _listeningSoundMedia;
        /// <summary>
        /// Media used to play a "listening" sound.
        /// </summary>
        public MediaElement ListeningSoundMedia
        {
            get { return _listeningSoundMedia; }
            set
            {
                if (_listeningSoundMedia != value)
                {
                    _listeningSoundMedia = value;
                    RaisePropertyChanged();
                }
            }
        }


        private MediaElement _speakMedia;
        /// <summary>
        /// Media used for TextToSpeech.
        /// </summary>
        public MediaElement SpeakMedia
        {
            get { return _speakMedia; }
            set
            {
                if (_speakMedia != value)
                {
                    _speakMedia = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #endregion

        // Keep track of how many times the user has failed a question; only allow one retry.
        private int tryAgainAttempts = 0;

        private string _currentDefinition;
        /// <summary>
        /// Holds the current definition that will be shown/read to the user.
        /// </summary>
        public string CurrentDefinition
        {
            get { return _currentDefinition; }
            set
            {
                if (_currentDefinition != value)
                {
                    _currentDefinition = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _currentWord;
        /// <summary>
        /// Holds the current answer that will be shown/read to the user.
        /// </summary>
        public string CurrentWord
        {
            get { return _currentWord; }
            set
            {
                if (_currentWord != value)
                {
                    _currentWord = value;
                    RaisePropertyChanged();
                }
            }
        }

        // Index used for keeping track of the position in the flashCard list.
        private int index = 0;

        private ObservableCollection<VoiceReviewModel> _reviewCardList = new ObservableCollection<VoiceReviewModel>();
        /// <summary>
        /// Collection that holds the list of Flashcards for this session.
        /// </summary>
        public ObservableCollection<VoiceReviewModel> ReviewCardList
        {
            get { return _reviewCardList; }
            set
            {   if (_reviewCardList != value)
                {
                    _reviewCardList = value;
                    RaisePropertyChanged();
                }
            }
        }


        private bool _inSession;
        /// <summary>
        /// Boolean for determining if there is currently a study session running.
        /// </summary>
        public bool InSession
        {
            get { return _inSession; }
            set
            {
                if (_inSession != value)
                {
                    _inSession = value;
                    RaisePropertyChanged();
                }
            }
        }

        //// Not needed thanks to the ShowScoreEvent??
        //private bool _showScore;
        ///// <summary>
        ///// Boolean to determine if the Score should be shown.
        ///// </summary>
        //public bool ShowScore
        //{
        //    get { return _showScore; }
        //    set
        //    {
        //        if (_showScore != value)
        //        {
        //            _showScore = value;
        //            RaisePropertyChanged();
        //        }
        //    }
        //}

        //// Not needed thanks to the HideScoreEvent??
        //private bool _showReview;
        ///// <summary>
        ///// Boolean for determining if the Review should be shown.
        ///// </summary>
        //public bool ShowReview
        //{
        //    get { return _showReview; }
        //    set
        //    {
        //        if (_showReview != value)
        //        {
        //            _showReview = value;
        //            RaisePropertyChanged();
        //        }
        //    }
        //}


        #endregion

        #region Commands

        private DelegateCommand _startSessionCommand;
        public DelegateCommand StartSessionCommand
        {
            get { return _startSessionCommand; }
        }

        private DelegateCommand _reviewCommand;
        public DelegateCommand ReviewCommand
        {
            get { return _reviewCommand; }
        }

        private DelegateCommand _endAndGoBackCommand;
        public DelegateCommand EndAndGoBackCommand
        {
            get { return _endAndGoBackCommand; }
        }

        private DelegateCommand _pauseSessionCommand;
        public DelegateCommand PauseSessionCommand
        {
            get { return _pauseSessionCommand; }
        }

        private DelegateCommand _unpauseSessionCommand;
        public DelegateCommand UnpauseSessionCommand
        {
            get { return _unpauseSessionCommand; }
        }

        public DelegateCommand _stopSessionCommand;
        public DelegateCommand StopSessionCommand
        {
            get { return _stopSessionCommand; }
        }

        #endregion

        #region Events

        // Events used to trigger storyboards.

        // Fire this when the session is beginning.
        public event EventHandler BeginSessionEvent;
        // Fire this when the answer is guessed correctly or being told.
        public event EventHandler FlipCardEvent;
        // Fire this when a new card is being read.
        public event EventHandler NextCardEvent;
        // Fire this when the score needs to be shown.
        public event EventHandler ShowScoreHideSessionEvent;
        // Fire this event when the score is being hidden.
        public event EventHandler HideScoreShowReviewEvent;

        #endregion

        #region Constructor

        public InteractiveVoiceViewModel()
        {
            // Set busy while waiting for data to load.
            Views.Busy.SetBusy(true, "Generating...");

            #region Command Initializers

            _startSessionCommand = new DelegateCommand(async delegate ()
            {
                await ReadCurrentQuestion();
            });

            _reviewCommand = new DelegateCommand(delegate ()
            {
                // ShowReview = true;
                HideScoreShowReviewEvent.Invoke(this, EventArgs.Empty);
            });

            _endAndGoBackCommand = new DelegateCommand(delegate ()
            {
                NavigationService.GoBack();
            });

            _pauseSessionCommand = new DelegateCommand(PauseSession, CanPauseSession);

            _unpauseSessionCommand = new DelegateCommand(UnpauseSession, CanUnpauseSession);

            _stopSessionCommand = new DelegateCommand(async delegate ()
            {
                await StopSession();
            }
            , CanStopSession);

            #endregion
        }

        #endregion

        #region Methods

        #region Data Loading Methods

        /// <summary>
        /// Load all of the data!
        /// </summary>
        /// <returns></returns>
        private async Task LoadData()
        {
            // Set the names of the items!
            ClassName = passedItem.ClassName;
            SubclassName = passedItem.SubclassName;
            FCStackName = passedItem.FCStackName;

            // Load the study session data.
            await LoadStudySessionData();

            // Load the lists of strings that will be used for TextToSpeech and recognition and load the media elements.
            SpeechConstants.LoadData();
            LoadMediaElements();
            await LoadFlashcards();
            Views.Busy.SetBusy(false);
        }

        /// <summary>
        /// Load the Flashcards based on the FCStack ID.
        /// </summary>
        /// <returns></returns>
        private async Task LoadFlashcards()
        {
            var fc = new FlashcardData();
            var query = App.MobileService.GetTable<FlashcardData>().CreateQuery();
            var list = await fc.GetQueriedList<FlashcardData>(query.Where(p => p.FCStack_ID == passedItem.FCStackID));
            flashCards = new List<FlashcardData>(list);
            // Shuffle the deck!
            var randomList = Randomizer.Shuffle1(flashCards);
            flashCards = new List<FlashcardData>(randomList);
        }

        /// <summary>
        /// Initialize and load the MediaElements.
        /// </summary>
        private void LoadMediaElements()
        {
            SpeakMedia = new MediaElement();

            if (ListeningSoundMedia == null)
            {
                ListeningSoundMedia = new MediaElement();
                ListeningSoundMedia.Source = new Uri("ms-appx:///Assets/cortanaListen.wav", UriKind.RelativeOrAbsolute);
                ListeningSoundMedia.AutoPlay = false;
            }
        }

        /// <summary>
        /// Create a new StudySessionData to keep track of this specific study session.
        /// </summary>
        /// <returns></returns>
        private async Task LoadStudySessionData()
        {
            // Hard coded ID; this is an interactive voice session, so it has the IntVoiceSession. (See database ERD).
            string interactiveVoiceNameID = "E243ECD7-A686-4D22-BA09-87D7F0EE2256";
            studySession = new StudySessionData() { SessionName_ID = interactiveVoiceNameID };
            await studySession.InsertItem(studySession);
        }

        #endregion

        #region Voice Interaction Methods

        /// <summary>
        /// Method used to await the MediaEnded event.
        /// </summary>
        /// <param name="wordsToSpeak"></param>
        /// <returns></returns>
        private async Task SpeakAndAwaitMediaEnded(string wordsToSpeak)
        {
            var tcs = new TaskCompletionSource<object>();
            SpeakMedia.MediaEnded += (o, e) => { tcs.TrySetResult(true); };
            await Speak(wordsToSpeak);
            await tcs.Task;

            // Reset the MediaElement.
            LoadMediaElements();

            await Task.CompletedTask;
        }

        /// <summary>
        /// Method that uses TextToSpeech to read the current Flashcard based on index.
        /// </summary>
        /// <returns></returns>
        private async Task ReadCurrentQuestion()
        {
            // Make sure the index is less than the total number of cards and that flashcards isn't null.
            if (index < flashCards.Count() && flashCards != null)
            {
                // Fire the event to move to the next card if the index isn't 0.
                // if (index > 0)
                    // NextCardEvent.Invoke(this, EventArgs.Empty);

                // Speak the current definition and wait for the MediaEnded event to fire.
                await SpeakAndAwaitMediaEnded(flashCards[index].Definition_Side2);

                // Wait for the user to answer and then store their response.
                string userAnswer = await ListenForAnswer(null);

                await CheckUserAnswer(userAnswer);
            }

            // Session is over.
            else
            {
                await SpeakAndAwaitMediaEnded("Okay, let's look at your score.");
                // save and show score.
                await SaveScoreList();
                // ShowScoreHideSessionEvent.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Checks the user's answer and determine what should be done.
        /// </summary>
        /// <param name="userAnswer"></param>
        /// <returns></returns>
        private async Task CheckUserAnswer(string userAnswer)
        {
            // If the user gave the correct answer...
            if (string.Equals(userAnswer, flashCards[index].Word_Side1))
            {
                // String that contains a random response from speechCorrectResponses list.
                string correctResponses = Utils.SpeechConstants.speechCorrectResponses[GetRandom0To10()];

                // Fire FlipCardEvent, speak, save one card's score, increment the index, reset tryAgainAttempts, and move onto the next card.
                await SaveIncrementResetRead(correctResponses, true);
            }

            // If the user says "I don't know" [the answer]...
            else if (string.Equals(userAnswer, "I don't know."))
            {
                // Load the strings containing the words to tell the user the correct answer.
                string movingOn = Utils.SpeechConstants.speechNextCardResponses[GetRandom0To10()];
                string okay = "Okay, the answer is ";
                string finalString = "";

                // If there isn't another card, don't say moving on. Else, do.
                if (index == flashCards.Count() - 1)
                {
                    finalString = $"{okay} {flashCards[index].Word_Side1}.";
                }
                else
                {
                    finalString = $"{okay} {flashCards[index].Word_Side1}. {movingOn}";
                }

                // Fire FlipCardEvent, speak, save one card's score, increment the index, reset tryAgainAttempts, and move onto the next card.
                await SaveIncrementResetRead(finalString, false);
            }

            // User got the answer wrong; if they can try again, try again. Else ...
            else
            {
                await TryAgainOrMoveOn();
            }
        }

        /// <summary>
        /// Method used for determining if the current card can be tried again; if so, ask "try again".
        /// </summary>
        /// <returns></returns>
        private async Task TryAgainOrMoveOn()
        {
            // If the user can try again...
            if (tryAgainAttempts == 0)
            {
                // User is being asked to try again; increment!
                tryAgainAttempts++;

                // Load the strings that be used for TTS to ask the user to try again.
                string incorrectResponse = Utils.SpeechConstants.speechIncorrectResponses[GetRandom0To10()];
                string finalString = $"{incorrectResponse} Want to try again?";

                // Ask if the user would like to try again.
                await SpeakAndAwaitMediaEnded(finalString);

                // Get the user's response and pass a list containing all of the possible ways they can say "yes."
                string userResponse = await ListenForAnswer(Utils.SpeechConstants.speechYesVersions);

                await CheckTryAgainAnswer(userResponse);
            }

            // User can't try again...
            else
            {
                // Okay, moving on...
                string movingOn = Utils.SpeechConstants.speechNextCardResponses[GetRandom0To10()];
                string incorrectResponse = Utils.SpeechConstants.speechIncorrectResponses[GetRandom0To10()];
                string okay = "The answer is ";
                string finalString = "";

                // If there isn't another card in the list, don't say a "moving on" phrase.
                if (index == flashCards.Count() - 1)
                {
                    finalString = $"{incorrectResponse} {okay} {flashCards[index].Word_Side1}. {movingOn}";
                }
                else
                {
                    finalString = $"{incorrectResponse} {okay} {flashCards[index].Word_Side1}. {movingOn}";
                }

                // Fire FlipCardEvent, speak, save one card's score, increment the index, reset tryAgainAttempts, and move onto the next card.
                await SaveIncrementResetRead(finalString, false);
            }
        }

        /// <summary>
        /// Check the user's answer when coming from the TryAgainOrMoveOn Task.
        /// </summary>
        /// <param name="userResponse"></param>
        /// <returns></returns>
        private async Task CheckTryAgainAnswer(string userResponse)
        {
            // Check to see if the user says "yes".
            bool userSaidYes = Utils.SpeechConstants.CheckForYes(userResponse);

            // User gave the correct answer instead of just saying they want to try again.
            if (string.Equals(userResponse, flashCards[index].Word_Side1))
            {
                // String that will be spoken to the user to let them know they have the correct answer.
                string correctResponses = Utils.SpeechConstants.speechCorrectResponses[GetRandom0To10()];

                // Fire FlipCardEvent, speak, save one card's score, increment the index, reset tryAgainAttempts, and move onto the next card.
                await SaveIncrementResetRead(correctResponses, true);
            }

            // User wants to try again.
            else if (userSaidYes)
            {
                // TTS, tell the user to give their answser.
                await SpeakAndAwaitMediaEnded("Okay, go ahead.");

                // Listen for the user's answer.
                string userResponse2 = await ListenForAnswer(null);

                if (string.Equals(userResponse2, flashCards[index].Word_Side1))
                {
                    // String that will be spoken to the user to let them know they have the correct answer.
                    string correctResponses = Utils.SpeechConstants.speechCorrectResponses[GetRandom0To10()];

                    // Fire FlipCardEvent, speak, save one card's score, increment the index, reset tryAgainAttempts, and move onto the next card.
                    await SaveIncrementResetRead(correctResponses, true);
                }
                // User did not get the answer correct.
                else
                {
                    // Incorrect, answer is, moving on...
                    string movingOn = Utils.SpeechConstants.speechNextCardResponses[GetRandom0To10()];
                    string incorrectResponse2 = Utils.SpeechConstants.speechIncorrectResponses[GetRandom0To10()];
                    string okay = "The answer is ";
                    string finalString2 = "";

                    // If there isn't another card in the list, don't say a "moving on" phrase.
                    if (index == flashCards.Count() - 1)
                    {
                        finalString2 = $"{incorrectResponse2} {okay} {flashCards[index].Word_Side1}.";
                    }
                    else
                    {
                        finalString2 = $"{incorrectResponse2} {okay} {flashCards[index].Word_Side1}. {movingOn}";
                    }

                    // Fire FlipCardEvent, speak, save one card's score, increment the index, reset tryAgainAttempts, and move onto the next card.
                    await SaveIncrementResetRead(finalString2, false);
                }
            } // end if (userSaidYes)

            // User doesn't want to try again.
            else
            {
                // Okay, moving on...
                string movingOn = Utils.SpeechConstants.speechNextCardResponses[GetRandom0To10()];
                string okay = "Okay, the answer is ";
                string finalString3 = "";

                // If there isn't another card in the list, don't say a "moving on" phrase.
                if (index == flashCards.Count() - 1)
                {
                    finalString3 = $"{okay} {flashCards[index].Word_Side1}.";
                }
                else
                {
                    finalString3 = $"{okay} {flashCards[index].Word_Side1}. {movingOn}";
                }

                // Fire FlipCardEvent, speak, save one card's score, increment the index, reset tryAgainAttempts, and move onto the next card.
                await SaveIncrementResetRead(finalString3, false);
            }
        }

        /// <summary>
        /// Task used to fire FlipCardEvent, speak, save one card's score, increment the index, reset tryAgainAttempts, and move onto the next card.
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        private async Task SaveIncrementResetRead(string speak, bool score)
        {
            // Fire the event to flip the card.
            // FlipCardEvent.Invoke(this, EventArgs.Empty);

            await SpeakAndAwaitMediaEnded(speak);

            // Save the score for this question.
            await SaveOneQuestionScore(score);
            index++;
            // Set tryAgainAttempts back to 0 because we are moving onto a new card.
            if (tryAgainAttempts != 0)
                tryAgainAttempts = 0;

            await ReadCurrentQuestion();
        }

        #endregion

        #region Pause/Play/Stop Methods

        /// <summary>
        /// Pauses any playing MediaElements. The command that calls this method with not be enabled
        /// when no MediaElements are playing.
        /// </summary>
        /// <returns></returns>
        private void PauseSession()
        {
            // Pause any playing MediaElements. 
            if (SpeakMedia.CanPause)
                SpeakMedia.Pause();

            if (ListeningSoundMedia.CanPause)
                ListeningSoundMedia.Pause();
        }

        /// <summary>
        /// Used for the PauseSessionCommand -- determines if the Command is enabled.
        /// </summary>
        /// <returns></returns>
        private bool CanPauseSession()
        {
            // If either element can pause, return true.
            return SpeakMedia.CanPause || ListeningSoundMedia.CanPause;
        }

        /// <summary>
        /// If a MediaElement is paused, play it.
        /// </summary>
        private void UnpauseSession()
        {
            // If the Media Element is paused, play it. Only one *should* be paused at a time; just in case, if, else if.
            if (SpeakMedia.CurrentState == MediaElementState.Paused)
                SpeakMedia.Play();
            else if (ListeningSoundMedia.CurrentState == MediaElementState.Paused)
                ListeningSoundMedia.Play();
        }

        /// <summary>
        /// Used for the UnpauseSessionCommand -- determines if the command is enabled.
        /// </summary>
        /// <returns></returns>
        private bool CanUnpauseSession()
        {
            // If either MediaElement is paused, return true.
            return SpeakMedia.CurrentState == MediaElementState.Paused || ListeningSoundMedia.CurrentState == MediaElementState.Paused;
        }

        /// <summary>
        /// If a MediaElement is playing, stop it. If a recognition is happening, stop it.
        /// This will also be called when the user tries to go back from this page.
        /// </summary>
        /// <returns></returns>
        private async Task StopSession()
        {
            if (SpeakMedia.CanPause)
                SpeakMedia.Stop();
            if (ListeningSoundMedia.CanPause)
                ListeningSoundMedia.Stop();

            if (recognitionOperation != null)
            {
                if (recognitionOperation.Status != AsyncStatus.Completed)
                {
                    try
                    {
                        recognitionOperation.Cancel();
                        recognitionOperation.Close();
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                    }
                }
            }

            if (speechRecognizer != null)
                await speechRecognizer.StopRecognitionAsync();
            

            // Show modal dialog and ask the user if they're sure they want to cancel; data will be lost.
            // If yes, go back.
        }

        /// <summary>
        /// Used to determine if the user can stop the current session based on playing media.
        /// </summary>
        /// <returns></returns>
        private bool CanStopSession()
        {
            return (SpeakMedia.CurrentState == MediaElementState.Playing || SpeakMedia.CurrentState == MediaElementState.Paused) ||
                (ListeningSoundMedia.CurrentState == MediaElementState.Playing || ListeningSoundMedia.CurrentState == MediaElementState.Paused);
        }

        #endregion

        #region Speak / Listen Methods

        /// <summary>
        /// Speaks the passed string via TTS and SpeakMedia.
        /// </summary>
        /// <param name="wordsToSpeak"></param>
        /// <returns></returns>
        private async Task Speak(string wordsToSpeak)
        {
            // Using statement for memory purposes.
            using (SpeechSynthesizer synthesizer = new SpeechSynthesizer())
            {
                // Create the stream that will be used.
                SpeechSynthesisStream synthesisStream;

                try
                {
                    // Set the stream with the string wordsToSpeak and set the source of SpeakMedia to the stream.
                    synthesisStream = await synthesizer.SynthesizeTextToStreamAsync(wordsToSpeak);
                    SpeakMedia.SetSource(synthesisStream, synthesisStream.ContentType);

                    SpeakMedia.AutoPlay = true;
                    SpeakMedia.Play();

                }
                // This device doesn't have the media player components.
                catch (System.IO.FileNotFoundException)
                {
                    // CHANGE TO MODAL DIALOG.
                    var messageDialog = new Windows.UI.Popups.MessageDialog("Media player components unavailable.");
                    await messageDialog.ShowAsync();

                }

                // No internet connection.
                catch (WebException)
                {
                    // Change to MODAL DIALOG.
                    var messageDialog = new Windows.UI.Popups.MessageDialog("Please check your internet connection.");
                    await messageDialog.ShowAsync();
                }

                // Catch all other exceptions.
                catch (Exception e)
                {
                    //var messageDialog = new Windows.UI.Popups.MessageDialog("I'm sorry, something went wrong. Please try again. If the problem persists, try restarting the app.");
                    //await messageDialog.ShowAsync();
                    Debug.WriteLine(e.ToString());
                }
            } // end using (SpeechSynthesizer)
        } // end Task SpeakQuestion()

        /// <summary>
        /// Uses SpeechRecognition to get what the user speaks based on constraints. If a list of strings
        /// of potential answers is required, pass the list of strings; else, pass null.
        /// </summary>
        /// <param name="constraintList"></param>
        /// <returns></returns>
        private async Task<string> ListenForAnswer(List<string> constraintList)
        {
            // Try to recognize voice input from user.
            try
            {
                // Using statement for memory purposes.
                using (speechRecognizer = new SpeechRecognizer())
                {
                    // Create the constraints for the recognition and add "I don't know."
                    var constraints = new List<string>();
                    constraints.Add(flashCards[index].Word_Side1);
                    constraints.Add("I don't know.");

                    // If a nonnull list was passed, add each item to the constraints list.
                    if (constraintList != null)
                    {
                        foreach (var item in constraintList)
                        {
                            constraints.Add(item);
                        }
                    }

                    // Create the SpeechRecognitionListConstraint based on the previous list.
                    var wordConstraints = new SpeechRecognitionListConstraint(constraints);

                    // Add the constraints to the recognizer.
                    speechRecognizer.Constraints.Add(wordConstraints);
                    await speechRecognizer.CompileConstraintsAsync();

                    // Play the "now listening" Cortana sound and wait for it to finish.
                    var tcs = new TaskCompletionSource<bool>();
                    ListeningSoundMedia.MediaEnded += (o, e) => { tcs.TrySetResult(true); };
                    ListeningSoundMedia.Play();
                    // Await MediaEnded event.
                    await tcs.Task;


                    // Start the recognizer.
                    recognitionOperation = speechRecognizer.RecognizeAsync();

                    // Get the result (word(s) the user says).
                    SpeechRecognitionResult speechResult = await recognitionOperation;

                    // Check for success.
                    if (speechResult.Status == SpeechRecognitionResultStatus.Success)
                    {
                        return speechResult.Text;
                    }

                    // User cancelled means the page was navigated away from.
                    else if (speechResult.Status == SpeechRecognitionResultStatus.UserCanceled)
                        return string.Empty;

                    // Else, the recognition has failed.
                    else
                    {
                        // Show error.
                        var messageDialog = new Windows.UI.Popups.MessageDialog("I'm sorry, I wasn't able to hear anything. Please check your microphone and try again.");
                        await messageDialog.ShowAsync();
                        return string.Empty;
                    }
                } // end using
            } // end try

            catch (TaskCanceledException exception)
            {
                // TaskCanceledException will be thrown if you exit the scenario while the recognizer is actively
                // processing speech. Since this happens here when we navigate out of the scenario, don't try to 
                // show a message dialog for this exception.
                System.Diagnostics.Debug.WriteLine("TaskCanceledException caught while recognition in progress (can be ignored):");
                System.Diagnostics.Debug.WriteLine(exception.ToString());
                return string.Empty;
            }

            // Catch for unknown exception or the microphone is disabled.
            catch (Exception exception)
            {

                // Handle the speech privacy policy error.
                if ((uint)exception.HResult == HResultPrivacyStatementDeclined)
                {
                    var messageDialog = new Windows.UI.Popups.MessageDialog("Please enable microphone access.");
                    await messageDialog.ShowAsync();
                    return string.Empty;
                }
                else
                {
                    Debug.WriteLine(exception.Message);
                    return string.Empty;
                }
            } // end catch (Exception exception)
        }

        #endregion

        #region Other Methods

        /// <summary>
        /// Returns a random number between 0 and 10.
        /// </summary>
        /// <returns>int</returns>
        private int GetRandom0To10()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            return rnd.Next(0, 11);
        }

        /// <summary>
        /// Save one question's score and add the item to the review list.
        /// </summary>
        /// <param name="score"></param>
        private async Task SaveOneQuestionScore(bool score)
        {
            ScoreData scoreItem = new ScoreData() { SessionData_ID = studySession.Id, FCData_ID = flashCards[index].Id, Correct = score };
            scoreList.Add(scoreItem);
            // If the user got the answer correct, set Color to green; else, red.
            string color = score == true ? "GREEN" : "RED";
            var reviewItem = new VoiceReviewModel() { Word = flashCards[index].Word_Side1, Definition = flashCards[index].Definition_Side2, Color = color };
            ReviewCardList.Add(reviewItem);

            await Task.CompletedTask;
        }

        /// <summary>
        /// Save the user's score into the database.
        /// </summary>
        /// <returns></returns>
        private async Task SaveScoreList()
        {
            Views.Busy.SetBusy(true, "Saving your score...");
            foreach (var item in scoreList)
            {
                await item.InsertItem(item);
            }
            Views.Busy.SetBusy(false);
        }

        #endregion

        #endregion


        #region Navigation

        private string _Value = "Default";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            this.NavigationService.FrameFacade.BackRequested += FrameFacade_BackRequested;
            passedItem = parameter as NamesAndIDs;
            await LoadData();
            Value = (suspensionState.ContainsKey(nameof(Value))) ? suspensionState[nameof(Value)]?.ToString() : parameter?.ToString();
            await Task.CompletedTask;
        }

        private async void FrameFacade_BackRequested(object sender, Template10.Common.HandledEventArgs e)
        {
            e.Handled = true;
            if (InSession)
            {
                await StopSession();
            }
        }

        public override async Task OnNavigatedFromAsync(IDictionary<string, object> suspensionState, bool suspending)
        {
            if (suspending)
            {
                suspensionState[nameof(Value)] = Value;
            }
            await Task.CompletedTask;
        }

        public override async Task OnNavigatingFromAsync(NavigatingEventArgs args)
        {
            args.Cancel = false;
            await Task.CompletedTask;
        }

        

        #endregion




    }
}
