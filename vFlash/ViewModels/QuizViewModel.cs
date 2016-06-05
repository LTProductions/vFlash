﻿using Prism.Commands;
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

        private bool _quizFinished = false;
        public bool QuizFinished
        {
            get { return _quizFinished; }
            set
            {
                if (_quizFinished != value)
                {
                    _quizFinished = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _showScoreModal = true;
        public bool ShowScoreModal
        {
            get { return _showScoreModal; }
            set
            {
                if (_showScoreModal != value)
                {
                    _showScoreModal = value;
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

        private DelegateCommand _showFinishedQuizCommand;
        public DelegateCommand ShowFinishedQuizCommand
        {
            get { return _showFinishedQuizCommand; }
        }

        private DelegateCommand<string> _rButtonCheckedCommand;
        public DelegateCommand<string> RButtonCheckedCommand
        {
            get { return _rButtonCheckedCommand; }
        }


        #endregion

        #region Constructor

        public QuizViewModel()
        {

            #region Command Initializers

            _submitAnswerCommand = new DelegateCommand(async delegate () {
                await SubmitAnswer();
            }, CanSubmitCheck);

            // _showFinishedQuizCommand = new DelegateCommand(null); // Method not made yet.

            _rButtonCheckedCommand = new DelegateCommand<string>(SetSelectedItem);

            // Views.Shell.Instance.SetScoreModal(true);

            #endregion




            #endregion

        }

        #region Methods

        private async Task SubmitAnswer()
        {
            if (SelectedItem != null)
            {
                bool isCorrect = String.Equals(SelectedItem, flashCards[index].Word_Side1);
                ScoreData score = new ScoreData() { SessionData_ID = studySession.Id, FCData_ID = flashCards[index].Id, Correct = isCorrect };
                scoreList.Add(score);

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
                        QuizFinished = true;

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

                        int finalCorrect = scoreList.Count(p => p.Correct == true);
                        int totalQuestions = flashCards.Count;
                        FinalScoreString = ($"Total: {finalCorrect.ToString()} / {totalQuestions.ToString()} ");
                        FinalPercentageString = ($"Score: {((finalCorrect / totalQuestions) * 100).ToString()}%");
                    }
                }
            }
        }

        private bool CanSubmitCheck()
        {
            return SelectedItem != null;
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
                var rnd = new Random(DateTime.Now.Millisecond);
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
                    ID = index,
                    Question = flashCards.ElementAt(index).Definition_Side2 != null ? flashCards.ElementAt(index).Definition_Side2 : string.Empty,
                    A = potentialAnswers[0],
                    B = potentialAnswers[1],
                    C = potentialAnswers[2],
                    D = potentialAnswers[3]
                };

                QuizObjectList.Add(QuizObject);
            }
            catch(ArgumentNullException)
            {
                QuizObject = null;
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