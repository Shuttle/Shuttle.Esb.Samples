﻿using System;
using Shuttle.Core.Contract;
using Shuttle.Core.Data;
using Shuttle.EMailSender.Messages;
using Shuttle.Esb;
using Shuttle.Process.Recall.Server.Domain;
using Shuttle.ProcessManagement.Messages;
using Shuttle.Recall;

namespace Shuttle.Process.Recall.Server.Handlers;

public class EMailSentHandler : IMessageHandler<EMailSent>
{
    private readonly IDatabaseContextFactory _databaseContextFactory;
    private readonly IEventStore _eventStore;

    public EMailSentHandler(IDatabaseContextFactory databaseContextFactory, IEventStore eventStore)
    {
        Guard.AgainstNull(databaseContextFactory, nameof(databaseContextFactory));
        Guard.AgainstNull(eventStore, nameof(eventStore));

        _databaseContextFactory = databaseContextFactory;
        _eventStore = eventStore;
    }

    public void ProcessMessage(IHandlerContext<EMailSent> context)
    {
        if (!context.TransportMessage.IsHandledHere())
        {
            return;
        }

        var orderProcessId = new Guid(context.TransportMessage.CorrelationId);

        var stream = _eventStore.Get(orderProcessId);

        if (stream.IsEmpty)
        {
            throw new ApplicationException(
                $"Could not find an order process with correlation id '{context.TransportMessage.CorrelationId}'.");
        }

        var orderProcess = new OrderProcess(orderProcessId);
        stream.Apply(orderProcess);

        stream.AddEvent(orderProcess.ChangeStatus("Dispatched-EMail Sent"));

        _eventStore.Save(stream);

        context.Send(new CompleteOrderProcess
        {
            OrderProcessId = orderProcessId
        }, c => c.Local());
    }
}