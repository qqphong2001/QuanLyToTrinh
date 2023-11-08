using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Database;
using Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Repositories;
using Services.DTO;
using System.Reflection.Metadata;
using Aspose.Words;
using Aspose.Cells;
using Aspose.Slides;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;
using Aspose.Pdf.Facades;
using Aspose.Words.Pdf2Word.FixedFormats;

namespace Services
{
    public interface IDocumentService
    {
        Task<IEnumerable<DocumentDTO>> GetAllDocumentByStatusCode(int? statusCode);
        Task<IEnumerable<DocumentDTO>> GetAll();
        Task<DocumentDTO> GetDocumentById(int? id);
        Task<int> UpdateDocument(DocumentDTO payload, IFormFile[] files);
        Task<int> UpdateDocumentStatus(int docId, int status);

        Task<int> DeleteDocument(int docId);
        Task<DocumentDTO> Create(DocumentDTO payload, IFormFile[] files);
        Task<IEnumerable<DocumentDTO>> GetDocumentsByDate(DateTime From, DateTime To);

        Task<ChartRequests> GetDocumentsByCharMonth();
        Task<pieData> pieData();

    }
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IDocumentFileRepository _documentFileRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IFieldRepository _fieldRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IDocumentApprovalRepository _documentApprovalRepository;
        private readonly INotificationService _notificationService;
        private readonly QLTTrContext _context;

        public DocumentService(
            IDocumentRepository documentRepository, 
            IMapper mapper, QLTTrContext context, 
            IDocumentFileRepository documentFileRepository, 
            IUserRepository userRepository,
            IFieldRepository fieldRepository,
            ICommentRepository commentRepository,
            IDocumentApprovalRepository documentApprovalRepository,
            INotificationService notificationService)
        {
            this._documentRepository = documentRepository;
            this._documentFileRepository = documentFileRepository;
            this._mapper = mapper;            
            this._userRepository = userRepository;
            this._fieldRepository = fieldRepository;
            this._commentRepository = commentRepository;
            this._documentApprovalRepository = documentApprovalRepository;
            this._notificationService = notificationService;
            this._context = context;    
        }
        public async Task<IEnumerable<DocumentDTO>> GetDocumentsByDate(DateTime From, DateTime To)
        {
            await UpdateOverdueStatus();
            await UpdateApprovalStatus();

            
            var data = _documentRepository.GetAll().Where(x => x.Created >= new DateTime(From.Year, From.Month, 1) && x.Created <=new DateTime(To.Year, To.Month, DateTime.DaysInMonth(To.Year, To.Month))).OrderByDescending(x => x.Id);
            if (data == null)
            {
                throw new Exception("Not be found list of document");
            }

            var resultList = _mapper.Map<IEnumerable<DocumentDTO>>(data);

            foreach (var result in resultList)
            {
                var userData = await _userRepository.FirstOrDefaultAsync(x => x.UserId == result.CreatedBy);
                if (userData != null) result.AuthorName = userData.UserFullName;
                var fieldData = await _fieldRepository.FirstOrDefaultAsync(x => x.Id == result.FieldId);
                if (fieldData != null) result.FieldName = fieldData.Title;
            }

            return resultList;
        }

