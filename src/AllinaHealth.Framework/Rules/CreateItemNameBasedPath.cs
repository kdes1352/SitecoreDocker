using Sitecore.Buckets.Rules.Bucketing;
using Sitecore.Buckets.Util;
using Sitecore.Diagnostics;
using Sitecore.Rules.Actions;

namespace AllinaHealth.Framework.Rules
{
    public class CreateItemNameBasedPath<T> : RuleAction<T> where T : BucketingRuleContext
    {
        public string Levels { get; set; }

        public override void Apply(T ruleContext)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");

            if (!int.TryParse(Levels, out var length))
            {
                Log.Warn("CreateItemNameBasedPath: Cannot resolve item path by this rule", this);
                return;
            }

            if (length > ruleContext.NewItemName.Length)
            {
                length = ruleContext.NewItemName.Length;
            }

            var charArray = ruleContext.NewItemName.Substring(0, length).ToCharArray();

            for (var i = 0; i < charArray.Length; i++)
            {
                if (string.IsNullOrEmpty(charArray[i].ToString()))
                {
                    charArray[i] = '_';
                }
            }

            ruleContext.ResolvedPath = string.Join(Constants.ContentPathSeperator, charArray).ToLowerInvariant();
        }
    }
}