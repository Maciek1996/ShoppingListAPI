using ShoppingListAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingListAPI.Services
{
    public interface ITagRepository
    {
        public void AddTag(Tag tag);

        public Tag GetTag(Guid tagId);

        public IEnumerable<Tag> GetTags();

        public void UpdateTag(Tag tag);

        public bool DeleteTag(Guid tagId);

    }
}
