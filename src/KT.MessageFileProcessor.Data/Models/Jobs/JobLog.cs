namespace KT.MessageFileProcessor.Data.Models.Jobs
{
    public class JobLog
    {
        public int JobLogID { get; set; }
        public int JobHistoryID { get; set; }
        public string Payload { get; set; }
        public bool Completed { get; set; }
        public string ErrorMessage { get; set; }
    }
}
