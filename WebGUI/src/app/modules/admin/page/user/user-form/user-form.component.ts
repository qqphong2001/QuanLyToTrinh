import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { GetAllRoleModel } from 'src/app/models/role.model';
import { UserSignUpModel } from 'src/app/models/user.model';
import { RoleService } from 'src/app/services/role.service';
import { UserService } from 'src/app/services/user.service';

@Component({
  selector: 'app-user-form',
  templateUrl: './user-form.component.html',
  styleUrls: ['./user-form.component.css']
})
export class UserFormComponent implements OnInit {

  RoleAll : GetAllRoleModel[] = [];
  CreateAccount1 : UserSignUpModel ={
  username : "",
    password : "",
    fullName : "",
    email : "",
    roleIds: [],
    phoneNumber : ""
  }
  constructor(private UserService: UserService ,private  RoleService: RoleService ,private toastr: ToastrService, private router: Router){

  }

  ngOnInit(): void {
      this.getAllRole();
  }

  onRoleChange(event: any, roleId: string) {
      if (event.target.checked) {
        // Nếu checkbox được chọn, thêm roleId vào mảng roleIds
        // this.CreateAccount1.roleIds.push(roleId);
        this.CreateAccount1.roleIds = [roleId];
      } else {
        // Nếu checkbox bị hủy chọn, xóa roleId khỏi mảng roleIds
        const index = this.CreateAccount1.roleIds.indexOf(roleId);
        if (index !== -1) {
            this.CreateAccount1.roleIds.splice(index, 1);
        }
      }
    }

    isRoleSelected(roleId: string): boolean {
      return this.CreateAccount1.roleIds.includes(roleId);
    }
    CreateAccount(){    
      const addDate = this.UserService.SignUp(this.CreateAccount1).subscribe(res =>{
        var toastrMessage = ''
      if (res.isSuccess){
        toastrMessage = 'Tạo tài khoản thành công';          
        this.toastr.success(toastrMessage);
        this.router.navigate(['/admin/User']);
      }
      else {
        toastrMessage = "Tạo tài khoản thất bại";
        this.toastr.error(toastrMessage);
        this.toastr.error(res.message);
      }
    })    
  }
  getAllRole(){
    const dataPromise = this.RoleService.GetAllRoleInfo().subscribe(res => {
      if(res.isSuccess){
        this.RoleAll = res.result as GetAllRoleModel[];
      }
    })
  }
}
