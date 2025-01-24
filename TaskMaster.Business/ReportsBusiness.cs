using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMaster.Domain.Entities;
using TaskMaster.Domain.Entities.Reports;
using TaskMaster.Domain.Interfaces;
using TaskMaster.Domain.ValueObjects;

namespace TaskMaster.Business
{
    public class ReportsBusiness(ITaskRepository taskRepository,
        ILogger<ProjectBusiness> logger) : IReportsBusiness
    {
        private readonly ITaskRepository _taskRepository = taskRepository;
        private readonly ILogger<ProjectBusiness> _logger = logger;

        public async Task<ActionResult<PerformanceReport>> GeneratePerformanceReport()
        {
            try
            {
                var report = new PerformanceReport
                {
                    ReportStartDate = DateTime.Now.AddDays(-30),
                    ReportEndDate = DateTime.Now,
                    UserTaskCompletionDetails = []
                };

                var tasks = await _taskRepository.GetTasksByDateRange(report.ReportStartDate, report.ReportEndDate);

                var userCompletionCounts = new Dictionary<int, UserTaskCompletionDetail>();

                foreach (var task in tasks)
                {
                    var lastChangeLog = task.TaskChangeLogs.LastOrDefault();
                    if (lastChangeLog is not null)
                    {
                        if (!userCompletionCounts.TryGetValue(lastChangeLog.User.Id, out var userDetail))
                        {
                            userDetail = new UserTaskCompletionDetail
                            {
                                UserId = lastChangeLog.User.Id,
                                UserName = lastChangeLog.User.UserName,
                                CompletedTasks = 0
                            };
                            userCompletionCounts[lastChangeLog.User.Id] = userDetail;
                        }

                        userDetail.CompletedTasks++;
                    }
                }

                report.UserTaskCompletionDetails = userCompletionCounts.Values.ToList();
                report.TotalUsers = userCompletionCounts.Count;

                return new ActionResult<PerformanceReport>
                {
                    Success = true,
                    Message = "Report generated successfully.",
                    Result = report
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while generating the performance report.");
                return new ActionResult<PerformanceReport>
                {
                    Success = false,
                    Message = "An error occurred while generating the performance report. Please try again later."
                };
            }
        }

    }
}
