using System.Linq.Expressions;
using System.Series;
using System.Uniques;

namespace RadicalR
{
    public partial class EntityRepository<TEntity>
    {
        public IPage<TEntity> AsPage(int pageIndex, int pageSize, int indexFrom = 0)
        {
            PageSize = pageSize;
            PageIndex = pageIndex;
            IndexFrom = indexFrom;
            TotalCount = this.Count();
            TotalPages = (int)Math.Ceiling(TotalCount / ((double)PageSize));
            return this as IPage<TEntity>;
        }

        public virtual Task<IList<TDto>> Filter<TDto>(IQueryable<TEntity> query)
        {
            return MapTo<TDto>(query);
        }

        public virtual Task<IList<TEntity>> Filter<TDto>(IQueryable<TDto> query)
        {
            return MapFrom<TDto>(query);
        }

        public virtual Task<IPagedSet<TResult>> Filter<TResult>(
            Expression<Func<TEntity, TResult>> selector
        ) where TResult : class
        {
            return Task.Run(
                () =>
                    (IPagedSet<TResult>)
                        new PagedSet<TResult>(
                            (PageSize > 0)
                                ? Query
                                    .Select(selector)
                                    .Skip((PageIndex - IndexFrom) * PageSize)
                                    .Take(PageSize)
                                    .ToArray()
                                : Query.Select(selector).ToArray(),
                            PageIndex,
                            PageSize,
                            IndexFrom
                        ),
                Cancellation
            );
        }

        public async Task<IPagedSet<TEntity>> Filter(SortExpression<TEntity> sortTerms)
        {
            Items = await Filter((PageIndex - IndexFrom) * PageSize, PageSize, sortTerms);
            return this;
        }

        public async Task<IPagedSet<TEntity>> Filter(Expression<Func<TEntity, bool>> predicate)
        {
            Items = await Filter((PageIndex - IndexFrom) * PageSize, PageSize, predicate);
            return this;
        }

        public async Task<IPagedSet<TDto>> Filter<TDto>(SortExpression<TEntity> sortTerms)
        {
            return new PagedSet<TDto>(
                await Filter<TDto>((PageIndex - IndexFrom) * PageSize, PageSize, sortTerms),
                PageIndex,
                PageSize,
                IndexFrom
            );
        }

        public async Task<IPagedSet<TDto>> Filter<TDto>(Expression<Func<TEntity, bool>> predicate)
        {
            return new PagedSet<TDto>(
                await Filter<TDto>((PageIndex - IndexFrom) * PageSize, PageSize, predicate),
                PageIndex,
                PageSize,
                IndexFrom
            );
        }

        public virtual async Task<IPagedSet<TDto>> Filter<TDto, TResult>(
            Expression<Func<TEntity, TResult>> selector
        ) where TResult : class
        {
            return new PagedSet<TDto>(
                await Filter<TDto, TResult>((PageIndex - IndexFrom) * PageSize, PageSize, selector),
                PageIndex,
                PageSize,
                IndexFrom
            );
        }

        public virtual Task<IPagedSet<TResult>> Filter<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate
        ) where TResult : class
        {
            return Task.Run(
                () =>
                    (IPagedSet<TResult>)
                        new PagedSet<TResult>(
                            base[
                                (PageIndex - IndexFrom) * PageSize,
                                PageSize,
                                this[predicate]
                            ].Select(selector),
                            PageIndex,
                            PageSize,
                            IndexFrom
                        ),
                Cancellation
            );
        }

