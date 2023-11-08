import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environments';
import { BaseResponseModel } from '../models/baseResponse.model';
// import { TokenResponseModel, UserLogInModel, UserSignUpModel } from '../models/user.model';
import { ChangePasswordModel } from '../models/changePassword.model';
import {
  CreateAccount,
  GetAllInfoUserModel,
  TokenResponseModel,
  UserLogInModel,
  UserSignUpModel
} from '../models/user.model';
import {DocumentModel} from "../models/document.model";
import {CommentModel} from "../models/comment.model";

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private baseUrl = `${environment.apiUrl}/AppUser`
  constructor(private http: HttpClient) { }

  SignUp(payload: UserSignUpModel): Observable<BaseResponseModel<string>>{
    const apiUrl = `${this.baseUrl}/SignUp`;
    return this.http.post<BaseResponseModel<string>>(apiUrl, payload);
  }

  GetAllUserInfo() :  Observable<BaseResponseModel<GetAllInfoUserModel[]>> {
    const apiUrl = `${this.baseUrl}/GetAllUserInfo`;
    return this.http.get<BaseResponseModel<GetAllInfoUserModel[]>>(apiUrl);
  }

  LogIn(payload: UserLogInModel): Observable<BaseResponseModel<TokenResponseModel>>{
    const apiRul = `${this.baseUrl}/Login`;
    return this.http.post<BaseResponseModel<TokenResponseModel>>(apiRul, payload)
  }


  LogOut(){
    // localStorage.removeItem('UserInfo');
    localStorage.clear();
  }

  GetLocalStorageUserInfo(){
    const userInfo = localStorage.getItem('UserInfo');
    return userInfo;
  }

  GetCurrentUserId(){
    const userInfo = this.GetLocalStorageUserInfo();
    if(!!userInfo) return JSON.parse(userInfo).userId;
    return 0;
  }

  ChangePassword(payload: ChangePasswordModel): Observable<BaseResponseModel<ChangePasswordModel>>{
    const apiUrl = `${this.baseUrl}/ChangePassword`;
    return this.http.put<BaseResponseModel<ChangePasswordModel>>(apiUrl, payload)
  }

  ResetPassword(userId: string): Observable<BaseResponseModel<boolean>>{
    const apiUrl = `${this.baseUrl}/ResetPassword/${userId}`;
    return this.http.put<BaseResponseModel<boolean>>(apiUrl, null);
  }
}
