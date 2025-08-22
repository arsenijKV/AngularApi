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

  constructor(private authService: AuthService, private router: Router) { }

  onRegister() {
    this.authService.register(this.firstName, this.lastName, this.username, this.password).subscribe({
      next: (res) => {
        this.message = '✅ Регистрация прошла успешно';
        this.router.navigate(['/login']);
      },
      error: (err) => {
        this.message = err.error?.message || '❌ Ошибка регистрации';
      }
    });
  }
}
