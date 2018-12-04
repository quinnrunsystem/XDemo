using System;
using Xamarin.Forms;
using Foundation;
using UIKit;
using XDemo.iOS.Renderers.ExtendedElements.Pages;
using XDemo.UI.Views.Base;
using Rg.Plugins.Popup.IOS.Renderers;

[assembly: ExportRenderer(typeof(PopupBase), typeof(PopupBaseRenderer))]
namespace XDemo.iOS.Renderers.ExtendedElements.Pages
{
    public class PopupBaseRenderer : PopupPageRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (Element is ViewBase page && !(page.Content is ScrollView scrollView))
                /* ==================================================================================================
                 * in case of root element is scrollview => do nothing more!
                 * ================================================================================================*/
                RegisterForKeyboardNotifications();
        }
        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            UnregisterForKeyboardNotifications();
        }
        #region For Keyboard iOS

        NSObject _keyboardShowObserver;
        NSObject _keyboardHideObserver;
        private bool _pageWasShiftedUp;
        private double _activeViewBottom;
        private bool _isKeyboardShown;

        /// <summary>
        /// Registers for keyboard notifications.
        /// </summary>
        void RegisterForKeyboardNotifications()
        {
            _keyboardShowObserver = _keyboardShowObserver ?? UIKeyboard.Notifications.ObserveWillShow(OnKeyboardWillShow);
            _keyboardHideObserver = _keyboardHideObserver ?? UIKeyboard.Notifications.ObserveWillHide(OnKeyboardWillHide);
        }

        private void OnKeyboardWillHide(object sender, UIKeyboardEventArgs e)
        {
            if (!IsViewLoaded)
                return;

            _isKeyboardShown = false;
            var keyboardFrame = UIKeyboard.FrameEndFromNotification(e.Notification);

            if (_pageWasShiftedUp)
            {
                ShiftPageDown(keyboardFrame.Height, _activeViewBottom);
            }
        }

        private void OnKeyboardWillShow(object sender, UIKeyboardEventArgs e)
        {
            if (!IsViewLoaded || _isKeyboardShown)
                return;

            _isKeyboardShown = true;
            var activeView = View.FindFirstResponder();

            if (activeView == null)
                return;
            var keyboardFrame = UIKeyboard.FrameEndFromNotification(e.Notification);
            var isOverlapping = activeView.IsKeyboardOverlapping(View, keyboardFrame);

            if (!isOverlapping)
                return;

            _activeViewBottom = activeView.GetViewRelativeBottom(View);
            ShiftPageUp(keyboardFrame.Height, _activeViewBottom);
        }

        /// <summary>
        /// Unregisters for keyboard notifications.
        /// </summary>
        void UnregisterForKeyboardNotifications()
        {
            _isKeyboardShown = false;
            if (_keyboardShowObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardShowObserver);
                _keyboardShowObserver.Dispose();
                _keyboardShowObserver = null;
            }

            if (_keyboardHideObserver != null)
            {
                NSNotificationCenter.DefaultCenter.RemoveObserver(_keyboardHideObserver);
                _keyboardHideObserver.Dispose();
                _keyboardHideObserver = null;
            }
        }

        /// <summary>
        /// Shifts the page up.
        /// </summary>
        /// <param name="keyboardHeight">Keyboard height.</param>
        /// <param name="activeViewBottom">Active view bottom.</param>
        private void ShiftPageUp(nfloat keyboardHeight, double activeViewBottom)
        {
            var pageFrame = Element.Bounds;

            var newY = pageFrame.Y + CalculateShiftByAmount(pageFrame.Height, keyboardHeight, activeViewBottom);

            Element.LayoutTo(new Rectangle(pageFrame.X, newY,
                pageFrame.Width, pageFrame.Height));

            _pageWasShiftedUp = true;
        }

        /// <summary>
        /// Shifts the page down.
        /// </summary>
        /// <param name="keyboardHeight">Keyboard height.</param>
        /// <param name="activeViewBottom">Active view bottom.</param>
        private void ShiftPageDown(nfloat keyboardHeight, double activeViewBottom)
        {
            var pageFrame = Element.Bounds;

            var newY = pageFrame.Y - CalculateShiftByAmount(pageFrame.Height, keyboardHeight, activeViewBottom);

            Element.LayoutTo(new Rectangle(pageFrame.X, newY,
                pageFrame.Width, pageFrame.Height));

            _pageWasShiftedUp = false;
        }

        /// <summary>
        /// Calculates the shift by amount.
        /// </summary>
        /// <returns>The shift by amount.</returns>
        /// <param name="pageHeight">Page height.</param>
        /// <param name="keyboardHeight">Keyboard height.</param>
        /// <param name="activeViewBottom">Active view bottom.</param>
        private double CalculateShiftByAmount(double pageHeight, nfloat keyboardHeight, double activeViewBottom)
        {
            return (pageHeight - activeViewBottom) - keyboardHeight;
        }
        #endregion
    }
}
