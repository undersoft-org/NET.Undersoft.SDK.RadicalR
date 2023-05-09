using System.Linq;
using System.Linq.Expressions;

namespace RadicalR
{
    internal class HostRepositoryExpressionVisitor : ExpressionVisitor
    {
        private readonly IQueryable newRoot;

        public HostRepositoryExpressionVisitor(IQueryable newRoot)
        {
            this.newRoot = newRoot;
        }

        protected override Expression VisitConstant(ConstantExpression node) =>
             node.Type.BaseType != null && 
             node.Type.BaseType.IsGenericType && 
             node.Type.BaseType.GetGenericTypeDefinition() == typeof(EntityRepository<>) ? 
             Expression.Constant(newRoot) : node;

    }
}
