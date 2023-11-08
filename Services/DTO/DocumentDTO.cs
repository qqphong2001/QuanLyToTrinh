using Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Services.DTO
{
    public class DocumentDTO
    {
        public DocumentDTO()
        {   
        }
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Note { get; set; }
        public int? FieldId { get; set; }
        public string? FieldName { get; set; }
        public DateTime? DateEndApproval { get; set; }
        public int? StatusCode { get; set; }
        public DateTime? Modified { get; set; }
        public Guid? ModifiedBy { get; set; }        
        public DateTime? Created { get; set; }
        public Guid? CreatedBy { get; set; }
        public string? AuthorName { get; set; }
        public bool? Deleted { get; set; }
        public List<DocumentFileDTO>? DocumentFiles { get; set; }
        public List<CommentDTO>? Comments { get; set; }
        public List<DocumentApprovalDTO>? Approvals { get; set; }


    }
}
