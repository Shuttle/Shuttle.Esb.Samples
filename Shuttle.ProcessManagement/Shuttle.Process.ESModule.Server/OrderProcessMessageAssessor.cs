using Shuttle.ESB.Core;
using Shuttle.ESB.Process;

namespace Shuttle.Process.ESModule.Server
{
    public class OrderProcessMessageAssessor : IProcessMessageAssessor
    {
        public bool IsSatisfiedBy(PipelineEvent candidate)
        {
            return candidate.Pipeline.State.GetTransportMessage().IsHandledHere();
        }
    }
}