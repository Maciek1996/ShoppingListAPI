using ShoppingListAPI.DbContexts;
using ShoppingListAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services
{
    public class TagRepository : ITagRepository
    {
        private readonly ShoppingListContext _context;

        public TagRepository(ShoppingListContext context)
        {
            _context = context;
        }

        public void AddTag(Tag tag)
        {
            _context.Tags.Add(tag);
            _context.SaveChanges();
        }

        public bool DeleteTag(Guid tagId)
        {
            var tag = GetTag(tagId);
            if (tag != null)
            {
                var existsOnLists = _context.Lists.Where(p => p.ListTagId == tagId).FirstOrDefault();
                if (existsOnLists != null)
                {
                    tag.IsActive = false;
                }
                else
                {
                    _context.Attach(tag).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                }
                _context.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public Tag GetTag(Guid tagId)
        {
            var tag = _context.Tags.FirstOrDefault(t => t.Id == tagId);
            return tag;
        }

        public IEnumerable<Tag> GetTags()
        {
            var tags = _context.Tags.Where(t => t.IsActive).AsEnumerable();
            return tags;
        }

        public void UpdateTag(Tag tag)
        {
            _context.SaveChanges();
        }
    }
}
