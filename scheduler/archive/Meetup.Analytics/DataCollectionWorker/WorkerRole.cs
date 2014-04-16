using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.ServiceRuntime;
using Meetup.Analytics.Services.Models;
using Meetup.Analytics.Services.Implementations;

namespace DataCollectionWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        // The name of your queue
        const string QueueName = "meetup-polling";

        // QueueClient is thread-safe. Recommended that you cache 
        // rather than recreating it on every request
        QueueClient Client;
        ManualResetEvent CompletedEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.WriteLine("Starting processing of messages");

            // Initiates the message pump and callback is invoked for each message that is received, calling close on the client will stop the pump.
            Client.OnMessage((receivedMessage) =>
                {
                    try
                    {
                        // Process the message
                        Trace.WriteLine("Processing Service Bus message: " + receivedMessage.SequenceNumber.ToString());

                        // Receive the message
                        var meetupCity = receivedMessage.GetBody<List<MeetupCity>>();

                        // Populated the graph database using LOAD CSV from Meetup.com API
                        Import.PopulateGraphDatabaseFromCsv(new MeetupClient("4b1b7a4c4027718192d1e73723135b").GetMeetupCityCsvByTagAndInterpolate(meetupCity, "NoSQL"));

                        // Trace to the log
                        //Trace.WriteLine("Data imported for: " + "NoSQL " + meetupCity.city);
                    }
                    catch (Exception ex)
                    {
                        // Handle any message processing specific exceptions here
                        Console.WriteLine(ex.Message);
                    }
                });

            CompletedEvent.WaitOne();
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // Create the queue if it does not exist already
            string connectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");
            var namespaceManager = NamespaceManager.CreateFromConnectionString(connectionString);
            if (!namespaceManager.QueueExists(QueueName))
            {
                namespaceManager.CreateQueue(QueueName);
            }

            // Initialize the connection to Service Bus Queue
            Client = QueueClient.CreateFromConnectionString(connectionString, QueueName);
            return base.OnStart();
        }

        public override void OnStop()
        {
            // Close the connection to Service Bus Queue
            Client.Close();
            CompletedEvent.Set();
            base.OnStop();
        }
    }
}
