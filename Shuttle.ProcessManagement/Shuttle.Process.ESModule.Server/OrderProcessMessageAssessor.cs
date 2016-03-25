using Shuttle.Core.Infrastructure;
using Shuttle.Esb;
using Shuttle.Esb.Process;

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