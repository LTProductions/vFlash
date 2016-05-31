using System;
using System.Collections.Generic;
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

        private List<ScoreData> scoreList;

        private List<FlashcardData> flashCards;

        private List<string> potentialAnswers;

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

        private int index = 0;


        #endregion

        #region Commands




        #endregion

        #region Constructor




        #endregion

        #region Methods

        public async void LoadData()
        {
            await LoadStudySessionData();
            await LoadFCardData();
            LoadRandomAnswers();
            SetQuizModel();
        }

        public async Task LoadStudySessionData()
        {
            DateTime now = DateTime.Now;
            studySession = new StudySessionData() { CreatedAt = now };
            await studySession.InsertItem(studySession);
            var ssQuery = App.MobileService.GetTable<StudySessionData>().CreateQuery();
            var listOfOne = await studySession.GetQueriedList(ssQuery.Where(p => p.CreatedAt == now));

            if (listOfOne != null)
                studySession = listOfOne[0];
        }

        public async Task LoadFCardData()
        {
            FlashcardData fc = new FlashcardData();
            var fcQuery = App.MobileService.GetTable<FlashcardData>().CreateQuery();
            var fcards = await fc.GetQueriedList<FlashcardData>(fcQuery.Where(p => p.FCStack_ID == passedItem.FCStackID));

            if (fcards != null)
            {
                var randomizedList = Randomizer.Shuffle1<FlashcardData>(fcards);
                flashCards = randomizedList.ToList();
            }
        }

        public void LoadRandomAnswers()
        {
            if (flashCards != null)
            {
                var rnd = new Random(DateTime.Now.Millisecond);
                potentialAnswers = new List<string>();
                potentialAnswers[0] = flashCards.ElementAt(index).Word_Side1;
                if (flashCards.Count() >= 2)
                    potentialAnswers[1] = flashCards.ElementAt(rnd.Next(0, flashCards.Count())).Word_Side1;
                if (flashCards.Count() >= 3)
                    potentialAnswers[2] = flashCards.ElementAt(rnd.Next(0, flashCards.Count())).Word_Side1;
                if (flashCards.Count() >= 4)
                    potentialAnswers[3] = flashCards.ElementAt(rnd.Next(0, flashCards.Count())).Word_Side1;

                var temp = Randomizer.Shuffle1(potentialAnswers);
                potentialAnswers = temp.ToList();
            }
        }

        public void SetQuizModel()
        {
            QuizObject = new QuizModel()
            {
                ID = index,
                Question = flashCards.ElementAt(index).Definition_Side2 != null ? flashCards.ElementAt(index).Definition_Side2 : string.Empty,
                A = potentialAnswers[0] != null ? potentialAnswers[0] : string.Empty,
                B = potentialAnswers[1] != null ? potentialAnswers[1] : string.Empty,
                C = potentialAnswers[2] != null ? potentialAnswers[2] : string.Empty,
                D = potentialAnswers[3] != null ? potentialAnswers[3] : string.Empty
            };
        }


        #endregion


        #region Navigation

        private string _Value = "Default";
        public string Value { get { return _Value; } set { Set(ref _Value, value); } }

        public override async Task OnNavigatedToAsync(object parameter, NavigationMode mode, IDictionary<string, object> suspensionState)
        {
            passedItem = (NamesAndIDs)parameter;
            FCStackName = passedItem.FCStackName;
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
