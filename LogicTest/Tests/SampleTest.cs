using System.Linq;
using NUnit.Framework;
using XDemo.Core.BusinessServices.Interfaces.Photos;
using System.Threading.Tasks;
using XDemo.Core.Shared;
using System.Collections.Generic;
using XDemo.Core.BusinessServices.Dtos.Photos;

namespace LogicTest.Tests
{
    /* ==================================================================================================
     * inherit SettingUp base class to make your class known as a test class
     * ================================================================================================*/
    public class SampleTest : SettingUp
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

        /* ==================================================================================================
         * Mark your method with a TestAttribute for NUnit known its a test method
         * ================================================================================================*/
        [Test]
        public async Task GetPhotosTest()
        {
            TestContext.WriteLine("GetPhotosTest");
            await _service.Get(OnSuccess, OnFailed);
            /* ==================================================================================================
            * The message if assert failed should be plain text from your expression
            * ================================================================================================*/
            Assert.IsTrue(_getSucceeded, "_getSucceeded");
        }
        private bool _getSucceeded = false;
        void OnFailed()
        {
            _getSucceeded = false;
        }

        private void OnSuccess(List<PhotoDto> photoDtos)
        {
            _getSucceeded = true;
        }

        [Test]
        public async Task GetAPhotoTest()
        {
            int idToGet = 3;
            var photo = await _service.Get(idToGet);
            /* ==================================================================================================
             * The message if assert failed should be plain text from your expression
             * ================================================================================================*/
            Assert.IsTrue(photo != null && photo.Id == idToGet, "photo != null && photo.Id == idToGet");
        }
    }
}
