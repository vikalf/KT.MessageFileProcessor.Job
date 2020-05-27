using System.Collections.Generic;

namespace KT.MessageFileProcessor.Business.Models.Payloads
{
    public class StudentEventPayload
    {
        public string FileName { get; set; }
        public List<StudentEvent> StudentEvents { get; set; }
    }
}
