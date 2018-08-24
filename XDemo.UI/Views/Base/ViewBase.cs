﻿using System;
using Xamarin.Forms;
using XDemo.UI.ViewModels.Base;
using System.Threading.Tasks;

namespace XDemo.UI.Views.Base
{
    public class ViewBase : ContentPage
    {
        private double _lastWidth, _lastHeight;

        //this event provide for code behind
        public event EventHandler<ScreenRotatedEventArg> ScreenRotated;

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (!_lastWidth.Equals(width) && !_lastHeight.Equals(height))
            {
                var orientation = width > height ? ScreenOrientation.LandScape : ScreenOrientation.Portrait;
                ScreenRotated?.Invoke(this, new ScreenRotatedEventArg { Orientation = orientation });
                //provide for viewmodel
                var vm = this.BindingContext as ViewModelBase;
                vm?.OnScreenRotatedAsync(orientation);//todo: review
            }

            _lastWidth = width;
            _lastHeight = height;
        }

        // todo: implement other base logics of a view
    }
}
