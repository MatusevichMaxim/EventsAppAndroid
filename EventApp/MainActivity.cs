using Android.App;
using Android.Widget;
using Android.OS;
using EventApp.Helpers;
using Android.Views;
using Android.Graphics;
using Android.Content;
using EventApp.Classes;
using Android.Database.Sqlite;
using EventApp.Resources.DataHelper;
using System.Collections.Generic;
using EventApp.Model;
using Android.Database;

namespace EventApp
{
    [Activity(Label = "EventApp", MainLauncher = true, Icon = "@mipmap/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        private SQLiteDatabase sqliteDatabase;
        private DataBase dataBase;
        private ICursor selectData;

        private List<AccountModel> accountsItems;
        private Button loginButton;
        private EditText loginText;
        private EditText passText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.Main);

            OnDataBaseLoad();

            Constants.Instance.InitializeConstants(Assets);

            loginText = FindViewById<EditText>(Resource.Id.login_textView);
            passText = FindViewById<EditText>(Resource.Id.password_text);

            loginButton = FindViewById<Button>(Resource.Id.login_button);

            loginButton.SetTypeface(Constants.Instance.AEH, TypefaceStyle.Normal);

            loginText.SetTypeface(Constants.Instance.AEH, TypefaceStyle.Normal);
            passText.SetTypeface(Constants.Instance.AEH, TypefaceStyle.Normal);

            loginText.SetHintTextColor(Color.White);
            passText.SetHintTextColor(Color.White);

            loginButton.Click += OnLogin;
        }

        protected override void OnRestart()
        {
            base.OnRestart();
            loginText.Text = passText.Text = string.Empty;
        }

        private void OnLogin(object sender, System.EventArgs e)
        {
            if (string.IsNullOrEmpty(loginText.Text))
            {
                Toast.MakeText(this, $"The '{loginText.Hint}' is empty.", ToastLength.Short).Show();
                return;
            }

            if (string.IsNullOrEmpty(passText.Text))
            {
                Toast.MakeText(this, $"The '{passText.Hint}' is empty.", ToastLength.Short).Show();
                return;
            }

            foreach (var item in accountsItems)
            {
                if (item.Login == loginText.Text && item.Password == passText.Text)
                {
                    var status = loginText.Text == "Admin" ? true : false;
                    OnSucces(status);
                    return;
                }
            }

            Toast.MakeText(this, "Incorrect data.", ToastLength.Short).Show();
        }

        private void OnSucces(bool status)
        {
            var intent = new Intent(this, typeof(MainMenuActivity));
            intent.PutExtra("login", loginText.Text);
            intent.PutExtra("loginStatus", status);
            StartActivity(intent);
        }

        private void OnDataBaseLoad()
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
    }
}

