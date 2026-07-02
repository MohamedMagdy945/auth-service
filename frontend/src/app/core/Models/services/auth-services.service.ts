import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { User } from '../auth/user';
import { AuthSuccessResponse } from '../auth/auth-success-response';
import { ApiResponse } from '../api-response.model';
import { LoginRequest } from '../auth/login-request';
import { RegisterRequest } from '../auth/register-request';

@Injectable({
    providedIn: 'root'
})
export class AuthService {

    private http = inject(HttpClient);

    private readonly TOKEN_KEY = 'access_token';

    private readonly API_URL = 'http://localhost:5158/api/auth';

    private currentUserSubject = new BehaviorSubject<User | null>(null);

    public currentUser$ = this.currentUserSubject.asObservable();

   

    login(credentials: LoginRequest): Observable<ApiResponse<AuthSuccessResponse>> {
        return this.http.post<ApiResponse<AuthSuccessResponse>>(
            `${this.API_URL}/login`,
            credentials,
            {
                withCredentials: true
            }
        ).pipe(
            tap(response => {
                if (!response.isSuccess || !response.data) {
                    return;
                }

                this.setAccessToken(response.data.accessToken);
                this.setCurrentUser(response.data.userId, response.data.email, response.data.ImageUrl);
            })
        );
    }

    register(credentials: RegisterRequest): Observable<ApiResponse<AuthSuccessResponse>> {
        return this.http.post<ApiResponse<AuthSuccessResponse>>(
            `${this.API_URL}/register`,
            credentials,
            {
                withCredentials: true
            }
        ).pipe(
            tap(response => {
                if (!response.isSuccess || !response.data) {
                    return;
                }

                this.setAccessToken(response.data.accessToken);
                this.setCurrentUser(response.data.userId, response.data.email, response.data.ImageUrl);
            })
        );
    }

    refreshToken(): Observable<ApiResponse<AuthSuccessResponse>> {
        return this.http.post<ApiResponse<AuthSuccessResponse>>(
            `${this.API_URL}/refresh-token`,
            {},
            { withCredentials: true }
        ).pipe(
            tap(response => {
                this.setAccessToken(response.data.accessToken);
                this.setCurrentUser(response.data.userId, response.data.email, response.data.ImageUrl);
            })
        );
    }

    logout(): void {
        localStorage.removeItem(this.TOKEN_KEY);
        this.currentUserSubject.next(null);
    }

    isLoggedIn(): boolean {
        return !!this.currentUserSubject.value;
    }

    getAccessToken(): string | null {
        return localStorage.getItem(this.TOKEN_KEY);
    }
    private setAccessToken(token: string): void {
        localStorage.setItem(this.TOKEN_KEY, token);
    }

    private setCurrentUser(userId: number, email: string, ImageUrl: string | undefined): void {
        this.currentUserSubject.next({
            userId: userId,
            email: email,
            ImageUrl: ImageUrl
        });
    }

    //   private loadCurrentUser(): void {
    //     const token = this.getAccessToken();
    //     if (token) {
    //       // هنا تقدر تحلل الـ JWT Token عشان تجيب الـ userId والـ email لو مش متخزنين
    //       // أو تكتفي بوضع التوكن في الـ state، كالأتي كـ شكل مبدئي:
    //       this.currentUserSubject.next({
    //         accessToken: token,
    //         userId: 0, // هتحتاج تفك الـ JWT لو عايز البيانات دي تفضل موجودة بعد الـ Refresh
    //         email: '',
    //         accessTokenExpiration: ''
    //       });
    //     }
    //   }
}