import {HttpClient, json} from 'aurelia-fetch-client';
import {inject} from 'aurelia-framework';
import {ApiResponse} from '../models/api-response';
import {Applicant} from '../models/applicant';

/**
 * Repository class managing all applicant api data exchange with backend.
 */
@inject(HttpClient)
export class ApplicantRepository {

  private httpClient: HttpClient;

  constructor(httpClient) {
    this.httpClient = httpClient;
  }

  /**
   * Retrieves all applicants.
   */
  async getApplicants(): Promise<Applicant[]> {
    const response = await this.httpClient.fetch('applicant');
    return <Applicant[]> await response.json();
  }

  /**
   * Retrieves a specific applicant identified by id.
   * @param id The id of the applicant
   */
  async getApplicant(id: number): Promise<Applicant> {
    const response = await this.httpClient.fetch(`applicant/${id}`);
    return <Applicant> await response.json();
  }

  /**
   * Creates a new applicant with following parameters and returns an api response object signaling success.
   * @param name
   * @param familyName
   * @param address
   * @param countryOfOrigin
   * @param eMailAddress
   * @param age
   * @param hired
   */
  async createApplicant(name: string, familyName: string, address: string, countryOfOrigin: string, eMailAddress: string, age: number, hired: boolean): Promise<ApiResponse> {
    const response = await this.httpClient.post('applicant', json({
      name,
      familyName,
      address,
      countryOfOrigin,
      eMailAddress,
      age,
      hired,
    }));
    return <ApiResponse>await response.json();
  }

  /**
   * Updates an existing applicant with following parameters and returns an api response object signaling success.
   * @param id
   * @param name
   * @param familyName
   * @param address
   * @param countryOfOrigin
   * @param eMailAddress
   * @param age
   * @param hired
   */
  async updateApplicant(id: number, name: string, familyName: string, address: string, countryOfOrigin: string, eMailAddress: string, age: number, hired: boolean): Promise<ApiResponse> {
    const response = await this.httpClient.put(`applicant/${id}`, json({
      name,
      familyName,
      address,
      countryOfOrigin,
      eMailAddress,
      age,
      hired,
    }));
    return <ApiResponse>await response.json();
  }

  /**
   * Deletes an existing applicant identified by id and returns an api response object signaling success.
   * @param id The id of the applicant
   */
  async deleteApplicant(id: number): Promise<ApiResponse> {
    const response = await this.httpClient.delete(`applicant/${id}`);
    return <ApiResponse>await response.json();
  }
}
