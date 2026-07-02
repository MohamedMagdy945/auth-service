export interface ApiResponse<T> {
  isSuccess: boolean;
  message: string;
  errors: string[];
  data: T;
  statusCode: number;
}