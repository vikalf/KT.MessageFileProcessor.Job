using KT.ActivityStream.Service.Definition;
using KT.MessageFileProcessor.Data.Models;
using KT.MessageFileProcessor.Data.Repositories.Definition;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static KT.ActivityStream.Service.Definition.ActivityStream;

namespace KT.MessageFileProcessor.Data.Repositories.Implementation
{
    public class ActivityStreamRepository : IActivityStreamRepository
    {
        private readonly ILogger<JobsRepository> _logger;
        private readonly ActivityStreamClient _activityStreamClient;

        public ActivityStreamRepository(ILogger<JobsRepository> logger, ActivityStreamClient activityStreamClient)
        {
            _logger = logger;
            _activityStreamClient = activityStreamClient;
        }

        public async Task<List<MessageType>> GetMessageTypes()
        {

            try
            {
                var reply = await _activityStreamClient.GetMessageTypesAsync(new EmptyRequest());

                return (from my in reply.MessageTypes.ToList()
                        select new MessageType
                        {
                            MessageTypeId = my.MessageTypeId,
                            Name = my.Name
                        }).ToList();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "GetMessageTypes()");
                throw;
            }

        }
    }
}