        public async Task<ChartRequests> GetDocumentsByCharMonth()
        {
            await UpdateOverdueStatus();
            await UpdateApprovalStatus();

            // Assuming _documentRepository.GetDocumentCountByMonth(month) is the method to get counts of documents per month.
            List<int> documentCounts = new List<int>();
            for (int month = 1; month <= 12; month++)
            {
                int count = await _documentRepository.GetDocumentCountByMonth(month);
                documentCounts.Add(count);
            }

            DatasetsChart datasetsChart = new DatasetsChart()
            {
                label = "Tờ trình trong tháng",
                fill = false,
                borderWidth = 2,
                lineTension = 0,
                spanGaps = true,
                borderColor = "#efefef",
                pointRadius = 3,
                pointHoverRadius = 7,
                pointColor = "#efefef",
                pointBackgroundColor = "#efefef",
                data = documentCounts.ToArray() // Assign the counts to the 'data' property
            };

            ChartRequests chart = new ChartRequests()
            {
                labels = new string[] { "Tháng 1", "Tháng 2", "Tháng 3", "Tháng 4", "Tháng 5", "Tháng 6", "Tháng 7", "Tháng 8", "Tháng 9", "Tháng 10", "Tháng 11", "Tháng 12" },
                datasets = datasetsChart
            };

            return chart;
        }
        public async Task<pieData> pieData()
        {
            var fieldIdGroups = _documentRepository.GetAll()
                .GroupBy(x => x.FieldId)
                .ToList();

            List<int> data = new List<int>();
            List<string> labels = new List<string>();
            List<string> backgroundColor = new List<string>();

            var fieldIds = fieldIdGroups.Select(g => g.Key).ToList();

            var fieldTitles = _context.TblFields
                .Where(field => fieldIds.Contains(field.Id))
                .ToDictionary(field => field.Id, field => field.Title);

            foreach (var group in fieldIdGroups)
            {
                int key = group.Key ?? 0;
                if (fieldTitles.TryGetValue(key, out var title))
                {
                    labels.Add(title);
                }
                else
                {
                    labels.Add($"FieldId: {group.Key}");
                }

                data.Add(group.Count());

                // Generating random hex color code
                string color = $"#{new Random().Next(0x1000000):X6}";
                backgroundColor.Add(color);
            }

            var datasetsPie = new List<datasetspie>
                {
                    new datasetspie
                    {
                        data = data.ToArray(),
                        backgroundColor = backgroundColor.ToArray()
                    }
                };

                        var pieData = new pieData
                        {
                            labels = labels.ToArray(),
                            datasetspies = datasetsPie
                        };

            return pieData;
        }


        public async Task<IEnumerable<DocumentDTO>> GetAllDocumentByStatusCode(int? statusCode)
        {
            await UpdateOverdueStatus();
            //await UpdateApprovalStatus();
            await UpdateApprovalStatus_V2();
            var data = (await _documentRepository.GetMulti(x => x.StatusCode == statusCode)).OrderByDescending(x => x.Id);            
            if (data == null)
            {
                throw new Exception("Not be found list of document");
            }

            var resultList = _mapper.Map<IEnumerable<DocumentDTO>>(data);

            foreach(var result in resultList) 
            {
                var userData = await _userRepository.FirstOrDefaultAsync(x => x.UserId == result.CreatedBy);
                if (userData != null) result.AuthorName = userData.UserFullName;
                var fieldData = await _fieldRepository.FirstOrDefaultAsync(x => x.Id == result.FieldId);
                if (fieldData != null) result.FieldName = fieldData.Title;
            }

            return resultList;
        }

        public async Task<IEnumerable<DocumentDTO>> GetAll()
        {
            await UpdateOverdueStatus();
            //await UpdateApprovalStatus();
            await UpdateApprovalStatus_V2();
            var data = _documentRepository.GetAll().OrderByDescending(x => x.Id);            
            if (data == null)
            {
                throw new Exception("Not be found list of document");
            }

            var resultList = _mapper.Map<IEnumerable<DocumentDTO>>(data);

            foreach (var result in resultList)
            {
                var userData = await _userRepository.FirstOrDefaultAsync(x => x.UserId == result.CreatedBy);
                if (userData != null) result.AuthorName = userData.UserFullName;
                var fieldData = await _fieldRepository.FirstOrDefaultAsync(x => x.Id == result.FieldId);
                if (fieldData != null) result.FieldName = fieldData.Title;
            }

            return resultList;
        }

        public async Task<DocumentDTO> GetDocumentById(int? id)
        {
            var data = _documentRepository.FirstOrDefault(x => x.Id == id);
            if (data == null)
            {
                throw new Exception("Not be found id");
            }
            
            var result = _mapper.Map<DocumentDTO>(data);
            var userData = await _userRepository.FirstOrDefaultAsync(x => x.UserId == data.CreatedBy);
            if (userData != null) result.AuthorName = userData.UserFullName;
            var fieldData = await _fieldRepository.FirstOrDefaultAsync(x => x.Id == data.FieldId);
            if (fieldData != null) result.FieldName = fieldData.Title;

            var fileData = await _documentFileRepository.GetMulti(x => x.DocId == data.Id);
            if (fileData != null && fileData.Count > 0)
            {
                result.DocumentFiles = _mapper.Map<List<DocumentFileDTO>>(fileData);                
            }

            var approvalData = await _documentApprovalRepository.GetMulti(x => x.DocId == data.Id);
            if(approvalData != null && approvalData.Count > 0)
            {                
                result.Approvals = _mapper.Map<List<DocumentApprovalDTO>>(approvalData);
                foreach (var item in result.Approvals)
                {
                    var approver = await _userRepository.FirstOrDefaultAsync(x => x.UserId == item.UserId);
                    if (approver != null) item.UserName = approver.UserFullName;
                }
            }
            return result;
        }

