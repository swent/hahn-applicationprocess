import { inject, bindable, observable, NewInstance } from 'aurelia-framework';
import { EventAggregator } from 'aurelia-event-aggregator';
import { ValidateResult, validateTrigger, ValidationController, ValidationRules } from 'aurelia-validation';
import { HttpClient } from 'aurelia-fetch-client';
import { DialogService } from 'aurelia-dialog';
import { I18N } from 'aurelia-i18n';
import { Applicant } from '../models/applicant';
import { Confirmation } from './dialogs/confirmation';
import { BootstrapFormRenderer } from '../utils/bootstrap-form-renderer';


@inject(EventAggregator, HttpClient, DialogService, I18N, NewInstance.of(ValidationController))
export class ApplicantForm {

  @bindable
  @observable
  public applicant: Applicant;
  @bindable public saveCallback: Function;
  @bindable
  @observable
  public serversideinvalidation: string[]; /* lower cased since the framework is buggy: https://stackoverflow.com/questions/29963074/aurelia-simple-binding-is-not-working */

  private eventAggregator: EventAggregator;
  private httpClient: HttpClient;
  private dialogService: DialogService;
  private i18n: I18N;
  private controller: ValidationController;
  private validationRules: ValidationRules;
  private formIsEmpty: boolean = true;
  private formIsValid: boolean = false;
  private validationDebounceTimer: any;
  private countryValidationCache: any = {};
  private tldCache: any = null;
  private serverSideValidateResults: ValidateResult[];


  constructor(eventAggregator, httpClient, dialogService, i18n, validationController) {
    this.eventAggregator = eventAggregator;
    this.httpClient = httpClient;
    this.dialogService = dialogService;
    this.i18n = i18n;
    
    /* Validation stuff */
    this.controller = validationController;
    this.controller.validateTrigger = validateTrigger.manual;
    this.controller.addRenderer(new BootstrapFormRenderer());
    this.validationRules = ValidationRules
      .ensure('name')
        .required()
        .minLength(5)
      .ensure('familyName')
        .required()
        .minLength(5)
      .ensure('address')
        .required()
        .minLength(10)
      .ensure('countryOfOrigin')
        .satisfies(this.cacheCheckValidCountry.bind(this))
      .ensure('eMailAddress')
        .required()
        .email()
        .satisfies(this.cacheCheckValidEMailAddressTld.bind(this))
      .ensure('age')
        .required()
        .range(20, 60)
      .ensure('hired')
        .satisfies(hired => hired !== null)
      .rules;
  }

  /**
   * Called whenever the applicant changes.
   * @param newApplicant The new applicant
   * @param oldApplicant The old applicant
   */
  applicantChanged(newApplicant, oldApplicant) {
    /* Since we swap out the whole object, validation seems to only work
       when applying it directly to each model we want to validate.
       Let's change validation objects here.*/
    if (oldApplicant) {
      this.controller.removeObject(oldApplicant);
    }
    if (newApplicant) {
      this.controller.addObject(newApplicant, this.validationRules);
    }

    this.onFormChange();
  }

  /**
   * Called whenever the server side invalidation changes.
   * Applies additional error messages into the validation controller.
   * @param newInvalidation The new invalidation info
   * @param oldInvalidation The old invalidation info
   */
  serversideinvalidationChanged(newInvalidation, oldInvalidation) {
    /* Remove any previous server side errors */
    if (oldInvalidation && oldInvalidation.length) {
      this.serverSideValidateResults.forEach(vr => this.controller.removeError(vr));
    }
    
    /* Apply new server side errors */
    if (newInvalidation && newInvalidation.length) {
      this.serverSideValidateResults = [];
      newInvalidation.forEach(inv => {
        const parts = inv.split(':'),
          propName = parts[1][0].toLowerCase() + parts[1].substring(1);
        
        let message;
        switch (parts[0]) {
          case 'required':
            message = this.i18n.tr('form.validation.required')
              .replace('${$displayName}', this.i18n.tr(`form.${propName}`));
            break;
          case 'minlen-5':
            message = this.i18n.tr('form.validation.minLength')
              .replace('${$displayName}', this.i18n.tr(`form.${propName}`))
              .replace('${$config.length}', '5');
            break;
          case 'minlen-10':
            message = this.i18n.tr('form.validation.minLength')
              .replace('${$displayName}', this.i18n.tr(`form.${propName}`))
              .replace('${$config.length}', '10');
            break;
          case 'range-20-60':
            message = this.i18n.tr('form.validation.range')
              .replace('${$displayName}', this.i18n.tr(`form.${propName}`))
              .replace('${$config.min}', '20')
              .replace('${$config.max}', '60');
            break;
          case 'default':
          case 'invalid':
          default:
            message = this.i18n.tr('form.validation.default')
              .replace('${$displayName}', this.i18n.tr(`form.${propName}`));
            break;
        }

        this.serverSideValidateResults.push(this.controller.addError(message, this.applicant, propName));
      });
    }
  }

