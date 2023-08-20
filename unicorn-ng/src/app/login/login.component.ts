import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { BehaviorSubject, Subscription } from 'rxjs';
import { User } from '../models/user.model';
import { AuthorizationService } from '../services/authorization.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  user: string = '';
  error: string = '';

  loggedIn: boolean = false;

  // pbkdf2 = require('pbkdf2');

  loginForm: FormGroup;
  userSubject: BehaviorSubject<boolean>;

  subscriptionLogin: Subscription = new Subscription();
  
  constructor(private authorizationService: AuthorizationService, private fb: FormBuilder, private router: Router) { 
    this.loginForm = this.fb.group({
      'username': new FormControl('', [Validators.required]),
      'password': new FormControl('', [Validators.required])
    });

    this.userSubject = new BehaviorSubject(false);
  }

  ngOnInit(): void {
    this.user = this.authorizationService.getUser();
  }

  login() {
    let username = this.loginForm.controls['username'];
    let password = this.loginForm.controls['password'];

    if(!username.valid) {
      this.error = 'Unesite i korisničko ime i lozinku';
    }
    else if(!password.valid) {
      this.error = 'Unesite i korisničko ime i lozinku';
    }
    else {
      this.error = '';

      //let key = this.pbkdf2.pbkdf2Sync(username.value, 'salt', 10000, 256, 'sha256');

      let user = new User(username.value, password.value);

      this.userSubject = this.authorizationService.login(user);

      this.userSubject.subscribe(res => { 
        this.loggedIn = res;

        if(this.loggedIn) {
          this.user = user.UserName;

          this.error = '';
      
          if(this.user === 'sales' || this.user === 'customer') this.router.navigate(['sales']);
          else if(this.user === 'warehouse') this.router.navigate(['warehouse']);
        }
        else {
          this.user = 'none';
          this.error = 'Pogreška kod prijave! Pokušajte ponovo';
        }
      });
    }
  }

  ngOnDestroy(): void {
    this.subscriptionLogin.unsubscribe()
  }

}
