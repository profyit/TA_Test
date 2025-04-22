using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TaskManagementCommon.Models; 
using TaskManagementDBLayer.Entities.TaskManagerApi.Data;


namespace TaskManagementDBLayer
{
    public interface ITasksDBService
    {
        Task<IEnumerable<TaskItem>> GetTasksFromDB();
        Task UpdateTask( TaskItem taskItem);
        Task<TaskItem> GetTaskByIDFromDB(int id);
        Task CreateTaskInDB(TaskItem taskItem);
        Task<bool> DeleteTaskInDB(int id);
    }

    public class TasksDBService : ITasksDBService
    {
        private readonly TaskDbContext _context;
        private readonly ICustomLogService _logService;
        private readonly ILogger<TasksDBService> logger;

        public TasksDBService(TaskDbContext context, ICustomLogService logService)
        {
            _context = context; 
            _logService = logService;
            this.logger = logger;
        }

        public async Task<IEnumerable<TaskItem>> GetTasksFromDB()
        {
            _logService.LogInfo(new LogInfo { FunctionName = nameof(GetTasksFromDB), UserName = "userName" });
            if (_context.Tasks == null)
            {

                _logService.LogInfo(new LogInfo { FunctionName = nameof(GetTasksFromDB), Message = "Tasks DbSet is null." });
                return Enumerable.Empty<TaskItem>();
            }
            return await _context.Tasks.ToListAsync();
        }

        public async Task<TaskItem> GetTaskByIDFromDB(int id)
        {
            if (_context==null || _context.Tasks == null)
            {
                var logInfo = new LogInfo { FunctionName = nameof(GetTaskByIDFromDB), Message = "Tasks DbSet is null." };
                _logService.LogToConsole(logInfo);
                _logService.LogInfo(logInfo);
                return null; 
            }
            return await _context.Tasks.FindAsync(id);
        }

        public async Task UpdateTask(TaskItem taskItem)
        {
            var logInfo = new LogInfo { FunctionName = nameof(UpdateTask) };
            if (taskItem == null)
            {
                logInfo.Message = $"Input taskItem is null";
                _logService.LogInfo(logInfo);
                _logService.LogToConsole(logInfo);

            }
            else
                if (!await TaskExists(taskItem.Id))
                {
                        logInfo.Message = $"Task with id {taskItem.Id} not exists";
                        _logService.LogInfo(logInfo);
                        _logService.LogToConsole(logInfo);  
                        return;
                
                }
            _context.Entry(taskItem).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                logInfo.Message = $"Task with id {taskItem.Id} updated successfully.";
                _logService.LogInfo(logInfo);
                _logService.LogToConsole(logInfo);

            }
            catch (DbUpdateConcurrencyException ex)
            {
                string message = $"Concurrency error updating task id {taskItem.Id}.";
                _logService.LogError(ex, message);
                logInfo.Message = message;
              
            }
            catch (Exception ex) 
            {
                string message = $"Error updating task id {taskItem.Id}.";
                logInfo.Message = message;
                _logService.LogError(ex, message);
                _logService.LogToConsole(logInfo);
                throw; 
            }
        }

     
        private async Task<bool> TaskExists(int id)
        {
            if (_context.Tasks == null) return false;
            return await _context.Tasks.AnyAsync(e => e.Id == id);
        }

    
        public async Task CreateTaskInDB(TaskItem taskItem)
        {
            if (taskItem == null)
            {
                _logService.LogInfo(new LogInfo { FunctionName = nameof(CreateTaskInDB), Message = "Attempted to create a null taskItem." });
                throw new ArgumentNullException(nameof(taskItem));
            }
            if (_context.Tasks == null)
            {
                _logService.LogInfo(new LogInfo { FunctionName = nameof(CreateTaskInDB), Message = "Tasks DbSet is null." });
                throw new InvalidOperationException("Tasks DbSet is not available.");
            }

            try
            {
               
                taskItem.CreatedDate = DateTime.UtcNow; 

                _context.Tasks.Add(taskItem);
                await _context.SaveChangesAsync(); 
                _logService.LogInfo(new LogInfo { FunctionName = nameof(CreateTaskInDB), Message = $"Task '{taskItem.Description}' created successfully." });
            }
            catch (Exception ex) 
            {
                _logService.LogError(ex,  "Error creating task." );
                throw;
            }
        }

        public async Task<bool> DeleteTaskInDB(int id)
        {
            if (_context.Tasks == null)
            {
                _logService.LogInfo(new LogInfo { FunctionName = nameof(DeleteTaskInDB), Message = "Tasks DbSet is null." });
                return false; // Or throw
            }

            var taskItem = await _context.Tasks.FindAsync(id);
            if (taskItem == null)
            {
                _logService.LogInfo(new LogInfo { FunctionName = nameof(DeleteTaskInDB), Message = $"Task with id {id} not found for deletion." });
                return false; 
            }

            try
            {
                _context.Tasks.Remove(taskItem);
                await _context.SaveChangesAsync();
                _logService.LogInfo(new LogInfo { FunctionName = nameof(DeleteTaskInDB), Message = $"Task with id {id} deleted successfully." });
                return true;
            }
            catch (Exception ex) // Catch specific exceptions like DbUpdateException if needed
            {
                _logService.LogError(ex, $"Error deleting task id {id}.");
                return false; 
            }
        }
    }
}