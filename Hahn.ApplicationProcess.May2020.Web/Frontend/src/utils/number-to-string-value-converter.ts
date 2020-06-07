/**
 * This value converter should be used to bind numeric values to a number-input-field.
 * Sadly value-convertes do not seem to work, so this class is not in use.
 * Maybe fix sometime later?
 */
export class NumberToStringValueConverter {
  toView(number) {
    return number.toString();
  }

  fromView(string) {
    return parseInt(string);
  }
}
