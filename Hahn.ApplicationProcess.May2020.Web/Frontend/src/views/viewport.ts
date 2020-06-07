import { inject } from 'aurelia-framework';
import { EventAggregator } from 'aurelia-event-aggregator';
import { DialogService } from 'aurelia-dialog';
import { I18N } from 'aurelia-i18n';
import { Applicant } from '../models/applicant';
import { ApplicantRepository } from '../repositories/applicant-repository';
import { ApiResponse } from '../models/api-response';
import { Confirmation } from './dialogs/confirmation';
import { ErrorNotification } from './dialogs/error-notification';


/**
 * Entrance point for all visible elements on the page.
 */
@inject(EventAggregator, DialogService, I18N, ApplicantRepository)
export class Viewport {

  private eventAggregator: EventAggregator;
  private dialogService: DialogService;
  private i18n: I18N;
  private applicantRepository: ApplicantRepository;
  
  public applicants: Applicant[];
  public editingApplicant: Applicant; /* bound to the form */
  public applicantserversideinvalidation: string[]; /* bound to the form */ /* lower cased since the framework is buggy: https://stackoverflow.com/questions/29963074/aurelia-simple-binding-is-not-working */

  constructor(eventAggregator, dialogService, i18n, applicantRepository) {
    this.eventAggregator = eventAggregator;
    this.dialogService = dialogService;
    this.i18n = i18n;
    this.applicantRepository = applicantRepository;
    this.applicantserversideinvalidation = [];

    /* No need to await */
    this.reloadApplicants();
  }

  /**
   * Reloads all applicants from repository async.
   */
  async reloadApplicants() {
    this.applicants = await this.applicantRepository.getApplicants();
  }

  /**
   * Handler for the add applicant button.
   */
  onAddPress() {
    this.editingApplicant = new Applicant();
    this.openForm();
  }

  /**
   * Handler for the floating edit applicant button.
   * @param applicant The applicant to be edited
   */
  onEditPress(applicant: Applicant) {
    this.editingApplicant = Object.assign(new Applicant(), applicant);
    this.openForm();
  }

  /**
   * Handler for the save button inside the form.
   * Saves the given applicant and acts on any errors occuring.
   * @param applicant The applicant to be saved
   */
  async onSavePress(applicant) {
    try {
      /* Since value converters seem to be unstable, work around by making sure age is a number */
      if (this.editingApplicant.age !== null && typeof this.editingApplicant.age !== 'number') {
        this.editingApplicant.age = parseInt(this.editingApplicant.age);
      }
      
      /* Reset error messages */
      this.applicantserversideinvalidation.splice(0);

      /* Update or add new? */
      let response: ApiResponse;
      if (this.editingApplicant.id) {
        response = await this.updateApplicant(this.editingApplicant);
      } else {
        response = await this.createApplicant(this.editingApplicant);
      }
      
      /* Check response */
      if (response.success) {
        /* Finally close the form & reload */
        this.closeForm();
        await this.reloadApplicants();
      } else {
        /* Check if we can parse the error keys */
        if (response.errorKeys && response.errorKeys.length) {
          // id-not-found
          if (response.errorKeys.length === 1 && response.errorKeys[0] === 'id-not-found') {
            console.warn("id-not-found error from api", response);
            this.dialogService.open({ viewModel: ErrorNotification, model: this.i18n.tr('communication.applicantNotFound'), lock: false });
          } else {
            /* Validation problems */
            this.applicantserversideinvalidation = response.errorKeys;
          }
        } else {
          console.warn("Unknown error from api", response);
          this.dialogService.open({ viewModel: ErrorNotification, model: this.i18n.tr('communication.savingFailedUnknown'), lock: false });
        }
      }
    } catch (error) {
      console.error(error.message, error.stack);
      this.dialogService.open({ viewModel: ErrorNotification, model: this.i18n.tr('communication.savingFailedUnknown'), lock: false });
    }
  }

  /**
   * Handler for the floating delete applicant button.
   * Deletes the given applicant and acts on any errors occuring.
   * @param applicant The applicant to be deleted
   */
  async onDeletePress(applicant: Applicant) {
    /* Let the user confirm deletion */
    this.dialogService.open({
        viewModel: Confirmation,
        model: this.i18n.tr('communication.deleteConfirmationMessage')
          .replace('{name}', applicant.name)
          .replace('{familyName}', applicant.familyName),
        lock: false }).whenClosed(async response => {
      if (!response.wasCancelled) {
        try {
          const response = await this.deleteApplicant(applicant);

          /* Check response */
          if (response.success) {
            /* Finally reload */
            await this.reloadApplicants();
          } else {
            /* Check if we can parse the error keys */
            if (response.errorKeys && response.errorKeys.length === 1 && response.errorKeys[0] === 'id-not-found') {
              /* Id not found error */
              this.dialogService.open({ viewModel: ErrorNotification, model: this.i18n.tr('communication.applicantNotFound'), lock: false });
            } else {
              /* Unknown api error */
              console.warn("Unknown error from api", response);
              this.dialogService.open({ viewModel: ErrorNotification, model: this.i18n.tr('communication.deletingFailedUnknown'), lock: false });
            }
          }
        } catch (error) {
          /* Other error */
          console.error(error.message, error.stack);
          this.dialogService.open({ viewModel: ErrorNotification, model: this.i18n.tr('communication.deletingFailedUnknown'), lock: false });
        }
      }
    });
  }

  /**
   * Creates a new applicant from the data in a given model.
   * @param applicant The new applicants data
   */
  async createApplicant(applicant: Applicant): Promise<ApiResponse> {
    return await this.applicantRepository
      /* Please don't do this at home kids, relying on the correct index is bad - but saves us some time ! */
      .createApplicant.apply(this.applicantRepository, Object.keys(applicant).splice(1).map(k => applicant[k]));
  }

  /**
   * Updates an existing applicant from the data in a given model.
   * @param applicant The updating applicants data
   */
  async updateApplicant(applicant: Applicant): Promise<ApiResponse> {
    return await this.applicantRepository
      /* Please don't do this at home kids, relying on the correct index is bad - but saves us some time ! */
      .updateApplicant.apply(this.applicantRepository, Object.keys(applicant).map(k => applicant[k]));
  }

  /**
   * Deletes an existing applicant from the data in a given model.
   * @param applicant The deleting applicants data
   */
  async deleteApplicant(applicant: Applicant): Promise<ApiResponse> {
      return await this.applicantRepository.deleteApplicant(applicant.id);
  }

  /**
   * Opens the applicant form modal.
   */
  openForm() {
    /* Open modal, usually we would use $().modal('show'),
       which does not work due to jQuery integration problems into typescript ?
       Let's use the workaround jQuery reference we set up in app.ts ... */
    (jQuery('#applicant-form') as any).modal('show');
  }

  /**
   * Closes the applicant form modal.
   */
  closeForm() {
    /* Close modal, usually we would use $().modal('hide'),
       which does not work due to jQuery integration problems into typescript ?
       Let's use the workaround jQuery reference we set up in app.ts ... */
    (jQuery('#applicant-form') as any).modal('hide');
  }
}
