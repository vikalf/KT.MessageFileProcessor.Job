using System;

namespace KT.MessageFileProcessor.Data.Models.Jobs
{
    public class Job
    {
        public int JobID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Enabled { get; set; }
        public DateTime? DateLastRun { get; set; }
        public int MaxRetries { get; set; }
    }
}
