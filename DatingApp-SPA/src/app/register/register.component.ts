import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { registerLocaleData } from '@angular/common';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {


  // In order to receive properties into a child 
  // component from a parent we need to use an input. 
  @Input() valuesFromHome: any;

  // (event emitter imported specifically from angular core)
  // The cancelRegister output property emits the boolean event
  // 'false' value to communicate to the parent component.
  @Output() cancelRegister = new EventEmitter();



  model: any = {};

  // Inject AuthService service
  constructor(private authService: AuthService) { }

  ngOnInit() {
  }

  register() {
    this.authService.register(this.model).subscribe(() => {
      console.log('registration successful. User ' + this.model.username + ' registered');
    }, error => {
      // returns http error response received from server
      console.log('error registering: ' + error);
    
    });
  }


  // Emit a value (output) which will be received by parent
  // component (home component)
  cancel(){
    // This cancels the register request and forces the webpage to
    // return to the home page.
    this.cancelRegister.emit(false);
    console.log('cancelled');
  }
}
