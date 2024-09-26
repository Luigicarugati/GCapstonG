import { UserService } from './../services/user.service';
import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogConfig } from '@angular/material/dialog';
import { SignupComponent } from '../signup/signup.component';
import { ForgotPasswordComponent } from '../forgot-password/forgot-password.component';
import { LoginComponent } from '../login/login.component';
import { Route, Router } from '@angular/router';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
})
export class HomeComponent implements OnInit {
  constructor(
    private dialog: MatDialog,
    private userService: UserService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.userService.checktoken().subscribe(
      (Response: any) => {
        this.router.navigate(['/cafe/dashboard']);
      },
      (error: any) => {
        console.log(error);
      }
    );
  }

  handlerSignupAction() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.width = '600px';
    this.dialog.open(SignupComponent, dialogConfig);
  }

  handlerForgotPasswordAction() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.width = '600px';
    this.dialog.open(ForgotPasswordComponent, dialogConfig);
  }

  handlerloginAction() {
    const dialogConfig = new MatDialogConfig();
    dialogConfig.width = '600px';
    this.dialog.open(LoginComponent, dialogConfig);
  }
}
