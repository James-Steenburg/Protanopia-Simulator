using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using Wpf.Ui.Controls;
using Wpf.Ui;

namespace ProtanopiaSimulator.ViewModels
{
    public partial class ProtanopiaViewModel : ObservableObject
    {
        private bool _isInitialized = false;

        [ObservableProperty]
        private string _applicationTitle = String.Empty;

        [ObservableProperty]
        private ObservableCollection<object> _navigationItems = new();

        [ObservableProperty]
        private ObservableCollection<object> _navigationFooter = new();

        [ObservableProperty]
        private ObservableCollection<MenuItem> _trayMenuItems = new();

        public ProtanopiaViewModel(INavigationService navigationService)
        {
            if (!_isInitialized)
                InitializeViewModel();
        }

        private void InitializeViewModel()
        {
            ApplicationTitle = "Protanopia Colorblindness Simulator";

            NavigationItems = new ObservableCollection<object>
        {
            new NavigationViewItem()
            {
                Content = "Home",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(Views.Pages.DashboardPage)
            },
            new NavigationViewItem()
            {
                Content = "Image Simulation",
                Icon = new SymbolIcon { Symbol = SymbolRegular.ImageMultiple24 },
                TargetPageType = typeof(Views.Pages.ImagesDisplayPage)
            }
        };

            NavigationFooter = new ObservableCollection<object>
        {
            new NavigationViewItem()
            {
                Content = "Settings",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(Views.Pages.SettingsPage)
            }
        };

            TrayMenuItems = new ObservableCollection<MenuItem>
        {
            new MenuItem { Header = "Home", Tag = "tray_home" }
        };

            _isInitialized = true;
        }
    }
}
