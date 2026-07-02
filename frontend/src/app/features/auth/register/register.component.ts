import { Component, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormsModule, Validators, ReactiveFormsModule, NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/Models/services/auth-services.service';
import { RegisterRequest } from '../../../core/Models/auth/register-request';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  credentials: RegisterRequest = {
    fullName: '',
    email: '',
    phoneNumber: '',
    password: '',
    confirmPassword: ''
  }; 
  isPasswordVisible = false;
  showPassword = signal(false);
  isLoading = false;
  errorMsg = '';

  togglePassword(): void {
    this.isPasswordVisible = !this.isPasswordVisible;
  }


  loginForm: FormGroup;
  private authService = inject(AuthService);

  constructor(private fb: FormBuilder, private router: Router) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      rememberMe: [false]
    });
  }

  onSubmit(form: NgForm) {
    if (form.invalid) return;

    this.isLoading = false;
    this.errorMsg = '';

    console.log('Attempting register with payload:', this.credentials);

    this.authService.register(this.credentials).subscribe({
      next: (response) => {
        this.isLoading = false;

        if (response.isSuccess) {
          console.log('Register successful:', response.message);

          this.router.navigate(['/home']);
        } else {
          this.errorMsg = response.message || 'Register failed. Please check your credentials.';
        }
      },
      error: (err) => {
        this.isLoading = false;
        console.error('Register request error:', err);

        this.errorMsg = err.error?.message || 'The email or password you entered is incorrect. Please try again.';
      }
    });
  }
}