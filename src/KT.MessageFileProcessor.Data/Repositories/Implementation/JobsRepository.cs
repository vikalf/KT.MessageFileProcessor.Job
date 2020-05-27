using AutoMapper;
using Grpc.Core;
using KT.Jobs.Service.Definition;
using KT.MessageFileProcessor.Data.Models.Jobs;
using KT.MessageFileProcessor.Data.Repositories.Definition;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KT.MessageFileProcessor.Data.Repositories.Implementation
{
    public class JobsRepository : IJobsRepository
    {

        private readonly ILogger<JobsRepository> _logger;
        private readonly Jobs.Service.Definition.Jobs.JobsClient _jobsClient;
        private readonly IMapper _mapper;

        public JobsRepository(ILogger<JobsRepository> logger, Jobs.Service.Definition.Jobs.JobsClient jobsClient, IMapper mapper)
        {
            _logger = logger;
            _jobsClient = jobsClient;
            _mapper = mapper;
        }

        public async Task<JobHistory> AddJobHistory(int jobID, DateTime dateRun)
        {
            try
            {

                var reply = await _jobsClient.AddJobHistoryAsync(new AddJobHistoryRequest
                {
                    DateRun = dateRun.ToString("g"),
                    JobID = jobID
                });

                return _mapper.Map<JobHistoryModel, Models.Jobs.JobHistory>(reply.JobHistory);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error AddJobHistory()");
                throw;
            }
        }

        public async Task<JobLog> AddJobLog(int jobHistoryID, string payload, bool completed, string errorMessage)
        {
            try
            {

                var reply = await _jobsClient.AddJobLogAsync(new AddJobLogRequest
                {
                    Completed = completed,
                    ErrorMessage = errorMessage,
                    JobHistoryID = jobHistoryID,
                    Payload = payload
                });

                return _mapper.Map<JobLogModel, Models.Jobs.JobLog>(reply.JobLog);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error AddJobLog()");
                throw;
            }

        }

        public async Task<List<JobLog>> GetJobLogsByJobHistoryID(int jobHistoryID)
        {
            try
            {
                _logger.LogTrace("GetJobLogsByJobHistoryID({jobHistoryID})", jobHistoryID);

                var reply = await _jobsClient.GetJobLogsByJobHistoryIDAsync(new JobLogsByJobHistoryIDRequest
                {
                    JobHistoryID = jobHistoryID
                });

                var response = _mapper.Map<List<JobLogModel>, List<Models.Jobs.JobLog>>(reply.JobLogs.ToList());

                return response;

            }
            catch (Exception ex)
            {

                if ((ex as RpcException)?.Status.StatusCode == StatusCode.NotFound || (ex.InnerException as RpcException)?.Status.StatusCode == StatusCode.NotFound)
                {
                    _logger.LogWarning("NOT FOUND GetJobLogsByJobHistoryID({jobHistoryID})", jobHistoryID);
                    return null;
                }
                else
                {
                    _logger.LogError(ex, "Error GetJobLogsByJobHistoryID({jobHistoryID})", jobHistoryID);
                    throw;
                }
            }
        }

        public async Task<JobRetryLog> AddJobRetryLog(int jobHistoryID, string payload)
        {
            try
            {

                var reply = await _jobsClient.AddJobRetryLogAsync(new AddJobRetryLogRequest
                {
                    JobHistoryID = jobHistoryID,
                    Payload = payload
                });

                return _mapper.Map<JobRetryLogModel, Models.Jobs.JobRetryLog>(reply.JobRetryLog);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error AddJobRetryLog()");
                throw;
            }
        }

        public async Task<Job> GetJobByName(string jobName)
        {
            try
            {

                var reply = await _jobsClient.GetJobByNameAsync(new JobByNameRequest
                {
                    JobName = jobName
                });

                return _mapper.Map<JobModel, Models.Jobs.Job>(reply.Job);

            }
            catch (Exception ex)
            {
                if ((ex as RpcException)?.Status.StatusCode == StatusCode.NotFound || (ex.InnerException as RpcException)?.Status.StatusCode == StatusCode.NotFound)
                {
                    _logger.LogWarning("NOT FOUND GetJobByName()");
                    return null;
                }
                else
                {
                    _logger.LogError(ex, "Error GetJobByName()");
                    throw;
                }

            }
        }

        public async Task<bool> UpdateJobDateLastRun(int jobID, DateTime dateRun)
        {
            try
            {

                var reply = await _jobsClient.UpdateJobDateLastRunAsync(new UpdateJobDateLastRunRequest
                {
                    DateLastRun = dateRun.ToString("g"),
                    JobID = jobID
                });

                return reply.Success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error UpdateJobDateLastRun()");
                throw;
            }
        }

        public async Task<bool> UpdateJobHistory(int jobHistoryID, bool completed, string errorMessage)
        {
            try
            {
                var reply = await _jobsClient.UpdateJobHistoryAsync(new UpdateJobHistoryRequest
                {
                    Completed = completed,
                    ErrorMessage = errorMessage ?? "",
                    JobHistoryID = jobHistoryID
                });

                return reply.Success;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error UpdateJobHistory()");
                throw;
            }
        }


        public async Task<List<JobRetryLog>> GetJobRetryLogsByLogID(int jobID)
        {
            try
            {
                List<JobRetryLog> jobRetryLogs = new List<JobRetryLog>();

                var srtreamRequest = _jobsClient.GetJobRetryLogsByJobID(new GetJobRetryLogsByJobIDRequest
                {
                    JobID = jobID
                });

                var stream = srtreamRequest.ResponseStream;

                while (await stream.MoveNext())
                {
                    jobRetryLogs.Add(new JobRetryLog
                    {
                        Completed = stream.Current.Completed,
                        JobHistoryID = stream.Current.JobHistoryID,
                        JobRetryLogID = stream.Current.JobRetryLogID,
                        Payload = stream.Current.Payload,
                        RetryCount = stream.Current.RetryCount
                    });
                }

                return jobRetryLogs;

            }
            catch (Exception ex)
            {
                if ((ex as RpcException)?.Status.StatusCode == StatusCode.NotFound || (ex.InnerException as RpcException)?.Status.StatusCode == StatusCode.NotFound)
                {
                    _logger.LogWarning("NOT FOUND UpdateJobHistory()");
                    return new List<JobRetryLog>();
                }
                else
                {
                    _logger.LogError(ex, "Error UpdateJobHistory()");
                    throw;
                }
            }
        }

        public async Task<bool> UpdateJobRetryLog(int jobRetryLogID, bool completed, int retryCount)
        {
            try
            {
                var reply = await _jobsClient.UpdateJobRetryLogAsync(new UpdateJobRetryLogRequest
                {
                    JobRetryLogID = jobRetryLogID,
                    Completed = completed,
                    RetryCount = retryCount,
                });

                return reply.Success;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error UpdateJobRetryLog()");
                throw;
            }
        }
    }
}
