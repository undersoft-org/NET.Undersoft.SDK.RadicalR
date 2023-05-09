namespace RadicalR
{
    public interface IObjectAccessor<out T>
    {

        T Value { get; }
    }

    public interface IObjectAccessor
    {

        object Value { get; }
    }
}