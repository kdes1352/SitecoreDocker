using Sitecore.Buckets.FieldTypes;
using Sitecore.ContentSearch.Utilities;
using Sitecore.Data.Items;
using Sitecore.Pipelines.GetRenderingDatasource;
using System;

namespace AllinaHealth.Framework.Pipelines.GetRenderingDatasource
{
    public class GetCodeDatasourceLocations : GetDatasourceAbstractProcessor<GetRenderingDatasourceArgs>
    {
        public const string Token = "code:";

        public override Item[] GetDatasourceRoots(string value)
        {
            if (!value.StartsWith(Token, StringComparison.InvariantCulture)) return Array.Empty<Item>();

            return ReflectionUtility.CreateInstance(Type.GetType(value.Substring(Token.Length))) is IDataSource datasource ? datasource.ListQuery(ContextItem) : Array.Empty<Item>();
        }

        public override string SetPropertiesAndGetSourceValue(GetRenderingDatasourceArgs args)
        {
            ContentDatabase = args.ContentDatabase;
            ContextItemPath = args.ContextItemPath;
            return args.RenderingItem["Datasource Location"];
        }

        public override void AddDatasourceRoots(GetRenderingDatasourceArgs args, Item[] list, string sourceValue)
        {
            args.DatasourceRoots.AddRange(list);
        }
    }
}