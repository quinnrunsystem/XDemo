using XDemo.UI.ViewModels.Base;
using XDemo.Core.BusinessServices.Interfaces.Photos;
using XDemo.UI.Models.Photos;
using System.Collections.Generic;
using System.Windows.Input;
using System.Threading.Tasks;
using Xamarin.Forms;
using Prism.Navigation;
using XDemo.Core.Extensions;
using XDemo.Core.BusinessServices.Dtos.Photos;
using System.Threading;
using System.Linq;

namespace XDemo.UI.ViewModels.Common
{
    public class TransactionPageViewModel : ViewModelBase
    {
        private readonly IPhotoService _photoService;
        private CancellationTokenSource _tcs;

        public TransactionPageViewModel(IPhotoService photoService, INavigationService navigationService) : base(navigationService)
        {
            this.Title = "Transactions";
            _photoService = photoService;
        }

        public List<Photo> Photos { get; private set; }

        public Photo SelectedPhoto { get; set; }

        /// <summary>
        /// Page is appearing, override this thi handle your logics in viewmodel. <para/>
        /// restrict to handle viewmodel's logic from code behind
        /// </summary>
        public override async void OnAppearing()
        {
            base.OnAppearing();
            /* ==================================================================================================
             * renew the token source, then pass the token through the api gateway
             * ================================================================================================*/
            _tcs = new CancellationTokenSource();
            await _photoService.Get(OnSuccess, OnFailed, _tcs.Token);
        }

        public override void OnDisappearing()
        {
            base.OnDisappearing();
            /* ==================================================================================================
             * in case of user navigate to other places => cancel current api call operator and do nothing for UI
             * ================================================================================================*/
            _tcs?.Cancel();
        }

        void OnFailed()
        {
            Photos = new List<Photo>();
        }

        private void OnSuccess(List<PhotoDto> photoDtos)
        {
            /* ==================================================================================================
             * Manual data type casting
             * ==================================================================================================
             * Photos = photoDtos.Select(arg => new Photo
             * {
             *  AlbumId = arg.AlbumId,
             *  Id = arg.Id,
             *  Title = arg.Title,
             *  ThumbnailUrl = arg.ThumbnailUrl,
             *  Url = arg.Url
             * }).ToList();
             * ================================================================================================*/

            /* ==================================================================================================
             * Auto mapper usage
             * ================================================================================================*/
            Photos = photoDtos.MapTo<Photo>();
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
                { "PhotoId", photo.Id },
                { "Photos", Photos }
            };
            await PushAsync<PhotoDetailPageViewModel>(paras);
        }

        #endregion
    }
}