        public virtual Task<IPagedSet<TResult>> Filter<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            params Expression<Func<TEntity, object>>[] expanders
        ) where TResult : class
        {
            return Task.Run(
                () =>
                    (IPagedSet<TResult>)
                        new PagedSet<TResult>(
                            base[
                                (PageIndex - IndexFrom) * PageSize,
                                PageSize,
                                this[expanders]
                            ].Select(selector),
                            PageIndex,
                            PageSize,
                            IndexFrom
                        ),
                Cancellation
            );
        }

        public async Task<IPagedSet<TEntity>> Filter(
            Expression<Func<TEntity, bool>> predicate,
            SortExpression<TEntity> sortTerms
        )
        {
            Items = await Filter(
                (PageIndex - IndexFrom) * PageSize,
                PageSize,
                predicate,
                sortTerms
            );
            return this;
        }

        public async Task<IPagedSet<TEntity>> Filter(
            Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] expanders
        )
        {
            Items = await Filter(
                (PageIndex - IndexFrom) * PageSize,
                PageSize,
                predicate,
                expanders
            );
            return this;
        }

        public async Task<IPagedSet<TEntity>> Filter(
            SortExpression<TEntity> sortTerms,
            params Expression<Func<TEntity, object>>[] expanders
        )
        {
            Items = await Filter(
                (PageIndex - IndexFrom) * PageSize,
                PageSize,
                sortTerms,
                expanders
            );
            return this;
        }

        public async Task<IPagedSet<TDto>> Filter<TDto>(
            Expression<Func<TEntity, bool>> predicate,
            SortExpression<TEntity> sortTerms
        )
        {
            return new PagedSet<TDto>(
                await Filter<TDto>(
                    (PageIndex - IndexFrom) * PageSize,
                    PageSize,
                    predicate,
                    sortTerms
                ),
                PageIndex,
                PageSize,
                IndexFrom
            );
        }

        public async Task<IPagedSet<TDto>> Filter<TDto>(
            Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] expanders
        )
        {
            return new PagedSet<TDto>(
                await Filter<TDto>(
                    (PageIndex - IndexFrom) * PageSize,
                    PageSize,
                    predicate,
                    expanders
                ),
                PageIndex,
                PageSize,
                IndexFrom
            );
        }

        public async Task<IPagedSet<TDto>> Filter<TDto>(
            SortExpression<TEntity> sortTerms,
            params Expression<Func<TEntity, object>>[] expanders
        )
        {
            return new PagedSet<TDto>(
                await Filter<TDto>(
                    (PageIndex - IndexFrom) * PageSize,
                    PageSize,
                    sortTerms,
                    expanders
                ),
                PageIndex,
                PageSize,
                IndexFrom
            );
        }

        public virtual async Task<IPagedSet<TDto>> Filter<TDto, TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate
        ) where TResult : class
        {
            return new PagedSet<TDto>(
                await Filter<TDto, TResult>(
                    (PageIndex - IndexFrom) * PageSize,
                    PageSize,
                    selector,
                    predicate
                ),
                PageIndex,
                PageSize,
                IndexFrom
            );
        }

        public virtual async Task<IPagedSet<TDto>> Filter<TDto, TResult>(
            Expression<Func<TEntity, TResult>> selector,
            params Expression<Func<TEntity, object>>[] expanders
        ) where TResult : class
        {
            return new PagedSet<TDto>(
                await Filter<TDto, TResult>(
                    (PageIndex - IndexFrom) * PageSize,
                    PageSize,
                    selector,
                    expanders
                ),
                PageIndex,
                PageSize,
                IndexFrom
            );
        }

        public virtual Task<IDeck<TDto>> Filter<TDto>(
            int skip,
            int take,
            SortExpression<TEntity> sortTerms
        )
        {
            return HashMapTo<TDto>(this[skip, take, sortTerms]);
        }

        public virtual Task<IDeck<TDto>> Filter<TDto>(
            int skip,
            int take,
            Expression<Func<TEntity, bool>> predicate
        )
        {
            return HashMapTo<TDto>(base[skip, take, predicate]);
        }

        public virtual Task<IList<TDto>> Filter<TDto, TResult>(
            int skip,
            int take,
            Expression<Func<TEntity, TResult>> selector
        ) where TResult : class
        {
            return MapTo<TDto>(
                (take > 0)
                    ? Query.Select(selector).Skip(skip).Take(take).ToArray()
                    : Query.Select(selector).ToArray()
            );
        }

        public async Task<IPagedSet<TEntity>> Filter(
            Expression<Func<TEntity, bool>> predicate,
            SortExpression<TEntity> sortTerms,
            params Expression<Func<TEntity, object>>[] expanders
        )
        {
            Items = await Filter(
                (PageIndex - IndexFrom) * PageSize,
                PageSize,
                predicate,
                sortTerms,
                expanders
            );
            return this;
        }

        public async Task<IPagedSet<TDto>> Filter<TDto>(
            Expression<Func<TEntity, bool>> predicate,
            SortExpression<TEntity> sortTerms,
            params Expression<Func<TEntity, object>>[] expanders
        )
        {
            return new PagedSet<TDto>(
                await Filter<TDto>(
                    (PageIndex - IndexFrom) * PageSize,
                    PageSize,
                    predicate,
                    sortTerms,
                    expanders
                ),
                PageIndex,
                PageSize,
                IndexFrom
            );
        }

        public virtual Task<IDeck<TDto>> Filter<TDto>(
            int skip,
            int take,
            Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] expanders
        )
        {
            return HashMapTo<TDto>(base[skip, take, predicate, expanders]);
        }

        public virtual Task<IDeck<TDto>> Filter<TDto>(
            int skip,
            int take,
            Expression<Func<TEntity, bool>> predicate,
            SortExpression<TEntity> sortTerms
        )
        {
            return HashMapTo<TDto>(base[skip, take, predicate, sortTerms]);
        }

        public virtual Task<IDeck<TDto>> Filter<TDto>(
            int skip,
            int take,
            SortExpression<TEntity> sortTerms,
            params Expression<Func<TEntity, object>>[] expanders
        )
        {
            return HashMapTo<TDto>(this[skip, take, sortTerms, expanders]);
        }

        public virtual IAsyncEnumerable<TDto> FilterAsync<TDto>(
          int skip,
          int take,
          SortExpression<TEntity> sortTerms,
          params Expression<Func<TEntity, object>>[] expanders
      )
        {
            return MapToAsync<TDto>(this[skip, take, sortTerms, expanders]);
        }

        public virtual Task<IList<TDto>> Filter<TDto, TResult>(
            int skip,
            int take,
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate
        ) where TResult : class
        {
            return MapTo<TDto>(base[skip, take, this[predicate]].Select(selector));
        }

        public virtual Task<IList<TDto>> Filter<TDto, TResult>(
            int skip,
            int take,
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, object>>[] expanders
        ) where TResult : class
        {
            return MapTo<TDto>(base[skip, take, this[expanders]].Select(selector));
        }

        public virtual Task<IPagedSet<TResult>> Filter<TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate,
            SortExpression<TEntity> sortTerms,
            params Expression<Func<TEntity, object>>[] expanders
        ) where TResult : class
        {
            return Task.Run(
                () =>
                    (IPagedSet<TResult>)
                        new PagedSet<TResult>(
                            base[
                                (PageIndex - IndexFrom) * PageSize,
                                PageSize,
                                this[predicate, sortTerms, expanders]
                            ].Select(selector),
                            PageIndex,
                            PageSize,
                            IndexFrom
                        ),
                Cancellation
            );
        }

        public virtual async Task<IPagedSet<TDto>> Filter<TDto, TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate,
            SortExpression<TEntity> sortTerms,
            params Expression<Func<TEntity, object>>[] expanders
        ) where TResult : class
        {
            return new PagedSet<TDto>(
                await Filter<TDto, TResult>(
                    (PageIndex - IndexFrom) * PageSize,
                    PageSize,
                    selector,
                    predicate,
                    sortTerms,
                    expanders
                ),
                PageIndex,
                PageSize,
                IndexFrom
            );
        }

        public virtual Task<IDeck<TDto>> Filter<TDto>(
            int skip,
            int take,
            Expression<Func<TEntity, bool>> predicate,
            SortExpression<TEntity> sortTerms,
            params Expression<Func<TEntity, object>>[] expanders
        )
        {
            return HashMapTo<TDto>(base[skip, take, predicate, sortTerms, expanders]);
        }

        public virtual IAsyncEnumerable<TDto> FilterAsync<TDto>(
            int skip,
            int take,
            Expression<Func<TEntity, bool>> predicate,
            SortExpression<TEntity> sortTerms,
            params Expression<Func<TEntity, object>>[] expanders
        )
        {
            return MapToAsync<TDto>(base[skip, take, predicate, sortTerms, expanders]);
        }

        public virtual Task<IList<TDto>> Filter<TDto, TResult>(
            int skip,
            int take,
            Expression<Func<TEntity, TResult>> selector,
            SortExpression<TEntity> sortTerms,
            Expression<Func<TEntity, bool>> predicate
        ) where TResult : class
        {
            return MapTo<TDto>(base[skip, take, this[predicate, sortTerms]].Select(selector));
        }

        public virtual Task<IList<TDto>> Filter<TDto, TResult>(
            int skip,
            int take,
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate,
            SortExpression<TEntity> sortTerms,
            params Expression<Func<TEntity, object>>[] expanders
        ) where TResult : class
        {
            return MapTo<TDto>(
                base[skip, take, this[predicate, sortTerms, expanders]].Select(selector)
            );
        }

        public virtual Task<TDto> Find<TDto>(params object[] keys)
        {
            return MapTo<TDto>(this[keys]);
        }

        public virtual Task<TDto> Find<TDto>(
            object[] keys,
            params Expression<Func<TEntity, object>>[] expanders
        )
        {
            return MapTo<TDto>(this[keys, expanders]);
        }

        public virtual Task<TDto> Find<TDto>(
            Expression<Func<TEntity, bool>> predicate,
            bool reverse
        )
        {
            return MapTo<TDto>(base[reverse, predicate]);
        }

        public virtual Task<TDto> Find<TDto, TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate
        ) where TResult : class
        {
            return MapTo<TDto>(base.Find(predicate, selector));
        }

        public virtual Task<TDto> Find<TDto, TResult>(
            Expression<Func<TEntity, TResult>> selector,
            object[] keys,
            params Expression<Func<TEntity, object>>[] expanders
        ) where TResult : class
        {
            return MapTo<TDto>(base.Find(keys, selector, expanders));
        }

        public virtual Task<TDto> Find<TDto>(
            Expression<Func<TEntity, bool>> predicate,
            bool reverse,
            params Expression<Func<TEntity, object>>[] expanders
        )
        {
            return MapTo<TDto>(base[reverse, predicate, expanders]);
        }

        public virtual Task<TDto> Find<TDto, TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, bool>> predicate,
            params Expression<Func<TEntity, object>>[] expanders
        ) where TResult : class
        {
            return MapTo<TDto>(base.Find(predicate, selector, expanders));
        }

        public virtual IQueryable<TDto> FindOneAsync<TDto>(
        Expression<Func<TEntity, bool>> predicate,
        params Expression<Func<TEntity, object>>[] expanders
        ) where TDto : class, IUnique
        {
            return QueryMapTo<TDto>(base[predicate, expanders]);
        }

        public virtual IQueryable<TDto> FindOneAsync<TDto>(
            object[] keys,
            params Expression<Func<TEntity, object>>[] expanders
        ) where TDto : class, IUnique
        {
            return QueryMapTo<TDto>(new[] { this[keys, expanders] }.AsQueryable());
        }

        public virtual Task<IList<TDto>> Get<TDto, TResult>(
            Expression<Func<TEntity, TResult>> selector
        ) where TResult : class
        {
            return MapTo<TDto>(Query.Select(selector));
        }

        public new async Task<IPagedSet<TEntity>> Get(
            params Expression<Func<TEntity, object>>[] expanders
        )
        {
            Items = await base.Get((PageIndex - IndexFrom) * PageSize, PageSize, expanders)
                .ConfigureAwait(false);
            return this;
        }

        public async Task<IPagedSet<TDto>> Get<TDto>(
            params Expression<Func<TEntity, object>>[] expanders
        )
        {
            return new PagedSet<TDto>(
                await Get<TDto>((PageIndex - IndexFrom) * PageSize, PageSize, expanders)
                    .ConfigureAwait(false),
                PageIndex,
                PageSize,
                IndexFrom
            );
        }

        public virtual Task<IList<TDto>> Get<TDto, TResult>(
            Expression<Func<TEntity, TResult>> selector,
            Expression<Func<TEntity, object>>[] expanders
        ) where TResult : class
        {
            return MapTo<TDto>(this[0, 0, base[expanders]].Select(selector));
        }

        public virtual Task<IDeck<TDto>> Get<TDto>(
            int skip,
            int take,
            params Expression<Func<TEntity, object>>[] expanders
        )
        {
            return HashMapTo<TDto>(base[skip, take, expanders]);
        }

        public virtual Task<IDeck<TDto>> Get<TDto>(
            int skip,
            int take,
            SortExpression<TEntity> sortTerms,
            params Expression<Func<TEntity, object>>[] expanders
        )
        {
            return HashMapTo<TDto>(base[skip, take, sortTerms, expanders]);
        }

        public virtual IQueryable<TDto> GetQuery<TDto>(
            params Expression<Func<TEntity, object>>[] expanders
        ) where TDto : class
        {
            return QueryMapTo<TDto>(base[expanders]);
        }

        public virtual IQueryable<TDto> GetQuery<TDto>(
            SortExpression<TEntity> sortTerms,
            params Expression<Func<TEntity, object>>[] expanders
        ) where TDto : class
        {
            return QueryMapTo<TDto>(base[sortTerms, expanders]);
        }

        public virtual Task<IQueryable<TDto>> GetQueryAsync<TDto>(
            params Expression<Func<TEntity, object>>[] expanders
        ) where TDto : class
        {
            return QueryMapAsyncTo<TDto>(base[expanders]);
        }

        public virtual Task<IQueryable<TDto>> GetQueryAsync<TDto>(
            SortExpression<TEntity> sortTerms,
            params Expression<Func<TEntity, object>>[] expanders
        ) where TDto : class
        {
            return QueryMapAsyncTo<TDto>(base[sortTerms, expanders]);
        }

        public virtual IAsyncEnumerable<TDto> GetAsync<TDto>(int skip, int take,
         params Expression<Func<TEntity, object>>[] expanders
        ) where TDto : class
        {
            return MapToAsync<TDto>(base[skip, take, expanders]);
        }

        public virtual IAsyncEnumerable<TDto> GetAsync<TDto>(int skip, int take,
            SortExpression<TEntity> sortTerms,
            params Expression<Func<TEntity, object>>[] expanders
        ) where TDto : class
        {
            return MapToAsync<TDto>(base[skip, take, sortTerms, expanders]);
        }
    }
}
