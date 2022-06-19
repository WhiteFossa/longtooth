using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace longtooth.Droid.Implementations.FilesPicker
{
    public class SimpleArrayAdaper : ArrayAdapter<string>
    {
        public SimpleArrayAdaper(IntPtr handle, JniHandleOwnership transfer) : base(handle, transfer)
        {
        }

        public SimpleArrayAdaper(Context context, int textViewResourceId) : base(context, textViewResourceId)
        {
        }

        public SimpleArrayAdaper(Context context, int resource, int textViewResourceId) : base(context, resource, textViewResourceId)
        {
        }

        public SimpleArrayAdaper(Context context, int textViewResourceId, string[] objects) : base(context, textViewResourceId, objects)
        {
        }

        public SimpleArrayAdaper(Context context, int resource, int textViewResourceId, string[] objects) : base(context, resource, textViewResourceId, objects)
        {
        }

        public SimpleArrayAdaper(Context context, int textViewResourceId, IList<string> objects) : base(context, textViewResourceId, objects)
        {
        }

        public SimpleArrayAdaper(Context context, int resource, int textViewResourceId, IList<string> objects) : base(context, resource, textViewResourceId, objects)
        {
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View v = base.GetView(position, convertView, parent);
            if (v is TextView)
            {
                // Enable list item (directory) text wrapping
                TextView tv = (TextView)v;
                tv.LayoutParameters.Height = ViewGroup.LayoutParams.WrapContent;
                tv.Ellipsize = null;
            }
            return v;
        }
    }
}