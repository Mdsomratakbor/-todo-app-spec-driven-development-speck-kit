import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';

const HEX_COLOR_PATTERN = /^#[0-9A-Fa-f]{6}$/;

export function hexColorValidator(): ValidatorFn {
  return (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;
    if (value && typeof value === 'string' && !HEX_COLOR_PATTERN.test(value)) {
      return { hexColor: 'Must be a valid hex color code (e.g., #FF0000).' };
    }
    return null;
  };
}
