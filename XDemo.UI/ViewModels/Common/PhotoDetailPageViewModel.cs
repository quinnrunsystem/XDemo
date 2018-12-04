﻿using System;
using Prism.Navigation;
using XDemo.UI.ViewModels.Base;
using XDemo.Core.BusinessServices.Interfaces.Photos;
using XDemo.UI.Models.Photos;
using XDemo.Core.Extensions;

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
            Photo = dto.MapTo<Photo>();
        }

        public Photo Photo { get; private set; }
    }
}
