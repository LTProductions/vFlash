using Microsoft.WindowsAzure.MobileServices;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using vFlash.Models;
using vFlash.Utils;
using Windows.UI.Xaml.Navigation;

namespace vFlash.ViewModels
{
    public class QuizViewModel : BaseDataPage
    {

        #region Fields/Properties

        // StudySessionData object; one study session per quiz.
        private StudySessionData studySession;

        // List of ScoreData that will be used for keeping track of user's score and inserting into the database.
        private List<ScoreData> scoreList = new List<ScoreData>();

        // List of the flashcards from the selected fcstack.
        private List<FlashcardData> flashCards;

        // List of potential answers that will be loaded into the QuizObject.
        private List<string> potentialAnswers;

        // List of the answers selected by the user; this is needed so that the quiz can be reviewed and the user can see what they got right/wrong.
        private List<string> selectedAnswers = new List<string>();

        // List of QuizObjects; holds each item that is created for the quiz using random answers/questions.
        private ObservableCollection<QuizModel> _quizObjectList = new ObservableCollection<QuizModel>();
        public ObservableCollection<QuizModel> QuizObjectList
        {
            get { return _quizObjectList; }
            set
            {
                if (_quizObjectList != value)
                {
                    _quizObjectList = value;
                    RaisePropertyChanged();
                }
            }
        }

        private QuizModel _quizObject;
        /// <summary>
        /// Holds the data for one question with 4 answers.
        /// </summary>
        public QuizModel QuizObject
        {
            get { return _quizObject; }
            set
            {
                if (_quizObject != value)
                {
                    _quizObject = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _selectedItem;
        /// <summary>
        /// Holds the data for the selected item by the user.
        /// </summary>
        public string SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    RaisePropertyChanged();
                    SubmitAnswerCommand.RaiseCanExecuteChanged();
                }
            }
        }

        // Keeps track of what position we are at in the flashcard list.
        private int index = 0;

