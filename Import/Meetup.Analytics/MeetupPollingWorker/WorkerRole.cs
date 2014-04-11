using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Meetup.Analytics.Services.Models;
using System.Xml.Serialization;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using Microsoft.ServiceBus.Messaging;

namespace MeetupPollingWorker
{
    public class WorkerRole : RoleEntryPoint
    {
        string _connectionString = string.Empty;
        string _serviceBusConnectionString = string.Empty;
        string _queueName = string.Empty;
        QueueClient DataCollectionClient = default(QueueClient);

        public override void Run()
        {
            // This is a sample worker implementation. Replace with your logic.
            Trace.TraceInformation("MeetupPollingWorker entry point called", "Information");

            Trace.TraceInformation("Processing messages...", "Information");
            while (true)
            {
                var cities = GetMessage(_connectionString, _queueName);

                if (cities != null)
                {
                 

                            DataCollectionClient.Send(new BrokeredMessage(cities));
                        
                    
                    
                    //Console.WriteLine(string.Format("Meetup city poll request sent: {0}, {1}", city.city, city.country));

                }

                Thread.Sleep(10000);
                Trace.TraceInformation("Working", "Information");
            }
        }

        public CloudQueue GetQueue(string connectionstring, string queueName)
        {
            // Create a queue client for the give storage account
            var storageAccount = CloudStorageAccount.Parse(connectionstring);
            var queueClient = storageAccount.CreateCloudQueueClient();

            // Get a reference to the queue with the given name.
            var queue = queueClient.GetQueueReference(queueName);

            // If the queue doesn't exist, this will create it with the given name.
            queue.CreateIfNotExists();
            return queue;
        }

        public List<MeetupCity> GetMessage(string connectionstring, string queueName)
        {
            var queue = GetQueue(connectionstring, queueName);
            var message = queue.GetMessage();
            if (message == null) return null;

            StorageQueueMessage storageQueueMessage = null;

            XmlSerializer serializer = new XmlSerializer(typeof(StorageQueueMessage));

            MemoryStream memoryStream = new MemoryStream(Encoding.Unicode.GetBytes(message.AsString));
            storageQueueMessage = (StorageQueueMessage)serializer.Deserialize(memoryStream);
            memoryStream.Close();

            queue.DeleteMessage(message);

            // Deserialize the MeetupCity JSON
            string meetupCityJson = storageQueueMessage.Message;
            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<MeetupCity>>(meetupCityJson);
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            var connectionstring = CloudConfigurationManager.GetSetting("DataConnectionString");

            // Create service bus messages for each city
            string serviceBusConnectionString = CloudConfigurationManager.GetSetting("Microsoft.ServiceBus.ConnectionString");

            _connectionString = connectionstring;
            _serviceBusConnectionString = serviceBusConnectionString;

            // Retrieve service bus client for meetup-polling queue
            DataCollectionClient = QueueClient.CreateFromConnectionString(_serviceBusConnectionString, "meetup-polling");

            _queueName = "meetup-queue";
            GetQueue(_connectionString, _queueName);

            return base.OnStart();
        }
    }
}
