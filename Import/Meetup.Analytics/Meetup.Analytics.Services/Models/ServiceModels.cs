using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meetup.Analytics.Services.Models
{
    public class Organizer
    {
        public string name { get; set; }
        public int member_id { get; set; }
    }

    public class Category
    {
        public int id { get; set; }
        public string name { get; set; }
        public string shortname { get; set; }
    }

    public class Topic
    {
        public int id { get; set; }
        public string urlkey { get; set; }
        public string name { get; set; }
    }

    public class GroupPhoto
    {
        public string photo_link { get; set; }
        public string highres_link { get; set; }
        public string thumb_link { get; set; }
        public int photo_id { get; set; }
    }

    public class Result
    {
        public double lon { get; set; }
        public string visibility { get; set; }
        public Organizer organizer { get; set; }
        public string link { get; set; }
        public string state { get; set; }
        public string join_mode { get; set; }
        public string who { get; set; }
        public string country { get; set; }
        public string city { get; set; }
        public int id { get; set; }
        public Category category { get; set; }
        public List<Topic> topics { get; set; }
        public string timezone { get; set; }
        public GroupPhoto group_photo { get; set; }
        public long created { get; set; }
        public string description { get; set; }
        public string name { get; set; }
        public double rating { get; set; }
        public string urlname { get; set; }
        public double lat { get; set; }
        public int members { get; set; }
    }

    public class Meta
    {
        public double lon { get; set; }
        public int count { get; set; }
        public string signed_url { get; set; }
        public string link { get; set; }
        public string next { get; set; }
        public int total_count { get; set; }
        public string url { get; set; }
        public string id { get; set; }
        public string title { get; set; }
        public long updated { get; set; }
        public string description { get; set; }
        public string method { get; set; }
        public double lat { get; set; }
    }

    public class MeetupGroups
    {
        public List<Result> results { get; set; }
        public Meta meta { get; set; }
    }

    public class MeetupCity
    {
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
    }


    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class StorageQueueMessage
    {

        private string executionTagField;

        private string clientRequestIdField;

        private System.DateTime expectedExecutionTimeField;

        private string schedulerJobIdField;

        private string schedulerJobcollectionIdField;

        private string regionField;

        private string messageField;

        /// <remarks/>
        public string ExecutionTag
        {
            get
            {
                return this.executionTagField;
            }
            set
            {
                this.executionTagField = value;
            }
        }

        /// <remarks/>
        public string ClientRequestId
        {
            get
            {
                return this.clientRequestIdField;
            }
            set
            {
                this.clientRequestIdField = value;
            }
        }

        /// <remarks/>
        public System.DateTime ExpectedExecutionTime
        {
            get
            {
                return this.expectedExecutionTimeField;
            }
            set
            {
                this.expectedExecutionTimeField = value;
            }
        }

        /// <remarks/>
        public string SchedulerJobId
        {
            get
            {
                return this.schedulerJobIdField;
            }
            set
            {
                this.schedulerJobIdField = value;
            }
        }

        /// <remarks/>
        public string SchedulerJobcollectionId
        {
            get
            {
                return this.schedulerJobcollectionIdField;
            }
            set
            {
                this.schedulerJobcollectionIdField = value;
            }
        }

        /// <remarks/>
        public string Region
        {
            get
            {
                return this.regionField;
            }
            set
            {
                this.regionField = value;
            }
        }

        /// <remarks/>
        public string Message
        {
            get
            {
                return this.messageField;
            }
            set
            {
                this.messageField = value;
            }
        }
    }
}
