using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Stack.Views
{
    public partial class HomePage : ContentPage
    {
        List<string> MonthList;
        List<string> YearList;

        private int yearIndex;
        public int YearIndex
        {
            get { return yearIndex; }

            set { yearIndex = value; OnPropertyChanged(nameof(YearIndex)); }
        }

        private int monthIndex;
        public int MonthIndex
        {
            get { return monthIndex; }

            set { monthIndex = value; OnPropertyChanged(nameof(MonthIndex)); }
        }

        public HomePage()
        {
            InitializeComponent();
            BindingContext = this;

            MonthList = new List<string> { "01", "02", "03", "04", "05", "06", "07", "08", "09", "10", "11", "12" };
            YearList = new List<string> { "2020", "2021", "2022", "2023", "2024", "2025", "2026", "2027"};

            monthPicker.ItemsSource = MonthList;
            YearPicker.ItemsSource = YearList;

        }



        async void Submit(object sender, EventArgs e)
        {
            var sd = new CardInfo
            {
                Card = cardEntry.Text,
                Cvc = cvcEntry.Text,
                Month = MonthList[MonthIndex],
                Year = YearList[YearIndex].Substring(2)
            };

            await Navigation.PushAsync(new PaymentPage(sd));
        }
    }
}
