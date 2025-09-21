import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from './services/user.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './register.component.html',
  styleUrls: ['./app.css']
})
export class RegisterComponent {
  firstName = '';
  lastName = '';
  username = '';
  password = '';
  message = '';
  loading = false;

  constructor(private authService: AuthService, private router: Router) { }

  onRegister(form: any) {
    if (!form || form.invalid) {
      this.message = 'Заполните все поля корректно';
      return;
    }

    this.loading = true;
    this.message = '';

    this.authService.register(this.firstName, this.lastName, this.username, this.password).subscribe({
      next: (res: any) => {
        // Регистрация успешна. Попробуем сразу залогинить пользователя для удобства.
        this.authService.login(this.username, this.password).subscribe({
          next: (loginRes: any) => {
            if (loginRes && loginRes.token) {
              this.authService.saveToken(loginRes.token);
              localStorage.setItem('user', JSON.stringify({ id: loginRes.id, username: loginRes.username }));
              this.message = '✅ Регистрация и вход прошли успешно';
              this.router.navigate(['/']);
            } else {
              this.message = '✅ Регистрация прошла успешно. Войдите, пожалуйста.';
              this.router.navigate(['/login']);
            }
            this.loading = false;
          },
          error: (loginErr) => {
            // регистрация успешна, но автологин не прошёл — отправим на страницу входа
            this.loading = false;
            this.message = '✅ Регистрация успешна. Пожалуйста, войдите.';
            this.router.navigate(['/login']);
          }
        });
      },
      error: (err) => {
        this.loading = false;
        this.message = err?.error?.message || '❌ Ошибка регистрации';
      }
    });
  }
}
