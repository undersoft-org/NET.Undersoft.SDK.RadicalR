namespace RadicalR
{
    public class ObjectAccessor<T> : ObjectAccessor, IObjectAccessor<T>
    {
        public new T Value { get; set; }

        public ObjectAccessor()
        {            
        }

        public ObjectAccessor(T obj)
        {
            Value = obj;
        }
    }

    public class ObjectAccessor : IObjectAccessor
    {
        public object Value { get; set; }

        public ObjectAccessor()
        {
        }

        public ObjectAccessor(object obj)
        {
            Value = obj;
        }
    }
}