        public async Task<int> UpdateDocumentStatus(int docId, int status)
        {
            try
            {
                var document = await _documentRepository.FirstOrDefaultAsync(x => x.Id == docId);
                if (document != null)
                {
                    document.StatusCode = status;
                    await _documentRepository.SaveChanges();
                }                
                return document.Id;
            }
            catch(Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> DeleteDocument(int docId)
        {
            try
            {
                var document = await _documentRepository.FirstOrDefaultAsync(x => x.Id == docId);
                if (document != null)
                {
                    _documentRepository.Remove(document);
                    await _documentRepository.SaveChanges();
                    return document.Id;
                }
                else throw new Exception("Not found");
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message);
            }
        }
        public async Task<int> UpdateDocument(DocumentDTO payload, IFormFile[] files)
        {

            try
            {
                var document = await _documentRepository.FirstOrDefaultAsync(x => x.Id == payload.Id);
                if (document != null)
                {
                    document.Title = payload.Title;
                    document.Note = payload.Note;
                    document.DateEndApproval = payload.DateEndApproval;                    
                    //document.Deleted = payload.Deleted;
                    document.StatusCode = payload.StatusCode;
                    document.FieldId = payload.FieldId;
                    _documentRepository.Update(document);
                    await _documentRepository.SaveChanges();

                    if (files != null)
                    {
                        foreach (var file in files)
                        {
                            if (file.Length > 0)
                            {
                                var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                                var fileExtension = Path.GetExtension(file.FileName);
                                var currentTime = DateTime.Now.ToString("yyyyMMddHHmmssfffttt");
                                var fileName = $"{originalFileName}_{currentTime}{fileExtension}";
                                var uploadFolderPath = Path.Combine("wwwroot", @"Files\Document_Attachments");
                                var filePath = Path.Combine(uploadFolderPath, fileName);


                                using (var fileStream = new FileStream(filePath, FileMode.Create))
                                {
                                    await file.CopyToAsync(fileStream);
                                }

                                var fullFilePath = "/Files/Document_Attachments/" + fileName;

                                var filePathToView = "";
                                var fullFilePathToView = "";
                                if (filePath.EndsWith(".doc"))
                                {
                                    filePathToView = filePath.Replace(".doc", ".pdf");
                                    fullFilePathToView = fullFilePath.Replace(".doc", ".pdf");
                                    Aspose.Words.Document doc = new Aspose.Words.Document(filePath);
                                    doc.Save(filePathToView, Aspose.Words.SaveFormat.Pdf);
                                }
                                else if (filePath.EndsWith(".docx"))
                                {
                                    filePathToView = filePath.Replace(".docx", ".pdf");
                                    fullFilePathToView = fullFilePath.Replace(".docx", ".pdf");
                                    Aspose.Words.Document doc = new Aspose.Words.Document(filePath);
                                    doc.Save(filePathToView, Aspose.Words.SaveFormat.Pdf);
                                }
                                else if (filePath.EndsWith(".xls"))
                                {
                                    filePathToView = filePath.Replace(".xls", ".pdf");
                                    fullFilePathToView = fullFilePath.Replace(".xls", ".pdf");
                                    Workbook doc = new Workbook(filePath);
                                    PdfSaveOptions saveOptions = new PdfSaveOptions();
                                    doc.Save(filePathToView, saveOptions);
                                }
                                else if (filePath.EndsWith(".xlsx"))
                                {
                                    filePathToView = filePath.Replace(".xlsx", ".pdf");
                                    fullFilePathToView = fullFilePath.Replace(".xlsx", ".pdf");
                                    Workbook doc = new Workbook(filePath);
                                    PdfSaveOptions saveOptions = new PdfSaveOptions();
                                    doc.Save(filePathToView, saveOptions);
                                }
                                else if (filePath.EndsWith(".ppt"))
                                {
                                    filePathToView = filePath.Replace(".ppt", ".pdf");
                                    fullFilePathToView = fullFilePath.Replace(".ppt", ".pdf");
                                    using (Presentation presentation = new Presentation(filePathToView))
                                    {
                                        presentation.Save(filePathToView, Aspose.Slides.Export.SaveFormat.Pdf);
                                    }
                                }
                                else if (filePath.EndsWith(".pptx"))
                                {
                                    filePathToView = filePath.Replace(".pptx", ".pdf");
                                    fullFilePathToView = fullFilePath.Replace(".pptx", ".pdf");
                                    using (Presentation presentation = new Presentation(filePathToView))
                                    {
                                        presentation.Save(filePathToView, Aspose.Slides.Export.SaveFormat.Pdf);
                                    }
                                }

                                if (fullFilePathToView == "") fullFilePathToView = fullFilePath;

                                var documentFile = new TblDocumentFile
                                {
                                    Id = 0,
                                    FileName = file.FileName,
                                    FilePath = fullFilePath,
                                    FilePathToView = fullFilePathToView,
                                    DocId = document.Id,
                                    UserId = payload.ModifiedBy,
                                    Modified = DateTime.UtcNow,
                                    Created = DateTime.UtcNow,
                                    Version = 1,
                                    Deleted = false,
                                    CreatedBy = payload.ModifiedBy,
                                    ModifiedBy = payload.ModifiedBy,
                                };

                                _documentFileRepository.Add(documentFile);
                                await _documentFileRepository.SaveChanges();
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("Not found");
                }

                return document.Id;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<DocumentDTO> Create(DocumentDTO payload, IFormFile[] files)
        {
            var documentData = _mapper.Map<TblDocument>(payload);

            try
            {
                await _documentRepository.AddAsync(documentData);
                await _documentRepository.SaveChanges();

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        if (file.Length > 0)
                        {
                            var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                            var fileExtension = Path.GetExtension(file.FileName);
                            var currentTime = DateTime.Now.ToString("yyyyMMddHHmmssfffttt");
                            var fileName = $"{originalFileName}_{currentTime}{fileExtension}";
                            var uploadFolderPath = Path.Combine("wwwroot", @"Files\Document_Attachments");
                            var filePath = Path.Combine(uploadFolderPath, fileName);


                            using (var fileStream = new FileStream(filePath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            var fullFilePath = "/Files/Document_Attachments/" + fileName;

                            var filePathToView = "";
                            var fullFilePathToView = "";
                            if(filePath.EndsWith(".doc"))
                            {
                                filePathToView = filePath.Replace(".doc", ".pdf");
                                fullFilePathToView = fullFilePath.Replace(".doc", ".pdf");
                                Aspose.Words.Document doc = new Aspose.Words.Document(filePath);
                                doc.Save(filePathToView, Aspose.Words.SaveFormat.Pdf);
                            }
                            else if (filePath.EndsWith(".docx"))
                            {
                                filePathToView = filePath.Replace(".docx", ".pdf");
                                fullFilePathToView = fullFilePath.Replace(".docx", ".pdf");
                                Aspose.Words.Document doc = new Aspose.Words.Document(filePath);
                                doc.Save(filePathToView, Aspose.Words.SaveFormat.Pdf);
                            }
                            else if (filePath.EndsWith(".xls"))
                            {
                                filePathToView = filePath.Replace(".xls", ".pdf");
                                fullFilePathToView = fullFilePath.Replace(".xls", ".pdf");
                                Workbook doc = new Workbook(filePath);
                                PdfSaveOptions saveOptions = new PdfSaveOptions();                                
                                doc.Save(filePathToView, saveOptions);
                            }
                            else if (filePath.EndsWith(".xlsx"))
                            {
                                filePathToView = filePath.Replace(".xlsx", ".pdf");
                                fullFilePathToView = fullFilePath.Replace(".xlsx", ".pdf");
                                Workbook doc = new Workbook(filePath);
                                PdfSaveOptions saveOptions = new PdfSaveOptions();
                                doc.Save(filePathToView, saveOptions);
                            }
                            else if (filePath.EndsWith(".ppt"))
                            {
                                filePathToView = filePath.Replace(".ppt", ".pdf");
                                fullFilePathToView = fullFilePath.Replace(".ppt", ".pdf");
                                using (Presentation presentation = new Presentation(filePathToView))
                                {
                                    presentation.Save(filePathToView, Aspose.Slides.Export.SaveFormat.Pdf);
                                }
                            }
                            else if (filePath.EndsWith(".pptx"))
                            {
                                filePathToView = filePath.Replace(".pptx", ".pdf");
                                fullFilePathToView = fullFilePath.Replace(".pptx", ".pdf");
                                using (Presentation presentation = new Presentation(filePathToView))
                                {
                                    presentation.Save(filePathToView, Aspose.Slides.Export.SaveFormat.Pdf);
                                }
                            }

                            if (fullFilePathToView == "") fullFilePathToView = fullFilePath;

                            var documentFile = new TblDocumentFile
                            {
                                Id = 0,
                                FileName = file.FileName,
                                FilePath = fullFilePath,
                                FilePathToView = fullFilePathToView,
                                DocId = documentData.Id,
                                UserId = payload.ModifiedBy,
                                Modified = DateTime.UtcNow,
                                Created = DateTime.UtcNow,
                                Version = 1,
                                Deleted = false,
                                CreatedBy = payload.ModifiedBy,
                                ModifiedBy = payload.ModifiedBy,
                            };

                            _documentFileRepository.Add(documentFile);
                            await _documentFileRepository.SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return _mapper.Map<DocumentDTO>(documentData);
        }

        private async Task UpdateOverdueStatus()
        {

            var overdueList = _documentRepository.Query(x => (x.StatusCode == 3 || x.StatusCode == 6) && (x.DateEndApproval != null && DateTime.Compare(DateTime.Today, (DateTime)x.DateEndApproval) > 0)).ToList();
            foreach (var overdue in overdueList)
            {
                overdue.StatusCode = 7;
                await _documentRepository.SaveChanges();
                await _notificationService.CreateNotifications(4, overdue.Id, null);
            }
        }

        private async Task UpdateApprovalStatus_V2()
        {
            var allPendingDocuments = _documentRepository.Query(x => x.StatusCode == 3).Select(x => x.Id).ToList();
            var allApprovalRecord = await _documentApprovalRepository.Query(x => allPendingDocuments.Contains((int)x.DocId)).ToListAsync();
            foreach(var item in allPendingDocuments)
            {
                var allApproval = allApprovalRecord.Where(x => x.DocId == item).ToList();
                if(allApproval.Count == 6)
                {
                    var needAction = 0;
                    if(allApproval.Where(x => x.StatusCode == 4 || x.StatusCode == 5).Count() >= 3)
                    {
                        needAction = 1;                        
                    }
                    else
                    {
                        needAction = -1;
                    }
                    if(needAction != 0)
                    {
                        var data = await _documentRepository.FirstOrDefaultAsync(x => x.Id == item);
                        if (data != null)
                        {
                            data.StatusCode = needAction == 1 ? 4 : 6;
                            _documentRepository.Update(data);
                            await _documentRepository.SaveChanges();

                            await _notificationService.CreateNotifications(4, data.Id, null);
                        }
                    }
                }
            }
        }

        private async Task UpdateApprovalStatus()
        {
            var allPendingDocuments = _documentRepository.Query(x => x.StatusCode == 3).Select(x => x.Id).ToList();
            var allApprovals = _documentApprovalRepository.Query(x => x.StatusCode == 4 || x.StatusCode == 5 && allPendingDocuments.Contains((int)x.DocId!)).ToList();
            foreach(var item in allApprovals)
            {
                var count = 0;
                foreach(var z in allApprovals)
                {
                    if (z.DocId == item.DocId)
                    {
                        count++;
                        if(count > 3)
                        {
                            var data = await _documentRepository.FirstOrDefaultAsync(x => x.Id == item.DocId);
                            if(data != null)
                            {
                                data.StatusCode = 4;
                                _documentRepository.Update(data);
                                await _documentRepository.SaveChanges();

                                await _notificationService.CreateNotifications(4, data.Id, null);
                            }
                        }
                    }
                }
            }
            allPendingDocuments = _documentRepository.Query(x => x.StatusCode == 3).Select(x => x.Id).ToList();
            var allDeclines = _documentApprovalRepository.Query(x => x.StatusCode == 6 && allPendingDocuments.Contains((int)x.DocId!)).ToList();
            foreach (var item in allDeclines)
            {
                var count = 0;
                foreach (var z in allDeclines)
                {
                    if (z.DocId == item.DocId)
                    {
                        count++;
                        if (count >= 3)
                        {
                            var data = await _documentRepository.FirstOrDefaultAsync(x => x.Id == item.DocId);
                            if (data != null)
                            {
                                data.StatusCode = 6;
                                _documentRepository.Update(data);
                                await _documentRepository.SaveChanges();
                                
                                await _notificationService.CreateNotifications(4, data.Id, null);
                            }
                        }
                    }
                }
            }
        }
    }
}
