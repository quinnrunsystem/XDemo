namespace XDemo.UI.Controls.GroupedElements.CarouselScrollViews
{
    public class SelectedItemChangedEventArgs
    {
        public SelectedItemChangedEventArgs(int selectedIndex, object selectedObject)
        {
            SelectedIndex = selectedIndex;
            SelectedObject = selectedObject;
        }

        public int SelectedIndex { get; private set; }
        public object SelectedObject { get; private set; }
    }
}
