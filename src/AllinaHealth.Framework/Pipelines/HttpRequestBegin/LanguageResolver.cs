using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.HttpRequest;

namespace AllinaHealth.Framework.Pipelines.HttpRequestBegin
{
    public class LanguageResolver : Sitecore.Pipelines.HttpRequest.LanguageResolver
    {
        public override void Process(HttpRequestArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            var languageFromRequest = GetLanguageFromRequest(args);
            //Language languageFromFilePath = Sitecore.Context.Data.FilePathLanguage; // remove this so we don't do file path langauge embedding.

            if (languageFromRequest == null)
            {
                return;
            }

            Context.Language = languageFromRequest;
            TraceLanguageSet(languageFromRequest, args);
        }
    }
}