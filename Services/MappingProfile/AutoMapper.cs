using AutoMapper;
using Database.Models;
using Services.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.MappingProfile
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
             
            CreateMap<TblDocument, DocumentDTO>().ReverseMap();
            CreateMap<TblDocumentApproval, DocumentApprovalDTO>().ReverseMap();
            CreateMap<TblComment, CommentDTO>().ReverseMap();
            CreateMap<TblField, FieldDTO>().ReverseMap();
            CreateMap<TblDocumentFile, DocumentFileDTO>().ReverseMap();
            CreateMap<TblComment, CommentDTO>().ReverseMap();
            CreateMap<TblNotification, NotificationDTO>().ReverseMap();
            CreateMap<UserInfoDTO,AppUser>().ReverseMap();
        }
    }
}
