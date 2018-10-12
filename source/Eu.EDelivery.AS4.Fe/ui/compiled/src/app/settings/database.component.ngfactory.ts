/**
 * @fileoverview This file is generated by the Angular template compiler.
 * Do not edit.
 * @suppress {suspiciousCode,uselessCode,missingProperties,missingOverride}
 */
 /* tslint:disable */


import * as i0 from '@angular/core';
import * as i1 from '@angular/forms';
import * as i2 from '../common/input/input.component.ngfactory';
import * as i3 from '../../../../src/app/common/input/input.component';
import * as i4 from '../../../../src/app/settings/runtime.store';
import * as i5 from '../../../../src/app/common/runtimetooltip.directive';
import * as i6 from '../../../../src/app/authentication/hasauth/hasauth.directive';
import * as i7 from '../../../../src/app/authentication/authentication.store';
import * as i8 from '@angular/router';
import * as i9 from '../../../../src/app/common/selectdirective';
import * as i10 from '../../../../src/app/common/fixformgroupstate.directive';
import * as i11 from '../../../../src/app/common/multiselect/multiselect.directive';
import * as i12 from '../../../../src/app/settings/database.component';
import * as i13 from '../../../../src/app/common/text.directive';
import * as i14 from '../../../../src/app/settings/settings.service';
import * as i15 from '../../../../src/app/common/dialog.service';
const styles_DatabaseSettingsComponent:any[] = ([] as any[]);
export const RenderType_DatabaseSettingsComponent:i0.RendererType2 = i0.ɵcrt({encapsulation:2,
    styles:styles_DatabaseSettingsComponent,data:{}});
