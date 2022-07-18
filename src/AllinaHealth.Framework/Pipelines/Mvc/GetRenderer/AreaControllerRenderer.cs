using System.IO;
using Sitecore.Mvc.Presentation;

namespace AllinaHealth.Framework.Pipelines.Mvc.GetRenderer
{
    public class AreaControllerRenderer : Renderer
    {
        private readonly ControllerInfo _controllerInfo;
        private readonly AreaControllerRunner _controllerRunner;

        public AreaControllerRenderer(ControllerInfo controllerInfo, AreaControllerRunner controllerRunner)
        {
            _controllerInfo = controllerInfo;
            _controllerRunner = controllerRunner;
        }

        public override void Render(TextWriter writer)
        {
            _controllerRunner.Execute(writer);
        }

        public override string CacheKey => string.Join("::", _controllerInfo.Area, _controllerInfo.Controller, _controllerInfo.Action);

        public override string ToString()
        {
            return string.Format("Area: {0}, Controller: {1}, Action: {2}", new object[]
            {
                _controllerInfo.Area,
                _controllerInfo.Controller,
                _controllerInfo.Action
            });
        }
    }
}