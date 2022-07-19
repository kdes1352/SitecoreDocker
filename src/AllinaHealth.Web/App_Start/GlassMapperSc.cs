using Glass.Mapper.Maps;
using Glass.Mapper.Sc.Configuration.Fluent;
using Glass.Mapper.Sc.IoC;
using Sitecore.Pipelines;

namespace AllinaHealth.Web
{
    public class GlassMapperSc
    {
        public void Process(PipelineArgs args)
        {
            Start();
        }

        public static void Start()
        {
            //install the custom services
            var resolver = GlassMapperScCustom.CreateResolver();

            //create a context
            var context = Glass.Mapper.Context.Create(resolver);

            LoadConfigurationMaps(resolver, context);

            context.Load(
                GlassMapperScCustom.GlassLoaders()
            );

            GlassMapperScCustom.PostLoad(resolver);


            //EditFrameBuilder.EditFrameItemPrefix = "Glass-";

        }

        public static void LoadConfigurationMaps(IDependencyResolver resolver, Glass.Mapper.Context context)
        {
            var dependencyResolver = resolver as DependencyResolver;
            if (dependencyResolver == null)
            {
                return;
            }

            if (dependencyResolver.ConfigurationMapFactory is ConfigurationMapConfigFactory)
            {
                GlassMapperScCustom.AddMaps(dependencyResolver.ConfigurationMapFactory);
            }

            IConfigurationMap configurationMap = new ConfigurationMap(dependencyResolver);
            var configurationLoader = configurationMap.GetConfigurationLoader<SitecoreFluentConfigurationLoader>();
            context.Load(configurationLoader);
        }
    }
}