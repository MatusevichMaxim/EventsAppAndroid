
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using EventApp.Adapter;
using EventApp.Helpers;
using EventApp.Model;

namespace EventApp.Classes
{
    [Activity(Label = "MainMenuActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainMenuActivity : Activity
    {
        private InputMethodManager inputManager;
        private CardsAdapter cardsAdapter;
        private List<CardModel> mItems;

        private ImageView sortButton;
        private ImageView searchButton;
        private ImageView moreButton;
        private EditText searchField;
        private ListView list;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.MainMenu);
            inputManager = (InputMethodManager)GetSystemService(InputMethodService);

            list = FindViewById<ListView>(Resource.Id.menu_list);
            sortButton = FindViewById<ImageView>(Resource.Id.menu_sort_button);
            searchButton = FindViewById<ImageView>(Resource.Id.menu_search_button);
            moreButton = FindViewById<ImageView>(Resource.Id.menu_more_button);
            searchField = FindViewById<EditText>(Resource.Id.menu_search_field);

            moreButton.Click += OnMore;

            mItems = new List<CardModel>();

            for (int i = 0; i < 4; i++)
            {
                mItems.Add(new CardModel()
                {
                    CardId = 1,
                    EventType = "Wedding",
                    EventDate = DateTime.Today
                });

                mItems.Add(new CardModel()
                {
                    CardId = 2,
                    EventType = "Birthday",
                    EventDate = DateTime.Today
                });

                mItems.Add(new CardModel()
                {
                    CardId = 3,
                    EventType = "Concert",
                    EventDate = DateTime.Today
                });

                mItems.Add(new CardModel()
                {
                    CardId = 4,
                    EventType = "Corporate party",
                    EventDate = DateTime.Today
                });
            }

            cardsAdapter = new CardsAdapter(this, mItems);
            list.Adapter = cardsAdapter;
            list.ItemClick += OnCardOpen;

            searchField.SetTypeface(Constants.Instance.AEH, Android.Graphics.TypefaceStyle.Normal);

            HideKeyboard();
        }

        private void OnMore(object sender, EventArgs e)
        {
            HideKeyboard();

            var panel = CreateView(Resource.Layout.AdminPanel);
        }

        private void OnCardOpen(object sender, AdapterView.ItemClickEventArgs e)
        {
            HideKeyboard();

            var builder = CreateView(Resource.Layout.CustomCardAlert);

            var cardTitle = builder.FindViewById<TextView>(Resource.Id.card_title);
            var eventType = builder.FindViewById<TextView>(Resource.Id.card_etype);
            var eventDate = builder.FindViewById<TextView>(Resource.Id.card_edate);
            var eventClient = builder.FindViewById<TextView>(Resource.Id.card_eclient);
            var eventPersons = builder.FindViewById<TextView>(Resource.Id.card_epersons);
            var eventLocation = builder.FindViewById<TextView>(Resource.Id.card_elocation);
            var spinner = builder.FindViewById<Spinner>(Resource.Id.card_emanager);
            var cost = builder.FindViewById<EditText>(Resource.Id.card_cost);

            var btnCancel = builder.FindViewById<Button>(Resource.Id.btn_cancel);
            var btnSend = builder.FindViewById<Button>(Resource.Id.btn_send);

            btnCancel.Click += (s, p) => 
            {
                HideKeyboard();
                builder.Hide(); 
            };

            btnSend.Click += (s, p) =>
            {
                OnSend();
                builder.Hide();
            };

            cardTitle.SetTypeface(Constants.Instance.AEH, TypefaceStyle.Normal);
            eventType.SetTypeface(Constants.Instance.AEH, TypefaceStyle.Normal);
            eventDate.SetTypeface(Constants.Instance.AEH, TypefaceStyle.Normal);
            eventClient.SetTypeface(Constants.Instance.AEH, TypefaceStyle.Normal);
            eventPersons.SetTypeface(Constants.Instance.AEH, TypefaceStyle.Normal);
            eventLocation.SetTypeface(Constants.Instance.AEH, TypefaceStyle.Normal);
            cost.SetTypeface(Constants.Instance.AEH, TypefaceStyle.Normal);
            btnCancel.SetTypeface(Constants.Instance.AEH, TypefaceStyle.Normal);
            btnSend.SetTypeface(Constants.Instance.AEH, TypefaceStyle.Normal);

            var data = new string[] { "Nicolas Cage", "Ivan Urgant", "Dmitry Nagiev", "Larry King", "Jay Leno" };
            var adapter = new ArrayAdapter(this, Resource.Drawable.spinner_item, data);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;
        }

        private AlertDialog CreateView(int resourceId)
        {
            var cardView = LayoutInflater.Inflate(resourceId, null);
            var builder = new AlertDialog.Builder(this).Create();
            builder.SetView(cardView);
            builder.SetCanceledOnTouchOutside(false);
            builder.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            builder.Window.RequestFeature(WindowFeatures.NoTitle);

            builder.Show();

            return builder;
        }

        private void OnSend()
        {
            HideKeyboard();
        }

        private void HideKeyboard()
        {
            inputManager.HideSoftInputFromWindow(CurrentFocus?.WindowToken, HideSoftInputFlags.NotAlways);
        }
    }
}