  /**
   * Fired whenever form data is being changed.
   * Triggers state refreshes.
   */
  async onFormChange() {
    /* Reset any server errors when editing the form */
    this.serversideinvalidation = [];
    
    /* Refresh validation & empty state */
    this.refreshEmptyState();
    this.queueValidation();
  }

  /**
   * Refreshes the empty state.
   */
  refreshEmptyState(): void {
    this.formIsEmpty = !(
      this.applicant.name ||
      this.applicant.familyName ||
      this.applicant.address ||
      this.applicant.countryOfOrigin ||
      this.applicant.eMailAddress ||
      this.applicant.age);
  }

  /**
   * Queues up a re-validation of all form inputs.
   * Queueing is used to not re-validate on each key-stroke.
   * (saves CPU + other resources e.g. web requests)
   */
  queueValidation(): void {
    this.formIsValid = false;
    if (this.validationDebounceTimer) {
      clearTimeout(this.validationDebounceTimer);
    }
    this.validationDebounceTimer = setTimeout(this.executeValidation.bind(this), 300);
  }

  /**
   * Triggers a form validation and refreshes the valid state.
   */
  async executeValidation(): Promise<void> {
    this.validationDebounceTimer = null;
    const result = await this.controller.validate();
    this.formIsValid = result.valid;
  }

  /**
   * Fired when the reset button is pressed.
   * Prompts the user to confirm data reset.
   */
  async onResetPress(): Promise<void> {
    this.dialogService.open({ viewModel: Confirmation, model: this.i18n.tr('form.resetConfirmationText'), lock: false }).whenClosed(async response => {
      if (!response.wasCancelled) {
        this.applicant = new Applicant();
      }
    });
  }

  /**
   * Fired when the save button is pressed.
   * Executes the save callback if available.
   */
  async onSavePress(): Promise<void> {
    this.saveCallback && this.saveCallback();
  }

  /**
   * Checks for valid country in cache.
   * If not found in cache, will propagate to live-check against web.
   * @param country The country to check
   */
  async cacheCheckValidCountry(country): Promise<boolean> {
    if (!country) {
      return false;
    }

    if (typeof this.countryValidationCache[country] === 'boolean') {
      return this.countryValidationCache[country];
    } else {
      return await this.webCheckValidCountry(country);
    }
  }

  /**
   * Checks for valid country against a web page.
   * @param country The country to check
   */
  async webCheckValidCountry(country): Promise<boolean> {
    try {
      const response = await this.httpClient
        .fetch(`https://restcountries.eu/rest/v2/name/${country}?fullText=true`);
      this.countryValidationCache[country] = !!(await response.json()).length;
      return this.countryValidationCache[country];
    } catch (error) {
      console.warn(error);
      return false;
    }
  }

  /**
   * Checks for valid email tld in cache.
   * If cache is not loaded, will load the cache first.
   * @param eMailAddress The email to check
   */
  async cacheCheckValidEMailAddressTld(eMailAddress): Promise<boolean> {
    if (!eMailAddress) {
      return false;
    }

    /* Load tld cache if not loaded already */
    if (!this.tldCache) {
      await this.loadTldCache();
    }

    /* Extract tld from email and check against tld cache */
    const eMailTld = eMailAddress.split('.').reverse()[0];
    return !!this.tldCache[eMailTld.toLowerCase()];
  }

  /**
   * Loads the tld cache containing all valid top level domains.
   */
  async loadTldCache(): Promise<void> {
    try {
      /* Get info for all countries */
      const response = await this.httpClient.fetch('https://restcountries.eu/rest/v2/all');

      /* Start out with well-known non-country tlds */
      this.tldCache = { com: true, net: true, info: true, org: true, at: true, ch: true, li: true, de: true, eu: true, };

      /* Add country-specific tlds from web result */
      (await response.json())
        .map(c => c.topLevelDomain)
        .forEach(tld => tld.forEach(tld => this.tldCache[tld.substring(1)] = true));
    } catch (error) {
      console.warn(error);
    }
  }
}
