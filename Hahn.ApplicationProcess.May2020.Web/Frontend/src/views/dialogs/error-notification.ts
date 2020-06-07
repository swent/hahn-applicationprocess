import { inject } from 'aurelia-framework';
import { DialogController } from 'aurelia-dialog';

/**
 * Shorthand class for showing error messages.
 */
@inject(DialogController)
export class ErrorNotification {

  private controller: DialogController;
  private errorMessage: string;

  constructor(dialogController) {
    this.controller = dialogController;
  }

  /**
   * Fired when this dialog is opened.
   * Applies the correct message.
   * @param errorMessage The message to show
   */
  activate(errorMessage: string) {
    this.errorMessage = errorMessage;
  }
}
