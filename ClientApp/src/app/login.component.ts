import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms'; // для ngModel и ngForm
import { Router } from '@angular/router';
import { AuthService } from './services/user.service';

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
  loading = false;

  constructor(private authService: AuthService, private router: Router) { }

  onLogin(form: any) {
    if (!form || form.invalid) {
      this.message = 'Заполните все поля';
      return;
    }

    this.message = '';
    this.loading = true;

    this.authService.login(this.username, this.password).subscribe({
      next: (res: any) => {
        // AuthService.login уже вызывает saveToken внутри pipe, но на всякий случай:
        if (res && res.token) {
          this.authService.saveToken(res.token);
          localStorage.setItem('user', JSON.stringify({ id: res.id, username: res.username }));
          this.message = '✅ Успешный вход';
          this.router.navigate(['/']);
        } else {
          this.message = 'Сервер вернул некорректный ответ';
        }
        this.loading = false;
      },
      error: (err) => {
        this.loading = false;
        this.message = err?.error?.message || '❌ Ошибка входа';
      }
    });
  }

  goToRegister() {
    this.router.navigate(['/register']);
  }
}
