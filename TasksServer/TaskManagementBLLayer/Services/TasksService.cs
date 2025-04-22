using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementCommon.Models;
using TaskManagementDBLayer;

namespace TaskManagementBLLayer.Services
{
    public interface ITasksService
    {
        Task<IEnumerable<TaskItem>> GetTasksListFromDB();
        Task<TaskItem> GetTaskByID(int id);
        Task CreateTask(TaskItem taskItem);
        Task<bool> DeleteTask(int id);
        Task UpdateTask(TaskItem taskItem);
    }

    public class TasksService : ITasksService
    {
        private readonly ITasksDBService tasksDBService;
        private readonly ILogger<TasksService> logger;

        public TasksService(ITasksDBService tasksDBService,ILogger<TasksService> logger)
        {
            this.tasksDBService = tasksDBService;
            this.logger = logger;
        }

        public async Task<IEnumerable<TaskItem>> GetTasksListFromDB()
        {
            return await tasksDBService.GetTasksFromDB();
        }

        public async Task<TaskItem> GetTaskByID(int id)
        {
            return await tasksDBService.GetTaskByIDFromDB(id);
        }

        public Task CreateTask(TaskItem task)
        {
            tasksDBService.CreateTaskInDB(task);
            return Task.CompletedTask;
        }

        public async Task<bool> DeleteTask(int id)
        {
            return await tasksDBService.DeleteTaskInDB(id);
        }

        public async Task UpdateTask(TaskItem taskItem)
        {
             await tasksDBService.UpdateTask(taskItem);
            
        }
    }
}
