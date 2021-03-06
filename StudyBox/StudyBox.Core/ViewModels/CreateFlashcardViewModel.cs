﻿using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using StudyBox.Core.Interfaces;
using StudyBox.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StudyBox.Core.Models;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using Windows.UI.Popups;

namespace StudyBox.Core.ViewModels
{

    public class CreateFlashcardViewModel : ExtendedViewModelBase
    {
        private enum Mode { EditFlashcard, CreateFlashcardAndDeck, AddNewFlashcardToDeck, None }

        private Mode _mode = Mode.None;
        private Deck _deck;
        private Flashcard _flashcard;
        private IRestService _restService;
        private IInternetConnectionService _internetConnectionService;

        private string _question;
        private bool _isPublic;
        private string _answer;
        private string _deckName;
        private string _headerMessage;
        private string _submitFormMessage;
        private string _addTipMessage;
        private bool _isGeneralError;
        private bool _isDataLoading = false;
        private bool _isPaneVisible = true;
        private bool _isRemoveButtonVisible = false;
        private bool _isAddingTipsEnabled;
        private ObservableCollection<TipViewModel> _tipsCollection;
        private ObservableCollection<Flashcard> _flashcardsCollection;
        private Flashcard _selectedFlashcard;

        private readonly int _maxQuestionCharacters = 1000;
        private readonly int _maxAnswerCharacters = 1000;
        private readonly int _maxDeckNameCharacters = 50;
        private readonly int _minQuestionCharacters = 1;
        private readonly int _minAnswerCharacters = 1;
        private readonly int _minDeckNameCharacters = 1;
        private readonly int _maxTipsCount = 5;

        private RelayCommand _addTip;
        private RelayCommand _submitForm;
        private RelayCommand _cancel;
        private RelayCommand _togglePaneVisibility;
        private RelayCommand _removeFlashcard;

        public CreateFlashcardViewModel(INavigationService navigationService, IRestService restService, IInternetConnectionService internetConnectionService, IStatisticsDataService statisticsService) : base(navigationService)
        {
            Question = "";
            Answer = "";
            DeckName = "";
            this._restService = restService;
            this._internetConnectionService = internetConnectionService;
            TipsCollection = new ObservableCollection<TipViewModel>();
            FlashcardsCollection = new ObservableCollection<Flashcard>();
            IsPaneVisible = false;
            Remove = new RelayCommand<string>(RemoveTip);
            FlashcardSelected = new RelayCommand(EditSelectedFlashcard);
            Messenger.Default.Register<DataMessageToCreateFlashcard>(this, x => HandleDataMessage(x.DeckInstance, x.FlashcardIntance));
            Messenger.Default.Register<ConfirmMessageToRemove>(this, x => HandleConfirmMessage(x.IsConfirmed));
        }

        private void EditSelectedFlashcard()
        {
            if(SelectedFlashcard!=null)
            {
                HandleDataMessage(_deck, SelectedFlashcard);
                SelectedFlashcard = null;
            }
        }

        public RelayCommand AddTip
        {
            get
            {
                return _addTip ?? (_addTip = new RelayCommand(AddNewTip));
            }
        }

        public RelayCommand SubmitForm
        {
            get
            {
                return _submitForm ?? (_submitForm = new RelayCommand(CreateOrEditFlashcard));
            }
        }

        public RelayCommand Cancel
        {
            get
            {
                return _cancel ?? (_cancel = new RelayCommand(LeaveForm));
            }
        }

        public RelayCommand TogglePaneVisibility
        {
            get
            {
                return _togglePaneVisibility ?? (_togglePaneVisibility = new RelayCommand(ChangePaneVisibility));
            }
        }


        public RelayCommand RemoveFlashcard
        {
            get
            {
                return _removeFlashcard ?? (_removeFlashcard = new RelayCommand(RemoveFlashcardOrDeck));
            }
        }



        public ICommand Remove { get; set; }
        public ICommand FlashcardSelected { get; set; }

        public ObservableCollection<TipViewModel> TipsCollection
        {
            get
            {
                return _tipsCollection;
            }
            set
            {
                if (_tipsCollection != value)
                {
                    _tipsCollection = value;
                    RaisePropertyChanged();                                     
                }
            }
        }

      

