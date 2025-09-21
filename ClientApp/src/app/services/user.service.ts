// src/app/services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { map } from 'rxjs/operators';
import { Observable } from 'rxjs';

interface AuthResponse {
  token: string;
  id?: number;
  username?: string;
  // другие поля, которые присылает сервер
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private base = 'http://localhost:5000';
  constructor(private http: HttpClient) { }

  login(username: string, password: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.base}/auth/authenticate`, { username, password })
      .pipe(map(res => {
        if (res && (res as any).token) {
          this.saveToken((res as any).token);
        }
        return res;
      }));
  }

  register(firstName: string, lastName: string, username: string, password: string) {
    return this.http.post(`${this.base}/auth/register`, { firstName, lastName, username, password });
  }

  saveToken(token: string) {
    localStorage.setItem('token', token);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  }

  // ещё полезные методы
  isLogged(): boolean {
    return !!this.getToken();
  }
}
