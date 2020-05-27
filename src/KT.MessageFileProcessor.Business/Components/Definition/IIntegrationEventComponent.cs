using KT.MessageFileProcessor.Business.Models.Payloads;

namespace KT.MessageFileProcessor.Business.Components.Definition
{
    public interface IIntegrationEventComponent
    {
        void PublishStudentEvent(StudentEvent studentEvent, string eventName);
    }
}
