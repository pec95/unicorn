import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../models/user.model';

@Injectable({
  providedIn: 'root'
})
export class AuthorizationService {

  currentUser: string = 'none';
  userSubject: BehaviorSubject<boolean>;

  constructor(private http: HttpClient, private router: Router) {
    this.userSubject = new BehaviorSubject(false);
  }

  login(user: User) {
    this.http.post('https://localhost:44378/api/login/loginUser', user)
    .subscribe(res => {
      let loggedIn: boolean;
      
      if(res == true) {
        loggedIn = true;

        this.currentUser = user.UserName;
        localStorage.setItem('user', JSON.stringify(user.UserName));
      }
      else {
        loggedIn = false;
      }

      this.userSubject.next(loggedIn);
    });

    return this.userSubject;
  } 
    
  logout(){
    this.currentUser = 'none';
    
    localStorage.removeItem('user');
  
    this.router.navigate(['']);
  }

  getUser() {     
    let userStr = localStorage.getItem('user');

    let user: string;
    if(userStr != null) {
      user = JSON.parse(userStr);
    }
    else {
      user = 'none';
    }

    return user;
  }

}
