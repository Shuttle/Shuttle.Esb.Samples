import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import state from '../state';
import { finalize } from 'rxjs/operators';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'shuttle-orders',
  templateUrl: './shuttle-orders.component.html',
  styleUrls: ['./shuttle-orders.component.css']
})
export class ShuttleOrdersComponent implements OnInit {
  orders = [];
  private shouldPoll: boolean;
  pollTimeout: number;

  constructor(
    private http: HttpClient
  ) {
    this.shouldPoll = true;
    this.pollTimeout = 0;
  }

  ngOnInit() {
    this.poll();
  }

  cancelOrder(order) {
    this.http
      .delete(environment.url + '/orders/' + order.id)
      .subscribe(() => {
        state.alerts.add({
          message: 'Your cancellation request has been sent for processing.',
          name: 'order-cancel'
        });
      }, () => {
        state.alerts.add({
          message: 'The selected order could not be cancelled.',
          name: 'order-cancel-error',
          type: 'danger'
        });
      });
  }

  archiveOrder(order) {
    this.http
      .delete(environment.url + '/archivedorders/' + order.id)
      .subscribe(() => {
        state.alerts.add({
          message: 'Your archive request has been sent for processing.',
          name: 'order-archive'
        });
      });
  }

  private poll() {
    if (this.pollTimeout) {
      clearTimeout(this.pollTimeout);
    }

    if (!this.shouldPoll) {
      return;
    }

    this.pollTimeout = window.setTimeout(() => {
      this.http
        .get<any>(environment.url + '/orders')
        .pipe(finalize(() => { this.poll(); }))
        .subscribe(data => {
          this.orders = data.data;
        }, () => {
          state.alerts.add({
            message: 'Could not fetch the orders.',
            name: 'order-fetch-error',
            type: 'danger'
          });
        });
    }, 500);
  }
}
