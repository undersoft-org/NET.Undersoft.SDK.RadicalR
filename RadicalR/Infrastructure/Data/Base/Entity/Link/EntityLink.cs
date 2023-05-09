using System.ComponentModel.DataAnnotations.Schema;

namespace RadicalR
{

    public class EntityLink<TLeft, TRight> : Entity, IEntityLink<TLeft, TRight> where TLeft : class, IIdentifiable where TRight : class, IIdentifiable
    {
        public virtual long RightEntityId { get; set; }

        public virtual TRight RightEntity { get; set; }

        public virtual long LeftEntityId { get; set; }

        public virtual TLeft LeftEntity { get; set; }
    }
}