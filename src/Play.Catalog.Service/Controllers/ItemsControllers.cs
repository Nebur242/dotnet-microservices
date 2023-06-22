using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Play.Catalog.Service.Dtos;
using Play.Catalog.Service.Repositories;
using Play.Catalog.Service.Extensions;
using Play.Catalog.Service.Entities;

namespace Play.Catalog.Service.Controllers
{
    [ApiController]
    [Route("items")]
    public class ItemController : ControllerBase
    {
        private readonly IItemsRepository itemsRepository;

        public ItemController(IItemsRepository itemsRepository)
        {
            this.itemsRepository = itemsRepository;
        }

        // GET  /items
        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetAsync()
        {
            var items = (await itemsRepository.GetAllAsync()).Select(item => item.asDto());
            return items;
        }

        // GET  /items/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await itemsRepository.GetAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return item.asDto();
        }

        // POST  /items
        [HttpPost]
        public async Task<ActionResult<ItemDto>> PostAsync(CreateItemDto createItemDto)
        {
            var item = new Item()
            {
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.Now,
            };
            await itemsRepository.CreateAsynnc(item);
            return CreatedAtAction(nameof(GetByIdAsync), new { Id = item.Id }, item.asDto());
        }

        // PUT  /items/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAsync(Guid id, UpdateItemDto updateItemDto)
        {
            var found = await itemsRepository.GetAsync(id);

            if (found == null)
            {
                return NotFound();
            }

            found.Name = updateItemDto.Name;
            found.Description = updateItemDto.Description;
            found.Price = updateItemDto.Price;

            await itemsRepository.UpdateAsync(found);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var found = await itemsRepository.GetAsync(id);

            if (found == null)
            {
                return NotFound();
            }
            await itemsRepository.DeleteAsync(found.Id);
            return NoContent();
        }
    }


}