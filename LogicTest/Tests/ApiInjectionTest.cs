using NUnit.Framework.Internal;
using NUnit.Framework;
using XDemo.Core.Shared;
using XDemo.Core.ApiDefinitions;
namespace LogicTest.Tests
{
    public class ApiInjectionTest : SettingUp
    {
        [Test]
        public void ApiSolveManualTest()
        {
            var api = DependencyRegistrar.Current.Resolve<IPhotoApi>();
            Assert.IsTrue(api != null, "api != null");
        }
    }
}
