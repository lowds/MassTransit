﻿namespace MassTransit.RabbitMqTransport.Tests
{
    using System;
    using NUnit.Framework;
    using RabbitMqTransport;
    using TestFramework.Fixtures;


    
	public class Given_a_rabbitmq_bus_with_vhost_mt_and_credentials :
		LocalTestFixture<RabbitMqTransportFactory>
	{
		protected Given_a_rabbitmq_bus_with_vhost_mt_and_credentials()
		{
            LocalUri = new Uri("rabbitmq://testUser:topSecret@localhost:5672/mttest/test_queue");
			ConfigureEndpointFactory(x => x.UseJsonSerializer());
		}

		protected override void ConfigureServiceBus(Uri uri, BusConfigurators.ServiceBusConfigurator configurator)
		{
			base.ConfigureServiceBus(uri, configurator);
		}
	}
}