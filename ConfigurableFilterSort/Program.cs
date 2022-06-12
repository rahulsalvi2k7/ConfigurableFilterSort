using Newtonsoft.Json.Linq;
using System;

namespace ConfigurableFilterSort
{
    class Program
    {
        static void Main(string[] args)
        {
            var orders = JArray.FromObject(new[]
            {
                new { Id = 2, name = "CO-1", createdOn = DateTime.Now.AddDays(-1) },
                new { Id = 1, name = "SO-1", createdOn = DateTime.Now.AddDays(0) },
                new { Id = 1, name = "BO-1", createdOn = DateTime.Now.AddDays(1) },
            });

            // filter

            // sort
            var sortService = new SortService(JToken.FromObject(new { Id = "asc", name = "asc" }));

            var sortedOrders = sortService.Sort(orders);

            Console.WriteLine(sortedOrders.ToString(Newtonsoft.Json.Formatting.Indented));
        }
    }
}
