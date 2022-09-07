﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyBoards.Entities
{
    public class WorkItem
    {
        public int Id { get; set; }
        public string Area { get; set; }
        public WorkItemState State { get; set; }
        public int StateId { get; set; }
        public string IterationPath { get; set; }
        public int Priority { get; set; }
        // Epic
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        // Issue
        public decimal Efford { get; set; }
        // Task
        public string Activity { get; set; }
        public decimal RemainWork { get; set; }


        public string Type { get; set; }

        public List<Comment> Comments { get; set; } = new List<Comment>();
        public User Author { get; set; }
        public Guid AuthorId  { get; set; }
        public List<Tag> Tags { get; set; }
    }
}
