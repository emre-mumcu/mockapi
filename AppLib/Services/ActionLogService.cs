using MockAPI.AppData;
using MockAPI.AppData.Entities;

namespace MockAPI.AppLib.Services
{
    public class ActionLogService(AppDbContext appDbContext) : IActionLogService
    {
        public async Task<int> InsertLog(ActionLog actionLog)
        {
            appDbContext.ActionLogs.Add(actionLog);
            return await appDbContext.SaveChangesAsync();
        }

        public async Task<int> InsertLog(ExceptionLog exceptionLog)
        {
            appDbContext.ExceptionLogs.Add(exceptionLog);
            return await appDbContext.SaveChangesAsync();
        }
    }
}