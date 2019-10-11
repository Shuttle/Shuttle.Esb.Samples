import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

import { AppComponent } from './app.component';
import { ShuttleBooksComponent } from './shuttle-books/shuttle-books.component';
import { ShuttleOrdersComponent } from './shuttle-orders/shuttle-orders.component';

@NgModule({
  declarations: [
    AppComponent,
    ShuttleBooksComponent,
    ShuttleOrdersComponent
  ],
  imports: [
    BrowserModule,
    NgbModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
