using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossControls.Sample.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CrossControls.Sample.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MenuPage : ContentPage
    {
        private const string GaugePage = "Gauge";

        /// <summary>
        /// All selectable pages.
        /// </summary>
        public List<string> Pages { get; set; } = new List<string>()
        {
            GaugePage
        };

        public MenuPage()
        {
            InitializeComponent();
            BindingContext = this; 
        }

        protected override void OnParentSet()
        {
            base.OnParentSet();
            SetPage(GaugePage);
        }

        private void SetPage(string name)
        {
            var masterDetail = (MasterDetailPage)Parent;
            switch (name)
            {
                case GaugePage:
                    masterDetail.Detail = new GaugePage
                    {
                        BindingContext = new GaugeViewModel()
                    };
                    break;
            }
        }

        private void PageSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                SetPage((string)e.SelectedItem);
                ((ListView) sender).SelectedItem = null;
            }
                
        }
    }
}