        public ObservableCollection<Flashcard> FlashcardsCollection
        {
            get
            {
                return _flashcardsCollection;
            }
            set
            {
                if (_flashcardsCollection != value)
                {
                    _flashcardsCollection = value;
                    RaisePropertyChanged();
                }
            }
        }



        public bool IsGeneralError
        {
            get
            {
                return _isGeneralError;
            }
            set
            {
                if (_isGeneralError != value)
                {
                    _isGeneralError = value;
                    RaisePropertyChanged("IsGeneralError");
                }
            }
        }

        public string Question
        {
            get
            {
                return _question;
            }
            set
            {
                if (_question != value)
                {
                    _question = value;
                    RaisePropertyChanged("Question");
                    RaisePropertyChanged("CurrentQuestionCharactersNumber");
                    RaisePropertyChanged("IsQuestionValid");
                }
            }
        }

        public string Answer
        {
            get
            {
                return _answer;
            }
            set
            {
                if (_answer != value)
                {
                    _answer = value;
                    RaisePropertyChanged("Answer");
                    RaisePropertyChanged("CurrentAnswerCharactersNumber");
                    RaisePropertyChanged("IsAnswerValid");
                }
            }
        }

        public bool IsPublic
        {
            get
            {
                return _isPublic;
            }
            set
            {
                if (_isPublic != value)
                {
                    _isPublic = value;
                    RaisePropertyChanged("IsPublic");
                }
            }
        }

        public string DeckName
        {
            get
            {
                return _deckName;
            }
            set
            {
                if (_deckName != value)
                {
                    _deckName = value;
                    RaisePropertyChanged("DeckName");
                    RaisePropertyChanged("CurrentDeckNameCharactersNumber");
                    RaisePropertyChanged("IsDeckNameValid");
                }
            }
        }

        public Flashcard SelectedFlashcard
        {
            get
            {
                return _selectedFlashcard;
            }
            set
            {
                if (_selectedFlashcard != value)
                {
                    _selectedFlashcard = value;
                    RaisePropertyChanged("SelectedFlashcard");
                }
            }
        }

        public int CurrentQuestionCharactersNumber
        {
            get
            {
                return Question.Length;
            }
        }

        public int CurrentAnswerCharactersNumber
        {
            get
            {
                return Answer.Length;
            }
        }

        public int CurrentDeckNameCharactersNumber
        {
            get
            {
                return DeckName.Length;
            }
        }

        public int MaxQuestionCharacters
        {
            get
            {
                return _maxQuestionCharacters;
            }
        }

        public int MaxAnswerCharacters
        {
            get
            {
                return _maxAnswerCharacters;
            }
        }

        public int MaxDeckNameCharacters
        {
            get
            {
                return _maxDeckNameCharacters;
            }
        }

        public int MaxTipsCount
        {
            get
            {
                return _maxTipsCount;
            }
        }

