using System;
using System.Collections.Generic;

namespace Database.Models
{
    public partial class TblDocument    
    {
        public TblDocument()
        {                       
        }
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Note { get; set; }
        public int? FieldId { get; set; }
        public DateTime? DateEndApproval { get; set; }
        public int? StatusCode { get; set; }
        public DateTime? Modified { get; set; }
        public bool? Deleted { get; set; }
        public Guid? ModifiedBy { get; set; }
        public Guid? CreatedBy { get; set; }
        public DateTime? Created { get; set; }        
    }
}
