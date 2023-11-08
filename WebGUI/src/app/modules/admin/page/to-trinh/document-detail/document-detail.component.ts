import { Component, OnInit } from '@angular/core';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ToastrService } from 'ngx-toastr';
import { CommentComponent } from 'src/app/components/comment/comment.component';
import { ConfirmationComponent } from 'src/app/components/confirmation/confirmation.component';
import { CommentModel } from 'src/app/models/comment.model';
import { DocumentModel } from 'src/app/models/document.model';
import { DocumentApprovalModel } from 'src/app/models/documentApproval.model';
import { DocumentFileModel } from 'src/app/models/documentFile.model';
import { CommentService } from 'src/app/services/admin/comment.service';
import { DocumentApprovalService } from 'src/app/services/admin/document-approval.service';
import { DocumentService } from 'src/app/services/admin/document.service';
import { DialogService } from 'src/app/services/dialog.service';
import * as common from 'src/app/utils/commonFunctions';

@Component({
  selector: 'app-document-detail',
  templateUrl: './document-detail.component.html',
  styleUrls: ['./document-detail.component.css']
})
export class DocumentDetailComponent implements OnInit {
  
  constructor(
    private router: Router, 
    private route: ActivatedRoute, 
    private documentService: DocumentService, 
    private documentApprovalService: DocumentApprovalService, 
    public sanitizer: DomSanitizer,
    private commentService: CommentService,
    private toastr: ToastrService,
    private modalService: NgbModal,
    private dialogService: DialogService) {
    
  }
  userRoles = common.GetRoleInfo();
  currentUserId = common.GetCurrentUserId();
  docId: number = 0;
  newComment = '';
  comments: CommentModel[] = [];
  documentApprovals: DocumentApprovalModel[] = [];  
  selectedDocument: DocumentFileModel | undefined;
  urlToView: SafeResourceUrl | undefined;
  document: DocumentModel | undefined;
  myApprovalStatus = '';

  ngOnInit(): void {
      this.loadData();
  }

  loadData(){
    this.route.params.subscribe(params => {      
      const id = params['id'];
      if(!!id){
        this.docId = id;
        this.documentService.getDocumentsById(id).subscribe(res => {
          if(res.isSuccess){
            this.document = res.result as DocumentModel;            
            this.getMyApprovalData();
            if(this.document.documentFiles){
              this.document.documentFiles.forEach(x => {
                x.filePath = common.GetFullFilePath(x.filePath);
                x.filePathToView = common.GetFullFilePath(x.filePathToView!);
              });
              if(this.document.documentFiles.length > 0){
                this.selectedDocument = this.document.documentFiles[0];                
                this.urlToView = this.sanitizer.bypassSecurityTrustResourceUrl(this.selectedDocument?.filePathToView!);                
              }              
            }            
          }
        }); 
        this.loadComments(id);
      }
    });
  }

  getMyApprovalData(){
    if(this.document?.statusCode == 2) this.myApprovalStatus = 'Tờ trình đang ở trạng thái tạm lưu, vui lòng không đánh giá'
    else if(this.document?.statusCode == 4) this.myApprovalStatus = 'Tờ trình này đã được duyệt'
    else if(this.document?.statusCode == 5) this.myApprovalStatus = 'Tờ trình này đã được duyệt và ý kiến' 
    else if(this.document?.statusCode == 6) this.myApprovalStatus = 'Tờ trình này không được duyệt'
    else if(this.document?.statusCode == 7) this.myApprovalStatus = 'Tờ trình này đã quá hạn duyệt'    
    else if(this.document?.statusCode == 3){          
      this.documentApprovalService.GetSingleByUserIdAndDocId(this.currentUserId, this.docId!).subscribe(res => {
        if(res.isSuccess && res.result != null){        
          const t = res.result as DocumentApprovalModel;
          this.myApprovalStatus = t.statusCode == 4 ? 'Bạn đã duyệt tờ trình này' : t.statusCode == 5 ? 'Bạn đã duyệt và ý kiến tờ trình này' : t.statusCode == 6 ? 'Bạn không duyệt tờ trình này' : '';
        }
      })
    }
    console.log(this.myApprovalStatus);
    
  }

  loadComments(docId: number){
    this.commentService.getDocumentCommnets(docId).subscribe(res => {
      if(res.isSuccess){
        this.comments = res.result as CommentModel[];
        console.log(this.comments);        
      }
    })
  }

