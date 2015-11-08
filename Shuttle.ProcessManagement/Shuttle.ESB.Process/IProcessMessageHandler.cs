using Shuttle.ESB.Core;
using Shuttle.Recall.Core;

namespace Shuttle.ESB.Process
{
    public interface IProcessMessageHandler<T> where T : class
    {
        void ProcessMessage(HandlerContext<T> context, EventStream stream);
    }
}