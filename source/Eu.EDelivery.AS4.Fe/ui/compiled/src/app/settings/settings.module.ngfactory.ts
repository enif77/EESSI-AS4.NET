/**
 * @fileoverview This file is generated by the Angular template compiler.
 * Do not edit.
 * @suppress {suspiciousCode,uselessCode,missingProperties,missingOverride}
 */
 /* tslint:disable */


import * as i0 from '@angular/core';
import * as i1 from '../../../../src/app/settings/settings.module';
import * as i2 from '../authentication/login/login.component.ngfactory';
import * as i3 from '../authentication/unauthorized/unauthorized.component.ngfactory';
import * as i4 from '../common/wrapper.component.ngfactory';
import * as i5 from '../pmodes/receivingpmode/receivingpmode.component.ngfactory';
import * as i6 from '../pmodes/sendingpmode/sendingpmode.component.ngfactory';
import * as i7 from './agent/agent.component.ngfactory';
import * as i8 from './portalsettings/portalsettings.component.ngfactory';
import * as i9 from './settings/settings.component.ngfactory';
import * as i10 from './receptionawarenessagent/receptionawarenessagent.component.ngfactory';
import * as i11 from './smpconfiguration/smpconfiguration.component.ngfactory';
import * as i12 from './smpconfiguration/smpconfigurationdetail.component.ngfactory';
import * as i13 from '@angular/common';
import * as i14 from '@angular/forms';
import * as i15 from 'angular2-jwt/angular2-jwt';
import * as i16 from '../../../../src/app/authentication/authentication.module';
import * as i17 from '../../../../src/app/common/spinner/spinner.service';
import * as i18 from '../../../../src/app/common/modal/modal.service';
import * as i19 from '../../../../src/app/common/dialog.service';
import * as i20 from '@angular/http';
import * as i21 from '../../../../src/app/authentication/authentication.store';
import * as i22 from '../../../../src/app/authentication/logout.service';
import * as i23 from '@angular/router';
import * as i24 from '../../../../src/app/authentication/authentication.service';
import * as i25 from '../../../../src/app/setup/setup.guard';
import * as i26 from '../../../../src/app/setup/setup.service';
import * as i27 from 'ngx-window-token/dist/src/ngx-window-token';
import * as i28 from 'ngx-clipboard/src/clipboard.service';
import * as i29 from '@angular/platform-browser';
import * as i30 from '../../../../src/app/common/as4components.module';
import * as i31 from '../../../../src/app/authentication/roles.service';
import * as i32 from '../../../../src/app/common/mustbeauthorized.guard';
import * as i33 from '../../../../src/app/common/candeactivate.guard';
import * as i34 from '../../../../src/app/common/router.service';
import * as i35 from '../../../../src/app/common/form.service';
import * as i36 from '../../../../src/app/common/spinner/customhttp';
import * as i37 from '../../../../src/app/common/error.handler';
import * as i38 from '../../../../src/app/pmodes/pmode.store';
import * as i39 from '../../../../src/app/settings/runtime.store';
import * as i40 from '../../../../src/app/pmodes/sendingpmode.service';
import * as i41 from '../../../../src/app/pmodes/receivingpmode.service';
import * as i42 from '../../../../src/app/settings/settings.store';
import * as i43 from '../../../../src/app/settings/settings.service';
import * as i44 from '../../../../src/app/settings/runtime.service';
import * as i45 from '../../../../src/app/settings/authorizationmap/authorizationmapservice';
import * as i46 from '../../../../src/app/settings/smpconfiguration/smpconfiguration.service';
import * as i47 from 'angular-sortablejs/dist/src/sortablejs.module';
import * as i48 from 'ngx-clipboard/src/index';
import * as i49 from 'angular2-text-mask/dist/angular2TextMask';
import * as i50 from 'ng2-select2/ng2-select2';
import * as i51 from '../../../../src/app/pmodes/pmodes.module';
import * as i52 from '../../../../src/app/runtime/runtime.module';
import * as i53 from '../../../../src/app/authentication/login/login.component';
import * as i54 from '../../../../src/app/authentication/unauthorized/unauthorized.component';
import * as i55 from '../../../../src/app/common/wrapper.component';
import * as i56 from '../../../../src/app/pmodes/receivingpmode/receivingpmode.component';
import * as i57 from '../../../../src/app/pmodes/sendingpmode/sendingpmode.component';
import * as i58 from '../../../../src/app/settings/agent/agent.component';
import * as i59 from '../../../../src/app/settings/portalsettings/portalsettings.component';
import * as i60 from '../../../../src/app/settings/settings/settings.component';
import * as i61 from '../../../../src/app/settings/receptionawarenessagent/receptionawarenessagent.component';
import * as i62 from '../../../../src/app/settings/smpconfiguration/smpconfiguration.component';
export const SettingsModuleNgFactory:i0.NgModuleFactory<i1.SettingsModule> = i0.ɵcmf(i1.SettingsModule,
    ([] as any[]),(_l:any) => {
      return i0.ɵmod([i0.ɵmpd(512,i0.ComponentFactoryResolver,i0.ɵCodegenComponentFactoryResolver,
          [[8,[i2.LoginComponentNgFactory,i3.UnauthorizedComponentNgFactory,i4.WrapperComponentNgFactory,
              i5.ReceivingPmodeComponentNgFactory,i6.SendingPmodeComponentNgFactory,
              i7.AgentSettingsComponentNgFactory,i8.PortalSettingsComponentNgFactory,
              i9.SettingsComponentNgFactory,i10.ReceptionAwarenessAgentComponentNgFactory,
              i11.SmpConfigurationComponentNgFactory,i12.SmpConfigurationDetailComponentNgFactory]],
              [3,i0.ComponentFactoryResolver],i0.NgModuleRef]),i0.ɵmpd(4608,i13.NgLocalization,
          i13.NgLocaleLocalization,[i0.LOCALE_ID]),i0.ɵmpd(4608,i14.ɵi,i14.ɵi,([] as any[])),
          i0.ɵmpd(4608,i14.FormBuilder,i14.FormBuilder,([] as any[])),i0.ɵmpd(5120,
              i15.JwtHelper,i16.jwtHelperFactory,([] as any[])),i0.ɵmpd(4608,i17.SpinnerService,
              i17.SpinnerService,([] as any[])),i0.ɵmpd(4608,i18.ModalService,i18.ModalService,
              [i0.ComponentFactoryResolver,i0.Injector]),i0.ɵmpd(4608,i19.DialogService,
              i19.DialogService,[i18.ModalService]),i0.ɵmpd(5120,i20.Http,i17.spinnerHttpServiceFactory,
              [i20.XHRBackend,i20.RequestOptions,i17.SpinnerService,i19.DialogService,
                  i0.Injector]),i0.ɵmpd(4608,i21.AuthenticationStore,i21.AuthenticationStore,
              [i15.JwtHelper]),i0.ɵmpd(4608,i22.LogoutService,i22.LogoutService,[i23.Router]),
          i0.ɵmpd(4608,i24.AuthenticationService,i24.AuthenticationService,[i20.Http,
              i21.AuthenticationStore,i23.Router,i17.SpinnerService,i19.DialogService,
              i22.LogoutService]),i0.ɵmpd(4608,i25.SetupGuard,i25.SetupGuard,[i26.SetupService,
              i23.Router]),i0.ɵmpd(5120,i27.WINDOW,i27._window,([] as any[])),i0.ɵmpd(5120,
              i28.ClipboardService,i28.CLIPBOARD_SERVICE_PROVIDER_FACTORY,[i29.DOCUMENT,
                  i27.WINDOW,[3,i28.ClipboardService]]),i0.ɵmpd(5120,i15.AuthHttp,
              i30.authHttpServiceFactory,[i20.RequestOptions,i20.XHRBackend,i17.SpinnerService,
                  i19.DialogService,i0.Injector]),i0.ɵmpd(4608,i31.RolesService,i31.RolesService,
              [i15.AuthHttp,i21.AuthenticationStore]),i0.ɵmpd(4608,i32.MustBeAuthorizedGuard,
              i32.MustBeAuthorizedGuard,[i23.Router,i31.RolesService,i19.DialogService,
                  i24.AuthenticationService]),i0.ɵmpd(4608,i33.CanDeactivateGuard,
              i33.CanDeactivateGuard,[i19.DialogService]),i0.ɵmpd(4608,i34.RouterService,
              i34.RouterService,[i13.Location,i23.Router]),i0.ɵmpd(4608,i35.FormBuilderExtended,
              i35.FormBuilderExtended,[i14.FormBuilder,i0.Injector]),i0.ɵmpd(5120,
              i36.CustomAuthNoSpinnerHttp,i30.authHttpNoSpinnerServiceFactory,[i20.RequestOptions,
                  i20.XHRBackend,i17.SpinnerService,i19.DialogService,i0.Injector]),
          i0.ɵmpd(5120,i0.ErrorHandler,i37.errorHandlerFactory,[i19.DialogService,
              i17.SpinnerService]),i0.ɵmpd(4608,i38.PmodeStore,i38.PmodeStore,([] as any[])),
          i0.ɵmpd(4608,i39.RuntimeStore,i39.RuntimeStore,([] as any[])),i0.ɵmpd(4608,
              i40.SendingPmodeService,i40.SendingPmodeService,[i15.AuthHttp,i38.PmodeStore,
                  i35.FormBuilderExtended,i39.RuntimeStore]),i0.ɵmpd(4608,i41.ReceivingPmodeService,
              i41.ReceivingPmodeService,[i15.AuthHttp,i38.PmodeStore,i35.FormBuilderExtended,
                  i39.RuntimeStore]),i0.ɵmpd(4608,i42.SettingsStore,i42.SettingsStore,
              ([] as any[])),i0.ɵmpd(4608,i43.SettingsService,i43.SettingsService,
              [i15.AuthHttp,i42.SettingsStore]),i0.ɵmpd(4608,i44.RuntimeService,i44.RuntimeService,
              [i15.AuthHttp,i20.Http,i39.RuntimeStore]),i0.ɵmpd(4608,i45.AuthorizationMapService,
              i45.AuthorizationMapService,[i15.AuthHttp]),i0.ɵmpd(4608,i46.SmpConfigurationService,
              i46.SmpConfigurationService,[i15.AuthHttp]),i0.ɵmpd(512,i13.CommonModule,
              i13.CommonModule,([] as any[])),i0.ɵmpd(512,i14.ɵba,i14.ɵba,([] as any[])),
          i0.ɵmpd(512,i14.FormsModule,i14.FormsModule,([] as any[])),i0.ɵmpd(512,i14.ReactiveFormsModule,
              i14.ReactiveFormsModule,([] as any[])),i0.ɵmpd(512,i23.RouterModule,
              i23.RouterModule,[[2,i23.ɵa],[2,i23.Router]]),i0.ɵmpd(512,i47.SortablejsModule,
              i47.SortablejsModule,([] as any[])),i0.ɵmpd(512,i16.AuthenticationModule,
              i16.AuthenticationModule,([] as any[])),i0.ɵmpd(512,i27.WindowTokenModule,
              i27.WindowTokenModule,([] as any[])),i0.ɵmpd(512,i48.ClipboardModule,
              i48.ClipboardModule,([] as any[])),i0.ɵmpd(512,i49.TextMaskModule,i49.TextMaskModule,
              ([] as any[])),i0.ɵmpd(512,i50.Select2Module,i50.Select2Module,([] as any[])),
          i0.ɵmpd(512,i30.As4ComponentsModule,i30.As4ComponentsModule,([] as any[])),
          i0.ɵmpd(512,i51.PmodesModule,i51.PmodesModule,([] as any[])),i0.ɵmpd(512,
              i52.RuntimeModule,i52.RuntimeModule,([] as any[])),i0.ɵmpd(512,i1.SettingsModule,
              i1.SettingsModule,([] as any[])),i0.ɵmpd(1024,i23.ROUTES,() => {
            return [[{path:'login',component:i53.LoginComponent,data:{isAuthCheck:false},
                canActivate:[i25.SetupGuard]},{path:'unauthorized',component:i54.UnauthorizedComponent}],
                [{path:'pmodes',component:i55.WrapperComponent,children:[{path:'',
                    pathMatch:'full',redirectTo:'receiving',canDeactivate:[i33.CanDeactivateGuard]},
                    {path:'receiving',component:i56.ReceivingPmodeComponent,data:{title:'Receiving PMode',
                        mode:'receiving'},canDeactivate:[i33.CanDeactivateGuard],canActivate:[i32.MustBeAuthorizedGuard]},
                    {path:'receiving/:pmode',component:i56.ReceivingPmodeComponent,
                        data:{title:'Receiving PMode',mode:'receiving',nomenu:true},
                        canDeactivate:[i33.CanDeactivateGuard],canActivate:[i32.MustBeAuthorizedGuard]},
                    {path:'sending',component:i57.SendingPmodeComponent,data:{title:'Sending PMode',
                        mode:'sending'},canDeactivate:[i33.CanDeactivateGuard]},{path:'sending/:pmode',
                        component:i57.SendingPmodeComponent,data:{title:'Sending PMode',
                            mode:'sending',nomenu:true},canDeactivate:[i33.CanDeactivateGuard]}],
                    data:{title:'PModes'},canActivate:[i32.MustBeAuthorizedGuard]}],
                [{path:'submit',component:i55.WrapperComponent,children:[{path:'',
                    component:i58.AgentSettingsComponent,data:{title:'Submit Agents',
                        type:'submitAgents',icon:'fa-cloud-upload',weight:-10,betype:0},
                    canActivate:[i32.MustBeAuthorizedGuard],canDeactivate:[i33.CanDeactivateGuard]}]},
                    {path:'receive',component:i55.WrapperComponent,children:[{path:'push',
                        component:i58.AgentSettingsComponent,data:{title:'Push Receive Agents',
                            type:'receiveAgents',icon:'fa-cloud-download',betype:1},
                        canActivate:[i32.MustBeAuthorizedGuard],canDeactivate:[i33.CanDeactivateGuard]},
                        {path:'pull',component:i58.AgentSettingsComponent,data:{title:'Pull Receive Agents',
                            type:'pullReceiveAgents',icon:'fa-cloud-download',betype:6},
                            canActivate:[i32.MustBeAuthorizedGuard],canDeactivate:[i33.CanDeactivateGuard]}],
                        data:{title:'Receive Agents',icon:'fa-cloud-download',weight:-9},
                        canActivate:[i32.MustBeAuthorizedGuard],canDeactivate:[i33.CanDeactivateGuard]},
                    {path:'settings',component:i55.WrapperComponent,children:[{path:'',
                        redirectTo:'portal',pathMatch:'full',canDeactivate:[i33.CanDeactivateGuard]},
                        {path:'portal',component:i59.PortalSettingsComponent,data:{title:'Portal settings'},
                            canDeactivate:[i33.CanDeactivateGuard]},{path:'runtime',
                            component:i60.SettingsComponent,data:{title:'Runtime settings'},
                            canDeactivate:[i33.CanDeactivateGuard]},{path:'agents',
                            data:{title:'Internal Agents'},children:[{path:'',redirectTo:'submit',
                                pathMatch:'full',canDeactivate:[i33.CanDeactivateGuard]},
                                {path:'outboundprocessing',component:i58.AgentSettingsComponent,
                                    data:{title:'Outbound processing',header:'Outbound processing agent',
                                        type:'outboundProcessingAgents',betype:8,showwarning:true},
                                    canDeactivate:[i33.CanDeactivateGuard]},{path:'send',
                                    component:i58.AgentSettingsComponent,data:{title:'Send',
                                        header:'Send agent',type:'sendAgents',betype:2,
                                        showwarning:true},canDeactivate:[i33.CanDeactivateGuard]},
                                {path:'deliver',component:i58.AgentSettingsComponent,
                                    data:{title:'Deliver',header:'Deliver agent',type:'deliverAgents',
                                        betype:3,showwarning:true},canDeactivate:[i33.CanDeactivateGuard]},
                                {path:'notify',component:i58.AgentSettingsComponent,
                                    data:{title:'Notify',header:'Notify agent',type:'notifyAgents',
                                        betype:4,showwarning:true},canDeactivate:[i33.CanDeactivateGuard]},
                                {path:'receptionawareness',component:i61.ReceptionAwarenessAgentComponent,
                                    data:{title:'Reception awareness',header:'Reception awareness agent',
                                        type:'receptionAwarenessAgent',betype:5,showwarning:true},
                                    canDeactivate:[i33.CanDeactivateGuard]},{path:'pullsend',
                                    component:i58.AgentSettingsComponent,data:{title:'Pull send',
                                        header:'Pull send agent',type:'pullSendAgents',
                                        betype:7,showwarning:true},canDeactivate:[i33.CanDeactivateGuard]}]}],
                        data:{title:'Settings',icon:'fa-toggle-on'},canActivate:[i32.MustBeAuthorizedGuard],
                        canDeactivate:[i33.CanDeactivateGuard]},{path:'smpconfiguration',
                        component:i55.WrapperComponent,canActivate:[i32.MustBeAuthorizedGuard],
                        children:[{path:'',component:i62.SmpConfigurationComponent,
                            data:{title:'Smp configuration'},canDeactivate:[i33.CanDeactivateGuard]}]}]];
          },([] as any[]))]);
    });
//# sourceMappingURL=data:application/json;base64,eyJmaWxlIjoiQzovRGV2L2NvZGl0LnZpc3VhbHN0dWRpby5jb20vQVM0Lk5FVC9zb3VyY2UvRmUvRXUuRURlbGl2ZXJ5LkFTNC5GZS91aS9zcmMvYXBwL3NldHRpbmdzL3NldHRpbmdzLm1vZHVsZS5uZ2ZhY3RvcnkudHMiLCJ2ZXJzaW9uIjozLCJzb3VyY2VSb290IjoiIiwic291cmNlcyI6WyJuZzovLy9DOi9EZXYvY29kaXQudmlzdWFsc3R1ZGlvLmNvbS9BUzQuTkVUL3NvdXJjZS9GZS9FdS5FRGVsaXZlcnkuQVM0LkZlL3VpL3NyYy9hcHAvc2V0dGluZ3Mvc2V0dGluZ3MubW9kdWxlLnRzIl0sInNvdXJjZXNDb250ZW50IjpbIiAiXSwibWFwcGluZ3MiOiJBQUFBOzs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7Ozs7OzsifQ==