        private string _finalScoreString;
        /// <summary>
        /// Holds the string showing the user's final score.
        /// </summary>
        public string FinalScoreString
        {
            get { return _finalScoreString; }
            set
            {
                if (_finalScoreString != value)
                {
                    _finalScoreString = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _finalPercentageString;
        /// <summary>
        /// Holds the string showing the user's final score as a percentage.
        /// </summary>
        public string FinalPercentageString
        {
            get { return _finalPercentageString; }
            set
            {
                if (_finalPercentageString != value)
                {
                    _finalPercentageString = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _showQuizBool = false;
        /// <summary>
        /// Boolean to track whether or not the quiz should be shown.
        /// </summary>
        public bool ShowQuizBool
        {
            get { return _showQuizBool; }
            set
            {
                if (_showQuizBool != value)
                {
                    _showQuizBool = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _showReviewBool = false;
        /// <summary>
        /// Boolean to track whether or not the Quiz Review should be shown.
        /// </summary>
        public bool ShowReviewBool
        {
            get { return _showReviewBool; }
            set
            {
                if (_showReviewBool != value)
                {
                    _showReviewBool = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _showScoreBool = false;
        /// <summary>
        /// Boolean to track whether or not the Score should be shown.
        /// </summary>
        public bool ShowScoreBool
        {
            get { return _showScoreBool; }
            set
            {
                if (_showScoreBool != value)
                {
                    _showScoreBool = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _showSubmitBool = false;
        /// <summary>
        /// Bool to track whether the submit button should be shown.
        /// </summary>
        public bool ShowSubmitBool
        {
            get { return _showSubmitBool; }
            set
            {
                if (_showSubmitBool != value)
                {
                    _showSubmitBool = value;
                    RaisePropertyChanged();
                }
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event used to fire the storyboard for showing the user's score.
        /// </summary>
        public event EventHandler ShowScoreEvent;

        /// <summary>
        /// Event used to fire the storyboard to fade in the quiz question + answers.
        /// Should fire when the quiz begins and after each answer submission.
        /// </summary>
        public event EventHandler FadeInQuizModelEvent;

        #endregion

        #region Commands

        private DelegateCommand _submitAnswerCommand;
        /// <summary>
        /// Command used for submitting an answer.
        /// </summary>
        public DelegateCommand SubmitAnswerCommand
        {
            get { return _submitAnswerCommand; }
        }

        private DelegateCommand _showQuizReviewCommand;
        /// <summary>
        /// Command used for showing the quiz in Review mode.
        /// </summary>
        public DelegateCommand ShowQuizReviewCommand
        {
            get { return _showQuizReviewCommand; }
        }

        private DelegateCommand<string> _rButtonCheckedCommand;
        /// <summary>
        /// This command fires when the user selects a radio button.
        /// </summary>
        public DelegateCommand<string> RButtonCheckedCommand
        {
            get { return _rButtonCheckedCommand; }
        }

        private DelegateCommand _endSessionGoBackCommand;
        /// <summary>
        /// This command fires when the user ends their study session; goes back to the previous page.
        /// </summary>
        public DelegateCommand EndSessionGoBackCommand
        {
            get { return _endSessionGoBackCommand; }
        }

        private DelegateCommand _nextReviewQuestionCommand;
        /// <summary>
        /// This command fires when the user selects to see the next review question.
        /// </summary>
        public DelegateCommand NextReviewQuestionCommand
        {
            get { return _nextReviewQuestionCommand; }
        }

        private DelegateCommand _prevReviewQuestionCommand;
        /// <summary>
        /// This command fires when the user selects to see the previous review question.
        /// </summary>
        public DelegateCommand PrevReviewQuestionCommand
        {
            get { return _prevReviewQuestionCommand; }
        }

        #endregion

        #region Constructor

        public QuizViewModel()
        {

            #region Command Initializers

            _submitAnswerCommand = new DelegateCommand(async delegate ()
            {
                await SubmitAnswer();
            }, CanSubmitCheck);

            _showQuizReviewCommand = new DelegateCommand(ReviewQuiz);

            _rButtonCheckedCommand = new DelegateCommand<string>(SetSelectedItem);

            _endSessionGoBackCommand = new DelegateCommand(delegate ()
            {
                NavigationService.GoBack();
            });

            _nextReviewQuestionCommand = new DelegateCommand(NextReviewQuestion);
            _prevReviewQuestionCommand = new DelegateCommand(PrevReviewQuestion);

            #endregion



        }

       #endregion

        #region Methods

        /// <summary>
        /// Method for submitting a selected item as an answer.
        /// </summary>
        /// <returns></returns>
        private async Task SubmitAnswer()
        {
            // Make sure the SelectedItem isn't null.
            if (SelectedItem != null)
            {
                // Determine whether or not the user has the correct answer and then save that information.
                bool isCorrect = String.Equals(SelectedItem, flashCards[index].Word_Side1);
                ScoreData score = new ScoreData() { SessionData_ID = studySession.Id, FCData_ID = flashCards[index].Id, Correct = isCorrect };
                scoreList.Add(score);
                selectedAnswers.Add(SelectedItem);

                // Increment the index; this card is done.
                index++;

                // Make sure there's another card to move onto before trying to load another question.
                if (index < flashCards.Count)
                {
                    // Fire the event for fading in the question + answers.
                    FadeInQuizModelEvent.Invoke(this, EventArgs.Empty);

                    // Reset the SelectedItem.
                    SelectedItem = null;
                    SubmitAnswerCommand.RaiseCanExecuteChanged();
                    LoadRandomAnswers();
                    SetQuizModel();

                    
                }

                // There are no cards left in this quiz.
                else
                {
                    // Make sure there's a score to save.
                    if (scoreList != null && scoreList.Count > 0)
                    {
                        // Hide the quiz.
                        ShowQuizBool = false;
                        ShowSubmitBool = false;

                        // Reset the SelectedItem to null.
                        SelectedItem = null;
                        SubmitAnswerCommand.RaiseCanExecuteChanged();

                        await SaveScore();

                        // Set visibility of the score panel to true.
                        ShowScoreBool = true;

                        // Fire the event that will set off the storyboard animation.
                        ShowScoreEvent.Invoke(this, EventArgs.Empty);

                        // Determine how many answers the user got correct and format this information into a string.
                        double finalCorrect = scoreList.Count(p => p.Correct == true);
                        double totalQuestions = flashCards.Count;
                        double finalPercent = Math.Round((finalCorrect / totalQuestions), 2) * 100;
                        FinalScoreString = ($"Total: {finalCorrect.ToString()} / {totalQuestions.ToString()} ");
                        FinalPercentageString = ($"Score: {finalPercent.ToString()}%");
                    }
                }
            }
        }

        private async Task SaveScore()
        {
            // Show BusyModal to indicate the user's score is being saved.
            Views.Busy.SetBusy(true, "Saving your score...");
            // Save each score into the database.
            foreach (var item in scoreList)
            {
                await item.InsertItem(item);
            }
            Views.Busy.SetBusy(false);
        }

        /// <summary>
        /// Method for ensuring the Submit command can be fired.
        /// </summary>
        /// <returns></returns>
        private bool CanSubmitCheck()
        {
            return SelectedItem != null && SelectedItem != string.Empty;
        }

        /// <summary>
        /// Called by rButtonCheckedCommand; sets the selected item.
        /// </summary>
        /// <param name="s"></param>
        private void SetSelectedItem(string s)
        {
            SelectedItem = s;
        }

        /// <summary>
        /// Load all of the data.
        /// </summary>
        private async void LoadData()
        {
            Views.Busy.SetBusy(true, "Generating...");
            await LoadStudySessionData();
            await LoadFCardData();
            LoadRandomAnswers();
            SetQuizModel();
            ShowQuizBool = true;
            ShowSubmitBool = true;
            
            Views.Busy.SetBusy(false);

            // Fire the event for fading in the question + answers.
            FadeInQuizModelEvent.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Create a new StudySessionData to keep track of this specific study session.
        /// </summary>
        /// <returns></returns>
        private async Task LoadStudySessionData()
        {
            // Hard coded ID; this is a quiz, so it has the Quiz ID. (See database ERD).
            string quizNameID = "674E630C-2EC3-40B5-B6A8-5E91BB7D78AE";
            studySession = new StudySessionData() { SessionName_ID = quizNameID };
            await studySession.InsertItem(studySession);
        }

        /// <summary>
        /// Load the Flashcard Data based on the selected Stack.
        /// </summary>
        /// <returns></returns>
        private async Task LoadFCardData()
        {
            // Load the proper Flashcards based on the StackID.
            FlashcardData fc = new FlashcardData();
            var fcQuery = App.MobileService.GetTable<FlashcardData>().CreateQuery();
            var fcards = await fc.GetQueriedList<FlashcardData>(fcQuery.Where(p => p.FCStack_ID == passedItem.FCStackID));

            // Make sure the list isn't null or empty and shuffle it.
            if (fcards != null && fcards.Count >= 1)
            {
                var randomizedList = Randomizer.Shuffle1<FlashcardData>(fcards);
                flashCards = randomizedList.ToList();
            }
        }

        /// <summary>
        /// Randomize the answers based on all of the flashcards in this stack.
        /// </summary>
        private void LoadRandomAnswers()
        {
            // Make sure the flashcard list isn't empty.
            if (flashCards != null && flashCards.Count >= 1)
            {
                // Potential indexes will be every index for the flashcard list, except for the current index because that one contains the correct answer.
                List<int> potentialIndexes = new List<int>();
                for (int i = 0; i < flashCards.Count; i++)
                {
                    if (i != index)
                        potentialIndexes.Add(i);
                }

                // Shuffle the indexes.
                var indexTemp = Randomizer.Shuffle1(potentialIndexes);
                potentialIndexes = indexTemp.ToList();

                // Create a list of potential answers based on the shuffled potential indexes.
                potentialAnswers = new List<string>();
                potentialAnswers.Add(flashCards.ElementAt(index).Word_Side1);

                // If there is a corresponding flashcard up to 4, set it. Otherwise, empty string.
                if (flashCards.Count() >= 2)
                    potentialAnswers.Add(flashCards.ElementAt(potentialIndexes[0]).Word_Side1);
                else
                    potentialAnswers.Add(string.Empty);

                if (flashCards.Count() >= 3)
                    potentialAnswers.Add(flashCards.ElementAt(potentialIndexes[1]).Word_Side1);
                else
                    potentialAnswers.Add(string.Empty);

                if (flashCards.Count() >= 4)
                    potentialAnswers.Add(flashCards.ElementAt(potentialIndexes[2]).Word_Side1);
                else
                    potentialAnswers.Add(string.Empty);

                var temp = Randomizer.Shuffle1(potentialAnswers);
                potentialAnswers = temp.ToList();
            }
        }

        /// <summary>
        /// Set the QuizObject based on the current index.
        /// </summary>
        private void SetQuizModel()
        {
            try
            {
                QuizObject = new QuizModel()
                {
                    ID = (index + 1).ToString() + ".",
                    Question = flashCards.ElementAt(index).Definition_Side2 != null ? flashCards.ElementAt(index).Definition_Side2 : string.Empty,
                    A = potentialAnswers[0],
                    B = potentialAnswers[1],
                    C = potentialAnswers[2],
                    D = potentialAnswers[3]
                };

                QuizObjectList.Add(QuizObject);
            }
            catch (ArgumentNullException)
            {
                QuizObject = null;
            }
        }

        /// <summary>
        /// Review the quiz, showing which answers are correct and which are incorrect.
        /// </summary>
        private void ReviewQuiz()
        {
            // Reset the index, hide the score, show the review, hide the submit button.
            index = 0;
            ShowScoreBool = false;
            ShowQuizBool = true;
            ShowSubmitBool = false;
            ShowReviewBool = true;

            QuizObject = new QuizModel()
            {
                ID = (index + 1).ToString() + ".",
                Question = QuizObjectList.ElementAt(index).Question,
                A = QuizObjectList.ElementAt(index).A,
                B = QuizObjectList.ElementAt(index).B,
                C = QuizObjectList.ElementAt(index).C,
                D = QuizObjectList.ElementAt(index).D
            };

            // Check this set's answer and mark right or wrong.
            SetBGCorrectOrIncorrect();
        }

        /// <summary>
        /// Check the user's answers for the current review card; if they're right, the background will be green. If they're wrong, the background will
        /// be red and the correct answer will have a green background.
        /// </summary>
        private void SetBGCorrectOrIncorrect()
        {
            // If the user got the answer right, _BG will be GREEN; else, RED.
            if (string.Equals(QuizObjectList.ElementAt(index).A, selectedAnswers.ElementAt(index)))
            {
                if (string.Equals(QuizObjectList.ElementAt(index).A, flashCards.ElementAt(index).Word_Side1))
                    QuizObject.ABG = "GREEN";
                else
                    QuizObject.ABG = "RED";
            }
            // If the user didn't select this answer but it is the correct answer, the BG should be GREEN.
            else if (string.Equals(QuizObjectList.ElementAt(index).A, flashCards.ElementAt(index).Word_Side1))
                QuizObject.ABG = "GREEN";

            if (string.Equals(QuizObjectList.ElementAt(index).B, selectedAnswers.ElementAt(index)))
            {
                if (string.Equals(QuizObjectList.ElementAt(index).B, flashCards.ElementAt(index).Word_Side1))
                    QuizObject.BBG = "GREEN";
                else
                    QuizObject.BBG = "RED";
            }
            else if (string.Equals(QuizObjectList.ElementAt(index).B, flashCards.ElementAt(index).Word_Side1))
                QuizObject.BBG = "GREEN";

            if (string.Equals(QuizObjectList.ElementAt(index).C, selectedAnswers.ElementAt(index)))
            {
                if (string.Equals(QuizObjectList.ElementAt(index).C, flashCards.ElementAt(index).Word_Side1))
                    QuizObject.CBG = "GREEN";
                else
                    QuizObject.CBG = "RED";
            }
            else if (string.Equals(QuizObjectList.ElementAt(index).C, flashCards.ElementAt(index).Word_Side1))
                QuizObject.CBG = "GREEN";

            if (string.Equals(QuizObjectList.ElementAt(index).D, selectedAnswers.ElementAt(index)))
            {
                if (string.Equals(QuizObjectList.ElementAt(index).D, flashCards.ElementAt(index).Word_Side1))
                    QuizObject.DBG = "GREEN";
                else
                    QuizObject.DBG = "RED";
            }
            else if (string.Equals(QuizObjectList.ElementAt(index).D, flashCards.ElementAt(index).Word_Side1))
                QuizObject.DBG = "GREEN";
        }
    
        /// <summary>
        /// Move to the next Review Question, if there is one. Otherwise, show the Score again.
        /// </summary>
        private void NextReviewQuestion()
        {
            // Make sure we aren't at the end of the list.
            if (index < QuizObjectList.Count())
            {
                index++;

                // Set the object to the new question based on the index.
                QuizObject = new QuizModel()
                {
                    ID = (index + 1).ToString() + ".",
                    Question = QuizObjectList.ElementAt(index).Question,
                    A = QuizObjectList.ElementAt(index).A,
                    B = QuizObjectList.ElementAt(index).B,
                    C = QuizObjectList.ElementAt(index).C,                  
                    D = QuizObjectList.ElementAt(index).D,                  
                };

                SetBGCorrectOrIncorrect();
            }

            else
            {
                // Hide the review and show the score again.
                ShowReviewBool = false;
                ShowScoreBool = true;
                ShowQuizBool = false;
            }
            
        }

        /// <summary>
        /// Move to the previous review question, if there is one. If not, show the score again.
        /// </summary>
        private void PrevReviewQuestion()
        {
            if (index > 0)
            {
                index--;

                // Set the object to the new question based on the index.
                QuizObject = new QuizModel()
                {
                    ID = (index + 1).ToString() + ".",
                    Question = QuizObjectList.ElementAt(index).Question,
                    A = QuizObjectList.ElementAt(index).A,
                    B = QuizObjectList.ElementAt(index).B,
                    C = QuizObjectList.ElementAt(index).C,
                    D = QuizObjectList.ElementAt(index).D,
                };

                SetBGCorrectOrIncorrect();
            }

            else
            {
                // Hide the review and show the score again.
                ShowReviewBool = false;
                ShowScoreBool = true;
                ShowQuizBool = false;
            }
        }


        #endregion


        #region Navigation

        private string _Value = "Default";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            passedItem = (NamesAndIDs)parameter;
            FCStackName = passedItem.FCStackName;
            ClassName = passedItem.ClassName;
            SubclassName = passedItem.SubclassName;
            LoadData();
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
