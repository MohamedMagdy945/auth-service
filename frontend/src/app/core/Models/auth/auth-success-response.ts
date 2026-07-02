export interface AuthSuccessResponse {
  userId: number;
  email: string;
  ImageUrl?: string;
  accessToken: string;
  accessTokenExpiration: string;
}