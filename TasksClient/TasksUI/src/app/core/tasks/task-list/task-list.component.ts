import { Component, OnInit, OnDestroy, inject, ViewChild, AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { Subscription } from 'rxjs';

import { DataService } from '../../../core/services/data.service';
import { Task } from '../../../core/models/task.model';
import { TaskDialogComponent } from '../task-dialog/task-dialog.component';



@Component({
  selector: 'app-task-list',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatButtonModule,
    MatIconModule,
    MatDialogModule,
    MatProgressSpinnerModule,
    MatCheckboxModule,
    MatSortModule,
    MatPaginatorModule,
    MatFormFieldModule,
    MatInputModule,
    DatePipe
  ],
  templateUrl: './task-list.component.html',
  styleUrls: ['./task-list.component.scss']
})
export class TaskListComponent implements OnInit, OnDestroy, AfterViewInit {
  displayedColumns: string[] = ['isCompleted', 'title', 'description', 'dueDate', 'actions'];
  dataSource = new MatTableDataSource<Task>([]);
  isLoading = false;
  errorMessage: string | null = null;

  private dataService = inject(DataService);
  private dialog = inject(MatDialog);
  private tasksSubscription: Subscription = new Subscription;

  @ViewChild(MatSort) sort!: MatSort; 
  @ViewChild(MatPaginator) paginator!: MatPaginator; 

  constructor( private cdr: ChangeDetectorRef) {

  }

  ngOnInit(): void {
    this.loadTasks();
  }

  ngAfterViewInit(): void {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }

  ngOnDestroy(): void {
    if (this.tasksSubscription) {
      this.tasksSubscription.unsubscribe();
    }
  }

  loadTasks(): void {
    this.isLoading = true;
    this.errorMessage = null;
    this.tasksSubscription = this.dataService.getTasks().subscribe({
      next: (tasks) => {
        this.dataSource.data = tasks;
        this.isLoading = false;
        this.cdr.detectChanges();
        this.dataSource.sort = this.sort;
        this.dataSource.paginator = this.paginator;
        
      },
      error: (err) => {
        this.errorMessage = `Error loading tasks: ${err.message}`;
        this.isLoading = false;
        console.error(err);
      }
    });
  }

  applyFilter(event: Event): void {
    const filterValue = (event.target as HTMLInputElement).value;
    this.dataSource.filter = filterValue.trim().toLowerCase();

    if (this.dataSource.paginator) {
      this.dataSource.paginator.firstPage();
    }
  }

  openAddTaskDialog(): void {
    const dialogRef = this.dialog.open(TaskDialogComponent, {
      width: '450px',
      data: { task: null }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        console.log('Dialog result (add):', result);
        this.isLoading = true;
        this.dataService.addTask(result).subscribe({
          next: () => this.loadTasks(),
          error: (err) => {
            this.errorMessage = `Error adding task: ${err.message}`;
            this.isLoading = false;
            console.error(err);
          }
        });
      }
    });
  }

  openEditTaskDialog(task: Task): void {
    const dialogRef = this.dialog.open(TaskDialogComponent, {
      width: '450px',
      data: { task: { ...task } }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        console.log('Dialog result (edit):', result);
        this.isLoading = true;
        this.dataService.updateTask(result).subscribe({
          next: () => this.loadTasks(),
          error: (err) => {
            this.errorMessage = `Error updating task: ${err.message}`;
            this.isLoading = false;
            console.error(err);
          }
        });
      }
    });
  }

  deleteTask(taskId: string | number | undefined): void {
    if (!taskId) return;

    if (confirm('Are you sure you want to delete this task?')) {
      this.isLoading = true;
      this.dataService.deleteTask(taskId).subscribe({
        next: () => this.loadTasks(),
        error: (err) => {
          this.errorMessage = `Error deleting task: ${err.message}`;
          this.isLoading = false;
          console.error(err);
        }
      });
    }
  }

  toggleCompletion(task: Task): void {
    const updatedTask = { ...task, isCompleted: !task.isCompleted };
    this.isLoading = true;
    this.dataService.updateTask(updatedTask).subscribe({
      next: () => {
        const index = this.dataSource.data.findIndex(t => t.id === task.id);
        if (index > -1) {
          this.dataSource.data[index] = updatedTask;
          this.dataSource._updateChangeSubscription();
        }
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = `Error updating task status: ${err.message}`;
        task.isCompleted = !task.isCompleted;
        this.isLoading = false;
        console.error(err);
      }
    });
  }
}

