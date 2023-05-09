using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.OData.Client;

namespace RadicalR
{
    public class RepositoryQueryProvider<TEntity> : IAsyncQueryProvider where TEntity : Entity
    {
        #region Fields

        private readonly Type queryType;
        private IQueryable<TEntity> query;

        #endregion

        #region Constructors

        public RepositoryQueryProvider(DbSet<TEntity> targetDbSet)
        {
            this.queryType = typeof(Repository<>);
            query = targetDbSet;
        }
        public RepositoryQueryProvider(DataServiceQuery<TEntity> targetDsSet)
        {
            this.queryType = typeof(Repository<>);
            query = targetDsSet;
        }

        #endregion

        #region Methods

        public IQueryable CreateQuery(Expression expression)
        {
            var elementType = expression.Type.FindElementType();
            try
            {
                return queryType.MakeGenericType(elementType).New<IQueryable>(this, expression);
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public IQueryable<T> CreateQuery<T>(Expression expression)
        {
            return queryType.MakeGenericType(expression.Type.FindElementType()).New<IQueryable<T>>(this, expression);
        }

        public object Execute(Expression expression)
        {
            try
            {
                return this.GetType().GetGenericMethod(nameof(Execute)).Invoke(this, new[] { expression });
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public TResult Execute<TResult>(Expression expression)
        {
            IQueryable<TEntity> newRoot = query;
            var treeCopier = new RepositoryExpressionVisitor(newRoot);
            var newExpressionTree = treeCopier.Visit(expression);
            var isEnumerable = (typeof(TResult).IsGenericType && typeof(TResult).GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (isEnumerable)
            {
                return (TResult)newRoot.Provider.CreateQuery(newExpressionTree);
            }
            var result = newRoot.Provider.Execute(newExpressionTree);
            return (TResult)result;
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Execute<TResult>(expression)).Result;
        }

        #endregion
    }
}
