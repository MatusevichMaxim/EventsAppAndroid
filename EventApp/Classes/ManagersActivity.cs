using System.Collections.Generic;
using Android.App;
using Android.Database;
using Android.Database.Sqlite;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using EventApp.Adapter;
using EventApp.Helpers;
using EventApp.Model;
using EventApp.Resources.DataHelper;

namespace EventApp.Classes
{
    [Activity(Label = "ManagersActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ManagersActivity : Activity
    {
        private InputMethodManager inputManager;
        private SQLiteDatabase sqliteDatabase;
        private DataBase dataBase;
        private ICursor selectData;
        private ManagersAdapter cardsAdapter;
        private bool dataLoaded;

        private List<ManagerModel> cardItems;
        private ListView managersList;
        private Button newManagerButton;
        private EditText searchField;
        private EditText newName;
        private EditText newPhone;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.ManagersMenu);
            inputManager = (InputMethodManager)GetSystemService(InputMethodService);

            managersList = FindViewById<ListView>(Resource.Id.managers_list);
            newManagerButton = FindViewById<Button>(Resource.Id.new_manager_btn);
            searchField = FindViewById<EditText>(Resource.Id.managers_search_field);
            newName = FindViewById<EditText>(Resource.Id.new_manager_name);
            newPhone = FindViewById<EditText>(Resource.Id.new_manager_phone);

            UpdateCards();

            searchField.SetTypeface(Constants.Instance.AEH, TypefaceStyle.Normal);
            newName.SetTypeface(Constants.Instance.AEH, TypefaceStyle.Normal);
            newPhone.SetTypeface(Constants.Instance.AEH, TypefaceStyle.Normal);
            newManagerButton.SetTypeface(Constants.Instance.AEH, TypefaceStyle.Normal);

            newManagerButton.Click += OnManagerAdded;
            searchField.TextChanged += OnSearchUpdate;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            newManagerButton.Click -= OnManagerAdded;
            searchField.TextChanged -= OnSearchUpdate;
        }

        private void OnSearchUpdate(object sender, Android.Text.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(searchField.Text))
            {
                CardsLoading($"SELECT * FROM Managers WHERE ManagerName LIKE '%{searchField.Text}%'");
            }
            else
            {
                LoadCards();
            }

            cardsAdapter = new ManagersAdapter(this, cardItems);
            managersList.Adapter = cardsAdapter;
        }

        private void UpdateCards()
        {
            LoadCards();

            cardsAdapter = new ManagersAdapter(this, cardItems);
            managersList.Adapter = cardsAdapter;
        }

        private void LoadCards()
        {
            CardsLoading("SELECT * FROM Managers");
        }

        private void CardsLoading(string request, bool breakRequest = false)
        {
            if (!dataLoaded)
            {
                dataBase = new DataBase(this);
                sqliteDatabase = dataBase.WritableDatabase;
            }

            cardItems = new List<ManagerModel>();

            selectData = sqliteDatabase.RawQuery(request, new string[] { });
            if (selectData.Count > 0)
            {
                selectData.MoveToFirst();

                if (!breakRequest)
                {
                    do
                    {
                        var model = new ManagerModel();
                        model.ManagerId = selectData.GetInt(selectData.GetColumnIndex("ManagerId"));
                        model.Name = selectData.GetString(selectData.GetColumnIndex("ManagerName"));
                        model.Phone = selectData.GetString(selectData.GetColumnIndex("ManagerPhone"));
                        model.Status = selectData.GetInt(selectData.GetColumnIndex("Status")) == 0 ? ManagersStatus.Free : ManagersStatus.Busy;

                        if (model.Status is ManagersStatus.Free)
                            cardItems.Add(model);
                    }
                    while (selectData.MoveToNext());
                }

                selectData.Close();
            }
        }

        private void OnManagerAdded(object sender, System.EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(newName.Text) || string.IsNullOrWhiteSpace(newPhone.Text))
            {
                Toast.MakeText(this, "Enter all data before adding.", ToastLength.Short).Show();
                return;
            }

            CardsLoading($"INSERT INTO Managers (ManagerName, ManagerPhone) VALUES ('{newName.Text}', '{newPhone.Text}')");
            CardsLoading("SELECT * FROM Managers");

            cardsAdapter = new ManagersAdapter(this, cardItems);
            managersList.Adapter = cardsAdapter;

            newName.Text = newPhone.Text = string.Empty;
        }
    }
}