export function View_DatabaseSettingsComponent_0(_l:any):i0.ɵViewDefinition {
  return i0.ɵvid(0,[(_l()(),i0.ɵted(-1,(null as any),['\n        '])),(_l()(),i0.ɵeld(1,
      0,(null as any),(null as any),46,'form',[['class','form-horizontal'],['novalidate',
          '']],[[2,'ng-untouched',(null as any)],[2,'ng-touched',(null as any)],[2,
          'ng-pristine',(null as any)],[2,'ng-dirty',(null as any)],[2,'ng-valid',
          (null as any)],[2,'ng-invalid',(null as any)],[2,'ng-pending',(null as any)]],
      [[(null as any),'submit'],[(null as any),'reset']],(_v,en,$event) => {
        var ad:boolean = true;
        if (('submit' === en)) {
          const pd_0:any = ((<any>i0.ɵnov(_v,3).onSubmit($event)) !== false);
          ad = (pd_0 && ad);
        }
        if (('reset' === en)) {
          const pd_1:any = ((<any>i0.ɵnov(_v,3).onReset()) !== false);
          ad = (pd_1 && ad);
        }
        return ad;
      },(null as any),(null as any))),i0.ɵdid(2,16384,(null as any),0,i1.ɵbf,([] as any[]),
      (null as any),(null as any)),i0.ɵdid(3,540672,(null as any),0,i1.FormGroupDirective,
      [[8,(null as any)],[8,(null as any)]],{form:[0,'form']},(null as any)),i0.ɵprd(2048,
      (null as any),i1.ControlContainer,(null as any),[i1.FormGroupDirective]),i0.ɵdid(5,
      16384,(null as any),0,i1.NgControlStatusGroup,[i1.ControlContainer],(null as any),
      (null as any)),(_l()(),i0.ɵted(-1,(null as any),['\n            '])),(_l()(),
      i0.ɵeld(7,0,(null as any),(null as any),25,'as4-input',([] as any[]),(null as any),
          (null as any),(null as any),i2.View_InputComponent_0,i2.RenderType_InputComponent)),
      i0.ɵdid(8,114688,(null as any),0,i3.InputComponent,[i4.RuntimeStore,[2,i5.RuntimetoolTipDirective]],
          {label:[0,'label']},(null as any)),(_l()(),i0.ɵted(-1,1,['\n                '])),
      (_l()(),i0.ɵeld(10,0,(null as any),1,21,'select',[['class','form-control'],['formControlName',
          'provider']],[[2,'ng-untouched',(null as any)],[2,'ng-touched',(null as any)],
          [2,'ng-pristine',(null as any)],[2,'ng-dirty',(null as any)],[2,'ng-valid',
              (null as any)],[2,'ng-invalid',(null as any)],[2,'ng-pending',(null as any)]],
          [[(null as any),'change'],[(null as any),'blur']],(_v,en,$event) => {
            var ad:boolean = true;
            if (('change' === en)) {
              const pd_0:any = ((<any>i0.ɵnov(_v,11).onChange($event.target.value)) !== false);
              ad = (pd_0 && ad);
            }
            if (('blur' === en)) {
              const pd_1:any = ((<any>i0.ɵnov(_v,11).onTouched()) !== false);
              ad = (pd_1 && ad);
            }
            return ad;
          },(null as any),(null as any))),i0.ɵdid(11,16384,(null as any),0,i1.SelectControlValueAccessor,
          [i0.Renderer2,i0.ElementRef],(null as any),(null as any)),i0.ɵprd(1024,(null as any),
          i1.NG_VALUE_ACCESSOR,(p0_0:any) => {
            return [p0_0];
          },[i1.SelectControlValueAccessor]),i0.ɵdid(13,671744,(null as any),0,i1.FormControlName,
          [[3,i1.ControlContainer],[8,(null as any)],[8,(null as any)],[2,i1.NG_VALUE_ACCESSOR]],
          {name:[0,'name']},(null as any)),i0.ɵprd(2048,(null as any),i1.NgControl,
          (null as any),[i1.FormControlName]),i0.ɵdid(15,16384,(null as any),0,i1.NgControlStatus,
          [i1.NgControl],(null as any),(null as any)),i0.ɵdid(16,409600,(null as any),
          0,i6.HasAuthDirective,[i0.ElementRef,i7.AuthenticationStore,i0.Renderer,
              i8.ActivatedRoute,[2,i1.NgControl],i0.NgZone,i0.ApplicationRef],{formControlName:[0,
              'formControlName']},(null as any)),i0.ɵdid(17,16384,(null as any),0,
          i9.SelectDirective,[i0.ElementRef,i0.Renderer],(null as any),(null as any)),
      i0.ɵdid(18,278528,(null as any),0,i10.FixFormGroupStateDirective,[i1.NgControl,
          [2,i1.NG_VALUE_ACCESSOR],[2,i6.HasAuthDirective]],(null as any),(null as any)),
      (_l()(),i0.ɵted(-1,(null as any),['\n                    '])),(_l()(),i0.ɵeld(20,
          0,(null as any),(null as any),4,'option',[['value','Sqlite']],(null as any),
          (null as any),(null as any),(null as any),(null as any))),i0.ɵdid(21,147456,
          (null as any),0,i1.NgSelectOption,[i0.ElementRef,i0.Renderer2,[2,i1.SelectControlValueAccessor]],
          {value:[0,'value']},(null as any)),i0.ɵdid(22,147456,(null as any),0,i1.ɵq,
          [i0.ElementRef,i0.Renderer2,[8,(null as any)]],{value:[0,'value']},(null as any)),
      i0.ɵdid(23,16384,(null as any),0,i11.OptionDirective,([] as any[]),(null as any),
          (null as any)),(_l()(),i0.ɵted(-1,(null as any),['SQLite'])),(_l()(),i0.ɵted(-1,
          (null as any),['\n                    '])),(_l()(),i0.ɵeld(26,0,(null as any),
          (null as any),4,'option',[['value','SqlServer']],(null as any),(null as any),
          (null as any),(null as any),(null as any))),i0.ɵdid(27,147456,(null as any),
          0,i1.NgSelectOption,[i0.ElementRef,i0.Renderer2,[2,i1.SelectControlValueAccessor]],
          {value:[0,'value']},(null as any)),i0.ɵdid(28,147456,(null as any),0,i1.ɵq,
          [i0.ElementRef,i0.Renderer2,[8,(null as any)]],{value:[0,'value']},(null as any)),
      i0.ɵdid(29,16384,(null as any),0,i11.OptionDirective,([] as any[]),(null as any),
          (null as any)),(_l()(),i0.ɵted(-1,(null as any),['Microsoft SQL server'])),
      (_l()(),i0.ɵted(-1,(null as any),['\n                '])),(_l()(),i0.ɵted(-1,
          1,['\n            '])),(_l()(),i0.ɵted(-1,(null as any),['\n            '])),
      (_l()(),i0.ɵeld(34,0,(null as any),(null as any),12,'as4-input',([] as any[]),
          (null as any),(null as any),(null as any),i2.View_InputComponent_0,i2.RenderType_InputComponent)),
      i0.ɵdid(35,114688,(null as any),0,i3.InputComponent,[i4.RuntimeStore,[2,i5.RuntimetoolTipDirective]],
          {label:[0,'label']},(null as any)),(_l()(),i0.ɵted(-1,1,['\n                '])),
      (_l()(),i0.ɵeld(37,0,(null as any),1,8,'input',[['class','form-control pull-right'],
          ['formControlName','connectionString'],['name','provider'],['type','text']],
          [[2,'ng-untouched',(null as any)],[2,'ng-touched',(null as any)],[2,'ng-pristine',
              (null as any)],[2,'ng-dirty',(null as any)],[2,'ng-valid',(null as any)],
              [2,'ng-invalid',(null as any)],[2,'ng-pending',(null as any)]],[[(null as any),
              'keydown.enter'],[(null as any),'input'],[(null as any),'blur'],[(null as any),
              'compositionstart'],[(null as any),'compositionend']],(_v,en,$event) => {
            var ad:boolean = true;
            var _co:i12.DatabaseSettingsComponent = _v.component;
            if (('input' === en)) {
              const pd_0:any = ((<any>i0.ɵnov(_v,38)._handleInput($event.target.value)) !== false);
              ad = (pd_0 && ad);
            }
            if (('blur' === en)) {
              const pd_1:any = ((<any>i0.ɵnov(_v,38).onTouched()) !== false);
              ad = (pd_1 && ad);
            }
            if (('compositionstart' === en)) {
              const pd_2:any = ((<any>i0.ɵnov(_v,38)._compositionStart()) !== false);
              ad = (pd_2 && ad);
            }
            if (('compositionend' === en)) {
              const pd_3:any = ((<any>i0.ɵnov(_v,38)._compositionEnd($event.target.value)) !== false);
              ad = (pd_3 && ad);
            }
            if (('keydown.enter' === en)) {
              const pd_4:any = ((<any>_co.save()) !== false);
              ad = (pd_4 && ad);
            }
            return ad;
          },(null as any),(null as any))),i0.ɵdid(38,16384,(null as any),0,i1.DefaultValueAccessor,
          [i0.Renderer2,i0.ElementRef,[2,i1.COMPOSITION_BUFFER_MODE]],(null as any),
          (null as any)),i0.ɵprd(1024,(null as any),i1.NG_VALUE_ACCESSOR,(p0_0:any) => {
        return [p0_0];
      },[i1.DefaultValueAccessor]),i0.ɵdid(40,671744,(null as any),0,i1.FormControlName,
          [[3,i1.ControlContainer],[8,(null as any)],[8,(null as any)],[2,i1.NG_VALUE_ACCESSOR]],
          {name:[0,'name']},(null as any)),i0.ɵprd(2048,(null as any),i1.NgControl,
          (null as any),[i1.FormControlName]),i0.ɵdid(42,16384,(null as any),0,i1.NgControlStatus,
          [i1.NgControl],(null as any),(null as any)),i0.ɵdid(43,409600,(null as any),
          0,i6.HasAuthDirective,[i0.ElementRef,i7.AuthenticationStore,i0.Renderer,
              i8.ActivatedRoute,[2,i1.NgControl],i0.NgZone,i0.ApplicationRef],{formControlName:[0,
              'formControlName']},(null as any)),i0.ɵdid(44,16384,(null as any),0,
          i13.TextDirective,[i0.ElementRef,i0.Renderer],(null as any),(null as any)),
      i0.ɵdid(45,278528,(null as any),0,i10.FixFormGroupStateDirective,[i1.NgControl,
          [2,i1.NG_VALUE_ACCESSOR],[2,i6.HasAuthDirective]],(null as any),(null as any)),
      (_l()(),i0.ɵted(-1,1,['\n            '])),(_l()(),i0.ɵted(-1,(null as any),['\n        '])),
      (_l()(),i0.ɵted(-1,(null as any),['\n    ']))],(_ck,_v) => {
    var _co:i12.DatabaseSettingsComponent = _v.component;
    const currVal_7:any = _co.form;
    _ck(_v,3,0,currVal_7);
    const currVal_8:any = 'Provider';
    _ck(_v,8,0,currVal_8);
    const currVal_16:any = 'provider';
    _ck(_v,13,0,currVal_16);
    const currVal_17:any = 'provider';
    _ck(_v,16,0,currVal_17);
    _ck(_v,18,0);
    const currVal_18:any = 'Sqlite';
    _ck(_v,21,0,currVal_18);
    const currVal_19:any = 'Sqlite';
    _ck(_v,22,0,currVal_19);
    const currVal_20:any = 'SqlServer';
    _ck(_v,27,0,currVal_20);
    const currVal_21:any = 'SqlServer';
    _ck(_v,28,0,currVal_21);
    const currVal_22:any = 'Connectionstring';
    _ck(_v,35,0,currVal_22);
    const currVal_30:any = 'connectionString';
    _ck(_v,40,0,currVal_30);
    const currVal_31:any = 'connectionString';
    _ck(_v,43,0,currVal_31);
    _ck(_v,45,0);
  },(_ck,_v) => {
    const currVal_0:any = i0.ɵnov(_v,5).ngClassUntouched;
    const currVal_1:any = i0.ɵnov(_v,5).ngClassTouched;
    const currVal_2:any = i0.ɵnov(_v,5).ngClassPristine;
    const currVal_3:any = i0.ɵnov(_v,5).ngClassDirty;
    const currVal_4:any = i0.ɵnov(_v,5).ngClassValid;
    const currVal_5:any = i0.ɵnov(_v,5).ngClassInvalid;
    const currVal_6:any = i0.ɵnov(_v,5).ngClassPending;
    _ck(_v,1,0,currVal_0,currVal_1,currVal_2,currVal_3,currVal_4,currVal_5,currVal_6);
    const currVal_9:any = i0.ɵnov(_v,15).ngClassUntouched;
    const currVal_10:any = i0.ɵnov(_v,15).ngClassTouched;
    const currVal_11:any = i0.ɵnov(_v,15).ngClassPristine;
    const currVal_12:any = i0.ɵnov(_v,15).ngClassDirty;
    const currVal_13:any = i0.ɵnov(_v,15).ngClassValid;
    const currVal_14:any = i0.ɵnov(_v,15).ngClassInvalid;
    const currVal_15:any = i0.ɵnov(_v,15).ngClassPending;
    _ck(_v,10,0,currVal_9,currVal_10,currVal_11,currVal_12,currVal_13,currVal_14,currVal_15);
    const currVal_23:any = i0.ɵnov(_v,42).ngClassUntouched;
    const currVal_24:any = i0.ɵnov(_v,42).ngClassTouched;
    const currVal_25:any = i0.ɵnov(_v,42).ngClassPristine;
    const currVal_26:any = i0.ɵnov(_v,42).ngClassDirty;
    const currVal_27:any = i0.ɵnov(_v,42).ngClassValid;
    const currVal_28:any = i0.ɵnov(_v,42).ngClassInvalid;
    const currVal_29:any = i0.ɵnov(_v,42).ngClassPending;
    _ck(_v,37,0,currVal_23,currVal_24,currVal_25,currVal_26,currVal_27,currVal_28,
        currVal_29);
  });
}
export function View_DatabaseSettingsComponent_Host_0(_l:any):i0.ɵViewDefinition {
  return i0.ɵvid(0,[(_l()(),i0.ɵeld(0,0,(null as any),(null as any),1,'as4-database-settings',
      ([] as any[]),(null as any),(null as any),(null as any),View_DatabaseSettingsComponent_0,
      RenderType_DatabaseSettingsComponent)),i0.ɵdid(1,49152,(null as any),0,i12.DatabaseSettingsComponent,
      [i14.SettingsService,i1.FormBuilder,i15.DialogService],(null as any),(null as any))],
      (null as any),(null as any));
}
export const DatabaseSettingsComponentNgFactory:i0.ComponentFactory<i12.DatabaseSettingsComponent> = i0.ɵccf('as4-database-settings',
    i12.DatabaseSettingsComponent,View_DatabaseSettingsComponent_Host_0,{settings:'settings'},
    {isDirty:'isDirty'},([] as any[]));
