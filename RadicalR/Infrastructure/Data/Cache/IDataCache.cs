//-----------------------------------------------------------------------
// <copyright file="IDataCache2.cs" company="Undersoft">
//     Author: Dariusz Hanc
//     Copyright (c) Undersoft. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Series;
using System.Threading.Tasks;

namespace RadicalR
{
    public interface IDataCache : IMassDeck<IUnique>
    {
        IMassDeck<IUnique> CacheSet<T>() where T : IUnique;

        T Lookup<T>(T item) where T : IUnique;

        IDeck<IUnique> Lookup<T>(Func<IMassDeck<IUnique>, IDeck<IUnique>> selector) where T : IUnique;

        T Lookup<T>(object keys) where T : IUnique;

        IDeck<IUnique> Lookup<T>(Tuple<string, object> valueNamePair) where T : IUnique;

        T Lookup<T>(T item, params string[] propertyNames) where T : IUnique;

        T[] Lookup<T>(Func<IDeck<IUnique>, IUnique> key, params Func<IMassDeck<IUnique>, IDeck<IUnique>>[] selectors)
            where T : IUnique;

        T[] Lookup<T>(object key, params Tuple<string, object>[] valueNamePairs) where T : IUnique;

        IDeck<IUnique> Lookup<T>(object key, string propertyNames) where T : IUnique;

        IEnumerable<T> Memorize<T>(IEnumerable<T> items) where T : IUnique;

        T Memorize<T>(T item) where T : IUnique;

        T Memorize<T>(T item, params string[] names) where T : IUnique;

        Task<T> MemorizeAsync<T>(T item) where T : IUnique;

        Task<T> MemorizeAsync<T>(T item, params string[] names) where T : IUnique;

        IMassDeck<IUnique> Catalog { get; }
    }
}