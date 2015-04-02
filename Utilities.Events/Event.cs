using System.Text;
using System.Threading.Tasks;

namespace Utilities.Events
{
    public class Event
    {
        public void Raise()
        {
            var handlers = EventContainer.GetHandlers(this.GetType());
            foreach (var h in handlers)
            {
                h.Handle(this);
            }
        }
    }
}
