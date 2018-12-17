using Prism;
using Prism.Ioc;
using XDemo.Core.BusinessServices.Interfaces.Hardwares.LocalAuthentications;
using XDemo.Droid.Services.Implementations.Fingerprints;

namespace XDemo.Droid
{
    public class AndroidPlatformInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            RegisterPlatformSpecifiedServices(containerRegistry);
        }

        void RegisterPlatformSpecifiedServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<ILocalAuthenticationService, FingerprintService>();
            //todo: register base on OS service, ie: TextToSpeechService...
        }
    }
}