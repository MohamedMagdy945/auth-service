import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { LoginResponse, UserData } from './auth.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  // استخدام inject() المتوافق مع نسخ Angular الحديثة
  private http = inject(HttpClient); 
  private readonly TOKEN_KEY = 'access_token';
  private readonly API_URL = 'https://api.yourdomain.com/auth'; // غير الرابط حسب الـ API عندك

  // BehaviorSubject لحفظ بيانات المستخدم الحالية وتحديث الـ Components فوراً
  private currentUserSubject = new BehaviorSubject<UserData | null>(null);
  // Observable مكشوف للـ Components عشان تعمل subscribe عليه
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor() {
    // أول ما الخدمة تشتغل، بنشوف هل فيه توكن قديم متسجل ولا لأ
    this.loadCurrentUser();
  }

  // الـ Login Method الاحترافية
  login(credentials: { email: string; password: string }): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${this.API_URL}/login`, credentials).pipe(
      tap(response => {
        if (response.isSuccess && response.data?.accessToken) {
          localStorage.setItem(this.TOKEN_KEY, response.data.accessToken);
          this.currentUserSubject.next(response.data);
        }
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

  private loadCurrentUser(): void {
    const token = this.getAccessToken();
    if (token) {
      // هنا تقدر تحلل الـ JWT Token عشان تجيب الـ userId والـ email لو مش متخزنين
      // أو تكتفي بوضع التوكن في الـ state، كالأتي كـ شكل مبدئي:
      this.currentUserSubject.next({
        accessToken: token,
        userId: 0, // هتحتاج تفك الـ JWT لو عايز البيانات دي تفضل موجودة بعد الـ Refresh
        email: '',
        accessTokenExpiration: ''
      });
    }
  }
}