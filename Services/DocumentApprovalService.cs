using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Database;
using Database.Models;
using Repositories;
using Services.DTO;

namespace Services
{
    public interface IDocumentApprovalService
    {
        Task<DocumentApprovalDTO> CreateDocumentApprovalAsync(DocumentApprovalDTO payload);
        Task<DocumentApprovalDTO> UpdateDocumentApprovalAsync(DocumentApprovalDTO payload);
        Task<DocumentApprovalDTO> GetSingleByUserIdAndDocId(Guid userId, int docId);
        Task<List<DocumentApprovalDTO>> GetIndividualApprovalList(Guid userId);
        Task<IEnumerable<DocumentApprovalDTO>> GetAllDocumentsApprovalAsync();
        Task<List<DocumentApprovalSummaryDTO>> GetApprovalSummary(int id);
    }
    public class DocumentApprovalService : IDocumentApprovalService
    {
        private readonly IMapper mapper;
        private readonly QLTTrContext qLTTrContext;
        private readonly IDocumentService documentService;
        private readonly IDocumentApprovalRepository documentApprovalRepository;    
        private readonly IUserRepository userRepository;
        private readonly IRoleRepository roleRepository;
        private readonly IUserInRoleRepository userInRoleRepository;

        public DocumentApprovalService(IDocumentApprovalRepository documentApprovalRepository, IMapper mapper, QLTTrContext qLTTrContext, IDocumentService documentService,
            IUserInRoleRepository userInRoleRepository, IUserRepository userRepository, IRoleRepository roleRepository)
        {
            this.mapper = mapper;
            this.qLTTrContext = qLTTrContext;
            this.documentService = documentService;
            this.documentApprovalRepository = documentApprovalRepository;   
            this.userRepository = userRepository;
            this.userInRoleRepository = userInRoleRepository;
            this.roleRepository = roleRepository;
        }
        public async Task<DocumentApprovalDTO> CreateDocumentApprovalAsync(DocumentApprovalDTO payload)
        {
            try
            {
                var docApprovalData = mapper.Map<DocumentApprovalDTO, TblDocumentApproval>(payload);

                /*if (docApprovalData.DocId != null)
                {
                    var docId = await documentService.GetDocumentById(docApprovalData.DocId);
                    payload.DocId = docId;
                }*/

                await documentApprovalRepository.AddAsync(docApprovalData);
                await documentApprovalRepository.SaveChanges();
                return mapper.Map<TblDocumentApproval, DocumentApprovalDTO>(docApprovalData);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<IEnumerable<DocumentApprovalDTO>> GetAllDocumentsApprovalAsync()
        {
            try
            {
                var docApprovalData = documentApprovalRepository.GetAll();

                
                return mapper.Map<IEnumerable<DocumentApprovalDTO>>(docApprovalData);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DocumentApprovalDTO> GetSingleByUserIdAndDocId(Guid userId, int docId)
        {
            try
            {
                //var data = qLTTrContext.TblDocumentApprovals.Where(d => d.UserId == userId).Where(d => d.DocId == docId);

                var data = await documentApprovalRepository.FirstOrDefaultAsync(d => d.UserId == userId && d.DocId == docId);
                if(data == null)
                {
                    throw new Exception("No document be founded");
                }

                return mapper.Map<TblDocumentApproval, DocumentApprovalDTO>(data);
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DocumentApprovalDTO> UpdateDocumentApprovalAsync(DocumentApprovalDTO payload)
        {
            try
            {
                var docApprovalData = await documentApprovalRepository.FirstOrDefaultAsync(x => x.DocId == payload.DocId && x.UserId == payload.UserId);
                if(docApprovalData != null)
                {
                    docApprovalData.Title = payload.Title;
                    docApprovalData.DocId = payload.DocId;
                    docApprovalData.StatusCode = payload.StatusCode;
                    docApprovalData.UserId = payload.UserId;
                    docApprovalData.Modified = payload.Modified;
                    docApprovalData.Deleted = payload.Deleted;
                    docApprovalData.ModifiedBy = payload.ModifiedBy;
                    docApprovalData.CreatedBy = payload.CreatedBy;
                    docApprovalData.Created = payload.Created;
                    docApprovalData.Comment = payload.Comment;

                    await documentApprovalRepository.SaveChanges();


                }
                else
                {
                    return await CreateDocumentApprovalAsync(payload);
                }
                return mapper.Map<TblDocumentApproval, DocumentApprovalDTO>(docApprovalData);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<DocumentApprovalSummaryDTO>> GetApprovalSummary(int id)
        {
            var documents = new List<DocumentDTO>();
            if(id == 0)
            {
                documents = (await documentService.GetAll()).Where(x => x.StatusCode != 2).ToList();
            }
            else
            {
                documents.Add((await documentService.GetAll()).FirstOrDefault(x => x.Id == id)!);
            }

            var approverRole = await roleRepository.FirstOrDefaultAsync(x => x.RoleName.ToLower() == "Approver".ToLower());

            var approverRoleId = new Guid();
            if (approverRole != null) approverRoleId = approverRole.RoleId;

            var approverIds = (await userInRoleRepository.GetMulti(x => x.RoleId == approverRoleId)).Select(x => x.UserId);

            var approvers = await userRepository.GetMulti(x => approverIds.Contains(x.UserId));

            List<DocumentApprovalSummaryDTO> result = new List<DocumentApprovalSummaryDTO>();
            foreach(var document in documents)
            {
                var yesIds = (await documentApprovalRepository.GetMulti(x => x.DocId == document.Id && (x.StatusCode == 4 || x.StatusCode == 5))).Select(x => x.UserId);
                var noIds = (await documentApprovalRepository.GetMulti(x => x.DocId == document.Id && x.StatusCode == 6)).Select(x => x.UserId);

                result.Add(new DocumentApprovalSummaryDTO()
                {
                    Title = document.Title,
                    Status = document.StatusCode == 2 ? "Lưu tạm" : document.StatusCode == 3 ? "Chờ duyệt" : document.StatusCode == 4 ? "Đã duyệt" : document.StatusCode == 5 ? "Đã duyệt kèm ý kiển bổ sung" : document.StatusCode == 6 ? "Không duyệt" : document.StatusCode == 7 ? "Quá hạn duyệt" : "",
                    Author = document.AuthorName,
                    Field = document.FieldName,
                    Submitter = document.AuthorName,
                    DeadlineAt = (DateTime)document.DateEndApproval!,
                    EndAt = (DateTime)document.DateEndApproval!,
                    SubmittedAt = (DateTime)document.Created!,
                    Approvals = approvers.Where(x => yesIds.Contains(x.UserId)).Select(a => a.UserFullName).ToList(),
                    Declines = approvers.Where(x => noIds.Contains(x.UserId)).Select(a => a.UserFullName).ToList(),
                    NoResponses = approvers.Where(x => !yesIds.Contains(x.UserId) && !noIds.Contains(x.UserId)).Select(a => a.UserFullName).ToList(),
                });
            }
            return result;
        }

        public async Task<List<DocumentApprovalDTO>> GetIndividualApprovalList(Guid userId)
        {
            try
            {
                var docApprovalData = await documentApprovalRepository.GetMulti(x => x.UserId == userId);


                return mapper.Map<List<DocumentApprovalDTO>>(docApprovalData);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
