using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MyBoards.Entities
{
    public class Epic : WorkItem
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class Issue : WorkItem
    {
        public decimal Efford { get; set; }
    }

    public class Task : WorkItem
    {
        public string Activity { get; set; }
        public decimal RemainWork { get; set; }
    }
    public abstract class WorkItem
    {
        public int Id { get; set; }
        public string Area { get; set; }
        public virtual WorkItemState State { get; set; }
        public int StateId { get; set; }
        public string IterationPath { get; set; }
        public int Priority { get; set; }

        public string Type { get; set; }

        public virtual List<Comment> Comments { get; set; } = new List<Comment>();
        public virtual User Author { get; set; }
        public Guid AuthorId { get; set; }
        public virtual List<Tag> Tags { get; set; }
    }
}
