import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import state from '../state';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'shuttle-books',
  templateUrl: './shuttle-books.component.html',
  styleUrls: ['./shuttle-books.component.css']
})
export class ShuttleBooksComponent implements OnInit {
  books = [];
  customerName: string;
  customerEMail: string;
  customerNameValidation: string;
  customerEMailValidation: string;

  constructor(
    private http: HttpClient
  ) {
  }

  ngOnInit() {
    this.http.get<any>(environment.url + '/products')
      .subscribe(data => {
        this.books = data.data as [];
      });
  }

  get total() {
    return this.books.reduce((result, item) => {
      return result + (item.buying ? item.price : 0);
    }, 0);
  }

  canOrder() {
    return this.total > 0;
  }

  toggle(book) {
    book.buying = !book.buying;
  }

  get canPlaceOrder() {
    // tslint:disable-next-line: max-line-length
    const email = /^(([^<>()\[\]\\.,;:\s@"]+(\.[^<>()\[\]\\.,;:\s@"]+)*)|(".+"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;

    this.customerNameValidation = '';
    this.customerEMailValidation = '';

    if (!this.customerName) {
      this.customerNameValidation = 'can\'t be blank';
    }

    if (!this.customerEMail) {
      this.customerEMailValidation = 'can\'t be blank';
    }

    if (!this.customerEMailValidation && !email.test(this.customerEMail)) {
      this.customerEMailValidation = 'not a valid e-mail address';
    }

    return !this.customerNameValidation && !this.customerEMailValidation;
  }

  orderCustom() {
    this.submitOrder('custom');
  }

  orderCustomEventSource() {
    this.submitOrder('custom / event-source');
  }

  orderEventSourceModule() {
    this.submitOrder('event-source / module');
  }

  private submitOrder(targetSystem: string) {
    if (!this.canPlaceOrder) {
      return false;
    }

    const order = {
      productIds: [],
      targetSystem,
      customerName: this.customerName,
      customerEMail: this.customerEMail
    };

    this.books.forEach((book) => {
      if (book.buying) {
        order.productIds.push(book.id);
      }
    });

    this.http
      .post(environment.url + '/orders', order)
      .subscribe(() => {
        this.clearOrder();

        state.alerts.add({
          message: 'Your order has been sent for processing.',
          name: 'order-placed'
        });
      },
      error => {
        state.alerts.add({
          message: error.message,
          name: 'order-error',
          type: 'danger'
        });
      });
  }

  private clearOrder() {
      this.books.forEach((book) => {
        book.buying = false;
      });
  }
}
