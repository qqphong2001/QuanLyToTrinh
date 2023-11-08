using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database;
using Database.Models;
using Repositories.BaseRepository;

namespace Repositories
{
    public interface IDocumentRepository : IBaseRepository<TblDocument> { }
    public class DocumentRepository : BaseRepository<TblDocument>, IDocumentRepository
    {
        public DocumentRepository(QLTTrContext context) : base(context)
        {

        }
    }
    
}
