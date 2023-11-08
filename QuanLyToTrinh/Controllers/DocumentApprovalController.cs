using Core.Domains;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services;
using Services.DTO;

namespace QuanLyToTrinh.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentApprovalController : ControllerBase
    {
        private readonly IDocumentApprovalService documentApprovalService;

        public DocumentApprovalController(IDocumentApprovalService documentApprovalService)
        {
            this.documentApprovalService = documentApprovalService;
        }

        [HttpPost("CreateDocumentApproval")]
        public async Task<IActionResult> CreateDocumentApproval(DocumentApprovalDTO payload)
        {

            try
            {
                var result = await documentApprovalService.CreateDocumentApprovalAsync(payload);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));

            }
        }

        [HttpPut("UpdateDocumentApproval")]
        public async Task<IActionResult> UpdateDocumentApproval(DocumentApprovalDTO payload)
        {

            try
            {
                var result = await documentApprovalService.UpdateDocumentApprovalAsync(payload);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));

            }
        }

        [HttpGet("GetSingleByUserIdAndDocId/{userId}/{docId}")]
        public async Task<IActionResult> GetSingleByUserIdAndDocId(Guid userId, int docId)
        {
            try
            {
                var result = await documentApprovalService.GetSingleByUserIdAndDocId(userId, docId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));

            }
        }

        [HttpGet("GetApprovalSummary/{id}")]
        public async Task<IActionResult> GetApprovalSummary(int id)
        {
            try
            {
                var result = await documentApprovalService.GetApprovalSummary(id);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));

            }
        }

        [HttpGet("GetAllDocumentApproval")]
        public async Task<IActionResult> GetAllDocumentApproval()
        {
            try
            {
                var result = await documentApprovalService.GetAllDocumentsApprovalAsync();
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));

            }
        }

        [HttpGet("GetIndividualDocumentApprovals/{userId}")]
        public async Task<IActionResult> GetIndividualDocumentApprovals(Guid userId)
        {
            try
            {
                var result = await documentApprovalService.GetIndividualApprovalList(userId);
                return Ok(new BaseResponseModel(result));
            }
            catch (Exception ex)
            {
                return Ok(new BaseResponseModel(null, false, StatusCodes.Status500InternalServerError, ex.Message));

            }
        }
    }
}
