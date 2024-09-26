import { SnackbarService } from './../services/snackbar.service';
import { MatDialogRef } from '@angular/material/dialog';
import { UserService } from './../services/user.service';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { NgxUiLoaderService } from 'ngx-ui-loader';
import { Globalconstats } from '../shared/global-costants';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.scss',
})
export class ForgotPasswordComponent implements OnInit {
  ForgotPasswordform: any = FormGroup;
  responseMessage: any;

  constructor(
    private formBuilder: FormBuilder,
    private userService: UserService,
    public DialogRef: MatDialogRef<ForgotPasswordComponent>,
    private ngxService: NgxUiLoaderService,
    private snackbarService: SnackbarService
  ) {}

  ngOnInit(): void {
    this.ForgotPasswordform = this.formBuilder.group({
      email: [
        null,
        [Validators.required, Validators.pattern(Globalconstats.emailRegex)],
      ],
    });
  }

  handleSubmit() {
    this.ngxService.start();
    var formData = this.ForgotPasswordform.value;
    var data = {
      email: formData.email,
    };
    this.userService.forgotPassword(data).subscribe(
      (Response: any) => {
        this.ngxService.stop();
        this.responseMessage = Response?.messages;
        this.DialogRef.close();
        this.snackbarService.openSnackBar(this.responseMessage, '');
      },
      (error) => {
        this.ngxService.stop();
        if (error.error?.message) {
          this.responseMessage = error.error?.message;
        } else {
          this.responseMessage = Globalconstats.genericError;
        }
        this.snackbarService.openSnackBar(
          this.responseMessage,
          Globalconstats.error
        );
      }
    );
  }
}
