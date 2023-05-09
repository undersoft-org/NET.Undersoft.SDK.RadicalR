//-----------------------------------------------------------------------
// <copyright file="IEntityCache.cs" company="Undersoft">
//     Author: Dariusz Hanc
//     Copyright (c) Undersoft. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;

namespace RadicalR
{
    public interface IEntityCache<TStore, TEntity> : IStoreCache<TStore>
    {
    }
}