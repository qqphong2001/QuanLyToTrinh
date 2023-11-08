import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { BaseResponseModel } from 'src/app/models/baseResponse.model';
import { DocumentModel } from 'src/app/models/document.model';
import { DocumentApprovalModel, DocumentApprovalSummaryModel } from 'src/app/models/documentApproval.model';
import { environment } from 'src/environments/environments';

@Injectable({
  providedIn: 'root'
})
export class DocumentApprovalService {

  private baseUrl = `${environment.apiUrl}/DocumentApproval`;

  constructor(private http: HttpClient) { }

  GetSingleByUserIdAndDocId(userId: string, docId: number) : Observable<BaseResponseModel<DocumentApprovalModel>> {
    const apiUrl = `${this.baseUrl}/GetSingleByUserIdAndDocId/${userId}/${docId}`;
    return this.http.get<BaseResponseModel<DocumentApprovalModel>>(apiUrl);
  }
  
  CreateDocumentApproval(documentApproval : DocumentApprovalModel) : Observable<BaseResponseModel<DocumentApprovalModel>>{
    const apiUrl = `${this.baseUrl}/CreateDocumentApproval`;
    return this.http.post<BaseResponseModel<DocumentApprovalModel>>(apiUrl, documentApproval);
  }

  UpdateDocumentApproval(documentApproval : DocumentApprovalModel) : Observable<BaseResponseModel<DocumentApprovalModel>>{
    const apiUrl = `${this.baseUrl}/UpdateDocumentApproval`;
    return this.http.put<BaseResponseModel<DocumentApprovalModel>>(apiUrl, documentApproval);
  }
  
  GetApprovalSummary(docId: number): Observable<BaseResponseModel<DocumentApprovalSummaryModel[]>>{
    const apiUrl = `${this.baseUrl}/GetApprovalSummary/${docId}`;
    return this.http.get<BaseResponseModel<DocumentApprovalSummaryModel[]>>(apiUrl);
  }

  GetIndividualApprovalList(userId: string): Observable<BaseResponseModel<DocumentApprovalModel[]>>{
    const apiUrl = `${this.baseUrl}/GetIndividualDocumentApprovals/${userId}`;
    return this.http.get<BaseResponseModel<DocumentApprovalModel[]>>(apiUrl);
  }
}
