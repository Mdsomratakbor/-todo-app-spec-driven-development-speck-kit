import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function maxLengthValidator(maxLength: number): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;
    if (value && typeof value === 'string' && value.length > maxLength) {
      return { maxLength: `Must not exceed ${maxLength} characters.` };
    }
    return null;
  };
}
