using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace ihealth.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ListViewPage : PopupPage
    {
        public ListViewPage()
        {
            InitializeComponent();
            listView.ItemsSource = new List<string>
            {
                "Test ListView",
                "Test ListView",
                "Test ListView",
                "Test ListView",
                "Test ListView",
                "Test ListView",
                "Test ListView",
                "Test ListView",
                "Test ListView",
                "Test ListView",
                "Test ListView",
                "Test ListView",
                "Test ListView",
                "Test ListView",
                "Test ListView",
                "Test ListView",
                "Test ListView",
                "Test ListView",
                "Test ListView",
                "Test ListView",
                "Test ListView",
                "Test ListView",
                "Test ListView",
                "Test ListView",
                "Test ListView"
            };
        }

        private async void OnClose(object sender, EventArgs e)
        {
            await Navigation.PopPopupAsync();
        }
    }
}