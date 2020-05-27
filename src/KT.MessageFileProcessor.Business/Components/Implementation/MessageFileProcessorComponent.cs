using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using KT.Framework.Common.Configuration.Definition;
using KT.Framework.Common.EventBus.Definition;
using KT.MessageFileProcessor.Business.Components.Definition;
using KT.MessageFileProcessor.Business.Models.Payloads;
using KT.MessageFileProcessor.Data.Models;
using KT.MessageFileProcessor.Data.Repositories.Definition;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KT.MessageFileProcessor.Business.Components.Implementation
{
    public class MessageFileProcessorComponent : IMessageFileProcessorComponent
    {

        private readonly ILogger<MessageFileProcessorComponent> _logger;
        private readonly IJobsRepository _jobsRepository;
        private readonly IActivityStreamRepository _activityStreamRepository;
        private readonly IXmlFileProcessor _xmlFileProcessor;
        private readonly IIntegrationEventComponent _integrationEventComponent;

        private readonly BlobContainerClient _filesBlobContainerClient;
        private readonly BlobContainerClient _processedBlobContainerClient;

        public MessageFileProcessorComponent(
            ILogger<MessageFileProcessorComponent> logger,
            IEnvironmentSettings environmentSettings,
            IJobsRepository jobsRepository,
            IActivityStreamRepository activityStreamRepository,
            IXmlFileProcessor xmlFileProcessor,
            IIntegrationEventComponent integrationEventComponent,
            BlobServiceClient blobServiceClient)
        {
            _logger = logger;
            _jobsRepository = jobsRepository;
            _activityStreamRepository = activityStreamRepository;
            _xmlFileProcessor = xmlFileProcessor;
            _integrationEventComponent = integrationEventComponent;
            _filesBlobContainerClient = blobServiceClient.GetBlobContainerClient(environmentSettings.GetSetting("FILES_BLOB_CONTAINER_NAME", "files"));
            _processedBlobContainerClient = blobServiceClient.GetBlobContainerClient(environmentSettings.GetSetting("PROCESSED_BLOB_CONTAINER_NAME", "processed"));
        }

        public async Task ProcessStudentEventsXmlFiles(int jobHistoryID)
        {

            var fileNames = await GetBlobFileNamesFromAzureStorage();
            var messageTypes = await _activityStreamRepository.GetMessageTypes();

            foreach (var fileName in fileNames)
            {
                var studentEvents = new List<StudentEvent>();

                try
                {
                    var blob = await _filesBlobContainerClient.GetBlobClient(fileName).DownloadAsync();
                    studentEvents = await _xmlFileProcessor.GetStudentEventsFromXmlStream(fileName, blob.Value.Content);
                    await PublishStudentEvents(studentEvents, messageTypes, jobHistoryID);
                    await MoveFileToProcessed(fileName);

                }
                catch (System.Exception ex)
                {
                    ex.Data.Add("FileName", fileName);
                    throw;
                }
            }
        }

        private async Task PublishStudentEvents(List<StudentEvent> studentEvents, List<MessageType> messageTypes, int jobHistoryID)
        {
            foreach (var studentEvent in studentEvents)
            {
                var errorMessage = "";
                var success = false;

                try
                {
                    var eventName = messageTypes.FirstOrDefault(e => e.MessageTypeId == studentEvent.MessageTypeId)?.Name ?? string.Empty;

                    _logger.LogInformation($"Publish {eventName} Event, UserId: {studentEvent.UserId}");

                    _integrationEventComponent.PublishStudentEvent(studentEvent, eventName);
                    success = true;

                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "PublishStudentEvent({studentEvent})", studentEvent.ToJsonString());
                    await _jobsRepository.AddJobRetryLog(jobHistoryID, studentEvent.ToJsonString());
                    errorMessage = ex.Message;
                }

                await _jobsRepository.AddJobLog(jobHistoryID, studentEvent.ToJsonString(), success, errorMessage);
            }
        }

        private async Task<List<string>> GetBlobFileNamesFromAzureStorage()
        {
            var files = new List<string>();
            var blobs = _filesBlobContainerClient.GetBlobsAsync();
            await foreach (BlobItem blobItem in blobs)
            {
                if (blobItem.Properties.ContentType.Contains("xml"))
                    files.Add(blobItem.Name);
            }

            return files;
        }

        private async Task<bool> MoveFileToProcessed(string fileName)
        {
            try
            {
                var fileBlobClient = _filesBlobContainerClient.GetBlobClient(fileName);
                var processedBlobClient = _processedBlobContainerClient.GetBlobClient(fileName);

                if (await fileBlobClient.ExistsAsync())
                {
                    var fileStream = await fileBlobClient.DownloadAsync();

                    if (!await processedBlobClient.ExistsAsync())
                        await processedBlobClient.UploadAsync(fileStream.Value.Content);

                    await fileBlobClient.DeleteIfExistsAsync(DeleteSnapshotsOption.IncludeSnapshots);

                    return true;
                }
                else
                    return false;

            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "MoveFileToProcessed({fileName})", fileName);
                throw;
            }
        }


        //public Task ProcessStudentEventsRetryLogs(int jobHistoryID, List<JobRetryLog<StudentEvent>> jobRetryLogs)
        //{
        //    throw new System.NotImplementedException();
        //}

    }
}
