import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environments';
import { BaseResponseModel } from '../models/baseResponse.model';

import {DocumentModel} from "../models/document.model";
import {GetAllRoleModel} from "../models/role.model";

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private baseUrl = `${environment.apiUrl}/AppRole`
  constructor(private http: HttpClient) { }


  GetAllRoleInfo() :  Observable<BaseResponseModel<GetAllRoleModel[]>> {
    const apiUrl = `${this.baseUrl}/GetAllRole`;
    return this.http.get<BaseResponseModel<GetAllRoleModel[]>>(apiUrl);
  }




}
