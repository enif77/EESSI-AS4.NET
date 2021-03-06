import { Http, RequestOptions } from '@angular/http';
import { ClipboardModule } from 'ngx-clipboard';
import { CommonModule } from '@angular/common';
import { NgModule, ApplicationRef, APP_INITIALIZER } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, FormControlDirective } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule } from '@angular/router';
import { removeNgStyles, createNewHosts, createInputTransfer } from '@angularclass/hmr';

import { SettingsModule } from './settings';
import { PmodesModule } from './pmodes/pmodes.module';
import { MonitorModule } from './monitor/monitor.module';

import { ENV_PROVIDERS } from './environment';
import { ROUTES } from './app.routes';
import { AppState, InternalStateType } from './app.service';
import { AuthenticationStore } from './authentication/authentication.store';

import { NoContentComponent } from './no-content';
import { AppComponent } from './app.component';

import { As4ComponentsModule } from './common';
import { AuthenticationModule } from './authentication';
import { errorHandlingServices } from '../app/common/as4components.module';
import { SubmittoolModule } from './submittool/submittool.module';
import { RuntimeModule } from './runtime/runtime.module';
import { RuntimeService } from './settings/runtime.service';
import { UserModule } from './users/user.module';
import { AuthenticationService } from './authentication/authentication.service';
import { SetupModule } from './setup/setup.module';

import '../styles/external.scss';

export function runtimeService(runtimeService: RuntimeService): () => Promise<boolean> {
    return () => runtimeService.getAll();
}

type StoreType = {
    state: InternalStateType,
    restoreInputValues: () => void,
    disposeOldHosts: () => void
};

@NgModule({
    bootstrap: [AppComponent],
    declarations: [
        AppComponent,
        NoContentComponent,
    ],
    imports: [
        RouterModule.forRoot(ROUTES, { useHash: false }),
        RuntimeModule,
        BrowserModule,
        FormsModule,
        HttpModule,
        CommonModule,
        MonitorModule,

        UserModule,
        SettingsModule,
        PmodesModule,
        SubmittoolModule,
        As4ComponentsModule,
        AuthenticationModule,
        ClipboardModule,
        SetupModule
    ],
    providers: [ // expose our Services and Providers into Angular's dependency injection
        AppState,
        // ...errorHandlingServices,
        { provide: APP_INITIALIZER, useFactory: runtimeService, deps: [RuntimeService], multi: true }
    ]
})
export class AppModule {
    constructor(public appRef: ApplicationRef, public appState: AppState) { }

    public hmrOnInit(store: StoreType) {
        if (!store || !store.state) {
            return;
        }
        console.log('HMR store', JSON.stringify(store, null, 2));
        // set state
        this.appState._state = store.state;
        // set input values
        if ('restoreInputValues' in store) {
            let restoreInputValues = store.restoreInputValues;
            setTimeout(restoreInputValues);
        }

        this.appRef.tick();
        delete store.state;
        delete store.restoreInputValues;
    }

    public hmrOnDestroy(store: StoreType) {
        const cmpLocation = this.appRef.components.map((cmp) => cmp.location.nativeElement);
        // save state
        const state = this.appState._state;
        store.state = state;
        // recreate root elements
        store.disposeOldHosts = createNewHosts(cmpLocation);
        // save input values
        store.restoreInputValues = createInputTransfer();
        // remove styles
        removeNgStyles();
    }

    public hmrAfterDestroy(store: StoreType) {
        // display new elements
        store.disposeOldHosts();
        delete store.disposeOldHosts;
    }
}
