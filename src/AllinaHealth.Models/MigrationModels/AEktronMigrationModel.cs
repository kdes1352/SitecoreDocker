using System.Collections.Generic;

namespace AllinaHealth.Models.MigrationModels
{
    public abstract class AEktronMigrationModel<T> where T : new()
    {
        public EktronInfo Info { get; set; }

        public abstract T FromXmlString(EktronInfo ei);
    }

    public static class ExtronMirgrationExtensions
    {
        public static List<T> GetList<T>(this List<EktronInfo> list) where T : AEktronMigrationModel<T>, new()
        {
            var returnList = new List<T>();
            foreach (var ei in list)
            {
                if (string.IsNullOrEmpty(ei.Data)) continue;
                var model = new T().FromXmlString(ei);
                if (model == null) continue;
                model.Info = ei;
                returnList.Add(model);
            }

            return returnList;
        }
    }
}