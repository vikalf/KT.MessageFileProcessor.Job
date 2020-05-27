namespace KT.MessageFileProcessor.Data.Models.Jobs
{
    public class JobRetryLog
    {
        public int JobRetryLogID { get; set; }
        public int JobHistoryID { get; set; }
        public string Payload { get; set; }
        public bool Completed { get; set; }
        public int RetryCount { get; set; }
    }
}
