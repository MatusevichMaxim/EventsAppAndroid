using Android.App;
using Android.Widget;
using Android.OS;
using EventApp.Helpers;
using Android.Views;
using Android.Graphics;
using Android.Content;
using EventApp.Classes;

namespace EventApp
{
    [Activity(Label = "EventApp", MainLauncher = true, Icon = "@mipmap/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivity : Activity
    {
        private Button loginButton;
        private TextView loginText;
        private TextView passText;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestWindowFeature(WindowFeatures.NoTitle);
            SetContentView(Resource.Layout.Main);

            Constants.Instance.InitializeConstants(Assets);

            loginText = FindViewById<TextView>(Resource.Id.login_textView);
            passText = FindViewById<TextView>(Resource.Id.password_text);

            loginButton = FindViewById<Button>(Resource.Id.login_button);

            loginButton.SetTypeface(Constants.Instance.AEH, TypefaceStyle.Normal);

            loginText.SetTypeface(Constants.Instance.AEH, TypefaceStyle.Normal);
            passText.SetTypeface(Constants.Instance.AEH, TypefaceStyle.Normal);

            loginText.SetHintTextColor(Color.White);
            passText.SetHintTextColor(Color.White);

            loginButton.Click += OnLogin;
        }

        private void OnLogin(object sender, System.EventArgs e)
        {
            var intent = new Intent(this, typeof(MainMenuActivity));
            StartActivity(intent);
        }
    }
}

