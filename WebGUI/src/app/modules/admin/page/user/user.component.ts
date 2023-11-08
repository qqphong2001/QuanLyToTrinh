import {Component, OnInit} from '@angular/core';
import * as common from "../../../../utils/commonFunctions";
import {DocumentApprovalModel} from "../../../../models/documentApproval.model";
import {ActivatedRoute, Router} from "@angular/router";
import {DocumentService} from "../../../../services/admin/document.service";
import {FieldServiceService} from "../../../../services/admin/field-service.service";
import {ToastrService} from "ngx-toastr";
import {FormBuilder} from "@angular/forms";
import {DocumentApprovalService} from "../../../../services/admin/document-approval.service";
import {NotificationService} from "../../../../services/nofitication.service";
import {UserService} from "../../../../services/user.service";
import {CreateAccount, GetAllInfoUserModel, UserSignUpModel} from "../../../../models/user.model";
import {Observable} from "rxjs";
import {GetAllRoleModel} from "../../../../models/role.model";
import {RoleService} from "../../../../services/role.service";
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ConfirmationComponent } from 'src/app/components/confirmation/confirmation.component';

@Component({
  selector: 'app-user',
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.scss']
})
export class UserComponent implements OnInit{

  UserInfoAll : GetAllInfoUserModel[] = [];

  constructor(private UserService : UserService,private  RoleService : RoleService,private toastr: ToastrService, private modalService: NgbModal) {

  }
  userId = common.GetCurrentUserId();
  specialistAction = true;

  ngOnInit(): void {
     this.getAllUser()    
  }

  getAllUser (){
    const dataPromise = this.UserService.GetAllUserInfo().subscribe(res =>{
      if(res.isSuccess){
        this.UserInfoAll = res.result as GetAllInfoUserModel[];
      }
    })
  }
  
  resetPassword(user: GetAllInfoUserModel){
    const modalRef = this.modalService.open(ConfirmationComponent);
    modalRef.componentInstance.message = 'Mật khẩu của tài khoản này sẽ được tự động đổi thành Canbo@1234';
    modalRef.result.then(result => {
      if(result){
        this.UserService.ResetPassword(user.userId).subscribe(res => {
          if(res.isSuccess){
            this.toastr.success(`Mật khẩu của tài khoản ${user.userName} - [${user.userFullName}] được thay đổi thành`)
          }
        })
      }
    })
  }
  

  isModalOpen: boolean = false;
  isModallOpen: boolean = false;
  openModal() {
    this.isModalOpen = true;
  }
  closeModal() {
    this.isModalOpen = false;
  }
    
  waitForResponse = false;
  itemsPerPage: number = 10;
  currentPage: number = 1;
}

