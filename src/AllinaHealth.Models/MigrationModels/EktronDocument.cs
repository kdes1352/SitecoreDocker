namespace AllinaHealth.Models.MigrationModels
{
    public class EktronDocument
    {
        private string _url;

        public string Url
        {
            get
            {
                if (!_url.StartsWith("http") && !_url.StartsWith("/"))
                {
                    return "/" + _url;
                }

                return _url;
            }
            set => _url = value;
        }

        public string Handle { get; set; }
    }
}