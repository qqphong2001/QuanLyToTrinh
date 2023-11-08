import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { DocumentApprovalSummaryModel } from 'src/app/models/documentApproval.model';
import { DocumentApprovalService } from 'src/app/services/admin/document-approval.service';

@Component({
  selector: 'app-approval-summary',
  templateUrl: './approval-summary.component.html',
  styleUrls: ['./approval-summary.component.css']
})
export class ApprovalSummaryComponent implements OnInit {

  docId: number = 0;
  summaryData: DocumentApprovalSummaryModel[] = [];
  constructor(private approvalService: DocumentApprovalService, private route: ActivatedRoute){}

  ngOnInit(): void {
      this.getParamValue('id');
      this.loadData();
  }

  getParamValue(key: string){
    if(this.route.snapshot.paramMap.get(key) != null) {
      this.docId = parseInt(this.route.snapshot.paramMap.get('id')!);      
    }
  }

  loadData(){
    this.approvalService.GetApprovalSummary(this.docId).subscribe(res => {            
      if(res.isSuccess){
        this.summaryData = res.result as DocumentApprovalSummaryModel[];        
        this.summaryData.forEach(e => {
          var t = Math.max(1, e.approvals.length, e.declines.length, e.noResponses.length);
          e.maxRows = t;
        })
      }
    })
  }
}
