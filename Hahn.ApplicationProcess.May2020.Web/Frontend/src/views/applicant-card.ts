import { bindable } from 'aurelia-framework';
import { Applicant } from '../models/applicant';

/**
 * Represents an applicant as card.
 */
export class ApplicantCard {

  @bindable public applicant: Applicant;
  
  private hasHiddenCls: boolean;
  private avatarCls: string = Math.round(Math.random() * 3 + 1).toString(); /* random avatar */

  @bindable public editCallback: Function;
  @bindable public deleteCallback: Function;

  constructor() {
    this.hasHiddenCls = true;
  }

  /**
   * Fired when the card is attached to DOM.
   * Will remove the hidden class after a short timeout for any css effects.
   */
  attached() {
    setTimeout(() => {
      this.hasHiddenCls = false;
    }, 17);
  }

  /**
   * Fired when the edit button is pressed.
   * Calls the edit callback if available.
   */
  onEditPress() {
    this.editCallback && this.editCallback();
  }

  /**
   * Fired when the delete button is pressed.
   * Calls the delete callback if available.
   */
  onDeletePress() {
    this.deleteCallback && this.deleteCallback();
  }
}
