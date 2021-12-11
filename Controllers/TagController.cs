using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using ShoppingListAPI.Entities;
using ShoppingListAPI.Models;
using ShoppingListAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListAPI.Controllers
{
    [Route("api/tags")]
    [ApiController]
    [EnableCors]
    public class TagController : ControllerBase
    {
        private readonly ITagRepository _repository;
        private readonly IMapper _mapper;

        public TagController(ITagRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<TagDto>> GetTags()
        {
            var tags = _repository.GetTags();
            return _mapper.Map<IEnumerable<TagDto>>(tags).ToList();
        }

        [HttpGet("{tagId}", Name = "GetTag")]
        public ActionResult<TagDto> GetTag(Guid tagId)
        {
            var tag = _repository.GetTag(tagId);

            if (tag == null)
            {
                return NotFound();
            }

            return _mapper.Map<TagDto>(tag);
        }

        [HttpPost]
        public ActionResult<Tag> CreateTag(TagCreationDto tag)
        {
            var tagEntity = _mapper.Map<Tag>(tag);
            _repository.AddTag(tagEntity);
            return CreatedAtRoute("GetTag", new { tagId = tagEntity.Id }, tagEntity);
        }


        [HttpPut("{tagId}")]
        public ActionResult UpdateTag(Guid tagId, TagEditionDto tag)
        {

            var existingTag = _repository.GetTag(tagId);
            if (existingTag == null)
            {
                return NotFound();
            }

            _mapper.Map(tag, existingTag);

            _repository.UpdateTag(existingTag);
            return NoContent();
        }


        [HttpDelete("{tagId}")]
        public IActionResult DeleteProduct(Guid tagId)
        {
            var result = _repository.DeleteTag(tagId);
            if (!result)
            {
                return NotFound();
            }
            return Ok();
        }

    }
}
