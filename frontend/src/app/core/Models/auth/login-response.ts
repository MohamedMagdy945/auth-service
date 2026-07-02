export interface LoginResponse {
  userId: number;
  email: string;
  accessToken: string;
  accessTokenExpiration: string;
}