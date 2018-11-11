using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UrlShortnerApi.DAL;
using UrlShortnerApi.Models;

namespace UrlShortnerApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {
        private readonly IDocumentDBRepository<TodoItem> _repository;
        private readonly ILogger _logger;

        public TodoController(IDocumentDBRepository<TodoItem> repository, ILogger<TodoController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public async Task<List<TodoItem>> GetAllAsyc()
        {
            var items = await _repository.GetAllItemsAsync();
            return items.ToList();
        }

        [HttpGet("{id}", Name = "GetTodo")]
        [ProducesResponseType(200, Type = typeof(TodoItem))]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(string id)
        {
            var item = await _repository.GetItemAsync(id);
            if (item.IsNull())
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(TodoItem))]
        [ProducesResponseType(409)]
        public async Task<IActionResult> Create(TodoItem item)
        {
            try
            {
                await _repository.CreateItemAsync(item);
                return this.CreatedAtRoute("GetTodo", new { id = item.Id }, item);
            }
            catch (DocumentClientException ex)
            {
                _logger.LogError(ex, "A resource with the specified id:{id}  or name:{name} already exists", item.Id, item.Name);
                return Conflict("A resource with the specified id or name already exists");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, TodoItem item)
        {
            var todo = await _repository.GetItemAsync(id);

            if (todo.IsNull())
            {
                return NotFound();
            }

            todo.Completed = item.Completed;
            todo.Name = item.Name;
            todo.Description = item.Description;

            await _repository.UpdateItemAsync(id, todo);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var todo = await _repository.GetItemAsync(id);

            if (todo.IsNull())
            {
                return NotFound();
            }

            await _repository.DeleteItemAsync(id);
            return NoContent();
        }
    }
}