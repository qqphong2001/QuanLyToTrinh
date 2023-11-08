import { HttpClient } from '@angular/common/http';
import { Injectable, OnInit } from '@angular/core';
import * as moment from 'moment';
import { Observable } from 'rxjs';
import { BaseResponseModel } from 'src/app/models/baseResponse.model';
import { DocumentModel } from 'src/app/models/document.model';
import { environment } from 'src/environments/environments';

@Injectable({
  providedIn: 'root'
})
export class DocumentService{

  private baseUrl = `${environment.apiUrl}/Document`;

  constructor(private http: HttpClient) { }

  getAll() : Observable<BaseResponseModel<DocumentModel[]>> {
    const apiUrl = `${this.baseUrl}/GetAll`;
    return this.http.get<BaseResponseModel<DocumentModel[]>>(apiUrl);
  }
  
  getAllDocumentsByStatusCode(statusCode: string | null): Observable<BaseResponseModel<DocumentModel[]>> {
    const apiUrl = `${this.baseUrl}/GetAllDocument/${statusCode}`;
    return this.http.get<BaseResponseModel<DocumentModel[]>>(apiUrl);
  }

  getDocumentsById(id: number): Observable<BaseResponseModel<DocumentModel>> {
    const apiUrl = `${this.baseUrl}/GetDocumentById/${id}`;
    return this.http.get<BaseResponseModel<DocumentModel>>(apiUrl);
  }
  create(
    payload: DocumentModel,
    files: File[]
  ): Observable<BaseResponseModel<DocumentModel>> {
    const formData = new FormData();
    payload.dateEndApproval = payload.dateEndApproval;
    payload.created = payload.created;

    for (let i = 0; i < files.length; i++) {
      formData.append('files', files[i]);
    }
    formData.append('title', payload.title!);
    formData.append('note', payload.note!);
    formData.append('fieldId', payload.fieldId?.toString() || '');
    formData.append('dateEndApproval', payload.dateEndApproval!.toDateString());
    formData.append('statusCode', payload.statusCode + '');
    formData.append('modified', moment(payload.modified!).format('YYYY-MM-DD hh:mm:ss'));
    formData.append('modifiedBy', payload.modifiedBy + '');
    formData.append('created', moment(payload.created!).format('YYYY-MM-DD hh:mm:ss'));
    formData.append('createdBy', payload.createdBy + '');
    formData.append('deleted', payload.deleted + '');

    const apiUrl = `${this.baseUrl}/Create`;
    return this.http.post<BaseResponseModel<DocumentModel>>(apiUrl, formData);
  }

  update(payload: DocumentModel, files: File[]): Observable<BaseResponseModel<number>> {
    const formData = new FormData();    

    for (let i = 0; i < files.length; i++) {
      formData.append('files', files[i]);
    }
    formData.append('id', payload.id! + '');
    formData.append('title', payload.title!);
    formData.append('note', payload.note!);
    formData.append('fieldId', payload.fieldId?.toString() || '');
    formData.append('dateEndApproval', payload.dateEndApproval!.toDateString());
    formData.append('statusCode', payload.statusCode + '');
    // formData.append('modified', moment(payload.modified!).format('YYYY-MM-DD hh:mm:ss'));
    // formData.append('modifiedBy', payload.modifiedBy + '');
    // formData.append('created', moment(payload.created!).format('YYYY-MM-DD hh:mm:ss'));
    // formData.append('createdBy', payload.createdBy + '');
    //formData.append('deleted', payload.deleted + '');
    const apiUrl = `${this.baseUrl}/Update`;
    return this.http.put<BaseResponseModel<number>>(apiUrl, formData);
  }  
  

  updateStatus(docId: number, status: number): Observable<BaseResponseModel<number>>{
    const apiUrl = `${this.baseUrl}/UpdateStatus/${docId}/${status}`;
    return this.http.put<BaseResponseModel<number>>(apiUrl, null);
  }

  delete(docId: number): Observable<BaseResponseModel<number>>{
    const apiUrl = `${this.baseUrl}/Delete/${docId}`;
    return this.http.delete<BaseResponseModel<number>>(apiUrl);
  }
}
