namespace AllinaHealth.Framework.Links
{
    public static class LinkHelper
    {
        public static string RemoveSslPort(string url)
        {
            if (string.IsNullOrEmpty(url)) return url;
            if (url.StartsWith("https://"))
            {
                url = url.Replace(":443", string.Empty);
            }

            return url;
        }
    }
}