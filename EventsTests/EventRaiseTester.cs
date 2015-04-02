using System; 
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using Utilities.Events;

namespace EventsTests
{
    [TestClass]
    public class EventRaiseTester
    {
        [TestInitialize]
        public void setup()
        {
            var container = new Container();
            container.Configure(cfg =>
            {
                cfg.AddRegistry(new EventRegistry(container));
            });

            EventRegistry.RegisterHandler<TestEvent, TestEventManager>();
        }
        [TestMethod]
        public void CanRaiseEvent()
        {
            var evnt = new TestEvent {MyName = "John Jacob Jingleheimer Schmidt"};

            evnt.Raise();
        }

        internal class TestEvent : Event
        {
            internal string MyName { get; set; }
        }

        internal class TestEventManager : IEventManager
        {
            public void Execute(Event @event)
            {
                var tst = (TestEvent) @event;
                Console.WriteLine(tst.MyName);
            }
        }
    }
}
