namespace Constoso.Orchestration.Dispatcher
{
    public class InMemoryEventDispatcher : IEventDispatcher
    {
        private readonly Dictionary<Type, List<Delegate>> _handlers = new();

        public void Publish<T>(T evt)
        {
            var type = typeof(T);
            if (_handlers.ContainsKey(type))
            {
                foreach (var handler in _handlers[type].OfType<Action<T>>())
                    Task.Run(() => handler(evt));
            }
        }

        public void Subscribe<T>(Action<T> handler)
        {
            var type = typeof(T);
            if (!_handlers.ContainsKey(type))
                _handlers[type] = new List<Delegate>();

            _handlers[type].Add(handler);
        }
    }
}
