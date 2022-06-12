using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ConfigurableFilterSort
{
    public class SortService
    {
        private readonly JToken _sortConfig;

        public SortService(JToken sortConfig)
        {
            _sortConfig = sortConfig;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceDocuments"></param>
        /// <returns></returns>
        public JArray Sort(JArray sourceDocuments)
        {
            var _sortConfigObj = _sortConfig as JObject;

            if (_sortConfigObj == null || !_sortConfigObj.HasValues)
            {
                return sourceDocuments;
            }

            var data = MultipleSort(sourceDocuments, _sortConfigObj);

            return JArray.FromObject(data);
        }

        private IEnumerable<JToken> MultipleSort(IEnumerable<JToken> data, JObject gridsorts)
        {
            var sortExpressions = new List<Tuple<string, string>>();

            foreach (var property in gridsorts.Properties())
            {
                var sortOrder = property.Value.Value<string>().Trim().ToLower();
                sortExpressions.Add(new Tuple<string, string>(property.Name.Trim(), sortOrder));
            }

            // No sorting needed  
            if ((sortExpressions == null) || (sortExpressions.Count <= 0))
            {
                return data;
            }

            // Let us sort it  
            IEnumerable<JToken> query = from item in data select item;
            IOrderedEnumerable<JToken> orderedQuery = null;

            for (int i = 0; i < sortExpressions.Count; i++)
            {
                // We need to keep the loop index, not sure why it is altered by the Linq.  
                var index = i;

                Func<JToken, object> expression = GetSortExpression(sortExpressions[index].Item1);

                if (sortExpressions[index].Item2 == "asc")
                {
                    orderedQuery = (index == 0) ? query.OrderBy(expression) :
                        orderedQuery.ThenBy(expression);
                }
                else
                {
                    orderedQuery = (index == 0) ? query.OrderByDescending(expression) :
                        orderedQuery.ThenByDescending(expression);
                }
            }
            query = orderedQuery;
            return query;
        }

        private Func<JToken, object> GetSortExpression(string path)
        {
            return (item) =>
            {                
                return item.SelectToken(path).Value<object>();
            };
        }
    }
}
