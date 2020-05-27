using KT.MessageFileProcessor.Business.Models.Payloads;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace KT.MessageFileProcessor.Business.Components.Definition
{
    public interface IXmlFileProcessor
    {
        Task<List<StudentEvent>> GetStudentEventsFromXmlStream(string fileName, Stream blob);
    }
}
