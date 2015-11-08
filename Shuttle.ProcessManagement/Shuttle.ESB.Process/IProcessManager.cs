using System;

namespace Shuttle.ESB.Process
{
    public interface IProcessManager
    {
        Guid CorrelationId { get; } 
    }
}