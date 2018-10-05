using Prism;
using Prism.Ioc;

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
            /* ==================================================================================================
             * todo: register base on OS service, ie: TextToSpeechService...
             * ================================================================================================*/
        }
    }
}
