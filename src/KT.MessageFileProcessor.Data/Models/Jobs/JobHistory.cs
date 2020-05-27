using System;

namespace KT.MessageFileProcessor.Data.Models.Jobs
{
    public class JobHistory
    {
        public int JobHistoryID { get; set; }
        public int JobID { get; set; }
        public DateTime DateRun { get; set; }
        public bool Completed { get; set; }
        public string ErrorMessage { get; set; }
    }
}
