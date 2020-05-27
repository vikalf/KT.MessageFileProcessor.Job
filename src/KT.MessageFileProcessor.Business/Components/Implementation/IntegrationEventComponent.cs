using KT.Framework.Common.EventBus.Definition;
using KT.Framework.Common.EventBus.Events;
using KT.MessageFileProcessor.Business.Components.Definition;
using KT.MessageFileProcessor.Business.Models.Payloads;
using System.Linq;
using System;

namespace KT.MessageFileProcessor.Business.Components.Implementation
{
    public class IntegrationEventComponent : IIntegrationEventComponent
    {
        private readonly IEventBus _eventBus;

        public IntegrationEventComponent(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public void PublishStudentEvent(StudentEvent studentEvent, string eventName)
        {

            IntegrationEvent @event = (eventName.ToLower().Trim()) switch
            {
                "classenrollment" => MapClassEnrollmentIntegrationEvent(studentEvent),
                "classwithdrawal" => MapClassWithdrawalIntegrationEvent(studentEvent),
                "documentaccepted" => MapDocumentAcceptedIntegrationEvent(studentEvent),
                "documentpastdue" => MapDocumentPastDueIntegrationEvent(studentEvent),
                "documentreceived" => MapDocumentReceivedIntegrationEvent(studentEvent),
                "documentscheduled" => MapDocumentScheduledIntegrationEvent(studentEvent),
                "finalgradeposted" => MapFinalGradePostedIntegrationEvent(studentEvent),
                "gradeposted" => MapGradePostedIntegrationEvent(studentEvent),
                "termstart" => MapTermStartIntegrationEvent(studentEvent),
                "upcomingseminar" => MapUpcomingSeminarIntegrationEvent(studentEvent),
                _ => null,
            };


            if (@event != null)
            {
                _eventBus.Publish(@event);
            }

        }

        private UpcomingSeminarIntegrationEvent MapUpcomingSeminarIntegrationEvent(StudentEvent studentEvent) => new UpcomingSeminarIntegrationEvent(studentEvent.UserId, studentEvent.MessageTypeId, studentEvent.Published ?? DateTime.UtcNow)
        {
            CoursePrefix = studentEvent.Content.FirstOrDefault(e => e.Key.Equals("CoursePrefix", StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty,
            Section = studentEvent.Content.FirstOrDefault(e => e.Key.Equals("Section", StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty,
            StartDate = studentEvent.Content.FirstOrDefault(e => e.Key.Equals("StartDate", StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty
        };

        private TermStartIntegrationEvent MapTermStartIntegrationEvent(StudentEvent studentEvent) => new TermStartIntegrationEvent(studentEvent.UserId, studentEvent.MessageTypeId, studentEvent.Published ?? DateTime.UtcNow)
        {
            StartDate = studentEvent.Content.FirstOrDefault(e => e.Key.Equals("StartDate", StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty,
            TermDescrip = studentEvent.Content.FirstOrDefault(e => e.Key.Equals("TermDescrip", StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty
        };

        private GradePostedIntegrationEvent MapGradePostedIntegrationEvent(StudentEvent studentEvent) => new GradePostedIntegrationEvent(studentEvent.UserId, studentEvent.MessageTypeId, studentEvent.Published ?? DateTime.UtcNow)
        {
            Grade = studentEvent.Content.FirstOrDefault(e => e.Key.Equals("Grade", StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty,
            CoursePrefix = studentEvent.Content.FirstOrDefault(e => e.Key.Equals("CoursePrefix", StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty,
            GradeType = studentEvent.Content.FirstOrDefault(e => e.Key.Equals("GradeType", StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty,
            Points = studentEvent.Content.FirstOrDefault(e => e.Key.Equals("Points", StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty,
            PointsPossible = studentEvent.Content.FirstOrDefault(e => e.Key.Equals("PointsPossible", StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty
        };

        private FinalGradePostedIntegrationEvent MapFinalGradePostedIntegrationEvent(StudentEvent studentEvent) => new FinalGradePostedIntegrationEvent(studentEvent.UserId, studentEvent.MessageTypeId, studentEvent.Published ?? DateTime.UtcNow)
        {
            CoursePrefix = studentEvent.Content.FirstOrDefault(e => e.Key.Equals("CoursePrefix", StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty,
            Section = studentEvent.Content.FirstOrDefault(e => e.Key.Equals("Section", StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty
        };

        private DocumentScheduledIntegrationEvent MapDocumentScheduledIntegrationEvent(StudentEvent studentEvent) => new DocumentScheduledIntegrationEvent(studentEvent.UserId, studentEvent.MessageTypeId, studentEvent.Published ?? DateTime.UtcNow)
        {
            DocumentName = studentEvent.Content.FirstOrDefault(e => e.Key.Equals("DocumentName", StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty,
            DueDate = studentEvent.Content.FirstOrDefault(e => e.Key.Equals("DueDate", StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty
        };

        private DocumentReceivedIntegrationEvent MapDocumentReceivedIntegrationEvent(StudentEvent studentEvent) => new DocumentReceivedIntegrationEvent(studentEvent.UserId, studentEvent.MessageTypeId, studentEvent.Published ?? DateTime.UtcNow)
        {
            DocumentName = studentEvent.Content.FirstOrDefault(e => e.Key.Equals("DocumentName", StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty
        };

        private DocumentPastDueIntegrationEvent MapDocumentPastDueIntegrationEvent(StudentEvent studentEvent) => new DocumentPastDueIntegrationEvent(studentEvent.UserId, studentEvent.MessageTypeId, studentEvent.Published ?? DateTime.UtcNow)
        {
            DocumentName = studentEvent.Content.FirstOrDefault(e => e.Key.Equals("DocumentName", StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty,
            DueDate = studentEvent.Content.FirstOrDefault(e => e.Key.Equals("DueDate", StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty
        };

        private DocumentAcceptedIntegrationEvent MapDocumentAcceptedIntegrationEvent(StudentEvent studentEvent) => new DocumentAcceptedIntegrationEvent(studentEvent.UserId, studentEvent.MessageTypeId, studentEvent.Published ?? DateTime.UtcNow)
        {
            DocumentName = studentEvent.Content.FirstOrDefault(e => e.Key.Equals("DocumentName", StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty,
        };

        private ClassEnrollmentIntegrationEvent MapClassEnrollmentIntegrationEvent(StudentEvent studentEvent) => new ClassEnrollmentIntegrationEvent(studentEvent.UserId, studentEvent.MessageTypeId, studentEvent.Published ?? DateTime.UtcNow)
        {
            CoursePrefix = studentEvent.Content.FirstOrDefault(e => e.Key.Equals("CoursePrefix", StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty,
            Section = studentEvent.Content.FirstOrDefault(e => e.Key.Equals("Section", StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty,
            StudentName = studentEvent.Content.FirstOrDefault(e => e.Key.Equals("StudentName", StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty
        };

        private ClassWithdrawalIntegrationEvent MapClassWithdrawalIntegrationEvent(StudentEvent studentEvent) => new ClassWithdrawalIntegrationEvent(studentEvent.UserId, studentEvent.MessageTypeId, studentEvent.Published ?? DateTime.UtcNow)
        {
            CoursePrefix = studentEvent.Content.FirstOrDefault(e => e.Key.Equals("CoursePrefix", StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty,
            Section = studentEvent.Content.FirstOrDefault(e => e.Key.Equals("Section", StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty,
            StudentName = studentEvent.Content.FirstOrDefault(e => e.Key.Equals("StudentName", StringComparison.InvariantCultureIgnoreCase))?.Value ?? string.Empty
        };

    }
}
