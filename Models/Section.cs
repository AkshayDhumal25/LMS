using System;

namespace LMS.Models
{
    public class Section
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Shelf> Shelves { get; set; }
    }

}
