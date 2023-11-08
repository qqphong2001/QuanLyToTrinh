using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.DTO
{
    public class DocumentFileDTO
    {

        public int Id { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? FilePathToView { get; set; }
        public int? DocId { get; set; }
        public int? Version { get; set; }
        public Guid? UserId { get; set; }
        public DateTime? Modified { get; set; }
        public bool? Deleted { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? Created { get; set; }
    }
}
