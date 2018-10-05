using Prism;
using Prism.Ioc;
using XDemo.Core.BusinessServices.Interfaces.Common;
using XDemo.iOS.Services.Implementations;

namespace XDemo.iOS
{
    public class IOSPlatformInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            RegisterPlatformSpecifiedServices(containerRegistry);
        }

        void RegisterPlatformSpecifiedServices(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<IClipboardService, ClipboardService>();
            /* ==================================================================================================
             * todo: register base on OS service, ie: TextToSpeechService...
             * ================================================================================================*/
        }
    }
}
