using System;
using Prism.Navigation;
using XDemo.UI.ViewModels.Base;
using XDemo.Core.BusinessServices.Interfaces.Photos;
using XDemo.UI.Models.Photos;
using XDemo.Core.Extensions;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xamarin.Forms;
using System.Linq;

namespace XDemo.UI.ViewModels.Common
{
    public class PhotoDetailPageViewModel : ViewModelBase
    {
        private readonly IPhotoService _photoService;
        public PhotoDetailPageViewModel(IPhotoService photoService, INavigationService navigationService) : base(navigationService)
        {
            _photoService = photoService;
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            var id = parameters.GetValue<int>("PhotoId");
            var dto = await _photoService.GetWithSilentRetryUntilSuccess(id);
            Photos = parameters.GetValue<List<Photo>>("Photos");

            /* ==================================================================================================
             Manual casting

            //if (dto != null)
            //Photo = new Photo
            //{
            //    Id = dto.Id,
            //    Title = dto.Title,
            //    Url = dto.Url
            //};
             * ================================================================================================*/

            /* ==================================================================================================
             * use auto mapper
             * ================================================================================================*/

            //Photo = dto.MapTo<Photo>();
            Photo = Photos.FirstOrDefault(arg => arg.Id == dto.Id) ?? Photos.FirstOrDefault();
        }

        public Photo Photo { get; set; }


        public List<Photo> Photos { get; set; }
    }
}
