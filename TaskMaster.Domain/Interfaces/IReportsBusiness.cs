using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskMaster.Domain.Entities.Reports;
using TaskMaster.Domain.ValueObjects;

namespace TaskMaster.Domain.Interfaces
{
    public interface IReportsBusiness
    {
        Task<ActionResult<PerformanceReport>> GeneratePerformanceReport();
    }
}
