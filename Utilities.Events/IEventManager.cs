namespace Utilities.Events
{
    public interface IEventManager
    {
        void Execute(Event @event);
    }
}