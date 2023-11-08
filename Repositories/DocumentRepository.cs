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
    public interface IDocumentRepository : IBaseRepository<TblDocument> {
        Task<int> GetDocumentCountByMonth(int month);
    }
    public class DocumentRepository : BaseRepository<TblDocument>, IDocumentRepository
    {
        private readonly QLTTrContext _context;
        public DocumentRepository(QLTTrContext context) : base(context)
        {
            _context = context;
        }

        public async Task<int> GetDocumentCountByMonth(int month)
        {
            // Assuming TblDocument has a DateTime field named 'DateCreated' or similar to check against
            DateTime startOfMonth = new DateTime(DateTime.Now.Year, month, 1);
            DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            int count =  _context.Set<TblDocument>().Where(d => d.Created >= startOfMonth && d.Created <= endOfMonth).Count();

            return count;
        }
    }
    
}
