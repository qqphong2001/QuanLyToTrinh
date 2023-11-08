import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Guid } from 'guid-typescript';
import * as moment from 'moment';
import { ToastrService } from 'ngx-toastr';
import { Subscription } from 'rxjs';
import { ConfirmationComponent } from 'src/app/components/confirmation/confirmation.component';
import { DocumentModel } from 'src/app/models/document.model';
import { DocumentApprovalModel } from 'src/app/models/documentApproval.model';
import { StatusCode } from 'src/app/models/enums/statusCode.enum';
import { FieldModel } from 'src/app/models/field.model';
import { DocumentApprovalService } from 'src/app/services/admin/document-approval.service';
import { DocumentService } from 'src/app/services/admin/document.service';
import { FieldServiceService } from 'src/app/services/admin/field-service.service';
import { NotificationService } from 'src/app/services/nofitication.service';
import * as common from 'src/app/utils/commonFunctions';

@Component({
  selector: 'app-to-trinh',
  templateUrl: './to-trinh.component.html',
  styleUrls: ['./to-trinh.component.scss'],
})
export class ToTrinhComponent implements OnInit {

  userRoles = common.GetRoleInfo();  
  
  userId = common.GetCurrentUserId();
  userInfo = common.GetCurrentUserInfo();
  specialistAction = true;
  approverAction = false;
  generalAction = false;
  approvalList: DocumentApprovalModel[] = [];

  waitForResponse = false;

  code: string | null = '';
  documents: DocumentModel[] = [];    
    
  constructor(
    private route: ActivatedRoute,
    private documentService: DocumentService,
    private router: Router,
    private fieldService: FieldServiceService,
    private toastr: ToastrService,
    private fb: FormBuilder,
    private approvalService: DocumentApprovalService,
    private notificationService: NotificationService,
    private modalService: NgbModal) {
    this.loadData();
  }

  loadData() {
    this.route.paramMap.subscribe((params) => {
      const param1 = params.get('statusCode');
      this.code = param1;
      if (param1 == null || param1 == '0') {
        this.getAllDocuments();
      } else {
        this.getAllDocumentsByStatusCode(param1);
      }
    });
  }

  getApprovalList() {
    var data = this.approvalService
      .GetIndividualApprovalList(this.userId)
      .subscribe((res) => {
        if (res.isSuccess) {
          this.approvalList = res.result;
          this.documents.forEach((e) => {
            var z = this.approvalList.find((x) => x.docId == e.id);
            if (z != null) {
              e.approverAction =
                z.statusCode == 2
                  ? 'Nháp'
                  : z.statusCode == 4
                  ? 'Đồng ý duyệt'
                  : z.statusCode == 5
                  ? 'Duyệt và ý kiến'
                  : z.statusCode == 6
                  ? 'Không duyệt'
                  : 'Chưa đánh giá';
              e.actionClass =
                z.statusCode == 2
                  ? 'text-bg-secondary'
                  : z.statusCode == 4
                  ? 'text-bg-success'
                  : z.statusCode == 5
                  ? 'text-bg-primary'
                  : z.statusCode == 6
                  ? 'text-bg-danger'
                  : '';
            } else {
              e.approverAction = 'Chưa đánh giá';
              e.actionClass = '';
            }
          });
        }
      });
  }

  ngOnInit() {       
    
  }


  getAllDocuments() {
    const dataPromise = this.documentService.getAll().subscribe((res) => {
      if (res.isSuccess) {
        this.documents = res.result as DocumentModel[];    
        if(this.userRoles.specialist && !this.userRoles.admin){
          this.documents = this.documents.filter(x => x.createdBy?.toLowerCase() == this.userId.toLowerCase())
        }else{
          this.documents = this.documents.filter(x => x.statusCode != 2)
        }
      }
    });
  }
  getAllDocumentsByStatusCode(statusCode: string | null) {
    const getDataPromise = this.documentService
      .getAllDocumentsByStatusCode(statusCode)
      .subscribe((res) => {
        if (res.isSuccess) {
          this.documents = res.result as DocumentModel[];    
          if(this.userRoles.specialist && !this.userRoles.admin){
            this.documents = this.documents.filter(x => x.createdBy?.toLowerCase() == this.userId.toLowerCase())
          }   
          else{
            this.documents = this.documents.filter(x => x.statusCode != 2)
          }             
          if(this.userRoles.approver){
            this.getApprovalList();            
          }
        }
      });
  }  

  remind(id: number){
    const modalRef = this.modalService.open(ConfirmationComponent);
    modalRef.result.then(result => {
        this.notificationService.CreateNotification(2, id).subscribe(res => {
          if(res.isSuccess){
            this.toastr.success("Thông báo sẽ được gửi tới cho cán bộ", "Nhắc duyệt thành công");
          }
        })
    })
  }

  updateStatus(docId: number, status: number){
    const modalRef = this.modalService.open(ConfirmationComponent);
    modalRef.result.then(result => {
      const updateStatusSub = this.documentService.updateStatus(docId, status).subscribe(res => {
        if(res.isSuccess){
          this.toastr.success('Cập nhật trạng thái tờ trình thảnh công')
          // this.router.navigate(['/admin/to-trinh/' + status]);  
          if(status == StatusCode.Pending){
            this.notificationService.CreateNotification(1, docId).subscribe(res => {
              if(res.isSuccess){
                this.toastr.info('Thông báo gửi duyệt đã được gửi cho cán bộ')
              }
            })
          }  
          this.loadData();    
        }
      })
    })
  }

  deleteDocument(docId: number){
    const modalRef = this.modalService.open(ConfirmationComponent);
    modalRef.result.then(result => {
    
      const deleteSub = this.documentService.delete(docId).subscribe(res => {
        if(res.isSuccess){
          this.toastr.success('Xóa tờ trình thảnh công')
          this.loadData();    
        }
      })
    })
  }

  actionOnItem = -1;
  toggleActions(docId: number) {
    if (this.actionOnItem == -1) {
      this.actionOnItem = docId;
    } else {
      this.actionOnItem = -1;
    }
  }

  //paignations

  itemsPerPage: number = 10;
  currentPage: number = 1;

  //-------------------
}

