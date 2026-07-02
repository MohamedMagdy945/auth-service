export interface AuthSuccessResponse {
  userId: number;
  email: string;
  accessToken: string;
  accessTokenExpiration: string;
}