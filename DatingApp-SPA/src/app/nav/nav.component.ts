import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
// import { ConsoleReporter } from 'jasmine';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model: any = {};

  // Inject auth service into constructor as arg
  constructor(private authService: AuthService) { }

  ngOnInit() {
  }

  login(){
    // Must subscribe to observables.
    // Login is successful or fails.
    this.authService.login(this.model).subscribe(next => {
      console.log('Logged in successfully');
    }, error => {
      console.log('Failed to login');
    });
  }

    // Checks localstorage for a token.
    // Returns true or false value; if there's
    // something in token, return T. Else, return F.
  loggedIn(){
    const token = localStorage.getItem('token');
  
    return !!token;
  }

  // Goes to localstorage and removes the token.
  logout(){ 
    localStorage.removeItem('token');
    console.log('Logged out.');
  }

}
