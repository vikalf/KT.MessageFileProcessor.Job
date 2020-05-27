using System.Threading.Tasks;

namespace KT.MessageFileProcessor.Business.Components.Definition
{
    public interface IMessageFileProcessorComponent
    {
        Task ProcessStudentEventsXmlFiles(int jobHistoryID);
        //Task ProcessStudentEventsRetryLogs(int jobHistoryID, List<JobRetryLog<StudentEvent>> jobRetryLogs);

    }
}
