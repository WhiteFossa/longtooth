using longtooth.Mobile.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace longtooth.Mobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddMountpointPageView : ContentPage
    {
        public AddMountpointPageViewModel ViewModel
        {
            get => BindingContext as AddMountpointPageViewModel;
            set => BindingContext = value;
        }

        public AddMountpointPageView()
        {
            InitializeComponent();

            ViewModel.SetNavigationProperty(Navigation);
        }
    }
}