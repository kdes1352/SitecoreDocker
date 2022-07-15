using Sitecore.Mvc.Helpers;

namespace AllinaHealth.Framework.Contexts.Base
{
    public class GenericContext<T> where T : class, new()
    {
        public static T Current
        {
            get
            {
                var threadData = ThreadHelper.GetThreadData<T>();
                if (threadData != null)
                {
                    return threadData;
                }

                var data = new T();
                ThreadHelper.SetThreadData(data);
                return data;
            }
        }
    }
}