  viewFile(event: any){
    const selectedFileId = event.target.value;
    this.selectedDocument = this.document!.documentFiles!.find(x => x.id == selectedFileId);    
    this.urlToView = this.sanitizer.bypassSecurityTrustResourceUrl(this.selectedDocument?.filePathToView!);    
  }

  addComment(){
    if(this.newComment.length > 0){      
      const obj = {
        id: 0,
        docId: this.docId,
        comment: this.newComment,
        userId: this.currentUserId,
        userName: '',
        deleted: false,
        modified: new Date(),
        modifiedBy: this.currentUserId,
        created: new Date(),
        createdBy: this.currentUserId
      } as CommentModel;
      this.commentService.createComment(obj).subscribe(res => {
        if(res.isSuccess){
          this.newComment = '';
          this.toastr.success('Gửi ý kiến thành công')
          this.loadComments(this.docId)
        }
        console.log(res);
        
      })
    }
  }
  handleAction(status: number){
    var message = '';
    var response = '';    
    if(status == 4){
      message = `Xác nhận duyệt tờ trình ${this.document?.title}`;
      this.submitApproval(status, response, message)
    } else if(status = 5){
      message = `Xác nhận duyệt tờ trình ${this.document?.title}, ý kiến của bạn sẽ được đính kèm`
      const modalRef = this.modalService.open(CommentComponent);
      modalRef.componentInstance.title = 'Ý kiến bổ sung';
      modalRef.result.then(result => {
        console.log(result);
        
        response = result
        this.submitApproval(status, response, message)
      })
    } else if (status == 6){
      message = `Xác nhận không duyệt tờ trình ${this.document?.title}, phản hồi của bạn sẽ được đính kèm`
      const modalRef = this.modalService.open(CommentComponent);
      modalRef.componentInstance.title = 'Lý do từ chối duyệt';
      modalRef.result.then(result => {
        console.log(result);
        response = result;
        this.submitApproval(status, response, message)
      })
    }        
  }

  handleAction_General(status: number){
    var message = '';
    if(status == 4){
      message = `Xác nhận duyệt tờ trình ${this.document?.title}? Hành động này sẽ bỏ qua ý kiến của cán bộ duyệt`;
    }
    else if(status == 6){
      message = `Xác nhận trả lại tờ trình ${this.document?.title}? Hành động này sẽ bỏ qua ý kiến của cán bộ duyệt`;      
    }

    const modalRef = this.modalService.open(ConfirmationComponent);
    modalRef.componentInstance.message = message;
    modalRef.result.then(result => {
      if(result){        
        this.documentService.updateStatus(this.docId, status).subscribe(res => {
          if(res.isSuccess){
            this.toastr.success('Thay đổi trạng thái tờ trình thành công')
            this.navigateToComponentBWithParam();
          }
        })
      }
    })
  }

  submitApproval(status: number, response: string, message: string){
    const obj = {
      id: 0,
      title: status == 4 ? 'Duyệt' : status == 5 ? 'Duyệt và ý kiến' : 'Không duyệt',
      docId: this.docId,
      statusCode: status,
      userId: this.currentUserId,
      userName: '',
      modified: new Date(),
      deleted: false,
      modifiedBy: this.currentUserId,
      createdBy: this.currentUserId,
      created: new Date(),
      comment: response
    } as DocumentApprovalModel;

    const modalRef = this.modalService.open(ConfirmationComponent);
    modalRef.componentInstance.message = message;
    modalRef.result.then(result => {
      if(result){
        this.documentApprovalService.CreateDocumentApproval(obj).subscribe(res => {
          if(res.isSuccess){
            this.toastr.success("Xứ lý tờ trình thành công")
            this.navigateToComponentBWithParam()
          }
        })
      }
    })    
  }

  navigateToComponentBWithParam() {
    this.route.paramMap.subscribe(param => {
      const paramValue = param.get('statusCode');      
      this.router.navigate(['/admin/to-trinh', paramValue]);
    });
  }

  getStatusString(status: number){
    if(status == 3) return 'Chờ duyệt'
    else if(status == 4) return 'Đã duyệt'
    else if(status == 5) return 'Đã duyệt và ý kiến'
    else if(status == 6) return 'Không duyệt'
    else if(status == 7) return 'Quá hạn duyệt'
    else return '';
  }

  getStatusClass(status: number){
    if(status == 3) return 'bg-warning'
    else if(status == 4) return 'bg-success'
    else if(status == 5) return 'bg-primary'
    else if(status == 6) return 'bg-danger'
    else if(status == 7) return 'bg-secondary'
    else return '';
  }
}
