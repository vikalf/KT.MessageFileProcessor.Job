using System.Collections.Generic;
using System.Threading.Tasks;

namespace KT.MessageFileProcessor.Data.Repositories.Definition
{
    public interface IActivityStreamRepository
    {
        Task<List<Models.MessageType>> GetMessageTypes();
    }
}
