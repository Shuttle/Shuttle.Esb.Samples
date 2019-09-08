<template>
  <div v-if="orders.length">
    <h4>Active orders</h4>
    <table class="table table-sm">
      <thead class="thead-light">
        <tr class="row">
          <th class="col-1"></th>
          <th class="col-2">Customer name</th>
          <th class="col-2">Order number</th>
          <th class="col-2">Order date</th>
          <th class="col-3">Status</th>
          <th class="col-2 text-right">Total</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="order in orders" v-bind:key="order.orderNumber" class="row">
          <td class="col-1">
            <button
              v-if="order.canCancel"
              v-on:click="cancelOrder(order)"
              class="btn btn-default btn-danger btn-sm"
            >Cancel</button>
            <button
              v-if="order.canArchive"
              v-on:click="archiveOrder(order)"
              class="btn btn-default btn-danger btn-sm"
            >Archive</button>
          </td>
          <td class="col-2">{{order.customerName}}</td>
          <td class="col-2">{{order.orderNumber}}</td>
          <td class="col-2">{{order.orderDate}}</td>
          <td class="col-3">{{order.status}}</td>
          <td class="col-2 text-right">{{order.orderTotal}}</td>
        </tr>
      </tbody>
    </table>
  </div>
  <div v-else>
    <b-card>
      <h4 slot="header">(fetching orders)</h4>
      <b-card-text>
        <b-progress :value="100" striped :animated="true" class="mt-2"></b-progress>
      </b-card-text>
    </b-card>
  </div>
</template>

<script>
import axios from "axios";
import configuration from "@/configuration.js";
import state from "@/state.js";

export default {
  name: "shuttle-order",
  data() {
    return {
      pollTimer: 0,
      orders: []
    };
  },
  methods: {
    cancelOrder(order) {
      axios
        .delete(configuration.url + "/orders/" + order.id)
        .then(() => {
          state.alerts.add({
            message: "Your cancellation request has been sent for processing.",
            name: "order-cancel"
          });
        })
        .catch(() => {
          state.alerts.add({
            message: "The selected order could not be cancelled.",
            name: "order-cancel-error",
            type: "danger"
          });
        });
    },
    archiveOrder(order) {
      axios
        .delete(configuration.url + "/archivedorders/" + order.id)
        .then(() => {
          state.alerts.add({
            message: "Your archive request has been sent for processing.",
            name: "order-archive"
          });
        });
    },
    _poll: function() {
      var found;
      var timeout = this.pollTimer;
      var orderIdsToRemove = [];

      if (timeout) {
        clearTimeout(timeout);
      }

      timeout = setTimeout(
        function() {
          var self = this;

          axios
            .get(configuration.url + "/orders")
            .then(response => {
              response.data.data.forEach(function(order) {
                found = false;

                self.orders.forEach(function(element) {
                  if (element.id === order.id) {
                    element.id = order.id;
                    element.customerName = order.customerName;
                    element.orderNumber = order.orderNumber;
                    element.orderDate = order.orderDate;
                    element.orderTotal = order.orderTotal;
                    element.status = order.status;
                    element.canCancel = order.canCancel;
                    element.canArchive = order.canArchive;

                    found = true;
                  }
                });

                if (!found) {
                  self.orders.push(order);
                }
              });

              self.orders.forEach(function(existingOrder) {
                found = false;

                response.data.data.forEach(function(order) {
                  if (found) {
                    return;
                  }

                  found = order.id === existingOrder.id;
                });

                if (!found) {
                  orderIdsToRemove.push(existingOrder.id);
                }
              });

              orderIdsToRemove.forEach(function(id) {
                self._removeOrder(id);
              });
            })
            .catch(() => {
              state.alerts.add({
                message: "Could not fetch the orders.",
                name: "order-fetch-error",
                type: "danger"
              });
            })
            .then(() => {
              self._poll();
            });
        }.bind(this),
        500
      );

      this.pollTimer = timeout;
    }
  },
  mounted() {
    var self = this;

    axios
      .get(configuration.url + "/orders")
      .then(response => {
        this.orders = response.data.data;
      })
      .then(() => {
        self._poll();
      });
  }
};
</script>
