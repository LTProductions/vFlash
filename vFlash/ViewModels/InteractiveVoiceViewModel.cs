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

namespace vFlash.ViewModels
{
    /// <summary>
    /// ViewModel used for an Interactive Voice Study Session.
    /// Corresponding View: InteractiveVoicePage.xaml
    /// </summary>
    public class InteractiveVoiceViewModel : BaseDataPage
    {

        #region Fields/Properties

        // Required field for speech.
        private static uint HResultPrivacyStatementDeclined = 0x80045509;

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

        // Keep track of how many times the user has failed a question; only allow one retry.
        private int tryAgainAttempts = 0;

        #endregion


        private string _currentDefinition;
        /// <summary>
        /// Holds the current definition that will be shown/read to the user.
        /// </summary>
        public string CurrentDefinition
        {
            get { return _currentDefinition; }
            set
            {
                _currentDefinition = value;
                RaisePropertyChanged();
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
                _currentWord = value;
                RaisePropertyChanged();
            }
        }

        // Index used for keeping track of the position in the flashCard list.
        private int index = 0;

        #endregion


        #region Commands

        private DelegateCommand _playSpeakQuestionCommand;
        public DelegateCommand PlaySpeakQuestionCommand
        {
            get { return _playSpeakQuestionCommand; }
        }

        #endregion



        #region Constructor

        public InteractiveVoiceViewModel()
        {

            _playSpeakQuestionCommand = new DelegateCommand(async delegate ()
            {
                await ReadCurrentQuestion();
            });
        }

        #endregion


        #region Methods

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
                // Speak the current definition and wait for the MediaEnded event to fire.
                await SpeakAndAwaitMediaEnded(flashCards[index].Definition_Side2);

                // Wait for the user to answer and then store their response.
                string userAnswer = await ListenForAnswer(null);

                // If the user gave the correct answer...
                if (string.Equals(userAnswer, flashCards[index].Word_Side1))
                {
                    // String that contains a random response from speechCorrectResponses list.
                    string correctResponses = Utils.SpeechConstants.speechCorrectResponses[GetRandom0To10()];

                    // Let the user know they were correct and move on.
                    await SpeakAndAwaitMediaEnded(correctResponses);

                    // Increment the index because this card is finished.
                    index++;
                    // Set tryAgainAttempts back to 0 because we are moving onto a new card.
                    if (tryAgainAttempts != 0)
                        tryAgainAttempts = 0;

                    // Save the score for this question.
                    SaveOneQuestionScore(true);

                    // Read the new card, if there is one.
                    await ReadCurrentQuestion();
                }
                // If the user says "I don't know" [the answer]...
                else if (string.Equals(userAnswer, "I don't know."))
                {
                    // Load the strings containing the words to tell the user the correct answer.
                    string movingOn = Utils.SpeechConstants.speechNextCardResponses[GetRandom0To10()];
                    string okay = "Okay, the answer is ";
                    string finalString = $"{okay} {flashCards[index].Word_Side1}. {movingOn}";
                    // Give answer and move on, incrementing index and resetting tryAgainAttempts.
                    await SpeakAndAwaitMediaEnded(finalString);
                    index++;
                    // Set tryAgainAttempts back to 0 because we are moving onto a new card.
                    if (tryAgainAttempts != 0)
                        tryAgainAttempts = 0;

                    // Save the score for this question.
                    SaveOneQuestionScore(false);

                    // Read the new card, if there is one.
                    await ReadCurrentQuestion();
                }
                else
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
                        // Check to see if the user says "yes".
                        bool userSaidYes = Utils.SpeechConstants.CheckForYes(userResponse);

