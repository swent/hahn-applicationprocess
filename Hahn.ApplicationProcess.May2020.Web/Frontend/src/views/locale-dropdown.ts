import { inject } from 'aurelia-framework';
import { I18N } from 'aurelia-i18n';
import { validationMessages } from 'aurelia-validation';

/**
 * Dropdown for local switch on the fly.
 */
@inject(I18N, validationMessages)
export class LocaleDropdown {

  private i18n: I18N;
  private validationMessages;
  private locales: string[] = ['en', 'de']; /* locale options to pick */
  private currentLocale: string = '';
  
  constructor(i18n, validationMessages) {
    this.i18n = i18n;
    this.validationMessages = validationMessages;
    this.currentLocale = this.i18n.getLocale();
  }

  /**
   * Called when a new locale is selected.
   * Will change the global app locale and trigger translation.
   * @param locale The selected locale
   */
  onLocaleSelect(locale) {
    this.currentLocale = locale;
    this.i18n.setLocale(locale);
    
    this.updateDefaultValidationMessages();
  }

  /**
   * Re-translates a given set of validation messages based on the current locale.
   */
  updateDefaultValidationMessages() {
    ['default', 'email', 'range', 'required', 'minLength']
      .forEach(key => this.validationMessages[key] = this.i18n.tr(`form.validation.${key}`));
  }
}
