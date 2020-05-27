using KT.Framework.Common.Configuration.Definition;
using KT.MessageFileProcessor.Business.Components.Definition;
using KT.MessageFileProcessor.Business.Managers.Definition;
using KT.MessageFileProcessor.Data.Repositories.Definition;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace KT.MessageFileProcessor.Business.Managers.Implementation
{
    public class MessageFileProcessorManager : IMessageFileProcessorManager
    {
        private readonly ILogger<MessageFileProcessorManager> _logger;
        private readonly IEnvironmentSettings _environmentSettings;
        private readonly IJobsRepository _jobsRepository;
        private readonly IMessageFileProcessorComponent _messageFileProcessorComponent;

        public MessageFileProcessorManager(
            ILogger<MessageFileProcessorManager> logger,
            IEnvironmentSettings environmentSettings,
            IJobsRepository jobsRepository,
            IMessageFileProcessorComponent messageFileProcessorComponent)
        {
            _logger = logger;
            _environmentSettings = environmentSettings;
            _jobsRepository = jobsRepository;
            _messageFileProcessorComponent = messageFileProcessorComponent;
        }

        public async Task ProcessStudentEventsXmlFiles()
        {
            try
            {
                var jobName = _environmentSettings.GetSetting("STUDENT_EVENTS_XML_JOB_NAME");
                var job = await _jobsRepository.GetJobByName(jobName);

                if (job == null)
                    throw new ArgumentException($"Job '{jobName}' does not exist");

                if (job.Enabled)
                {
                    // Create JobHistory..
                    DateTime dateRun = DateTime.UtcNow;
                    var jobHistory = await _jobsRepository.AddJobHistory(job.JobID, dateRun);

                    _logger.LogInformation($"LogHistoryID : {jobHistory.JobHistoryID}");

                    try
                    {
                        // First Process the failed records in previous executions
                        _logger.LogInformation($"Getting JobRetryLogs for JobID: {job.JobID}..");

                        //var jobRetryLogs = await _jobsRepository.GetStudentEventRetryLogs(job.JobID);

                        //_logger.LogInformation($"JobRetryLogs found: {jobRetryLogs.Count}");
                        //if (jobRetryLogs.Any())
                        //    await _messageFileProcessorComponent.ProcessStudentEventsRetryLogs(jobHistory.JobHistoryID, jobRetryLogs);



                        // Now process new records..
                        await _messageFileProcessorComponent.ProcessStudentEventsXmlFiles(jobHistory.JobHistoryID);


                        await _jobsRepository.UpdateJobDateLastRun(job.JobID, dateRun);
                        await _jobsRepository.UpdateJobHistory(jobHistory.JobHistoryID, true, string.Empty);

                    }
                    catch (Exception ex)
                    {
                        var fileName = ex.Data["FileName"] ?? string.Empty;

                        await _jobsRepository.UpdateJobHistory(jobHistory.JobHistoryID, false, $"Error: {ex.Message} - FileName : {fileName}" );
                        throw;
                    }
                }
                else
                    _logger.LogWarning($"Process Student Events Xml Files Job is Disabled, JObID: {job.JobID}");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ProcessStudentEventsXmlFiles()");
            }
        }
    }
}
