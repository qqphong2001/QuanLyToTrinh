import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-comment',
  templateUrl: './comment.component.html',
  styleUrls: ['./comment.component.css']
})
export class CommentComponent implements OnChanges {

  @Input() title = '';
  comment: string = '';
  constructor(private activeModal: NgbActiveModal){}
  ngOnInit(): void {
      
  } 

  ngOnChanges(changes: SimpleChanges): void {
      console.log(this.title);
      
  }
  closeModal(){
    this.activeModal.dismiss();
  }
  confirmAction(){    
    if(this.comment == ''){
      alert(`${this.title} không được để trống`)
    }
    else{
      this.activeModal.close(this.comment);
    }
  }  
}
