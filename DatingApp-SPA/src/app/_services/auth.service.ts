import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';



// An injectable decorator allows us to inject services
// into things. Services require an injectable decorator,
// unlike components.
@Injectable({
  // Tells our service, and any components using this service
  // which module is providing this service. In this case
  // its the root module (in this case the 'app' module)
  providedIn: 'root'
})
export class AuthService {

  // URL path for authentication method
  baseUrl = 'http://localhost:5000/api/auth/';

  // Injects and makes use of the HttpClient service module
  constructor(private http: HttpClient) { }

  // Takes the model object passed from navbar
  login(model: any) {
    // POST login request ../login. Second arg
    // provides the model we are specifying. (we are not
    // specifying a third arg 'options' such as headers in this case)
    return this.http.post(this.baseUrl + 'login', model)
    // returned token observable must be passed through a
    // pipe method, allowing us to chain rxjs operators 
    .pipe(
      map((response: any) => {
        // Store token object returned as 'response'
        const user = response;
        if (user){
          localStorage.setItem('token', user.token);
        }
      })
    );
  }
}
