using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace RadicalR
{
    public static class GlobalCache
    {
        static GlobalCache()
        {
            IConfigurationSection cacheconfig = ServiceManager
                .GetConfiguration()
                .DataCacheLifeTime();
            Catalog = new DataCache(
                TimeSpan.FromMinutes(
                    cacheconfig.GetValue<uint>("Minutes") + cacheconfig.GetValue<uint>("Hours") * 60
                ),
                null,
                513
            );
        }

        public static async Task<T> Lookup<T>(object keys) where T : IUnique
        {
            return await Task.Run(() =>
            {
                if (Catalog.TryGet(keys, typeof(T).ProxyRetypeKey32(), out IUnique output))
                    return (T)output;
                return default;
            });
        }

        public static T ToCache<T>(this T item) where T : IUnique
        {
            return Catalog.Memorize(item);
        }

        public static T ToCache<T>(this T item, params string[] names) where T : IUnique
        {
            return Catalog.Memorize(item, names);
        }

        public static async Task<T> ToCacheAsync<T>(this T item) where T : IUnique
        {
            return await Task.Run(() => item.ToCache()).ConfigureAwait(false);
        }

        public static async Task<T> ToCacheAsync<T>(this T item, params string[] names)
            where T : IUnique
        {
            return await Task.Run(() => item.ToCache(names)).ConfigureAwait(false);
        }

        public static IDataCache Catalog { get; }
    }
}
