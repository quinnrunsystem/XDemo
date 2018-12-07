using System;
using Prism.Navigation;
using Xamarin.Forms;
using XDemo.UI.ViewModels.Base;

namespace XDemo.UI.ViewModels
{
    public class GaugePageViewModel : ViewModelBase
    {
        private int _currentValue;
        private int _maxValue = 5000;


        public GaugePageViewModel(INavigationService navigationService) : base(navigationService)
        {
            int delta = 50;
            Device.StartTimer(TimeSpan.FromMilliseconds(5), () =>
            {
                CurrentValue += delta;
                var temp = CurrentValue + delta;
                if (temp > MaxValue)
                {
                    CurrentValue = MaxValue;
                    delta = -delta;
                }
                else if (CurrentValue < 0)
                {
                    CurrentValue = 0;
                    delta = -delta;
                }
                else CurrentValue = temp;
                return true;
            });
        }
        public int CurrentValue
        {
            get => _currentValue; set
            {
                _currentValue = value;
                OnPropertyChanged();
            }
        }
        public int MaxValue
        {
            get => _maxValue; set
            {
                _maxValue = value;
                OnPropertyChanged();
            }
        }
    }
}
