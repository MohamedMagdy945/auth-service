import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/Models/services/auth-services.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './login.component.html',
  styleUrl: './login.component.css'
})
export class LoginComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  credentials = {
    email: '',
    password: '',
    rememberMe: false
  };

  isPasswordVisible = false;
  isLoading = false;
  errorMessages: string[] = [];

  togglePassword(): void {
    this.isPasswordVisible = !this.isPasswordVisible;
  }

  onSubmit(form: NgForm) {
    if (form.invalid) return;

    this.isLoading = true; 
    this.errorMessages = [];

    console.log('Attempting login with payload:', this.credentials);

    this.authService.login(this.credentials).subscribe({
      next: (response) => {
        this.isLoading = false;

        if (response.isSuccess) {
          console.log('Login successful:', response.message);
          this.router.navigate(['/dashboard']); 
        } else {
          this.errorMessages.push(response.message || 'Login failed. Please check your credentials.');
        }
      },
      error: (err) => {
        this.isLoading = false;
        console.error('Login request error:', err);

        const apiError = err.error?.message || 'The email or password you entered is incorrect. Please try again.';
        this.errorMessages.push(apiError);
      }
    });
  }
}