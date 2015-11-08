using System;

namespace Shuttle.ESB.Process
{
    public class ProcessException : Exception
    {
        public ProcessException(string message) : base(message)
        {
        }
    }
}