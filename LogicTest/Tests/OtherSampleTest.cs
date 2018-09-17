using XDemo.Core.BusinessServices.Interfaces.Photos;
using XDemo.Core.Shared;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Linq;
using System.Collections.Generic;
using XDemo.Core.BusinessServices.Dtos.Photos;

namespace LogicTest.Tests
{
    public class OtherSampleTest : SettingUp
    {
        private IPhotoService _service;

        /* ==================================================================================================
         * over this method to prepare your stuffs
         * ================================================================================================*/
        protected override void SetUp()
        {
            /* ==================================================================================================
             * must call the base method to prepare container
             * ================================================================================================*/
            base.SetUp();
            /* ==================================================================================================
             * resolve the service
             * ================================================================================================*/
            _service = DependencyRegistrar.Current.Resolve<IPhotoService>();
        }
        private List<PhotoDto> _photoDtos = new List<PhotoDto>();
        [Test]
        public async Task AnotherTestMethod()
        {
            await _service.Get(OnSuccess, OnFailed);
            var idToGet = 25;
            var photo = await _service.Get(idToGet);
            var firstPhoto = _photoDtos.FirstOrDefault(arg => arg.Id == idToGet);
            var equalOnValues = (photo == null && firstPhoto == null)
               || (photo.Id == firstPhoto.Id
                   && photo.AlbumId == firstPhoto.AlbumId
                   && photo.Title == firstPhoto.Title
                   && photo.Url == firstPhoto.Url
                   && photo.ThumbnailUrl == firstPhoto.ThumbnailUrl);
            /* ==================================================================================================
            * The message if assert failed should be plain text from your expression
            * ================================================================================================*/
            Assert.IsTrue(photo.Id == idToGet && equalOnValues, "photo.Id == idToGet && equalOnValues");
        }

        void OnFailed()
        {
            _photoDtos = new List<PhotoDto>();
        }

        private void OnSuccess(List<PhotoDto> photoDtos)
        {
            _photoDtos = photoDtos;
        }

    }
}
