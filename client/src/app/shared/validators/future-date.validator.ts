import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function futureDateValidator(maxYears = 5): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;
    if (!value) return null;
    const date = new Date(value);
    if (isNaN(date.getTime())) return { futureDate: 'Invalid date format.' };
    const maxDate = new Date();
    maxDate.setFullYear(maxDate.getFullYear() + maxYears);
    if (date > maxDate) {
      return { futureDate: `Date cannot be more than ${maxYears} years in the future.` };
    }
    return null;
  };
}
