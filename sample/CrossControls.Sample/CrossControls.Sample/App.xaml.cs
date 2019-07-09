using CrossControls.Sample.Pages;
using Xamarin.Forms; 

namespace CrossControls.Sample
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent(); 
            MainPage = new NavigationPage(new MasterDetailPage
            {
                Master = new MenuPage()
            });
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
