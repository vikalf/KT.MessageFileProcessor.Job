using AutoMapper;
using KT.Jobs.Service.Definition;
using System;

namespace KT.MessageFileProcessor.Job.AutoMapper
{
    public class MappingProfile : Profile
    {

        public MappingProfile()
        {

            #region Service.Definition -> Data Models

            CreateMap<JobModel, Data.Models.Jobs.Job>().ConvertUsing((src, dest) =>
            {
                var model = new Data.Models.Jobs.Job
                {
                    Description = src.Description ?? "",
                    Enabled = src.Enabled,
                    JobID = src.JobID,
                    MaxRetries = src.MaxRetries,
                    Name = src.Name ?? ""
                };

                if (DateTime.TryParse(src.DateLastRun, out DateTime dtLastRun))
                    model.DateLastRun = dtLastRun;
                else
                    model.DateLastRun = null;

                return model;

            });

            CreateMap<JobHistoryModel, Data.Models.Jobs.JobHistory>().ConvertUsing((src, dest) =>
            {
                return new Data.Models.Jobs.JobHistory
                {
                    Completed = src.Completed,
                    DateRun = DateTime.Parse(src.DateRun),
                    ErrorMessage = src.ErrorMessage,
                    JobHistoryID = src.JobHistoryID,
                    JobID = src.JobID
                };

            });

            CreateMap<JobLogModel, Data.Models.Jobs.JobLog>().ConvertUsing((src, dest) =>
            {
                return new Data.Models.Jobs.JobLog
                {
                    Completed = src.Completed,
                    ErrorMessage = src.ErrorMessage ?? "",
                    JobHistoryID = src.JobHistoryID,
                    JobLogID = src.JobLogID,
                    Payload = src.Payload ?? ""
                };
            });

            CreateMap<JobRetryLogModel, Data.Models.Jobs.JobRetryLog>().ConvertUsing((src, dest) =>
            {
                return new Data.Models.Jobs.JobRetryLog
                {
                    Completed = src.Completed,
                    JobHistoryID = src.JobHistoryID,
                    JobRetryLogID = src.JobRetryLogID,
                    Payload = src.Payload ?? "",
                    RetryCount = src.RetryCount
                };
            });


            #endregion
        }

    }
}