                        // User gave the correct answer instead of just saying they want to try again.
                        if (string.Equals(userResponse, flashCards[index].Word_Side1))
                        {
                            // String that will be spoken to the user to let them know they have the correct answer.
                            string correctResponses = Utils.SpeechConstants.speechCorrectResponses[GetRandom0To10()];

                            // Speak to the user and let them know they're correct.
                            await SpeakAndAwaitMediaEnded(correctResponses);
                            index++;
                            // Set tryAgainAttempts back to 0 because we are moving onto a new card.
                            if (tryAgainAttempts != 0)
                                tryAgainAttempts = 0;

                            // Save the score for this question.
                            SaveOneQuestionScore(true);

                            // Read the next card, if there is one.
                            await ReadCurrentQuestion();
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

                                // Speak to the user and let them know they're correct.
                                await SpeakAndAwaitMediaEnded(correctResponses);
                                index++;
                                // Set tryAgainAttempts back to 0 because we are moving onto a new card.
                                if (tryAgainAttempts != 0)
                                    tryAgainAttempts = 0;

                                // Save the score for this question.
                                SaveOneQuestionScore(true);

                                // Read the next card, if there is one.
                                await ReadCurrentQuestion();
                            }
                            // User did not get the answer correct.
                            else
                            {
                                // Incorrect, answer is, moving on...
                                string movingOn = Utils.SpeechConstants.speechNextCardResponses[GetRandom0To10()];
                                string incorrectResponse2 = Utils.SpeechConstants.speechIncorrectResponses[GetRandom0To10()];
                                string okay = "The answer is ";
                                string finalString2 = $"{incorrectResponse2} {okay} {flashCards[index].Word_Side1}. {movingOn}";
                                // Give answer and move on.
                                await SpeakAndAwaitMediaEnded(finalString);
                                index++;
                                // Set tryAgainAttempts back to 0 because we are moving onto a new card.
                                if (tryAgainAttempts != 0)
                                    tryAgainAttempts = 0;

                                // Save the score for this question.
                                SaveOneQuestionScore(false);

                                // Read the next card, if there is one.
                                await ReadCurrentQuestion();
                            }
                        } // end if (userSaidYes)
                        
                        // User doesn't want to try again.
                        else
                        {
                            // Okay, moving on...
                            string movingOn = Utils.SpeechConstants.speechNextCardResponses[GetRandom0To10()];
                            string okay = "Okay, the answer is ";
                            string finalString2 = $"{okay} {flashCards[index].Word_Side1}. {movingOn}";
                            // Give answer and move on.
                            await SpeakAndAwaitMediaEnded(finalString2);
                            index++;
                            // Set tryAgainAttempts back to 0 because we are moving onto a new card.
                            if (tryAgainAttempts != 0)
                                tryAgainAttempts = 0;

                            // Save the score for this question.
                            SaveOneQuestionScore(false);

                            await ReadCurrentQuestion();
                        }
                    }

                    // User can't try again...
                    else
                    {
                        // Okay, moving on...
                        string movingOn = Utils.SpeechConstants.speechNextCardResponses[GetRandom0To10()];
                        string incorrectResponse = Utils.SpeechConstants.speechIncorrectResponses[GetRandom0To10()];
                        string okay = "The answer is ";
                        string finalString = $"{incorrectResponse} {okay} {flashCards[index].Word_Side1}. {movingOn}";
                        // Give answer and move on.
                        await SpeakAndAwaitMediaEnded(finalString);
                        index++;
                        // Set tryAgainAttempts back to 0 because we are moving onto a new card.
                        if (tryAgainAttempts != 0)
                            tryAgainAttempts = 0;

                        // Save the score for this question.
                        SaveOneQuestionScore(false);

                        await ReadCurrentQuestion();
                    }

                }
            }

            // Session is over.
            else
            {
                await SpeakAndAwaitMediaEnded("Good job; let's look at your score.");
                // save and show score.
                await SaveScoreList();
            }
        }

        #region Speech Methods

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

        /// <summary>
        /// Returns a random number between 0 and 10.
        /// </summary>
        /// <returns>int</returns>
        private int GetRandom0To10()
        {
            Random rnd = new Random(DateTime.Now.Millisecond);
            return rnd.Next(0, 10);
        }

        /// <summary>
        /// Save one question's score.
        /// </summary>
        /// <param name="score"></param>
        private void SaveOneQuestionScore(bool score)
        {
            ScoreData scoreItem = new ScoreData() { SessionData_ID = studySession.Id, FCData_ID = flashCards[index].Id, Correct = score };
            scoreList.Add(scoreItem);
        }

        /// <summary>
        /// Save the user's score into the database.
        /// </summary>
        /// <returns></returns>
        private async Task SaveScoreList()
        {
            foreach (var item in scoreList)
            {
                await item.InsertItem(item);
            }
        }

        #endregion


        #region Navigation

        private string _Value = "Default";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            passedItem = parameter as NamesAndIDs;
            await LoadData();
            Value = (suspensionState.ContainsKey(nameof(Value))) ? suspensionState[nameof(Value)]?.ToString() : parameter?.ToString();
            await Task.CompletedTask;
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
