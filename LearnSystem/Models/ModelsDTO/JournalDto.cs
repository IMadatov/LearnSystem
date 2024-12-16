﻿using BaseCrud.Entities;

namespace LearnSystem.Models.ModelsDTO
{
    public class JournalDto:IDataTransferObject<Journal>
    {
        public int ClassId { get; set; }

        public Class? Class { get; set; }

        public ICollection<Subject>? Subject { get; set; } = new List<Subject>();

        public ICollection<Teacher>? Teachers { get; set; } = new List<Teacher>();

        public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}
