using System.Threading.Tasks;

namespace KT.MessageFileProcessor.Business.Managers.Definition
{
    public interface IMessageFileProcessorManager
    {
        Task ProcessStudentEventsXmlFiles();
    }
}
