import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {


  registerMode = false;
  values: any;

  constructor(private http: HttpClient) { }

  ngOnInit() {
    this.getValues();
  }

  // Toggles registerMode as true. This is
  // toggled when the home page is in view.
  registerToggle(){
    this.registerMode = true;
  }

  cancel(){
  }

  getValues() {
    this.http.get('http://localhost:5000/api/values').subscribe(response => {
      this.values = response;
    }, error => {
      console.log(error);
    });
  }


  // Takes boolean arg. This is toggled with the boolean arg
  // (false) emitted when the cancel button is clicked, in 
  // order to  return the webpage from the register component 
  // to the home component.
  cancelRegisterMode(registerMode: boolean){
    this.registerMode = registerMode;
  }
}
