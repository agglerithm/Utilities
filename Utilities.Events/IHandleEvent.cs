using System;

namespace Utilities.Events
{
    public interface IHandleEvent 
    {
        bool CanHandle(Type type);
        void Handle(Event @event);
    }

    public class HandlesEvent<T> : IHandleEvent where T : Event
    {
        private readonly IEventManager _eventManager;

        public HandlesEvent(IEventManager eventManager)
        {
            _eventManager = eventManager;
        }

        public bool CanHandle(Type type)
        {
            return typeof (T) == type;
        }

        public void Handle(Event @event)
        {
            _eventManager.Execute(@event);
        }
    }
}