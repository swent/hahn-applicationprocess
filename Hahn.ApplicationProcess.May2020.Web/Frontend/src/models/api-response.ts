/**
 * Api response model.
 * Represents an answer from backend api.
 * Can be initialized partially.
 */
export class ApiResponse {
  success: boolean;
  id?: number;
  uri?: string;
  errorKeys?: string[];

  public constructor(init?: Partial<ApiResponse>) {
    Object.assign(this, init);
  }
}
