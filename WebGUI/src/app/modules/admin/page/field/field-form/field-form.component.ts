import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { FieldModel } from 'src/app/models/field.model';
import { FieldServiceService } from 'src/app/services/admin/field-service.service';
import * as common from 'src/app/utils/commonFunctions';

@Component({
  selector: 'app-field-form',
  templateUrl: './field-form.component.html',
  styleUrls: ['./field-form.component.css']
})
export class FieldFormComponent implements OnInit {

  userId = common.GetCurrentUserId();

  dataField: FieldModel = {
    id: 0,
    title: '',
    active: true,
    modified: new Date(),
    deleted: false,
    created: new Date(),
    createdBy: this.userId,
    modifiedBy: this.userId
  };

  constructor(private fieldService: FieldServiceService, private toastr: ToastrService, private router: Router){}
  ngOnInit(): void {
      
  }

  submit(sendToApprove: boolean) {
    var checkForm = this.dataField.title?.trim() == '';

    if (!checkForm) {            
      const addDataPromise = this.fieldService.create(this.dataField).subscribe((res) => {
        if (res.isSuccess) {
          
          this.toastr.success('Thêm danh mục thành công');
          this.router.navigate(['/admin/field'])
        } else {
          this.toastr.error(res.message)
        }
      });
    } else {
      this.toastr.warning('Cần nhập đầy đủ thông tin')
    }
  }
}
