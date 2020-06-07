/**
 * Straight forward applicant model.
 * Can be initialized partially.
 */
export class Applicant {
  id: number = null;
  name: string = null;
  familyName: string = null;
  address: string = null;
  countryOfOrigin: string = null;
  eMailAddress: string = null;
  age: number = null;
  hired: boolean = false;

  public constructor(init?: Partial<Applicant>) {
    Object.assign(this, init);
  }
}
