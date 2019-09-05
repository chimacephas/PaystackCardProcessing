using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace Stack.Views
{
    public partial class PaymentPage : ContentPage
    {
        public PaymentPage(CardInfo on)
        {
            InitializeComponent();


            JObject Ree = new JObject();
            Ree["email"] = "chima.okoli@cyhermes.com";
            Ree["amount"] = 10;
            Ree["key"] = "";
         
            JObject Info = new JObject();
            Info["number"] = on.Card;
            Info["cvv"] = on.Cvc;
            Info["month"] = on.Month;
            Info["year"] = on.Year;


            JObject product = new JObject();

            product["Data"] = Ree;
            product["Info"] = Info;

            hybridWebView.Data = product.ToString();

            hybridWebView.RegisterAction(async (sd) => { await Finished(sd); });
        }


        private async Task Finished(object obj)
        {

        }
    }
}

