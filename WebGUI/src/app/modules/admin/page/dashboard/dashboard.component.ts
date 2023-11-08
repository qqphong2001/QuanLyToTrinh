import { Component, OnInit } from '@angular/core';
import { DocumentModel } from 'src/app/models/document.model';
import { DocumentService } from 'src/app/services/admin/document.service';
import * as common from 'src/app/utils/commonFunctions';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {

  userId = common.GetCurrentUserId();
  userInfo = common.GetCurrentUserInfo();
  documents: DocumentModel[] = [];
  dashboardInfo: any = {};

  specialistAction = true;
  approverAction = false;
  generalAction = false;

  constructor(private documentService: DocumentService){}
  
  ngOnInit(): void {
    if(this.userInfo.roles.includes('Admin')){
      this.specialistAction = true;
      this.approverAction = true;
      this.generalAction = true;
    }
    else if(!this.userInfo.roles.includes('Approver') && !this.userInfo.roles.includes('General Specialist')){
      this.specialistAction = true;
      this.approverAction = false;
      this.generalAction = false;
    }else{
      if(this.userInfo.roles.includes('General Specialist')){
        this.specialistAction = false;
        this.approverAction = false;
        this.generalAction = true;
      }
      if(this.userInfo.roles.includes('Approver')){
        this.generalAction = false;
        this.specialistAction = false;
        this.approverAction = true;
      }
    }
    this.getData();
  }

  getData(){
    this.documentService.getAll().subscribe(res => {
      if(res.isSuccess){
        this.documents = res.result;
        if(this.userInfo.roles.includes('Specialist')){
          this.documents = this.documents.filter(x => x.createdBy == this.userId);
        }          
        this.dashboardInfo.approvedCount = this.documents.filter(x => x.statusCode == 4 || x.statusCode == 5).length;
        this.dashboardInfo.declinedCount = this.documents.filter(x => x.statusCode == 6).length;
        this.dashboardInfo.pendingCount = this.documents.filter(x => x.statusCode == 3).length;   
      }
    })
  }

  itemsPerPage: number = 10;
  currentPage: number = 1;
}
