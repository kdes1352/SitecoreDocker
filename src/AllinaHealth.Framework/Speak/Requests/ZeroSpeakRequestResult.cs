using Sitecore.ExperienceEditor.Speak.Server.Contexts;
using Sitecore.ExperienceEditor.Speak.Server.Requests;
using Sitecore.ExperienceEditor.Speak.Server.Responses;

namespace AllinaHealth.Framework.Speak.Requests
{
    public class ZeroSpeakRequestResult : PipelineProcessorRequest<ItemContext>
    {
        public override PipelineProcessorResponseValue ProcessRequest()
        {
            return new PipelineProcessorResponseValue
            {
                Value = 0
            };
        }
    }
}