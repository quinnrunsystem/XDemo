using System.Linq;
using NUnit.Framework;
using XDemo.Core.BusinessServices.Implementations.Photos;
using XDemo.Core.BusinessServices.Interfaces.Photos;
using System.Threading.Tasks;

namespace LogicTest.Tests
{
    [TestFixture]
    public class SampleTest
    {
        private IPhotoService _service;
        [SetUp]
        public void SettingUp()
        {
            TestContext.WriteLine("SettingUp");
            _service = new PhotoService();
        }

        [Test]
        public async Task GetPhotosTest()
        {
            TestContext.WriteLine("GetPhotosTest");
            var photos = await _service.Get();
            Assert.IsTrue(photos.Any(), "photos.Any()");
        }
        [Test]
        public async Task GetAPhotoTest()
        {
            int idToGet = 3;
            var photo = await _service.Get(idToGet);
            Assert.IsTrue(photo != null && photo.Id == idToGet, "photo != null && photo.Id == idToGet");
        }
    }
}
