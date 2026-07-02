import { inject, Injectable } from "@angular/core";
import { AuthService } from "./auth-services.service";
import { catchError, firstValueFrom, map, of, tap } from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class AppInitializerService {

  private readonly authService = inject(AuthService);

    initialize(): Promise<void> {

  console.log('App Initializer Started');

  return firstValueFrom(
    this.authService.refreshToken().pipe(
      tap(() => {
        console.log('Refresh Success');
      }),
      map(() => void 0),
      catchError(err => {
        console.log('Refresh Failed', err);
        return of(void 0);
      })
    )
  );
}
}