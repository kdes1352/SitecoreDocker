using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Sitecore.ContentSearch;

namespace AllinaHealth.Framework.ContentSearch.GlobalSearch
{
    public class IndexableSearchWebPage : IIndexable
    {
        private IEnumerable<IIndexableDataField> _fields;
        private readonly SearchWebPageItem _searchPage;

        public IndexableSearchWebPage(SearchWebPageItem searchPage)
        {
            _searchPage = searchPage;
            LoadAllFields();
        }

        public IIndexableId Id => new IndexableId<string>(_searchPage.Url);

        public IIndexableUniqueId UniqueId => new IndexableUniqueId<string>(_searchPage.Url);

        public string DataSource => !string.IsNullOrEmpty(_searchPage.SitecoreID) ? "sitecore" : "external";

        public string AbsolutePath => "/";

        public CultureInfo Culture => new CultureInfo("en");

        public IEnumerable<IIndexableDataField> Fields => _fields;

        public IIndexableDataField GetFieldById(object fieldId)
        {
            return _fields.FirstOrDefault(f => f.Id == fieldId);
        }

        public IIndexableDataField GetFieldByName(string fieldName)
        {
            fieldName = fieldName.ToLower();
            return _fields.FirstOrDefault(f => f.Name.ToLower() == fieldName);
        }

        public void LoadAllFields()
        {
            if (_searchPage == null)
            {
                return;
            }

            var fieldNames = _searchPage.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase).Select(fi => fi.Name).ToArray();
            _fields = IndexableDataField.CreateFromProperties(_searchPage, string.Empty, fieldNames);
        }
    }
}