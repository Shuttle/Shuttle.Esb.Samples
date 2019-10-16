import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { HttpClientModule } from '@angular/common/http';

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
    FormsModule,
    NgbModule,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
