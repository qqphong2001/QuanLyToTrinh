import { Component, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Guid } from 'guid-typescript';
import { CommentModel } from 'src/app/models/comment.model';
import { DocumentModel } from 'src/app/models/document.model';
import { DocumentApprovalModel } from 'src/app/models/documentApproval.model';
import { StatusCode } from 'src/app/models/enums/statusCode.enum';
import { CommentService } from 'src/app/services/admin/comment.service';
import { DocumentApprovalService } from 'src/app/services/admin/document-approval.service';
import { DocumentService } from 'src/app/services/admin/document.service';
import { environment } from 'src/environments/environments';
import * as common from 'src/app/utils/commonFunctions';
import { ToastrService } from 'ngx-toastr';
import * as moment from 'moment';
import { NotificationService } from 'src/app/services/nofitication.service';
import { FieldModel } from 'src/app/models/field.model';
import { FieldServiceService } from 'src/app/services/admin/field-service.service';

@Component({
  selector: 'app-xu-ly',
  templateUrl: './xu-ly.component.html',
  styleUrls: ['./xu-ly.component.scss']
})
export class XuLyComponent implements OnInit, OnChanges {

  userId = common.GetCurrentUserId();
  userRoles = common.GetCurrentUserInfo().roles;

  isAuthor = false;

  approvalAction = true;
  specialistAction = false;
  generalAction = false;

  docId: number | undefined | null;
  document: DocumentModel | undefined | null;

  DateEndApproval: string = '';
  files: File[] = [];
  selectedFileNames: string = 'Chưa có file nào được chọn.';
  selectImg:boolean = false;

  documentApproval : DocumentApprovalModel = {
    id: 0,
    title: '',
    docId: 0,
    statusCode: 0,
    userId: this.userId,
    modified: new Date(),
    deleted: false,
    modifiedBy: this.userId,
    createdBy: this.userId,
    created: new Date(),
    comment: ''
  };

  isShow :boolean = false;

  fieldData: FieldModel[] = [];

  constructor(private route: ActivatedRoute, 
    private documentService: DocumentService, 
    private router: Router, 
    private documentApprovalService: DocumentApprovalService,
    private commentService: CommentService,
    private toastr: ToastrService,
    private notificationService: NotificationService,
    private fieldService: FieldServiceService) {

  }
  get selectedFileNamesArray(): string[] {
    return this.selectedFileNames.split(', ');
  }
  onFileSelected(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    if (inputElement.files) {
      this.files = Array.from(inputElement.files);
      if (this.files.length > 0) {
        this.selectedFileNames = this.files.map(file => file.name).join(';');
        this.selectImg = true;
      } else {
        this.selectedFileNames = 'Chưa có file nào được chọn.';
      }
    }
  }

  loadFieldData(){
    this.fieldService.getAll().subscribe(res => {
      if(res.isSuccess){
        this.fieldData = res.result as FieldModel[];
      }
    })
  }

  submit(send: boolean) {
    if(this.document){
      this.document.dateEndApproval = moment(this.DateEndApproval).toDate();                
      var checkForm = this.document.title?.trim() == '' || this.document.fieldId == 0 || this.document.note?.trim() == '';
      if(!checkForm){    
        if(send){
          this.document.statusCode = 3; 
        }   
        const addDataPromise = this.documentService.update(this.document, this.files).subscribe((res) => {
          if (res.isSuccess) {                         
            this.toastr.success("Gửi thành công");
            // this.getDocumentById(this.docId!);
            this.navigateToComponentBWithParam();
          }else{
            this.toastr.error(res.message)
          }
        });                   
      }else{
        this.toastr.warning('Cần nhập đầy đủ thông tin')
      }
    }
  }
  ngOnInit(): void {
    this.loadFieldData();
    if(this.userRoles.includes('Approver')){
      this.approvalAction = true;
      this.specialistAction = false;
      this.generalAction = false;
    } 
    if(this.userRoles.includes('Specialist')){
      this.approvalAction = false;
      this.specialistAction = true;
      this.generalAction = false;
    }
    if(this.userRoles.includes('General Specialist')){
      this.approvalAction = false;
      this.specialistAction = false;
      this.generalAction = true;
    }
    if(this.userRoles.includes('Admin')){
      this.approvalAction = true;
      this.specialistAction = true;
      this.generalAction = true;
    }
    this.route.params.subscribe(params => {      
      const id = params['id'];
      this.docId = parseInt(this.route.snapshot.paramMap.get('id')!);
      this.getDocumentById(this.docId!);
      this.GetApprovalData();
    });
    // if(this.route.snapshot.paramMap.get('id') != null) {
    //   this.docId = parseInt(this.route.snapshot.paramMap.get('id')!);
    //   this.getDocumentById(this.docId!);
    //   this.GetApprovalData();
    // }    
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(this.route.snapshot.paramMap.get('id') != null) {
      this.docId = parseInt(this.route.snapshot.paramMap.get('id')!);
      this.getDocumentById(this.docId!);
      this.GetApprovalData();
    }  
  }

