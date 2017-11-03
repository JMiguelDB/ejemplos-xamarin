using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace ihealth.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginSuccessPopupPage : PopupPage
    {
        public LoginSuccessPopupPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            HidePopup();
        }

        private async void HidePopup()
        {
            await Task.Delay(4000);
            await PopupNavigation.RemovePageAsync(this);
        }
    }
}