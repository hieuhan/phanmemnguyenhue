using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;

namespace phanmemnguyenhue.helper
{
    public static class EnumerableHelper
    {
        public static bool IsAny<TSource>(this IEnumerable<TSource> source)
        {
            switch (source)
            {
                case null:
                    return false;
                case TSource[] array:
                    return array.Length != 0;
                case ICollection<TSource> collection:
                    return collection.Count != 0;
                case ICollection baseCollection:
                    return baseCollection.Count != 0;
            }

            using (var enumerator = source.GetEnumerator())
            {
                if (enumerator.MoveNext())
                    return true;
            }

            return false;
        }

        public static IEnumerable<SelectListItem> AddDefaultOption(this IEnumerable<SelectListItem> list, string dataTextField, string selectedValue)
        {
            var items = new List<SelectListItem> { new SelectListItem { Text = dataTextField, Value = selectedValue } };
            items.AddRange(list);
            return items;
        }
    }
}
