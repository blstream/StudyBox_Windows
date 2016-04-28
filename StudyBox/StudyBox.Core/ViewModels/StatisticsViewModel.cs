﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Views;
using StudyBox.Core.Interfaces;
using StudyBox.Core.Messages;
using StudyBox.Core.Models;

namespace StudyBox.Core.ViewModels
{
    public class StatisticsViewModel : ExtendedViewModelBase
    {
        private int _goodAnswersCount;
        private int _answersCount;
        private int _countOfDecks;
        private int _testsCount;
        private Statistics _statistics;
        private IInternetConnectionService _internetConnetioConnectionService;
        private IRestService _restService;
        private IStatisticsDataService _statisticsService;
        private ObservableCollection<TestsHistory> _testsHistoryCollection;
        private RelayCommand _sortByResultCommand;
        private RelayCommand _sortByDateCommand;
        private RelayCommand _sortByDeckNameCommand;
        private List<TestsHistory> _testsHistoryList;
        private bool _sortResultDescending;

        public StatisticsViewModel(INavigationService navigationService, IInternetConnectionService internetConnectionService, IRestService restService, IStatisticsDataService statisticsSercice) : base(navigationService)
        {
            SortResultDescending = false;
            Messenger.Default.Register<ReloadMessageToStatistics>(this,x=>HandleReloadMessage(x.Reload));
            _internetConnetioConnectionService = internetConnectionService;
            _restService = restService;
            _statisticsService = statisticsSercice;
            GetStatistics();
        }

        public ObservableCollection<TestsHistory> TestsHistoryCollection
        {
            get
            {
                return _testsHistoryCollection;
            }
            set
            {
                if (_testsHistoryCollection != value)
                {
                    _testsHistoryCollection = value;
                    RaisePropertyChanged();
                }
            }
        } 

        public int GoodAnwersCount
        {
            get
            {
                return _goodAnswersCount; 
                
            }
            set
            {
                if (_goodAnswersCount != value)
                {
                    _goodAnswersCount = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int AnswersCount
        {
            get
            {
                return _answersCount;

            }
            set
            {
                if (_answersCount != value)
                {
                    _answersCount = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int CountOfDecks
        {
            get
            {
                return _countOfDecks;
            }
            set
            {
                if (_countOfDecks != value)
                {
                    _countOfDecks = value;
                    RaisePropertyChanged();
                }
            }
        }

        public int TestesCount
        {
            get
            {
                return _testsCount;
            }
            set
            {
                if (_testsCount != value)
                {
                    _testsCount = value;
                    RaisePropertyChanged();
                }
            }
        }


        private void GetStatistics()
        {
            bool isConnection = _internetConnetioConnectionService.CheckConnection();
            if (isConnection)
            {
                _statistics = _statisticsService.GetStatistics();
                GoodAnwersCount = _statistics.GoodAnswersCount;
                AnswersCount = _statistics.AnswersCount;
                CountOfDecks = _statistics.CountOfDecks;
                TestesCount = _statistics.TestsCount;
            }
            //MOCK
            _testsHistoryList = new List<TestsHistory>()
            {
                new TestsHistory("2012-11-24", "Geografia", 12, 34),
                new TestsHistory("2015-02-14", "Fizyka", 5, 10),
                new TestsHistory("2014-01-30", "Wf", 3, 9),
                new TestsHistory("2014-04-11", "Informatyka", 39, 40),
                new TestsHistory("2010-12-23", "asdasd", 28, 30),
                new TestsHistory("2016-09-12", "Geogra123123fia", 11, 15)
            };
            TestsHistoryCollection=new ObservableCollection<TestsHistory>();
            _testsHistoryList.ForEach(x => TestsHistoryCollection.Add(x));
        }

        private void HandleReloadMessage(bool reload)
        {
            if (reload)
                GetStatistics();
        }


        public RelayCommand SortByResultCommand
        {
            get
            {
                return _sortByResultCommand ?? (_sortByResultCommand = new RelayCommand(SortListByResult));
            }
        }

        public RelayCommand SortByDateCommand
        {
            get
            {
                return _sortByDateCommand ?? (_sortByDateCommand = new RelayCommand(SortListByDate));
            }
        }

        public RelayCommand SortByDeckNameCommand
        {
            get
            {
                return _sortByDeckNameCommand ?? (_sortByDeckNameCommand = new RelayCommand(SortListByDeckName));   
            }
        }

        public bool SortResultDescending
        {
            get
            {
                return _sortResultDescending;
            }
            set
            {
                if (_sortResultDescending != value)
                {
                    _sortResultDescending = value;
                    RaisePropertyChanged();
                }
            }
        }

        private void SortListByResult()
        {
            SortResultDescending = !SortResultDescending;
            if (_sortResultDescending)
                _testsHistoryList = _testsHistoryList?.OrderBy(x => x.SortingResult).ToList();        
            else
                _testsHistoryList = _testsHistoryList?.OrderByDescending(x => x.SortingResult).ToList();
            RefreshCollection();
        }

        private void SortListByDate()
        {
            SortResultDescending = !SortResultDescending;
            if (_sortResultDescending)
                _testsHistoryList = _testsHistoryList?.OrderBy(x => x.TestsDate).ToList();
            else
                _testsHistoryList = _testsHistoryList?.OrderByDescending(x => x.TestsDate).ToList();
            RefreshCollection();
        }

        private void SortListByDeckName()
        {
            SortResultDescending = !SortResultDescending;
            if (_sortResultDescending)
                _testsHistoryList = _testsHistoryList?.OrderBy(x => x.DeckName).ToList();
            else
                _testsHistoryList = _testsHistoryList?.OrderByDescending(x => x.DeckName).ToList();
            RefreshCollection();
        }

        private void RefreshCollection(int limitCounter=1)
        {
            if (_testsHistoryList != null && TestsHistoryCollection != null)
            {
                int addsLimit = limitCounter * 5;
                int startIndex = 0;
                TestsHistoryCollection.Clear();
                _testsHistoryList.ForEach(x =>
                {
                    if (startIndex < addsLimit)
                    {
                        TestsHistoryCollection.Add(x);
                        startIndex++;
                    }
                });

            }
        }
    }
}
