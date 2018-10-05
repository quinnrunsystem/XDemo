using Prism;
using Prism.Ioc;

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
            //todo: register base on OS service, ie: TextToSpeechService...
        }
    }
}