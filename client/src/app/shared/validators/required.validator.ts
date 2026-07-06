import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

export function requiredValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;
    if (value == null || value === '' || (typeof value === 'string' && value.trim().length === 0)) {
      return { required: 'This field is required.' };
    }
    return null;
  };
}
