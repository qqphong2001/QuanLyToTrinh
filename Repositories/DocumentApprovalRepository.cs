using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Repositories.BaseRepository;

namespace Repositories
{
    public interface IDocumentApprovalRepository : IBaseRepository<TblDocumentApproval>
    {

    }
    public class DocumentApprovalRepository : BaseRepository<TblDocumentApproval>, IDocumentApprovalRepository
    {
        public DocumentApprovalRepository(QLTTrContext context) : base(context)
        {
        }
    }
}
