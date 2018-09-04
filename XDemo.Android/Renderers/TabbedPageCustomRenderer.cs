using System.Collections.Generic;
using System.ComponentModel;
using Android.Content;
using Android.Support.Design.Internal;
using Android.Support.Design.Widget;
using Android.Views;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;
using XDemo.Droid.Renderers;
using XDemo.UI.Controls.ExtendedElements;

[assembly: ExportRenderer(typeof(TabbedPageCustom), typeof(TabbedPageCustomRenderer))]
namespace XDemo.Droid.Renderers
{
    public class TabbedPageCustomRenderer : TabbedPageRenderer, BottomNavigationView.IOnNavigationItemSelectedListener
    {
        BottomNavigationMenuView _bottomNavigationMenuView;
        //readonly int[][] states = new int[][]
        //{
        //    new int[] { global::Android.Resource.Attribute.StateSelected }, // 
        //    new int[] { global::Android.Resource.Attribute.StateEnabled },// enabled
        //    new int[] { global::Android.Resource.Attribute.StateCheckable },// 
        //    new int[] { global::Android.Resource.Attribute.StateActivated }  // 
        //};
        //readonly int[] colors = new int[]
        //{
        //    Color.Yellow.ToAndroid().ToArgb(),
        //    Color.Red.ToAndroid().ToArgb(),
        //    Color.Green.ToAndroid().ToArgb(),
        //    Color.Blue.ToAndroid().ToArgb()
        //};

        public TabbedPageCustomRenderer(Context context) : base(context)
        {
        }

        Dictionary<int, int> checkOneRun = new Dictionary<int, int>();
        public new bool OnNavigationItemSelected(IMenuItem item)
        {
            if (Element == null)
                return false;

            if (checkOneRun.ContainsKey(_bottomNavigationMenuView.SelectedItemId) && checkOneRun[_bottomNavigationMenuView.SelectedItemId] == item.ItemId)
            {
                checkOneRun.Clear();
                return false;
            }

            if (_bottomNavigationMenuView.SelectedItemId != item.ItemId && Element.Children.Count > item.ItemId && item.ItemId >= 0)
            {
                checkOneRun.TryAdd(_bottomNavigationMenuView.SelectedItemId, item.ItemId);
                // Change icon
                Element.Children[item.ItemId].Icon = Element.Children[item.ItemId].Icon.File.Replace("_2", "");
                Element.Children[_bottomNavigationMenuView.SelectedItemId].Icon
                       = Element.Children[_bottomNavigationMenuView.SelectedItemId].Icon.File + "_2";

                // Change CurrentPage
                Element.CurrentPage = Element.Children[item.ItemId];
            }
            return true;
        }



        protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
        {
            base.OnElementChanged(e);

            if (ViewGroup != null && ViewGroup.ChildCount > 0)
            {
                _bottomNavigationMenuView = FindChildOfType<BottomNavigationMenuView>(ViewGroup);
                if (_bottomNavigationMenuView != null)
                {
                    var shiftMode = _bottomNavigationMenuView.Class.GetDeclaredField("mShiftingMode");

                    shiftMode.Accessible = true;
                    shiftMode.SetBoolean(_bottomNavigationMenuView, false);
                    shiftMode.Accessible = false;
                    shiftMode.Dispose();

                    for (var i = 0; i < _bottomNavigationMenuView.ChildCount; i++)
                    {
                        if (!(_bottomNavigationMenuView.GetChildAt(i) is BottomNavigationItemView item)) continue;
                        item.SetIconTintList(null); // use Original icon color
                        item.SetShiftingMode(false);
                        item.SetChecked(item.ItemData.IsChecked);
                        // Set text color
                        //item.SetTextColor(new ColorStateList(states,colors));
                    }
                    Element.Children[_bottomNavigationMenuView.SelectedItemId].Icon
                           = Element.Children[_bottomNavigationMenuView.SelectedItemId].Icon.File.Replace("_2", "");

                    if (_bottomNavigationMenuView.ChildCount > 0) _bottomNavigationMenuView.UpdateMenuView();
                }
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            if (e.PropertyName == nameof(TabbedPage.CurrentPage))
            {

            }
        }

        /// <summary>
        /// Finds the type of the child of.
        /// </summary>
        /// <returns>The child of type.</returns>
        /// <param name="viewGroup">View group.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        private T FindChildOfType<T>(ViewGroup viewGroup) where T : Android.Views.View
        {
            if (viewGroup == null || viewGroup.ChildCount == 0) return null;

            for (var i = 0; i < viewGroup.ChildCount; i++)
            {
                var child = viewGroup.GetChildAt(i);

                if (child is T typedChild) return typedChild;

                if (!(child is ViewGroup)) continue;

                var result = FindChildOfType<T>(child as ViewGroup);

                if (result != null) return result;
            }

            return null;
        }
    }
}
