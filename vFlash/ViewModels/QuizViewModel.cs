using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template10.Services.NavigationService;
using vFlash.Models;
using vFlash.Utils;
using Windows.UI.Xaml.Navigation;

namespace vFlash.ViewModels
{
    public class QuizViewModel : Template10.Mvvm.ViewModelBase
    {

        #region Fields/Properties

        public NamesAndIDs passedItem;

        private StudySessionData studySession;

        private List<ScoreData> scoreList = new List<ScoreData>();

        private List<FlashcardData> flashCards;

        private List<string> potentialAnswers;

        private List<string> selectedAnswers = new List<string>();

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

        private string _fcStackName;
        public string FCStackName
        {
            get { return _fcStackName; }
            set
            {
                if (_fcStackName != value)
                {
                    _fcStackName = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _selectedItem;
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

        private int index = 0;

        private string _finalScoreString;
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

        private bool _showQuizBool = true;
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

        private bool _showScoreBool = true;
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

        #endregion

        #region Commands

        private DelegateCommand _submitAnswerCommand;
        public DelegateCommand SubmitAnswerCommand
        {
            get { return _submitAnswerCommand; }
        }

        private DelegateCommand _showQuizReviewCommand;
        public DelegateCommand ShowQuizReviewCommand
        {
            get { return _showQuizReviewCommand; }
        }

        private DelegateCommand<string> _rButtonCheckedCommand;
        public DelegateCommand<string> RButtonCheckedCommand
        {
            get { return _rButtonCheckedCommand; }
        }

        private DelegateCommand _endSessionGoBackCommand;
        public DelegateCommand EndSessionGoBackCommand
        {
            get { return _endSessionGoBackCommand; }
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

            #endregion



        }

       #endregion

        #region Methods

        private async Task SubmitAnswer()
        {
            if (SelectedItem != null)
            {
                bool isCorrect = String.Equals(SelectedItem, flashCards[index].Word_Side1);
                ScoreData score = new ScoreData() { SessionData_ID = studySession.Id, FCData_ID = flashCards[index].Id, Correct = isCorrect };
                scoreList.Add(score);
                selectedAnswers.Add(SelectedItem);

                index++;

                if (index < flashCards.Count)
                {
                    SelectedItem = null;
                    SubmitAnswerCommand.RaiseCanExecuteChanged();
                    LoadRandomAnswers();
                    SetQuizModel();
                }

                else
                {
                    if (scoreList != null && scoreList.Count > 0)
                    {
                        ShowQuizBool = false;

                        SelectedItem = null;
                        SubmitAnswerCommand.RaiseCanExecuteChanged();

                        Views.Busy.SetBusy(true, "Saving your score...");
                        foreach (var item in scoreList)
                        {
                            bool success = await item.InsertItem(item);
                            if (!success)
                            {
                                // **HANDLE ERROR** An item has failed to be inserted, delete saved data and don't save any more.
                                return;
                            }
                        }
                        Views.Busy.SetBusy(false);

                        float finalCorrect = scoreList.Count(p => p.Correct == true);
                        float totalQuestions = flashCards.Count;
                        FinalScoreString = ($"Total: {finalCorrect.ToString()} / {totalQuestions.ToString()} ");
                        FinalPercentageString = ($"Score: {((finalCorrect / totalQuestions) * 100).ToString()}%");
                    }
                }
            }
        }

        private bool CanSubmitCheck()
        {
            return SelectedItem != null && SelectedItem != string.Empty;
        }

        private void SetSelectedItem(string s)
        {
            SelectedItem = s;
        }

        private async void LoadData()
        {
            Views.Busy.SetBusy(true, "Generating...");
            await LoadStudySessionData();
            await LoadFCardData();
            LoadRandomAnswers();
            SetQuizModel();
            Views.Busy.SetBusy(false);
        }

        private async Task LoadStudySessionData()
        {
            string quizNameID = "979E4989-21DF-450F-A2A6-05634C4F87BA";
            studySession = new StudySessionData() { SessionName_ID = quizNameID };
            await studySession.InsertItem(studySession);
        }

        private async Task LoadFCardData()
        {
            FlashcardData fc = new FlashcardData();
            var fcQuery = App.MobileService.GetTable<FlashcardData>().CreateQuery();
            var fcards = await fc.GetQueriedList<FlashcardData>(fcQuery.Where(p => p.FCStack_ID == passedItem.FCStackID));

            if (fcards != null && fcards.Count >= 1)
            {
                var randomizedList = Randomizer.Shuffle1<FlashcardData>(fcards);
                flashCards = randomizedList.ToList();
            }
        }

        private void LoadRandomAnswers()
        {
            if (flashCards != null && flashCards.Count >= 1)
            {
                List<int> potentialIndexes = new List<int>();
                for (int i = 0; i < flashCards.Count; i++)
                {
                    if (i != index)
                        potentialIndexes.Add(i);
                }

                var indexTemp = Randomizer.Shuffle1(potentialIndexes);
                potentialIndexes = indexTemp.ToList();

                potentialAnswers = new List<string>();
                potentialAnswers.Add(flashCards.ElementAt(index).Word_Side1);

                if (flashCards.Count() >= 2)
                    potentialAnswers.Add(flashCards.ElementAt(potentialIndexes[0]).Word_Side1);
                else
                    potentialAnswers.Add("");

                if (flashCards.Count() >= 3)
                    potentialAnswers.Add(flashCards.ElementAt(potentialIndexes[1]).Word_Side1);
                else
                    potentialAnswers.Add("");

                if (flashCards.Count() >= 4)
                    potentialAnswers.Add(flashCards.ElementAt(potentialIndexes[2]).Word_Side1);
                else
                    potentialAnswers.Add("");

                var temp = Randomizer.Shuffle1(potentialAnswers);
                potentialAnswers = temp.ToList();
            }
        }

        private void SetQuizModel()
        {
            try
            {
                QuizObject = new QuizModel()
                {
                    ID = index + 1,
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

        private void ReviewQuiz()
        {
            index = 0;
            ShowScoreBool = false;
            ShowReviewBool = true;

            QuizObject = new QuizModel()
            {
                ID = index + 1,
                Question = QuizObjectList.ElementAt(index).Question,
                A = QuizObjectList.ElementAt(index).A,
                B = QuizObjectList.ElementAt(index).B,
                C = QuizObjectList.ElementAt(index).C,
                D = QuizObjectList.ElementAt(index).D
            };

            if (string.Equals(QuizObjectList.ElementAt(index).A, selectedAnswers.ElementAt(index)))
            {
                if (string.Equals(QuizObjectList.ElementAt(index).A, flashCards.ElementAt(index).Word_Side1))
                    QuizObject.ABG = "GREEN";
                else
                    QuizObject.ABG = "RED";
            }

            if (string.Equals(QuizObjectList.ElementAt(index).B, selectedAnswers.ElementAt(index)))
            {
                if (string.Equals(QuizObjectList.ElementAt(index).B, flashCards.ElementAt(index).Word_Side1))
                    QuizObject.BBG = "GREEN";
                else
                    QuizObject.BBG = "RED";
            }

            if (string.Equals(QuizObjectList.ElementAt(index).C, selectedAnswers.ElementAt(index)))
            {
                if (string.Equals(QuizObjectList.ElementAt(index).C, flashCards.ElementAt(index).Word_Side1))
                    QuizObject.CBG = "GREEN";
                else
                    QuizObject.CBG = "RED";
            }

            if (string.Equals(QuizObjectList.ElementAt(index).D, selectedAnswers.ElementAt(index)))
            {
                if (string.Equals(QuizObjectList.ElementAt(index).D, flashCards.ElementAt(index).Word_Side1))
                    QuizObject.DBG = "GREEN";
                else
                    QuizObject.DBG = "RED";
            }
        }
    

        private void NextReviewQuestion()
        {
            if (index < QuizObjectList.Count())
            {
                index++;

                QuizObject = new QuizModel()
                {
                    ID = index + 1,
                    Question = QuizObjectList.ElementAt(index).Question,
                    A = QuizObjectList.ElementAt(index).A,
                    ABG = string.Equals(QuizObjectList.ElementAt(index).A, flashCards.ElementAt(index).Word_Side1) ? "GREEN" : "RED",
                    B = QuizObjectList.ElementAt(index).B,
                    BBG = string.Equals(QuizObjectList.ElementAt(index).B, flashCards.ElementAt(index).Word_Side1) ? "GREEN" : "RED",
                    C = QuizObjectList.ElementAt(index).C,
                    CBG = string.Equals(QuizObjectList.ElementAt(index).C, flashCards.ElementAt(index).Word_Side1) ? "GREEN" : "RED",
                    D = QuizObjectList.ElementAt(index).D,
                    DBG = string.Equals(QuizObjectList.ElementAt(index).D, flashCards.ElementAt(index).Word_Side1) ? "GREEN" : "RED"
                };
            }

            else
            {
                ShowReviewBool = false;
                ShowScoreBool = true;
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
