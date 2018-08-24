using System;

namespace XDemo.UI.Views.Base
{
    public class ScreenRotatedEventArg : EventArgs
    {
        public ScreenOrientation Orientation { get; set; }
    }
}