        public bool IsQuestionValid
        {
            get
            {
                if (CurrentQuestionCharactersNumber > MaxQuestionCharacters || CurrentQuestionCharactersNumber < _minQuestionCharacters)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }


        public bool IsAnswerValid
        {
            get
            {
                if (CurrentAnswerCharactersNumber > MaxAnswerCharacters || CurrentAnswerCharactersNumber < _minAnswerCharacters)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }


        public bool IsDeckNameValid
        {
            get
            {
                if (CurrentDeckNameCharactersNumber > MaxDeckNameCharacters || CurrentDeckNameCharactersNumber < _minDeckNameCharacters)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public bool IsPaneVisible
        {
            get
            {
                return _isPaneVisible;
            }
            set
            {
                if (_isPaneVisible != value)
                {
                    _isPaneVisible = value;
                    RaisePropertyChanged("IsPaneVisible");
                }
            }
        }

        public bool IsRemoveButtonVisible
        {
            get
            {
                return _isRemoveButtonVisible;
            }
            set
            {
                if (_isRemoveButtonVisible != value)
                {
                    _isRemoveButtonVisible = value;
                    RaisePropertyChanged("IsRemoveButtonVisible");
                }
            }
        }

        public bool IsAddingTipsEnabled
        {
            get
            {
                return _isAddingTipsEnabled;
            }
            set
            {
                if (_isAddingTipsEnabled != value)
                {
                    _isAddingTipsEnabled = value;
                    RaisePropertyChanged("IsAddingTipsEnabled");
                }
            }
        }
        

        public string HeaderMessage
        {
            get
            {
                return _headerMessage;
            }
            set
            {
                if (_headerMessage != value)
                {
                    _headerMessage = value;
                    RaisePropertyChanged("HeaderMessage");
                }
            }
        }

        public string SubmitFormMessage
        {
            get
            {
                return _submitFormMessage;
            }
            set
            {
                if (_submitFormMessage != value)
                {
                    _submitFormMessage = value;
                    RaisePropertyChanged("SubmitFormMessage");
                }
            }
        }

        public string AddTipMessage
        {
            get
            {
                return _addTipMessage;
            }
            set
            {
                if (_addTipMessage != value)
                {
                    _addTipMessage = value;
                    RaisePropertyChanged("AddTipMessage");
                }
            }
        }

        public bool IsDataLoading
        {
            get
            {
                return _isDataLoading;
            }
            set
            {
                if (_isDataLoading != value)
                {
                    _isDataLoading = value;
                    RaisePropertyChanged();
                }
            }
        }


        public void RemoveTip(string id)
        {
            TipViewModel tip = TipsCollection.Where(x => x.ID == id).FirstOrDefault();
            if (tip != null)
            {
                TipsCollection.Remove(tip);
            }
            HandleTipsCountChanged();
        }

        public void AddNewTip()
        {
            TipsCollection.Add(new TipViewModel(System.Guid.NewGuid().ToString(), ""));
            HandleTipsCountChanged();
        }


        public async void RemoveFlashcardOrDeck()
        {
            if (!await _internetConnectionService.IsNetworkAvailable())
            {
                Messenger.Default.Send<MessageToMessageBoxControl>(new MessageToMessageBoxControl(true, false, true, true,
                    StringResources.GetString("NoInternetConnection")));
                return;
            }
            else if (!_internetConnectionService.IsInternetAccess())
            {
                Messenger.Default.Send<MessageToMessageBoxControl>(new MessageToMessageBoxControl(true, false,
                    StringResources.GetString("AccessDenied")));
                return;
            }
            else
            {
                int flashcardsCount = FlashcardsCollection.Count;
                if (flashcardsCount == 1)
                {
                    Messenger.Default.Send<MessageToMessageBoxControl>(new MessageToMessageBoxControl(true, true, true,
                        StringResources.GetString("RemoveLastFlashcardMessage")));
                    return;
                }
                else
                {
                    Messenger.Default.Send<MessageToMessageBoxControl>(new MessageToMessageBoxControl(true, true, true,
                        StringResources.GetString("RemoveMessage")));
                    return;
                }
            }
        }


        public async void CreateOrEditFlashcard()
        {
            if (!await _internetConnectionService.IsNetworkAvailable())
            {
                Messenger.Default.Send<MessageToMessageBoxControl>(new MessageToMessageBoxControl(true, false, true, true,
                    StringResources.GetString("NoInternetConnection")));
                return;
            }
            else if (!_internetConnectionService.IsInternetAccess())
            {
                Messenger.Default.Send<MessageToMessageBoxControl>(new MessageToMessageBoxControl(true, false, StringResources.GetString("AccessDenied")));
                return;
            }
            else
            {
                if (!ValidateForm())
                {
                    return;
                }

                _flashcard.Answer = Answer.TrimEnd();
                _flashcard.Question = Question.TrimEnd();

                string oldDeckName = _deck.Name;
                bool oldIsPublic = _deck.IsPublic;
                _deck.Name = DeckName.Trim();
                _deck.IsPublic = IsPublic;


                List<Tip> tips = new List<Tip>();
                foreach (TipViewModel tip in TipsCollection)
                {
                    tips.Add(new Tip(tip.ID, tip.Essence.Trim()));
                }

                try
                {
                    IsDataLoading = true;

                    switch (_mode)
                    {
                        case Mode.AddNewFlashcardToDeck:

                            if (oldDeckName != DeckName || oldIsPublic != IsPublic)
                            {
                                await _restService.UpdateDeck(_deck);
                            }

                            Flashcard createdFlashcard = await _restService.CreateFlashcard(_flashcard, _deck.ID);

                            foreach (var tip in tips)
                            {
                                await _restService.CreateTip(tip, _deck.ID, createdFlashcard.Id);
                            }

                            break;

                        case Mode.CreateFlashcardAndDeck:

                            Deck createdDeck = await _restService.CreateDeck(_deck);
                            Flashcard addedFlashcard = await _restService.CreateFlashcard(_flashcard, createdDeck.ID);

                            foreach (var tip in tips)
                            {
                                await _restService.CreateTip(tip, createdDeck.ID, addedFlashcard.Id);
                            }

                            break;

                        case Mode.EditFlashcard:

                            if (oldDeckName != DeckName || oldIsPublic != IsPublic)
                            {
                                await _restService.UpdateDeck(_deck);
                            }

                            string flashcardId = _flashcard.Id;
                            string deckId = _flashcard.DeckID;

                            bool result = await _restService.UpdateFlashcard(_flashcard, deckId);

                            var oldTips = await _restService.GetTips(deckId, flashcardId);

                            if (oldTips != null)
                            {
                                foreach (var oldTip in oldTips)
                                {
                                    await _restService.RemoveTip(deckId, flashcardId, oldTip.ID);
                                }
                            }

                            foreach (var tip in tips)
                            {
                                await _restService.CreateTip(tip, deckId, flashcardId);
                            }

                            break;
                    }
                }
                catch
                {
                    Messenger.Default.Send<MessageToMessageBoxControl>(new MessageToMessageBoxControl(true, false, StringResources.GetString("OperationFailed")));
                }
                finally
                {
                    IsDataLoading = false;
                }

                LeaveForm();
            }
        }

        private bool ValidateForm()
        {
            if (!IsQuestionValid || !IsAnswerValid || !IsDeckNameValid)
            {
                IsGeneralError = true;
                return false;
            }

            foreach (var tip in TipsCollection)
            {
                if (!tip.IsEssenceValid)
                {
                    IsGeneralError = true;
                    return false;
                }
            }

            IsGeneralError = false;
            return true;
        }

        private void LeaveForm()
        {
            switch (_mode)
            {
                case Mode.CreateFlashcardAndDeck:
                    NavigationService.NavigateTo("DecksListView");
                    Messenger.Default.Send<ReloadMessageToDecksList>(new ReloadMessageToDecksList(true));
                    Messenger.Default.Send<MessageToMenuControl>(new MessageToMenuControl(false, false));
                    break;

                default:
                    NavigationService.NavigateTo("ManageDeckView");
                    Messenger.Default.Send<MessageToMenuControl>(new MessageToMenuControl(false, false));
                    Messenger.Default.Send<DataMessageToMenageDeck>(new DataMessageToMenageDeck(_deck));
                    break;

            }
            FlashcardsCollection.Clear();
            TipsCollection.Clear();
            IsRemoveButtonVisible = false;
            IsPaneVisible = false;
        }

        public void ChangePaneVisibility()
        {
            IsPaneVisible = !IsPaneVisible;
        }

        private void HandleTipsCountChanged()
        {
            int tipsCount = TipsCollection.Count;
            if (tipsCount >= _maxTipsCount)
            {
                IsAddingTipsEnabled = false;
            }
            else if (tipsCount == 0)
            {
                IsAddingTipsEnabled = true;
                AddTipMessage = StringResources.GetString("AddTip");
            }
            else
            {
                IsAddingTipsEnabled = true;
                AddTipMessage = StringResources.GetString("AddNextTip");
            }
        }

        private async void HandleDataMessage(Deck deckInstance, Flashcard flashcardInstance)
        {
            FlashcardsCollection.Clear();
            TipsCollection.Clear();
            IsDataLoading = false;
            if (deckInstance != null)
            {
                if (flashcardInstance != null)
                {
                    _mode = Mode.EditFlashcard;
                    Question = flashcardInstance.Question;
                    Answer = flashcardInstance.Answer;
                    IsPublic = deckInstance.IsPublic;
                    IsRemoveButtonVisible = true;

                    HeaderMessage = StringResources.GetString("EditFlashcard");
                    SubmitFormMessage = StringResources.GetString("Edit"); ;

                    try
                    {
                        var tips = await _restService.GetTips(deckInstance.ID, flashcardInstance.Id);
                        if (tips != null)
                        {
                            foreach (var tip in tips)
                            {
                                TipsCollection.Add(new TipViewModel(tip.ID, tip.Essence));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Messenger.Default.Send<MessageToMessageBoxControl>(new MessageToMessageBoxControl(true, false, StringResources.GetString("OperationFailed")));
                    }
                    
                    _flashcard = flashcardInstance;
                }
                else
                {
                    _mode = Mode.AddNewFlashcardToDeck;
                    Question = "";
                    Answer = "";
                    IsRemoveButtonVisible = false;

                    _flashcard = new Flashcard();
                    HeaderMessage = StringResources.GetString("AddFlashcard");
                    SubmitFormMessage = StringResources.GetString("AddFlashcard");
                    TipsCollection = new ObservableCollection<TipViewModel>();
                }

                try
                {
                    var flashcards = await _restService.GetFlashcards(deckInstance.ID);
                    if (flashcards != null && flashcards.Count > 0)
                    {
                        foreach (var f in flashcards)
                        {
                            FlashcardsCollection.Add(f);
                        }
                    }
                }
                catch
                {
                    FlashcardsCollection = new ObservableCollection<Flashcard>();
                }
                HandleTipsCountChanged();
                DeckName = deckInstance.Name;
                IsPublic = deckInstance.IsPublic;
                _deck = deckInstance;
            }
            else
            {
                _mode = Mode.CreateFlashcardAndDeck;
                Question = "";
                Answer = "";
                DeckName = "";
                IsRemoveButtonVisible = false;
                _deck = new Deck();
                _flashcard = new Flashcard();

                HeaderMessage = StringResources.GetString("CreateNewDeck");
                SubmitFormMessage = StringResources.GetString("CreateNewDeck");
                FlashcardsCollection = new ObservableCollection<Flashcard>();
                TipsCollection = new ObservableCollection<TipViewModel>();
                HandleTipsCountChanged();
            }

            IsGeneralError = false;
            IsPaneVisible = false;
        }

        private async void HandleConfirmMessage(bool shouldBeRemoved)
        {
            IsDataLoading = true;
            int flashcardsCount = FlashcardsCollection.Count;

            if (shouldBeRemoved)
            {
                if (!ValidateForm())
                {
                    return;
                }

                _flashcard.Answer = Answer.TrimEnd();
                _flashcard.Question = Question.TrimEnd();

                List<Tip> tips = new List<Tip>();
                foreach (TipViewModel tip in TipsCollection)
                {
                    tips.Add(new Tip(tip.ID, tip.Essence.TrimEnd()));
                }

                try
                {
                    IsDataLoading = true;

                    switch (_mode)
                    {
                        case Mode.EditFlashcard:

                            string flashcardId = _flashcard.Id;
                            string deckId = _flashcard.DeckID;

                            await _restService.RemoveFlashcard(deckId, flashcardId);

                            var oldTips = await _restService.GetTips(deckId, flashcardId);

                            if (oldTips != null)
                            {
                                foreach (var oldTip in oldTips)
                                {
                                    await _restService.RemoveTip(deckId, flashcardId, oldTip.ID);
                                }
                                LeaveForm();
                            }

                            if (flashcardsCount == 1)
                            {
                                await _restService.RemoveDeck(deckId);
                                LeaveForm();
                                NavigationService.NavigateTo("DecksListView");
                                Messenger.Default.Send<ReloadMessageToDecksList>(new ReloadMessageToDecksList(true));
                                Messenger.Default.Send<MessageToMenuControl>(new MessageToMenuControl(false, false));
                            }

                            break;
                    }
                }
                catch
                {
                    Messenger.Default.Send<MessageToMessageBoxControl>(new MessageToMessageBoxControl(true, false, false,
                        StringResources.GetString("OperationFailed")));
                }
                finally
                {
                    IsDataLoading = false;
                }
            }
        }
    }
}
