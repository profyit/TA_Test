using Microsoft.AspNetCore.Mvc;
using TaskManagementBLLayer.Services;
using TaskManagementCommon.Models;

namespace TaskManagement.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly ITasksService tasksService;
        private readonly ICustomLogService logService;

        public TasksController(ITasksService tasksService, ICustomLogService logService)
        {
            this.tasksService = tasksService;
            this.logService = logService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetTasks()
        {
            var tasks = await tasksService.GetTasksListFromDB();
            return Ok(tasks); 
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTaskByID(int id)
        {
            var taskItem = await tasksService.GetTaskByID(id);
                

            if (taskItem == null)
            {
                return NotFound(); 
            }

            return Ok(taskItem); 
        }

     
        [HttpPost]
        public async Task<ActionResult<TaskItem>> CreateTask(TaskItem taskItem)
        {

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                 tasksService.CreateTask(taskItem);


                return CreatedAtAction(nameof(CreateTask), new { id = taskItem.Id }, taskItem);
            }
            catch (Exception ex)
            {
                logService.LogError(ex);
                return BadRequest(ex.Message);
            }
        }

     
        [HttpPut]
        public async Task<IActionResult> UpdateTask( TaskItem taskItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
           
            await  tasksService.UpdateTask(taskItem);
 
            return NoContent(); 
        }

  
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
           var isSuccess =  await tasksService.DeleteTask(id);
         
            if (!isSuccess)
                return NotFound();

            return NoContent(); 
        }

 
        
    }
}
