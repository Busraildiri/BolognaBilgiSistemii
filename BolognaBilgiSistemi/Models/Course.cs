﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BolognaBilgiSistemi.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        [Required]
        public string Name { get; set; }
        public int DepartmentId { get; set; }
        public Department? Department { get; set; }
        public string Description { get; set; } // Yeni özellik eklendi
        public ICollection<CourseAssignment> CourseAssignments { get; set; } = new List<CourseAssignment>();
        public string Content { get; set; } // 14 Haftalık İçerik
        public string SourceBooks { get; set; } // Kaynak Kitaplar
        public string Prerequisites { get; set; } // Ön Koşullar

    }
}
