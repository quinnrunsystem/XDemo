using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;
using iOS = Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using android = Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace XDemo.UI.Controls.ExtendedElements
{
    public class LoadMoreListView : ListView
    {
        #region CollectionProperty
        public static BindableProperty CollectionProperty =
               BindableProperty.Create(nameof(Collection), typeof(ICollection), typeof(LoadMoreListView));

        public ICollection Collection
        {
            get => (IList)GetValue(CollectionProperty);
            set => SetValue(CollectionProperty, value);
        }
        #endregion

        #region HasMoreItemsProperty
        public static readonly BindableProperty HasMoreItemsProperty =
            BindableProperty.Create(nameof(HasMoreItems), typeof(bool), typeof(LoadMoreListView), false);

        public bool HasMoreItems
        {
            get => (bool)GetValue(HasMoreItemsProperty);
            set => SetValue(HasMoreItemsProperty, value);
        }
        #endregion

        #region IsLoadingMoreProperty
        public static readonly BindableProperty IsLoadingMoreProperty =
            BindableProperty.Create(nameof(IsLoadingMore), typeof(bool), typeof(LoadMoreListView), false,
                BindingMode.TwoWay);

        public bool IsLoadingMore
        {
            get => (bool)GetValue(IsLoadingMoreProperty);
            set => SetValue(IsLoadingMoreProperty, value);
        }
        #endregion

        #region LoadingCommandProperty
        public static readonly BindableProperty LoadingCommandProperty =
            BindableProperty.Create(nameof(LoadingCommand), typeof(ICommand), typeof(LoadMoreListView));

        public ICommand LoadingCommand
        {
            get => (ICommand)GetValue(LoadingCommandProperty);
            set => SetValue(LoadingCommandProperty, value);
        }
        #endregion

        public LoadMoreListView() : base(ListViewCachingStrategy.RecycleElement)
        {
            PropertyChanged += OnPropertyChanged;
            ItemAppearing += ViewItemAppearing;
            ItemTemplate = new CustomTemplateSelector(GetItemTemplate);
            //platform spec
            android.ListView.SetIsFastScrollEnabled(this, true);
            iOS.ListView.SetSeparatorStyle(this, iOS.SeparatorStyle.FullWidth);
        }

        public DataTemplate ListItemTemplate { get; set; }

        public DataTemplate LoadingItemTemplate { get; set; }

        private DataTemplate GetItemTemplate(object item, BindableObject bindableObject)
        {
            return item is LoadingModel ? LoadingItemTemplate : ListItemTemplate;
        }

        private void ReloadContainerList(NotifyCollectionChangedAction? changedAction = null)
        {
            ItemAppearing -= ViewItemAppearing;
            ItemsSource = GetContainerList(changedAction);
            ItemAppearing += ViewItemAppearing;
        }

        private void ViewItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (!(e.Item is LoadingModel))
                return;
            if (IsRefreshing || !HasMoreItems || IsLoadingMore)
                return;

            IsLoadingMore = true;
            if (Device.RuntimePlatform == Device.iOS)
            {
                ScrollTo(e.Item, ScrollToPosition.End, false);
            }
            LoadingCommand?.Execute(null);
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(HasMoreItems):
                    OnHasMoreItemsChanged();
                    break;
                case nameof(Collection):
                    ReloadContainerList();
                    if (Collection is INotifyCollectionChanged collection)
                    {
                        collection.CollectionChanged -= CollectionChanged;
                        collection.CollectionChanged += CollectionChanged;
                    }
                    break;
                default:
                    break;
            }

            void OnHasMoreItemsChanged()
            {
                if (!HasMoreItems)
                {
                    /* ==================================================================================================
                     * remove the loading model
                     * ================================================================================================*/
                    if (ItemsSource is IList source && source.Count > 0)
                    {
                        var item = source[source.Count - 1];
                        if (item is LoadingModel)
                            source.Remove(item);
                    }
                }
                else
                {
                    if (ItemsSource is IList source && source.Count > 0)
                    {
                        var item = source[source.Count - 1];
                        if (!(item is LoadingModel))
                            source.Add(new LoadingModel());
                    }
                }
            }
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        AdItems();
                        break;
                    case NotifyCollectionChangedAction.Move:
                        break;
                    case NotifyCollectionChangedAction.Remove:
                        RemoveItems();
                        break;
                    case NotifyCollectionChangedAction.Replace:
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        ReloadContainerList(e.Action);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });

            void AdItems()
            {
                var i = 0;
                if (ItemsSource is IList source)
                    foreach (var item in e.NewItems)
                    {
                        var index = e.NewStartingIndex + i++;
                        source.Insert(index, item);
                    }
            }

            void RemoveItems()
            {
                if (ItemsSource is IList source)
                    foreach (var item in e.OldItems)
                    {
                        source.Remove(item);
                    }
            }
        }


        private ObservableCollection<object> GetContainerList(NotifyCollectionChangedAction? changedAction)
        {
            if (Collection == null)
                return new ObservableCollection<object>();

            var tempList = new List<object>();

            if (Collection != null && Collection.Count == 0)
            {
                if (HasMoreItems && !IsRefreshing && changedAction != NotifyCollectionChangedAction.Reset)
                    tempList.Add(new LoadingModel());
            }
            else
            {
                foreach (var item in Collection)
                    tempList.Add(item);

                if (HasMoreItems && !IsRefreshing)
                    tempList.Add(new LoadingModel());
            }

            return new ObservableCollection<object>(tempList);
        }


        private class LoadingModel
        {
        }

        private class CustomTemplateSelector : DataTemplateSelector
        {
            private readonly Func<object, BindableObject, DataTemplate> _getDataTemplate;

            public CustomTemplateSelector(Func<object, BindableObject, DataTemplate> getDataTemplate)
            {
                _getDataTemplate = getDataTemplate;
            }

            protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
            {
                return _getDataTemplate(item, container);
            }
        }
    }
}
