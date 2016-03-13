﻿using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using GalaSoft.MvvmLight;
using StudyBox.Core.Interfaces;
using StudyBox.Core.Services;
using StudyBox.Core.ViewModels;
using StudyBox.View;

namespace StudyBox.ViewModels
{
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator()
        {
            var navigationService = CreateNavigationService();
            SimpleIoc.Default.Register(() => navigationService);

            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainPageViewModel>();
            SimpleIoc.Default.Register<DecksListViewModel>();
            SimpleIoc.Default.Register<ExamViewModel>();
            SimpleIoc.Default.Register<SummaryViewModel>();

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDeserializeJsonService, DeserializeJsonService>();
                SimpleIoc.Default.Register<IRestService, RestService>();
            }
            else
            {
                SimpleIoc.Default.Register<IDeserializeJsonService, DeserializeJsonService>();
                SimpleIoc.Default.Register<IRestService, RestService>();
            }
        }

        private INavigationService CreateNavigationService()
        {
            var navigationService = new NavigationService();
            navigationService.Configure("DecksListView", typeof(DecksListView));
            navigationService.Configure("ExamView", typeof(ExamView));
            navigationService.Configure("MainPage", typeof(MainPage));
            navigationService.Configure("SummaryView", typeof(SummaryView));

            return navigationService;
        }

        public MainPageViewModel MainViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<MainPageViewModel>();
            }
        }

        public DecksListViewModel DecksViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<DecksListViewModel>();
            }
        }

        public ExamViewModel ExamsViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ExamViewModel>();
            }
        }

        public SummaryViewModel SummaryViewModel
        {
            get
            {
                return ServiceLocator.Current.GetInstance<SummaryViewModel>();
            }
        }

        public static void Cleanup()
        {
            // TODO: Clear the ViewModels
        }
    }
}