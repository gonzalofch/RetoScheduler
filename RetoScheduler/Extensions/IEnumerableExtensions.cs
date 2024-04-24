namespace RetoScheduler.Extensions
{
    public static class IEnumerableExtensions
    {

        public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source,bool where, Func<TSource, bool> predicate)
        {
            return where
                ? source.Where(predicate)
                : source;
        }
    }
}
