import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; // для ngModel
import { AuthService } from './services/user.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrls: ['./app.css']
})
export class LoginComponent {
  username = '';
  password = '';
  message = '';

  constructor(private authService: AuthService, private router: Router) { }

  onLogin() {
    this.authService.login(this.username, this.password).subscribe({
      next: (res) => {
        this.message = '✅ Успешный вход';
        localStorage.setItem('user', JSON.stringify(res));
        this.router.navigate(['/']); // после логина можно редиректнуть на главную
      },
      error: (err) => {
        this.message = err.error?.message || '❌ Ошибка входа';
      }
    });
  }
  goToRegister() {
    this.router.navigate(['/register']);
  }
}
