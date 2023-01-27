import { ChangeDetectionStrategy, Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserInfo } from './user-info';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.css']
})
export class AuthComponent implements OnInit {

  constructor(private readonly httpClient: HttpClient) { }

  public userInfo: UserInfo = {
    claims: [],
    isAuthenticated: false,
    scheme: ''
  };

  public login: string;
  public password: string;

  ngOnInit() {
    this.loadUserInfo();
  }

  public loadUserInfo(): void {
    this
      .httpClient
      .post('protected/getUserInfo', null)
      .subscribe((userInfo: UserInfo) => {
        this.userInfo = userInfo;
      });
  }

  public logout(): void {
    this
      .httpClient
      .post('auth/logout', null)
      .subscribe(result => {
        this.loadUserInfo();
      });
  }

  public authenticate(login: string, password: string): void {
    this
      .httpClient
      .post(`auth/login`, { login: login, password: password })
      .subscribe(result => {
        if (result) {
          alert('Успех');

          this.loadUserInfo();
        } else {
          alert('Ошибка');
        }
      });
  }

  public checkAnonymousRoute(): void {
    this
      .httpClient
      .post('protected/method', null)
      .subscribe(result => alert(result), error => alert(error.status));
  }

  public checkCookiesProtectedRoute(): void {
    this
      .httpClient
      .post('protected/methodRequiringAuthorization', null)
      .subscribe(result => alert(result), error => alert(error.status));
  }
}
