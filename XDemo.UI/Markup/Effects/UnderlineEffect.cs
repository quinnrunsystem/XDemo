using Xamarin.Forms;
using XDemo.UI.Markup.Effects;


namespace XDemo.UI.Markup.Effects
{
    public class UnderlineEffect : RoutingEffect
    {
        public const string EffectNamespace = "Example";

        public UnderlineEffect() : base($"{EffectNamespace}.{nameof(UnderlineEffect)}")
        {
        }
    }
}
