using AllinaHealth.Framework.Contexts.Base;
using AllinaHealth.Models.Contexts;

namespace AllinaHealth.Framework.Contexts
{
    public class SiteContext
    {
        public static SiteContextModel Current => GenericContext<SiteContextModel>.Current;
    }
}