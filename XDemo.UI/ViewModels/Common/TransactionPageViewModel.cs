using XDemo.UI.ViewModels.Base;
using XDemo.Core.BusinessServices.Interfaces.Photos;
using XDemo.Core.Infrastructure.Networking.Base;
using XDemo.UI.Models.Photos;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System;
using Xamarin.Forms;
using Prism.Navigation;
using XDemo.UI.Extensions;

namespace XDemo.UI.ViewModels.Common
{
    public class TransactionPageViewModel : ViewModelBase
    {
        private readonly IPhotoService _photoService;
        private readonly INavigationService _navigationService;

        public TransactionPageViewModel(IPhotoService photoService, INavigationService navigationService)
        {
            this.Title = "Transactions";
            _photoService = photoService;
            _navigationService = navigationService;
        }

        private bool _initialized = false;

        public List<Photo> Photos { get; private set; }

        public Photo SelectedPhoto { get; set; }

        /// <summary>
        /// Page is appearing, override this thi handle your logics in viewmodel. <para/>
        /// restrict to handle viewmodel's logic from code behind
        /// </summary>
        public override async void OnAppearing()
        {
            base.OnAppearing();
            if (_initialized)
                return;
            var photos = await _photoService.Get();
            Photos = photos.Select(arg => new Photo
            {
                AlbumId = arg.AlbumId,
                Id = arg.Id,
                Title = arg.Title,
                ThumbnailUrl = arg.ThumbnailUrl,
                Url = arg.Url
            }).ToList();
            _initialized = true;
        }

        #region SelectPhotoCommand

        private ICommand _SelectPhotoCommand;

        public ICommand SelectPhotoCommand => _SelectPhotoCommand ?? (_SelectPhotoCommand = new Command<Photo>(async arg => await SelectPhotoCommandExecute(arg)));

        private async Task SelectPhotoCommandExecute(Photo photo)
        {
            if (photo == null)
                return;
            var paras = new NavigationParameters
            {
                { "PhotoId", photo.Id }
            };
            await _navigationService.PushAsync<PhotoDetailPageViewModel>(paras);
        }

        #endregion
    }
}
