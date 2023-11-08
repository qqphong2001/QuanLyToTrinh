import { Component, OnInit } from '@angular/core';
import {AdminMenu, ApproverMenu, SpecialistAdmin} from '../../config/admin-menu';
import { UserService } from 'src/app/services/user.service';
import { ActivatedRoute, Router } from '@angular/router';
import * as common from 'src/app/utils/commonFunctions';

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss']
})
export class AdminComponent implements OnInit {
    
  constructor(private userService: UserService, private router: Router, private route: ActivatedRoute){}
  showMobileMenu = false;
  menuItems = AdminMenu;
  showDetails = false;
  ngOnInit(): void {
    this.checkLogin();        
  }

  checkLogin(){
    const userInfo = this.userService.GetLocalStorageUserInfo();
    if(userInfo == null) this.router.navigate(['/auth']);
    else{
      var info = JSON.parse(userInfo);
      if(info.roles.includes('Approver')) {
        this.menuItems = ApproverMenu;
      }  
      if(info.roles.includes('Specialist')) {
        this.menuItems = SpecialistAdmin;
      }  
    }
  }

  toggleMobileMenu(){
    this.showMobileMenu = !this.showMobileMenu;
  }

  logOut(){
    if(confirm('Xác nhận đăng xuất')){
      this.userService.LogOut();
      this.router.navigate(['/auth'])
    }
  }

  toggle(){
    this.showDetails = !this.showDetails
  }
}
