﻿using Shuttle.Esb;
using Shuttle.StreamProcessing.Messages;

namespace Shuttle.StreamProcessing.Consumer;

public class TemperatureReadHandler : IMessageHandler<TemperatureRead>
{
    public void ProcessMessage(IHandlerContext<TemperatureRead> context)
    {
        Console.WriteLine($"[TEMPERATURE READ] : name = '{context.Message.Name}' / minute = {context.Message.Minute} / celsius = {context.Message.Celsius:F}");
    }
}