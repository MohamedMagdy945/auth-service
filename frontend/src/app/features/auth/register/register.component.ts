import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/Models/services/auth-services.service';
import { RegisterRequest } from '../../../core/Models/auth/register-request';

@Component({
  selector: 'app-register', // تعديل الـ selector ليكون معبراً عن الـ Register
  standalone: true,
  imports: [CommonModule, FormsModule], // شيلنا الـ ReactiveFormsModule لعدم الحاجة إليه
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  credentials: RegisterRequest = {
    fullName: '',
    email: '',
    phoneNumber: '',
    password: '',
    confirmPassword: ''
  }; 

  isPasswordVisible = false;
  isLoading = false;
  errorMessages: string[] = [];

  togglePassword(): void {
    this.isPasswordVisible = !this.isPasswordVisible;
  }

  onSubmit(form: NgForm) {
    if (form.invalid) return;

    if (this.credentials.password !== this.credentials.confirmPassword) {
      this.errorMessages = ['Passwords do not match.'];
      return;
    }

    this.isLoading = true;
    this.errorMessages = [];

    console.log('Attempting register with payload:', this.credentials);

    this.authService.register(this.credentials).subscribe({
      next: (response) => {
        this.isLoading = false;

        if (response.isSuccess) {
          console.log('Register successful:', response.message);
          this.router.navigate(['/auth/login']); // يفضل توجيهه للـ login أو الـ home مباشرة حسب الـ flow عندك
        } else {
          this.errorMessages.push(response.message || 'Register failed. Please check your inputs.');
        }
      },
      error: (err) => {
        this.isLoading = false;
        console.error('Register request error:', err);

        const apiError = err.error?.message || 'An unexpected error occurred. Please try again.';
        this.errorMessages.push(apiError);
      }
    });
  }
}