using Shuttle.Core.Pipelines;
using Shuttle.Esb;
using Shuttle.Esb.Process;

namespace Shuttle.Process.ESModule.Server
{
    public class OrderProcessMessageAssessor : IProcessMessageAssessor
    {
        public bool IsSatisfiedBy(IPipelineEvent candidate)
        {
            return candidate.Pipeline.State.GetTransportMessage().IsHandledHere();
        }
    }
}