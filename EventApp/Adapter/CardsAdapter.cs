using System;
using System.Collections.Generic;
using Android.Content;
using Android.Views;
using Android.Widget;
using EventApp.Helpers;
using EventApp.Model;

namespace EventApp.Adapter
{
    public class CardsAdapter : BaseAdapter<CardModel>
    {
        private Context mContext;
        public List<CardModel> mItems;

        private TextView type;
        private TextView date;
        private TextView client;
        private TextView personsCount;
        private TextView location;

        public override CardModel this[int position] => mItems[position];

        public override int Count => mItems.Count;

        public CardsAdapter(Context context, List<CardModel> items)
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
                row = LayoutInflater.From(mContext).Inflate(Resource.Layout.MainMenuCard, null, false);

            type = row.FindViewById<TextView>(Resource.Id.event_type);
            date = row.FindViewById<TextView>(Resource.Id.event_date);
            client = row.FindViewById<TextView>(Resource.Id.event_client);
            personsCount = row.FindViewById<TextView>(Resource.Id.event_persons);
            location = row.FindViewById<TextView>(Resource.Id.event_location);

            type.SetTypeface(Constants.Instance.AEH, Android.Graphics.TypefaceStyle.Bold);
            date.SetTypeface(Constants.Instance.AEH, Android.Graphics.TypefaceStyle.Bold);

            type.Text = mItems[position].EventType;

            return row;
        }
    }
}
