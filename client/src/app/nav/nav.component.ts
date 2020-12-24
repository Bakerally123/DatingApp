import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { User } from '../_models/user';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model:any = {}


  constructor(public accountService : AccountService) {
    
   }

  ngOnInit(): void {
    console.log(this.accountService.currentUser$);
  }

  login(){
    this.accountService.login(this.model);
  }

  logout(){
    this.accountService.logout();
  }

}
