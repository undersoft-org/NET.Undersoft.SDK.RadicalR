using System.Threading.Tasks;

namespace RadicalR
{
    public interface IEventHandler<in TEvent> : IEventHandler
    {
        Task HandleEventAsync(TEvent eventData);
    }

    public interface IEventHandler
    {

    }
}
