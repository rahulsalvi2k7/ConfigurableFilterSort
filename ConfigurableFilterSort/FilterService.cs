using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace ConfigurableFilterSort
{
    public class FilterService
    {
        private readonly JToken _filterConfig;
        private readonly bool _logicalAnd;
        private Func<JToken, bool> _filter;

        public FilterService(JToken filterConfig, bool logicalAnd = true)
        {
            _filterConfig = filterConfig;
            _logicalAnd = logicalAnd;
            _filter = GetFilter();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceDocuments"></param>
        /// <returns></returns>
        public JArray Filter(JArray sourceDocuments)
        {
            return JArray.FromObject(sourceDocuments.Where(_filter));
        }

        private Func<JToken, bool> GetFilter()
        {
            var filterObject = _filterConfig as JObject;

            if (filterObject == null || !filterObject.HasValues)
            {
                return (data) => { return true; };
            }

            return (data) =>
            {
                if (_logicalAnd)
                {
                    return filterObject
                            .Children()
                            .All(filterProperty =>
                                data.Value<string>(filterProperty.Path) == filterProperty.Value<string>());

                }

                return filterObject
                            .Children()
                            .Any(filterProperty =>
                                data.Value<string>(filterProperty.Path) == filterProperty.Value<string>());
            };
        }
    }
}
