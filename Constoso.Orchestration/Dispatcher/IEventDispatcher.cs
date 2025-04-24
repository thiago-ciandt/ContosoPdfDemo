namespace Constoso.Orchestration.Dispatcher
{
    public interface IEventDispatcher
    {
        void Publish<T>(T evt);
        void Subscribe<T>(Action<T> handler);
    }
}
