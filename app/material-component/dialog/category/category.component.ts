// category.component.ts
import { Component, EventEmitter, Inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { SnackbarService } from 'src/app/services/snackbar.service';
import { CategoryService } from 'src/app/services/category.service';
import { Globalconstats } from '../../../shared/global-costants';

@Component({
  selector: 'app-category',
  templateUrl: './category.component.html',
  styleUrls: ['./category.component.scss'],
})
export class CategoryComponent implements OnInit {
  onAddCategory = new EventEmitter();
  onEditCategory = new EventEmitter();
  categoryForm: FormGroup;
  dialogAction = 'Add';
  action = 'Add';
  responseMessage = '';

  constructor(
    @Inject(MAT_DIALOG_DATA) public dialogData: any,
    private formBuilder: FormBuilder,
    private categoryService: CategoryService,
    public dialogRef: MatDialogRef<CategoryComponent>,
    private snackBarService: SnackbarService
  ) {
    this.categoryForm = this.formBuilder.group({
      name: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    if (this.dialogData?.action === 'Edit') {
      this.dialogAction = 'Edit';
      this.action = 'Update';
      this.categoryForm.patchValue(this.dialogData.data);
    }
  }

  handleSubmit(): void {
    if (this.categoryForm.valid) {
      if (this.dialogAction === 'Edit') {
        this.edit();
      } else {
        this.add();
      }
    }
  }

  add(): void {
    const formData = this.categoryForm.value;
    this.categoryService.add(formData).subscribe(
      (response: any) => {
        this.dialogRef.close();
        this.onAddCategory.emit();
        this.responseMessage = response.message;
        this.snackBarService.openSnackBar(this.responseMessage, 'success');
      },
      (error) => {
        this.dialogRef.close();
        console.error(error);
        this.responseMessage =
          error.error?.message || Globalconstats.genericError;
        this.snackBarService.openSnackBar(
          this.responseMessage,
          Globalconstats.error
        );
      }
    );
  }

  edit(): void {
    const formData = this.categoryForm.value;
    this.categoryService.update(formData).subscribe(
      (response: any) => {
        this.dialogRef.close();
        this.onEditCategory.emit();
        this.responseMessage = response.message;
        this.snackBarService.openSnackBar(this.responseMessage, 'success');
      },
      (error) => {
        this.dialogRef.close();
        console.error(error);
        this.responseMessage =
          error.error?.message || Globalconstats.genericError;
        this.snackBarService.openSnackBar(
          this.responseMessage,
          Globalconstats.error
        );
      }
    );
  }
}
