using Shuttle.Core.Logging;
using Shuttle.Esb;
using Shuttle.Deferred.Messages;

namespace Shuttle.Deferred.Server
{
	public class RegisterMemberHandler : IMessageHandler<RegisterMemberCommand>
	{
	    private readonly ILog _log;

	    public RegisterMemberHandler()
	    {
	        _log = Log.For(this);
	    }

	    public void ProcessMessage(IHandlerContext<RegisterMemberCommand> context)
		{
		    _log.Information($"[MEMBER REGISTERED] : user name = '{context.Message.UserName}'");
		}
	}
}