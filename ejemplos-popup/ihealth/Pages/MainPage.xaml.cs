using Rg.Plugins.Popup.Extensions;
using System;
using Xamarin.Forms;
using Rg.Plugins.Popup.Pages;

namespace ihealth.Pages
{

    public partial class MainPage : ContentPage
    {
        private LoginPopupPage _loginPopup;

        public MainPage()
        {
            InitializeComponent();

            _loginPopup = new LoginPopupPage();
        }

        private async void OnOpenPupup(object sender, EventArgs e)
        {
            //var page = new LoginPopupPage();
            await Navigation.PushPopupAsync(_loginPopup);
        }

        private async void OnUserAnimationPupup(object sender, EventArgs e)
        {
            var page = new UserAnimationPage();
            await Navigation.PushPopupAsync(page);
        }

        private async void OnOpenSystemOffsetPage(object sender, EventArgs e)
        {
            var page = new SystemOffsetPage();
            await Navigation.PushPopupAsync(page);
        }

        private async void OnOpenListViewPage(object sender, EventArgs e)
        {
            var page = new ListViewPage();
            await Navigation.PushPopupAsync(page);
        }
    }
}