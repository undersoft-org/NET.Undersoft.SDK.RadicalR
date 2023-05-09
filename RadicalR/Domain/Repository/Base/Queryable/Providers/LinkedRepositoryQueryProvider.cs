using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.OData.Client;

namespace RadicalR
{
    public class LinkedRepositoryQueryProvider<TEntity> : IAsyncQueryProvider where TEntity : class, IIdentifiable
    {
        #region Fields

        private readonly Type queryType;
        private IQueryable<TEntity> query;

        #endregion

        #region Constructors

        public LinkedRepositoryQueryProvider(DataServiceQuery<TEntity> targetDsSet)
        {
            this.queryType = typeof(RemoteRepository<>);
            query = targetDsSet;
        }

        #endregion

        #region Methods

        public IQueryable CreateQuery(Expression expression)
        {
            var elementType = expression.Type.FindElementType();
            try
            {
                return queryType.MakeGenericType(elementType
                                                ).New<IQueryable>(this, expression);
            }
            catch (TargetInvocationException tie)
            {
                throw tie.InnerException;
            }
        }

        public IQueryable<T> CreateQuery<T>(Expression expression)
        {
            return queryType.MakeGenericType(expression
                                            .Type
                                            .FindElementType()
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
            var treeCopier = new LinkedRepositoryExpressionVisitor(newRoot);
            var newExpressionTree = treeCopier.Visit(expression);
            var isEnumerable = (typeof(TResult).IsGenericType &&
                                typeof(TResult).GetGenericTypeDefinition() == typeof(IEnumerable<>));
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
