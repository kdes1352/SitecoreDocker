using System.Linq;

namespace AllinaHealth.Framework.Pipelines.Mvc.GetRenderer
{
    public class ControllerInfo
    {
        public string Controller { get; }

        public string Action { get; }

        public string Area { get; }

        public string Namespace { get; }

        public ControllerInfo(string controllerName, string action, string area)
        {
            Action = action;
            if (controllerName.IndexOf(',') > -1)
            {
                Controller = controllerName;
            }
            else
            {
                var controllerSegments = controllerName.Split('.');
                if (controllerSegments.Any())
                {
                    Namespace = string.Join(".", controllerSegments.Take(controllerSegments.Length - 1));
                }

                Controller = controllerSegments.Last().Replace("Controller", string.Empty);
                Area = area;
            }
        }
    }
}