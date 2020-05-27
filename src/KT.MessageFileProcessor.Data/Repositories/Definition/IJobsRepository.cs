using KT.MessageFileProcessor.Data.Models.Jobs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KT.MessageFileProcessor.Data.Repositories.Definition
{
    public interface IJobsRepository
    {
        Task<JobRetryLog> AddJobRetryLog(int jobHistoryID, string payload);
        Task<JobLog> AddJobLog(int jobHistoryID, string payload, bool completed, string errorMessage);
        Task<bool> UpdateJobDateLastRun(int jobID, DateTime dateRun);
        Task<bool> UpdateJobHistory(int jobHistoryID, bool completed, string errorMessage);
        Task<Job> GetJobByName(string jobName);
        Task<JobHistory> AddJobHistory(int jobID, DateTime dateRun);
        Task<List<JobRetryLog>> GetJobRetryLogsByLogID(int jobID);
        Task<bool> UpdateJobRetryLog(int jobRetryLogID, bool completed, int retryCount);
        Task<List<JobLog>> GetJobLogsByJobHistoryID(int jobHistoryID);
    }
}