//# sourceMappingURL=data:application/json;base64,eyJmaWxlIjoiQzovRGV2L2NvZGl0LnZpc3VhbHN0dWRpby5jb20vQVM0Lk5FVC9zb3VyY2UvRmUvRXUuRURlbGl2ZXJ5LkFTNC5GZS91aS9zcmMvYXBwL3NldHRpbmdzL2RhdGFiYXNlLmNvbXBvbmVudC5uZ2ZhY3RvcnkudHMiLCJ2ZXJzaW9uIjozLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJuZzovLy9DOi9EZXYvY29kaXQudmlzdWFsc3R1ZGlvLmNvbS9BUzQuTkVUL3NvdXJjZS9GZS9FdS5FRGVsaXZlcnkuQVM0LkZlL3VpL3NyYy9hcHAvc2V0dGluZ3MvZGF0YWJhc2UuY29tcG9uZW50LnRzIiwibmc6Ly8vQzovRGV2L2NvZGl0LnZpc3VhbHN0dWRpby5jb20vQVM0Lk5FVC9zb3VyY2UvRmUvRXUuRURlbGl2ZXJ5LkFTNC5GZS91aS9zcmMvYXBwL3NldHRpbmdzL2RhdGFiYXNlLmNvbXBvbmVudC50cy5EYXRhYmFzZVNldHRpbmdzQ29tcG9uZW50Lmh0bWwiLCJuZzovLy9DOi9EZXYvY29kaXQudmlzdWFsc3R1ZGlvLmNvbS9BUzQuTkVUL3NvdXJjZS9GZS9FdS5FRGVsaXZlcnkuQVM0LkZlL3VpL3NyYy9hcHAvc2V0dGluZ3MvZGF0YWJhc2UuY29tcG9uZW50LnRzLkRhdGFiYXNlU2V0dGluZ3NDb21wb25lbnRfSG9zdC5odG1sIl0sInNvdXJjZXNDb250ZW50IjpbIiAiLCJcbiAgICAgICAgPGZvcm0gW2Zvcm1Hcm91cF09XCJmb3JtXCIgY2xhc3M9XCJmb3JtLWhvcml6b250YWxcIj5cbiAgICAgICAgICAgIDxhczQtaW5wdXQgW2xhYmVsXT1cIidQcm92aWRlcidcIj5cbiAgICAgICAgICAgICAgICA8c2VsZWN0IGZvcm1Db250cm9sTmFtZT1cInByb3ZpZGVyXCIgY2xhc3M9XCJmb3JtLWNvbnRyb2xcIj5cbiAgICAgICAgICAgICAgICAgICAgPG9wdGlvbiB2YWx1ZT1cIlNxbGl0ZVwiPlNRTGl0ZTwvb3B0aW9uPlxuICAgICAgICAgICAgICAgICAgICA8b3B0aW9uIHZhbHVlPVwiU3FsU2VydmVyXCI+TWljcm9zb2Z0IFNRTCBzZXJ2ZXI8L29wdGlvbj5cbiAgICAgICAgICAgICAgICA8L3NlbGVjdD5cbiAgICAgICAgICAgIDwvYXM0LWlucHV0PlxuICAgICAgICAgICAgPGFzNC1pbnB1dCBbbGFiZWxdPVwiJ0Nvbm5lY3Rpb25zdHJpbmcnXCI+XG4gICAgICAgICAgICAgICAgPGlucHV0IHR5cGU9XCJ0ZXh0XCIgY2xhc3M9XCJmb3JtLWNvbnRyb2wgcHVsbC1yaWdodFwiIG5hbWU9XCJwcm92aWRlclwiIChrZXlkb3duLmVudGVyKT1cInNhdmUoKVwiIGZvcm1Db250cm9sTmFtZT1cImNvbm5lY3Rpb25TdHJpbmdcIi8+XG4gICAgICAgICAgICA8L2FzNC1pbnB1dD5cbiAgICAgICAgPC9mb3JtPlxuICAgICIsIjxhczQtZGF0YWJhc2Utc2V0dGluZ3M+PC9hczQtZGF0YWJhc2Utc2V0dGluZ3M+Il0sIm1hcHBpbmdzIjoiQUFBQTs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7OztvQkNBQSxrREFDUTtNQUFBO1VBQUE7VUFBQTtVQUFBO01BQUE7UUFBQTtRQUFBO1VBQUE7VUFBQTtRQUFBO1FBQUE7VUFBQTtVQUFBO1FBQUE7UUFBQTtNQUFBLHVDQUFBO01BQUEsb0NBQUE7TUFBQSw4RUFBQTtNQUFBLGdGQUFBO01BQUE7TUFBQSxlQUFpRCxzREFDN0M7YUFBQTtVQUFBO2FBQUE7VUFBQSxtQ0FBZ0M7TUFDNUI7VUFBQTtVQUFBO2NBQUE7VUFBQTtZQUFBO1lBQUE7Y0FBQTtjQUFBO1lBQUE7WUFBQTtjQUFBO2NBQUE7WUFBQTtZQUFBO1VBQUEsdUNBQUE7VUFBQSxpRUFBQTsrQkFBQTtZQUFBO1VBQUEsMENBQUE7VUFBQTtVQUFBLHdDQUFBO1VBQUEsMkNBQUE7VUFBQSxtREFBQTtVQUFBO2dDQUFBO2NBQUEsMENBQUE7NkJBQUE7YUFBQTtVQUFBO01BQXdELDhEQUNwRDtVQUFBO1VBQUEsaUVBQUE7VUFBQTtVQUFBLDBDQUFBO1VBQUE7YUFBQTtVQUFBLGVBQXVCLDhDQUFlO1VBQUEsMkNBQ3RDO1VBQUE7VUFBQSxtREFBQTtVQUFBO1VBQUEsMENBQUE7VUFBQTthQUFBO1VBQUEsZUFBMEI7TUFBNkIsMERBQ2xEO1VBQUEsdUJBQ0Q7TUFDWjtVQUFBO2FBQUE7VUFBQSxtQ0FBd0M7TUFDcEM7VUFBQTtVQUFBO2NBQUE7Y0FBQTtjQUFBO2NBQUE7WUFBQTtZQUFBO1lBQUE7Y0FBQTtjQUFBO1lBQUE7WUFBQTtjQUFBO2NBQUE7WUFBQTtZQUFBO2NBQUE7Y0FBQTtZQUFBO1lBQUE7Y0FBQTtjQUFBO1lBQUE7WUFBbUU7Y0FBQTtjQUFBO1lBQUE7WUFBbkU7VUFBQSx1Q0FBQTtVQUFBO1VBQUEsc0JBQUE7UUFBQTtNQUFBLG9DQUFBO1VBQUE7VUFBQSx3Q0FBQTtVQUFBLDJDQUFBO1VBQUEsbURBQUE7VUFBQTtnQ0FBQTtjQUFBLDBDQUFBOzRCQUFBO2FBQUE7VUFBQTtNQUFnSSwwQ0FDeEg7TUFDVDs7SUFWRDtJQUFOLFdBQU0sU0FBTjtJQUNlO0lBQVgsV0FBVyxTQUFYO0lBQ1k7SUFBUixZQUFRLFVBQVI7SUFBUTtJQUFSLFlBQVEsVUFBUjtJQUFBO0lBQ1k7SUFBUixZQUFRLFVBQVI7SUFBUTtJQUFSLFlBQVEsVUFBUjtJQUNRO0lBQVIsWUFBUSxVQUFSO0lBQVE7SUFBUixZQUFRLFVBQVI7SUFHRztJQUFYLFlBQVcsVUFBWDtJQUNnRztJQUE1RixZQUE0RixVQUE1RjtJQUE0RjtJQUE1RixZQUE0RixVQUE1RjtJQUFBOztJQVJSO0lBQUE7SUFBQTtJQUFBO0lBQUE7SUFBQTtJQUFBO0lBQUEsV0FBQSxxRUFBQTtJQUVRO0lBQUE7SUFBQTtJQUFBO0lBQUE7SUFBQTtJQUFBO0lBQUEsWUFBQSwyRUFBQTtJQU1BO0lBQUE7SUFBQTtJQUFBO0lBQUE7SUFBQTtJQUFBO0lBQUEsWUFBQTtRQUFBLFVBQUE7Ozs7b0JDVGhCO01BQUE7MENBQUEsVUFBQTtNQUFBOzs7OzsifQ==