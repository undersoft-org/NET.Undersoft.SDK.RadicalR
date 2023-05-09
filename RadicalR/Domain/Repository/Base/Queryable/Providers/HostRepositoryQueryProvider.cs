using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.OData.Client;
using Microsoft.Extensions.DependencyInjection;

namespace RadicalR
{
    public class HostRepositoryQueryProvider<TEntity> : IQueryProvider where TEntity : class, IIdentifiable
    {
        #region Fields

        private readonly Type queryType;
        private IQueryable<TEntity> query;

        #endregion

        #region Constructors

        public HostRepositoryQueryProvider(DbSet<TEntity> dbSet)
        {
            this.queryType = typeof(EntityRepository<>);
            query = dbSet;
        }
        public HostRepositoryQueryProvider(DataServiceQuery<TEntity> targetDsSet)
        {
            this.queryType = typeof(EntityRepository<>);
            query = targetDsSet;
        }

        #endregion

        #region Methods

        public IQueryable CreateQuery(Expression expression)
        {                      
            try
            {     
                return queryType.MakeGenericType(typeof(TEntity)
                                                ).New<IQueryable>(this, expression);
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public IQueryable<T> CreateQuery<T>(Expression expression)
        {
            return queryType.MakeGenericType(typeof(TEntity)
                                            ).New<IQueryable<T>>(this, expression);
        }

        public object Execute(Expression expression)
        {
            try
            {
                return GetType().GetGenericMethod(nameof(Execute)).Invoke(this, new[] { expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public TResult Execute<TResult>(Expression expression)
        {
            IQueryable<TEntity> newRoot = query;
            var treeCopier = new HostRepositoryExpressionVisitor(newRoot);
            var newExpressionTree = treeCopier.Visit(expression);
            var isEnumerable = (typeof(TResult).IsGenericType &&
                                typeof(TResult).GetGenericTypeDefinition() == typeof(IEnumerable<TEntity>));
            if (isEnumerable)
            {
                return (TResult)newRoot.Provider.CreateQuery(newExpressionTree);
            }
            var result = newRoot.Provider.Execute(newExpressionTree);
            return (TResult)result;
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var task = Task.FromResult(Execute<TResult>(expression));
            task.ConfigureAwait(true);
            return task.Result;
        }

        #endregion
    }
}
