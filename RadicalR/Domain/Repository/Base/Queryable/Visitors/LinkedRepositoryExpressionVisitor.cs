using System.Linq;
using System.Linq.Expressions;

namespace RadicalR
{
    internal class LinkedRepositoryExpressionVisitor : ExpressionVisitor
    {
        #region Fields

        private readonly IQueryable newRoot;

        #endregion

        #region Constructors

        public LinkedRepositoryExpressionVisitor(IQueryable newRoot)
        {
            this.newRoot = newRoot;
        }

        #endregion

        #region Methods

        protected override Expression VisitConstant(ConstantExpression node) =>
             node.Type.BaseType != null && 
             node.Type.BaseType.IsGenericType && 
             node.Type.BaseType.GetGenericTypeDefinition() == typeof(RemoteRepository<>) ? 
             Expression.Constant(newRoot) : node;

        #endregion
    }
}
