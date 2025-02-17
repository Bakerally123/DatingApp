import { HttpClient } from '@angular/common/http';
import { nullSafeIsEquivalent } from '@angular/compiler/src/output/output_ast';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import {map} from 'rxjs/operators';
import { User } from '../_models/user';
@Injectable({
  providedIn: 'root'
})
export class AccountService {
  model:User;
  baseUrl = 'https://localhost:5001/api/';
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http:HttpClient) { }

  login(model:any){
   return this.http.post(this.baseUrl + 'account/login', model).pipe(
     map( (response:User) => {
      console.log(response);
       const user = response;
       if(user) {
         localStorage.setItem('user',JSON.stringify(user));
         this.currentUserSource.next(user);
       }
     }      
     )
   ).subscribe(response=> { console.log(response); });
  }

  setCurrentUser(user:User){
    this.currentUserSource.next(user)
  }
  logout(){
    localStorage.removeItem('user');     
    this.currentUserSource.next(null);
  }
  register(model:any){
    return this.http.post(this.baseUrl + "account/register", model).pipe(
      map((response:User)=>{
        if(response){
          localStorage.setItem('user',JSON.stringify(response));
          this.currentUserSource.next(response);
        }
        
      })
    )
  }
}
