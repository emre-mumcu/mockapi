using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockAPI.AppData.Entities;

namespace MockAPI.AppLib.Services
{
    public interface IActionLogService
    {
        Task<int> InsertLog(ActionLog actionLog);
        Task<int> InsertLog(ExceptionLog exceptionLog);
    }
}