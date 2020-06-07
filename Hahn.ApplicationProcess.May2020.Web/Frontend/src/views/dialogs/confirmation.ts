import { inject } from 'aurelia-framework';
import { DialogController } from 'aurelia-dialog';

/**
 * Shorthand class for showing confirmation prompts.
 */
@inject(DialogController)
export class Confirmation {

  private controller: DialogController;
  private message: string;
  
  constructor(dialogController) {
    this.controller = dialogController;
  }

  /**
   * Fired when this dialog is opened.
   * Applies the correct message.
   * @param message The message to ask
   */
  activate(message: string) {
    this.message = message;
  }
}
