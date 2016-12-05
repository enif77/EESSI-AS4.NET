/* tslint:disable */
import { FormBuilder, FormGroup, FormArray } from '@angular/forms';

export class Transformer {
	type: string;

	static FIELD_type: string = 'type';	

	static getForm(formBuilder: FormBuilder, current: Transformer): FormGroup {
		return formBuilder.group({
			type: [current && current.type],
		});
	}
	/// Patch up all the formArray controls
	static patchFormArrays(formBuilder: FormBuilder, form: FormGroup, current: Transformer) {
	}
}
