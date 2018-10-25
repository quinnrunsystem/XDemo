using Xamarin.Forms;

namespace XDemo.UI.Controls.ExtendedElements
{
    public class ExtendedFrame : Frame
    {
        public new Thickness Padding { get; set; } = 0;
        public int BorderThickness { get; set; }
        public ExtendedFrame()
        {
            base.Padding = this.Padding;
        }
    }
}