  navigateToComponentBWithParam() {
    this.route.paramMap.subscribe(param => {
      const paramValue = param.get('statusCode');      
      this.router.navigate(['/admin/to-trinh', paramValue]);
    });
  }

  getDocumentById(id: number) {
    
    const dataPromise = this.documentService.getDocumentsById(id).subscribe(res => {
      if (res.isSuccess) {        
        this.document = res.result as DocumentModel;
        this.isShow = true;        
        this.document.documentFiles!.forEach(x => {
          x.filePath = environment.hostUrl + x.filePath;
          x.filePathToView = environment.hostUrl + x.filePathToView;         
        })
        if(this.document.createdBy?.toString().toLowerCase() === this.userId.toLowerCase()){
          this.isAuthor = true;
        }
        this.DateEndApproval = moment(this.document.dateEndApproval).format('yyyy-MM-DD');
      }else{
        this.navigateToComponentBWithParam();
      }
    });
  }

  GetApprovalData(){
    this.documentApprovalService.GetSingleByUserIdAndDocId(this.userId, this.docId!).subscribe(res => {
      if(res.isSuccess){        
        this.documentApproval = res.result;
      }
    })
  }

  // Xu ly form documentApproval
  Approved() {
    this.documentApproval.docId = this.docId!;
    this.documentApproval.statusCode = 4;
    this.documentApproval.title = 'Duyệt'
    this.documentApprovalService.UpdateDocumentApproval(this.documentApproval).subscribe(res => {
      if(res.isSuccess){
        this.toastr.success('Đánh giá tờ trình thành công');        
        this.navigateToComponentBWithParam();
      }
    })
  }

  ApprovedAndIdea() {
    this.documentApproval.docId = this.docId!;
    this.documentApproval.statusCode = 5;
    this.documentApproval.title = 'Duyệt và ý kiến'
    this.documentApprovalService.UpdateDocumentApproval(this.documentApproval).subscribe(res => {
      if(res.isSuccess){
        this.toastr.success('Đánh giá tờ trình thành công');
        this.notificationService.CreateNotification(3, this.docId!, this.userId).subscribe(res => {
          if(res.isSuccess){
            this.toastr.info('Đánh giá đã được gửi đến chuyên viên');
          }
        })
        this.navigateToComponentBWithParam();
      }
    })
  }

  NotApproved() {
    this.documentApproval.docId = this.docId!;
    this.documentApproval.statusCode = 6;
    this.documentApproval.title = 'Không duyệt'
    this.documentApprovalService.UpdateDocumentApproval(this.documentApproval).subscribe(res => {
      if(res.isSuccess){
        this.toastr.success('Đánh giá tờ trình thành công');
        this.navigateToComponentBWithParam();
      }
    })
  }

  TemporarySave() {
    this.documentApproval.docId = this.docId!;
    this.documentApproval.statusCode = 2;
    this.documentApproval.title = 'Lưu tạm'
    this.documentApprovalService.UpdateDocumentApproval(this.documentApproval).subscribe(res => {
      if(res.isSuccess){
        this.toastr.success('Đánh giá tờ trình thành công');
        this.navigateToComponentBWithParam();
      }
    })
  }
}
