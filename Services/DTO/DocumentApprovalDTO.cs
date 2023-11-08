using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Models;

namespace Services.DTO
{
    public class DocumentApprovalDTO
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public int? DocId { get; set; }
        public int? StatusCode { get; set; }
        public string? Comment { get; set; }
        public Guid? UserId { get; set; }
        public string? UserName { get; set; }
        public DateTime? Modified { get; set; }
        public bool? Deleted { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? Created { get; set; }
    }

    public class DocumentApprovalSummaryDTO
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Status { get; set; }
        public string Field { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string Submitter { get; set; }
        public DateTime DeadlineAt { get; set; }
        public DateTime EndAt { get; set; }
        public List<string> Approvals { get; set; }
        public List<string> Declines { get; set; }
        public List<string> NoResponses { get; set; }

    }
    
}
