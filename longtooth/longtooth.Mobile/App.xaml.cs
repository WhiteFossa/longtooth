using longtooth.Views;
using Nancy.TinyIoc;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace longtooth
{
    public partial class App : Application
    {
        public static TinyIoCContainer Container;

        public App()
        {
            InitializeComponent();

            MainPage = new MainPageView();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
