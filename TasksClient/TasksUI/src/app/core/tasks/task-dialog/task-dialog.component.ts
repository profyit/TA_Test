import { Component, Inject, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatDatepickerModule } from '@angular/material/datepicker'; 
import { MatCheckboxModule } from '@angular/material/checkbox'; 
import { Task } from '../../../core/models/task.model';

@Component({
  selector: 'app-task-dialog',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatDatepickerModule, 
    MatCheckboxModule   
  ],
  templateUrl: './task-dialog.component.html',
  styleUrls: ['./task-dialog.component.scss']
})
export class TaskDialogComponent implements OnInit {
  taskForm: FormGroup | any ;
  isEditMode: boolean;

  private fb = inject(FormBuilder);

  constructor(
    public dialogRef: MatDialogRef<TaskDialogComponent>,
   
    @Inject(MAT_DIALOG_DATA) public data: { task: Task | null }
  ) {
    this.isEditMode = !!data?.task; 
  }

  ngOnInit(): void {
    this.initForm();
  }

  initForm(): void {
    const taskData = this.data?.task;
    this.taskForm = this.fb.group({
      title: [taskData?.title || '', [Validators.required, Validators.minLength(3)]],
      description: [taskData?.description || '', Validators.required],
      dueDate: [taskData?.dueDate ? new Date(taskData.dueDate) : null, Validators.required], 
      isCompleted: [taskData?.isCompleted || false] 
    });
  }

 
  onSave(): void {
    if (this.taskForm.valid) {
      const formData = this.taskForm.value;
      const resultData: Task = {
        ...this.data?.task, 
        ...formData,
         
      };
      this.dialogRef.close(resultData); 
    } else {
     
      this.taskForm.markAllAsTouched();
    }
  }


  onCancel(): void {
    this.dialogRef.close(); 
  }

  get title() { return this.taskForm.get('title'); }
  get description() { return this.taskForm.get('description'); }
  get dueDate() { return this.taskForm.get('dueDate'); }
}
