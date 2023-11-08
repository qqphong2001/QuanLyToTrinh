import { Component, OnInit } from '@angular/core';

import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
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
  selector: 'app-document-form',
  templateUrl: './document-form.component.html',
  styleUrls: ['./document-form.component.css']
})
export class DocumentFormComponent implements OnInit {

  userRoles = common.GetRoleInfo();  
  
  userId = common.GetCurrentUserId();
  userInfo = common.GetCurrentUserInfo();
  waitForResponse = false;
  data: DocumentModel = {
    id: 0,
    title: '',
    note: '',
    fieldId: 0,
    dateEndApproval: new Date(),
    statusCode: StatusCode.Draft,
    deleted: false,
    created: new Date(),
    createdBy: this.userId,
    modified: new Date(),
    modifiedBy: this.userId,
  };
  selectedFileNamesArray: string[] = [];

  fieldData: FieldModel[] = [];
  CreateDate = moment(this.data.created).format('YYYY-MM-DDTHH:mm');
  DateEndApproval = moment(this.data.dateEndApproval).format('yyyy-MM-DD');
  files: File[] = [];
  selectedFileNames: string = 'Chưa có file nào được chọn.';
  selectImg: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private documentService: DocumentService,
    private router: Router,
    private fieldService: FieldServiceService,
    private toastr: ToastrService,    
    private approvalService: DocumentApprovalService,
    private notificationService: NotificationService,
    private modalService: NgbModal) {    
  }

  ngOnInit(): void {
      this.loadtenfield();
  }

  loadtenfield() {
    const getAllPromise = this.fieldService.getAll().subscribe((res) => {
      if (res.isSuccess) {
        this.fieldData = res.result as FieldModel[];
      } else {
        this.toastr.error(res.message);
      }
    });
  }

  onFileSelected(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    if (inputElement.files) {
      const newFiles = Array.from(inputElement.files);
      if (newFiles.length > 0) {
        this.files = this.files.concat(newFiles);
        const newFileNames = newFiles.map((file) => file.name);
        this.selectedFileNamesArray = this.selectedFileNamesArray.concat(newFileNames);
        this.selectImg = true;
      }
    }
  }

  moveFileUp(index: number): void {
    if (index > 0) {
      const temp = this.files[index];
      this.files[index] = this.files[index - 1];
      this.files[index - 1] = temp;
      this.updateSelectedFileNamesArray();
    }
  }
  
  moveFileDown(index: number): void {
    if (index < this.files.length - 1) {
      const temp = this.files[index];
      this.files[index] = this.files[index + 1];
      this.files[index + 1] = temp;
      this.updateSelectedFileNamesArray();
    }
  }
  
  deleteFile(index: number): void {
    this.files.splice(index, 1);
    this.updateSelectedFileNamesArray();
  }
  
  updateSelectedFileNamesArray(): void {
    this.selectedFileNamesArray = this.files.map((file) => file.name);
  }
  resetData(): void {
    this.data = {
      id: 0,
      title: '',
      note: '',
      fieldId: 0,
      dateEndApproval: new Date(),
      statusCode: StatusCode.Draft,
      deleted: false,
      created: new Date(),
      createdBy: this.userId,
      modified: new Date(),
      modifiedBy: this.userId,
    };
    this.selectedFileNamesArray = [];
    this.files = [];
  }

  submit(sendToApprove: boolean) {       
    this.data.dateEndApproval = moment(this.DateEndApproval).toDate();            
    var checkForm = this.data.title?.trim() == '' || this.data.fieldId == 0 || this.data.note?.trim() == '';
    if(!checkForm){
      if(sendToApprove){
        this.data.statusCode = StatusCode.Pending;
      }    
      const modalRef = this.modalService.open(ConfirmationComponent);
      modalRef.result.then(result => {        
        this.waitForResponse = true;
        
        const addDataPromise = this.documentService.create(this.data, this.files).subscribe((res) => {
          if (res.isSuccess) {     
            this.waitForResponse = false;    
                               
            var toastrMessage = '';
            if(this.data.statusCode == StatusCode.Pending){
              toastrMessage = 'Tờ trình đã được khởi tạo và gửi duyệt'
            }else{
              toastrMessage = 'Tờ trình đã được khởi tạo và tạm lưu'
            }
            this.toastr.success(toastrMessage);
            if(this.data.statusCode == StatusCode.Pending){
              this.notificationService.CreateNotification(1, res.result.id).subscribe(res => {
                if(res.isSuccess){
                  this.toastr.info('Thông báo gửi duyệt đã được gửi cho cán bộ')
                }
              })
            }
            this.navigateToComponentBWithParam();
          }else{
            this.waitForResponse = false;   
          }
        });                   
      })     
    }else{      
      this.toastr.warning('Cần nhập đầy đủ thông tin')
    }
  }  

  navigateToComponentBWithParam() {
    this.route.paramMap.subscribe(param => {
      const paramValue = param.get('statusCode');      
      this.router.navigate(['/admin/to-trinh', paramValue]);
    });
  }
}
