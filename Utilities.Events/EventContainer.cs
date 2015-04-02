using System;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace Utilities.Events
{
    public class EventRegistry:Registry
    {
        public EventRegistry(IContainer container)
        {
            EventContainer.SetContainer(container);
        }
        public static void RegisterHandler<E, M>()
            where E : Event
            where M : IEventManager
        {
            EventContainer.Register<E, M>();
        }
    }

    internal static class EventContainer
    {
        private static IContainer _currentContainer;

        internal static void SetContainer(IContainer container)
        {
            _currentContainer = container;
        }

        internal static IEnumerable<IHandleEvent> GetHandlers(Type type)
        {
            return _currentContainer.GetAllInstances<IHandleEvent>().Where(h => h.CanHandle(type));
        }

        public static void Register<E, M>()
            where E : Event
            where M : IEventManager
        {
            var managerName = typeof (M).Name;
            _currentContainer.Configure(cfg => cfg.For<IEventManager>().Use<M>().Named(typeof(M).Name));

            var mgr = _currentContainer.GetInstance<IEventManager>(managerName);

            _currentContainer.Configure(cfg => cfg.For<IHandleEvent>()
                .Use<HandlesEvent<E>>()
                .Ctor<IEventManager>("eventManager")
                .Is(mgr));
        }
    }
}