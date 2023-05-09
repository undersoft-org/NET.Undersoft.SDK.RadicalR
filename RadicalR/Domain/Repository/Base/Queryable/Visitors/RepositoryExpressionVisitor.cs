using System.Linq;
using System.Linq.Expressions;

namespace RadicalR
{
    internal class RepositoryExpressionVisitor : ExpressionVisitor
    {
        #region Fields

        private readonly IQueryable newRoot;

        #endregion

        #region Constructors

        public RepositoryExpressionVisitor(IQueryable newRoot)
        { this.newRoot = newRoot;
        }

        #endregion

        #region Methods

        protected override Expression VisitConstant(ConstantExpression node) =>
             node.Type.BaseType != null && 
             node.Type.BaseType.IsGenericType && 
             node.Type.BaseType.GetGenericTypeDefinition() == typeof(Repository<>) ? 
                 Expression.Constant(newRoot) : node;

        #endregion
    }
}
