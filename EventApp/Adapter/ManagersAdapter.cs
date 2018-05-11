using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using EventApp.Helpers;
using EventApp.Model;

namespace EventApp.Adapter
{
    public class ManagersAdapter : BaseAdapter<ManagerModel>
    {
        private TextView name;
        private TextView phone;

        private Context mContext;
        public List<ManagerModel> mItems;

        public override ManagerModel this[int position] => mItems[position];

        public override int Count => mItems.Count;

        public ManagersAdapter(Context context, List<ManagerModel> items)
        {
            mItems = items;
            mContext = context;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View row = convertView;

            if (row == null)
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.ManagerCard, null, false);

            name = row.FindViewById<TextView>(Resource.Id.card_mname);
            phone = row.FindViewById<TextView>(Resource.Id.card_mphone);

            name.SetTypeface(Constants.Instance.AEH, Android.Graphics.TypefaceStyle.Normal);
            phone.SetTypeface(Constants.Instance.AEH, Android.Graphics.TypefaceStyle.Normal);

            name.Text = mItems[position].Name;
            phone.Text = $"#{mItems[position].Phone}";

            return row;
        }
    }
}
