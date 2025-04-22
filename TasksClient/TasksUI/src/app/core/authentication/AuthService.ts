import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, of, throwError, BehaviorSubject } from 'rxjs';
import { tap, delay, catchError } from 'rxjs/operators';

interface User {
  id: number;
  name: string;
  email: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
 
  private currentUserSubject$ = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject$.asObservable();

 
  private readonly TOKEN_KEY = 'authToken';

  constructor(private router: Router) {
   
    const storedUser = localStorage.getItem(this.TOKEN_KEY);
    if (storedUser) {
      try {
        this.currentUserSubject$.next(JSON.parse(storedUser));
      } catch (error) {
        console.error('Error parsing user from localStorage', error);
        localStorage.removeItem(this.TOKEN_KEY); 
      }
    }
  }

  
  login(email: string, password: string): Observable<User> {
   
    return of({ id: 1, name: 'John Doe', email }) 
      .pipe(
        delay(1000),
        tap((user) => {
         
          localStorage.setItem(this.TOKEN_KEY, JSON.stringify(user));
          this.currentUserSubject$.next(user);
          this.router.navigate(['/tasks']); 
        }),
        catchError((error) => {
          
          console.error('Login failed:', error);
          return throwError(() => new Error('Invalid credentials. Please try again.'));
        })
      );
  }

 
  public get currentUser(): User | null {
    return this.currentUserSubject$.value;
  }


  isLoggedIn(): boolean {
    return !!localStorage.getItem(this.TOKEN_KEY);
  }

  
  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY); 
    this.currentUserSubject$.next(null);
    this.router.navigate(['/login']); 
  }

    getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }
}