﻿// Copyright 2007-2016 Chris Patterson, Dru Sellers, Travis Smith, et. al.
//  
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//     http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software distributed
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.
namespace MassTransit.RabbitMqTransport
{
    using System;
    using System.Collections.Generic;


    public static class RabbitMqSendContextExtensions
    {
        public static void SetTransportHeader(this RabbitMqSendContext context, string key, object value)
        {
            if (context.BasicProperties.Headers == null)
                context.BasicProperties.Headers = new Dictionary<string, object>();

            context.BasicProperties.Headers[key] = value;
        }

        /// <summary>
        /// Sets the priority of a message sent to the broker
        /// </summary>
        /// <param name="context"></param>
        /// <param name="priority"></param>
        public static void SetPriority(this SendContext context, byte priority)
        {
            RabbitMqSendContext sendContext;
            if (!context.TryGetPayload(out sendContext))
                throw new ArgumentException("The RabbitMqSendContext was not available");

            sendContext.BasicProperties.Priority = priority;
        }

        /// <summary>
        /// Sets whether the send should wait for the ack from the broker, or if it should 
        /// return immediately after the message is sent by the client.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="awaitAck"></param>
        public static void SetAwaitAck(this SendContext context, bool awaitAck)
        {
            RabbitMqSendContext sendContext;
            if (!context.TryGetPayload(out sendContext))
                throw new ArgumentException("The RabbitMqSendContext was not available");

            sendContext.AwaitAck = awaitAck;
        }
    }
}