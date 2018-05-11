
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Database;
using Android.Database.Sqlite;
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
using EventApp.Resources.DataHelper;

namespace EventApp.Classes
{
    [Activity(Label = "MainMenuActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainMenuActivity : Activity
    {
        private InputMethodManager inputManager;
        private CardsAdapter cardsAdapter;
        private SQLiteDatabase sqliteDatabase;
        private DataBase dataBase;
        private ICursor selectData;
        private List<CardModel> cardItems;
        private List<ManagerModel> managersItems;
        private List<AccountModel> accountsItems;
        private string userLogin;
        private bool isAdmin;
        private bool isSorted;

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
            userLogin = Intent.GetStringExtra("login");
            isAdmin = Intent.GetBooleanExtra("loginStatus", false);

            list = FindViewById<ListView>(Resource.Id.menu_list);
            sortButton = FindViewById<ImageView>(Resource.Id.menu_sort_button);
            searchButton = FindViewById<ImageView>(Resource.Id.menu_search_button);
            moreButton = FindViewById<ImageView>(Resource.Id.menu_more_button);
            searchField = FindViewById<EditText>(Resource.Id.menu_search_field);

            UpdateCards();

            moreButton.Click += OnMore;
            sortButton.Click += OnSort;
            list.ItemClick += OnCardOpen;

            searchField.TextChanged += OnSearchUpdate;
                       
            searchField.SetTypeface(Constants.Instance.AEH, TypefaceStyle.Normal);

            HideKeyboard();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            moreButton.Click -= OnMore;
            sortButton.Click -= OnSort;
            list.ItemClick -= OnCardOpen;

            searchField.TextChanged -= OnSearchUpdate;
        }

        private void OnSort(object sender, EventArgs e)
        {
            if (!isSorted)
                CardsLoading("SELECT * FROM CardModel ORDER BY EventType, Persons", true);
            else
                CardsLoading("SELECT * FROM CardModel");

            cardsAdapter = new CardsAdapter(this, cardItems);
            list.Adapter = cardsAdapter;
        }

        private void OnSearchUpdate(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(searchField.Text))
            {
                CardsLoading($"SELECT * FROM CardModel WHERE EventType LIKE '%{searchField.Text}%'");
            }
            else
            {
                LoadCards();
            }

            cardsAdapter = new CardsAdapter(this, cardItems);
            list.Adapter = cardsAdapter;
        }

        private void LoadCards()
        {
            CardsLoading("SELECT * FROM CardModel");
        }

        private void CardsLoading(string request, bool sort = false)
        {
            isSorted = sort;
            cardItems = new List<CardModel>();
            dataBase = new DataBase(this);
            sqliteDatabase = dataBase.WritableDatabase;

            selectData = sqliteDatabase.RawQuery(request, new string[] { });
            if (selectData.Count > 0)
            {
                selectData.MoveToFirst();

                do
                {
                    var model = new CardModel();
                    model.CardId = selectData.GetInt(selectData.GetColumnIndex("CardId"));
                    model.EventType = selectData.GetString(selectData.GetColumnIndex("EventType"));
                    model.EventDate = selectData.GetString(selectData.GetColumnIndex("EventDate"));
                    model.ClientName = selectData.GetString(selectData.GetColumnIndex("ClientName"));
                    model.Persons = selectData.GetInt(selectData.GetColumnIndex("Persons"));
                    model.Location = selectData.GetString(selectData.GetColumnIndex("Location"));

                    cardItems.Add(model);
                }
                while (selectData.MoveToNext());

                selectData.Close();
            }
        }

        private void OnMore(object sender, EventArgs e)
        {
            HideKeyboard();

            var panel = CreateView(Resource.Layout.AdminPanel);

            var login = panel.FindViewById<TextView>(Resource.Id.alert_title);
            var status = panel.FindViewById<TextView>(Resource.Id.alert_subtitle);
            var adminPanel = panel.FindViewById<LinearLayout>(Resource.Id.admin_panel);
            var accounts = panel.FindViewById<ListView>(Resource.Id.alert_accounts);
            var btnManagers = panel.FindViewById<Button>(Resource.Id.btn_managers);
            var btnLogout = panel.FindViewById<Button>(Resource.Id.btn_logout);

            status.Text = userLogin;

            btnLogout.Click += (s, p) => { OnBackPressed(); };
            btnManagers.Click += (s, p) =>
            {
                var intent = new Intent(this, typeof(ManagersActivity));
                StartActivity(intent);
            };

            if (!isAdmin)
                adminPanel.Visibility = ViewStates.Gone;
            else
            {
                OnAccountsLoad();
                var data = new List<string>();
                foreach (var item in accountsItems)
                {
                    data.Add($"{item.Login} - {item.Password}");
                }

                var adapter = new ArrayAdapter(this, Resource.Drawable.spinner_item, data);
                adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                accounts.Adapter = adapter;

                btnManagers.Click += OnManagers;
            }
        }

        private void OnAccountsLoad()
        {
            accountsItems = new List<AccountModel>();
            dataBase = new DataBase(this);
            sqliteDatabase = dataBase.WritableDatabase;

            selectData = sqliteDatabase.RawQuery("SELECT * FROM Accounts", new string[] { });
            if (selectData.Count > 0)
            {
                selectData.MoveToFirst();

                do
                {
                    var model = new AccountModel();
                    model.Id = selectData.GetInt(selectData.GetColumnIndex("AccountId"));
                    model.Login = selectData.GetString(selectData.GetColumnIndex("AccountLogin"));
                    model.Password = selectData.GetString(selectData.GetColumnIndex("AccountPass"));

                    accountsItems.Add(model);
                }
                while (selectData.MoveToNext());

                selectData.Close();
            }
        }

        private void OnManagers(object sender, EventArgs e)
        {

        }

        private void OnCardOpen(object sender, AdapterView.ItemClickEventArgs e)
        {
            HideKeyboard();
            searchField.Text = string.Empty;

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
                builder.Dismiss();
                HideKeyboard();
            };

            btnSend.Click += (s, p) =>
            {
                builder.Dismiss();
                OnSend(cardItems[e.Position].CardId);
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

            eventType.Text = cardItems[e.Position].EventType;
            eventDate.Text = cardItems[e.Position].EventDate;
            eventClient.Text = cardItems[e.Position].ClientName;
            eventPersons.Text = $"{cardItems[e.Position].Persons} persons";
            eventLocation.Text = cardItems[e.Position].Location;

            LoadManagers();

            var data = new List<string>();
            foreach (var item in managersItems)
            {
                data.Add(item.Name);
            }

            if (data.Count == 0)
                data.Add("No available managers");

            var adapter = new ArrayAdapter(this, Resource.Drawable.spinner_item, data);
            adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            spinner.Adapter = adapter;
        }

        private void LoadManagers()
        {
            managersItems = new List<ManagerModel>();

            selectData = sqliteDatabase.RawQuery("SELECT * FROM Managers", new string[] { });
            if (selectData.Count > 0)
            {
                selectData.MoveToFirst();

                do
                {
                    var model = new ManagerModel();
                    model.ManagerId = selectData.GetInt(selectData.GetColumnIndex("ManagerId"));
                    model.Name = selectData.GetString(selectData.GetColumnIndex("ManagerName"));
                    model.Phone = selectData.GetString(selectData.GetColumnIndex("ManagerPhone"));
                    model.Status = selectData.GetInt(selectData.GetColumnIndex("Status")) == 0 ? ManagersStatus.Free : ManagersStatus.Busy;

                    if (model.Status is ManagersStatus.Free)
                        managersItems.Add(model);
                }
                while (selectData.MoveToNext());

                selectData.Close();
            }
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

        private void OnSend(int id)
        {
            HideKeyboard();

            dataBase = new DataBase(this);
            sqliteDatabase = dataBase.WritableDatabase;
            sqliteDatabase.ExecSQL($"DELETE FROM CardModel WHERE CardId = '{id}';");

            UpdateCards();
        }

        private void UpdateCards()
        {
            LoadCards();

            cardsAdapter = new CardsAdapter(this, cardItems);
            list.Adapter = cardsAdapter;
        }

        private void HideKeyboard()
        {
            inputManager.HideSoftInputFromWindow(CurrentFocus?.WindowToken, HideSoftInputFlags.NotAlways);
        }
    }
}
