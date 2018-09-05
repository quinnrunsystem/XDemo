﻿using System;
using Xamarin.Forms;
using XDemo.UI.ViewModels.Base;

namespace XDemo.UI.Views.Base
{
    public class ViewBase : ContentPage
    {
        private double _lastWidth, _lastHeight;

        public ViewBase()
        {
            //binding the base property 'Title'
            this.SetBinding(TitleProperty, nameof(ViewModelBase.Title), BindingMode.TwoWay);
            //global busy
            this.SetBinding(IsBusyProperty, nameof(ViewModelBase.IsBusy), BindingMode.TwoWay);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            // restrict execute many times of built-in Page.OnSizeAllocated()
            if (!_lastWidth.Equals(width) && !_lastHeight.Equals(height))
            {
                var orientation = width > height ? ScreenOrientation.LandScape : ScreenOrientation.Portrait;
                ScreenRotated?.Invoke(this, new ScreenRotatedEventArg { Orientation = orientation });
                //provide for viewmodel, it execute once
                var vm = this.BindingContext as ViewModelBase;
                vm?.OnScreenRotated(orientation);
            }

            _lastWidth = width;
            _lastHeight = height;
        }

        //this event provide for code behind usage => only using this in a case of no more solutions within viewmodel
        public event EventHandler<ScreenRotatedEventArg> ScreenRotated;

        // todo: implement other base logics of a view, anythings you want.
    }
}
