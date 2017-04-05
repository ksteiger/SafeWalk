﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Hopper_Rides
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage
    {
        public event EventHandler Selected;

        public SearchPage()
        {
            InitializeComponent();
        }

        protected void OnAppearing(Object sender, EventArgs e)
        {
            base.OnAppearing();
            searchBar.Focus();
        }

        protected void OnLayoutChange(Object sender, EventArgs e)
        {
            searchBar.Focus();
        }

        async void OnTextChanged(Object sender, EventArgs e)
        {
            string googleKey = "AIzaSyA3aaKi6HVMDLcvez0EGcMn6Fsngl5lC5g";
            string dirUrl = "https://maps.googleapis.com/maps/api/place/autocomplete/json?input=";
            dirUrl += searchBar.Text;
            dirUrl += "&location=43.068152,-89.409759&radius=10000&key=" + googleKey;

            //Get new autocomplete suggestions when search text is changed
            var response = await SendRequest(dirUrl);
            if (!response.IsSuccessStatusCode)
            {
                System.Diagnostics.Debug.WriteLine("Bad Address?");
            }
            else
            {
                string content = await response.Content.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(content);
                PlacesResponse pParsed = JsonConvert.DeserializeObject<PlacesResponse>(content);

                //Set up list template to display the descriptions of the autocomplete predictions
                var temp = new DataTemplate(typeof(TextCell));
                temp.SetBinding(TextCell.TextProperty, "description");

                //Set up the list with the autocomplete predictions
                list.ItemsSource = pParsed.predictions;
                list.ItemTemplate = temp;
            }

            searchBar.Focus();
            var stack = new StackLayout { Spacing = 0 };
            stack.Children.Add(searchBar);
            stack.Children.Add(list);
            Content = stack;
        }

        void OnSelection(Object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
            }

            Selected(this, e);

        }

        //Sends HTTP request using given url
        private async Task<HttpResponseMessage> SendRequest(string url)
        {
            HttpClient client = new HttpClient();
            var uri = new Uri(url);

            HttpResponseMessage response = await client.GetAsync(uri);
            return response;
        }
    }
}
