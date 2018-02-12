﻿import DefineMap from 'can-define/map/';
import DefineList from 'can-define/list/';
import Component from 'can-component';
import view from './orders.stache';
import Api from 'shuttle-can-api';
import {alerts} from 'shuttle-canstrap/alerts/';
import each from 'can-util/js/each/';

var Order = DefineMap.extend({
    id: {
        type: 'string',
        default: '',
    },
    customerName: {
        type: 'string',
        default: '',
    },
    orderNumber: {
        type: 'string',
        default: '',
    },
    orderDate: {
        type: 'date',
        default: '',
    },
    orderTotal: {
        type: 'number',
        default: '',
    },
    status: {
        type: 'string',
        default: '',
    },
    canCancel: {
        type: 'boolean',
        default: false,
    },
    canArchive: {
        type: 'boolean',
        default: false,
    },
    applyValues: function (order) {
        this.id = order.id;
        this.customerName = order.customerName;
        this.orderNumber = order.orderNumber;
        this.orderDate = order.orderDate;
        this.orderTotal = order.orderTotal;
        this.status = order.status;
        this.canCancel = order.canCancel;
        this.canArchive = order.canArchive;
    },
    archiveOrder() {
        api.archivedOrders.delete({id: this.id})
            .then(function () {
                    alerts.show({message: 'Your archive request has been sent for processing.', name: 'order-archive'});
                },
                function () {
                    alerts.show({
                        message: 'The selected order could not be archived.',
                        name: 'order-archive-error',
                        type: 'danger'
                    });
                });
    },
    cancelOrder: function () {
        api.orders.delete({id: this.id})
            .then(
                function () {
                    alerts.show({
                        message: 'Your cancellation request has been sent for processing.',
                        name: 'order-cancel'
                    });
                },
                function () {
                    alerts.show({
                        message: 'The selected order could not be cancelled.',
                        name: 'order-cancel-error',
                        type: 'danger'
                    });
                });
    }
});

const Orders = DefineList.extend({
    '#': Order
})

var api = {
    orders: new Api({
        endpoint: 'orders/{id}',
        Map: Order
    }),
    archivedOrders: new Api("archivedorders/{id}")
};

var ViewModel = DefineMap.extend({
    fetching: {
        type: 'boolean',
        default: true
    },
    orders: {
        value: new Orders()
    },
    refreshTimestamp: {
        type: 'string'
    },
    get ordersPromise() {
        const refreshTimestamp = this.refreshTimestamp;
        return api.orders.list();
    },
    _pollTimer: {
        type: 'number'
    },
    init: function () {
        var self = this;

        this.fetching = true;

        api.orders.list()
            .then(
                function (data) {
                    each(data, function (item) {
                        self.orders.push(new Order(item));
                    });
                },
                function (error) {
                    alerts.show({
                        message: 'Could not fetch the orders.',
                        name: 'order-fetch-error',
                        type: 'danger'
                    });
                })
            .then(function () {
                self.fetching = false;
                self._poll();
            });
    },
    _removeOrder: function (id) {
        var removeIndex = undefined;

        this.orders.forEach(function (element, index) {
            if (removeIndex !== undefined) {
                return;
            }

            if (element.id === id) {
                removeIndex = index;
            }
        });

        if (removeIndex === undefined) {
            return;
        }

        this.orders.splice(removeIndex, 1);
    },
    refresh: function () {
        this.refreshTimestamp = Date.now();
    },
    _poll: function () {
        var found;
        var self = this;
        var timeout = this._pollTimer;
        var orderIdsToRemove = [];

        if (timeout) {
            clearTimeout(timeout);
        }

        timeout = setTimeout(function () {
            api.orders.list()
                .then(
                    function (data) {
                        each(data, function (order) {
                            found = false;

                            self.orders.forEach(function (element) {
                                if (element.id === order.id) {
                                    element.applyValues(order);

                                    found = true;
                                }
                            });

                            if (!found) {
                                self.orders.push(new Order(order));
                            }
                        });

                        self.orders.forEach(function (existingOrder) {
                            found = false;

                            each(data, function (order) {
                                if (found) {
                                    return;
                                }

                                found = (order.id === existingOrder.id);
                            });

                            if (!found) {
                                orderIdsToRemove.push(existingOrder.id);
                            }
                        });

                        each(orderIdsToRemove, function (id) {
                            self._removeOrder(id);
                        });
                    },
                    function (error) {
                        alerts.show({
                            message: 'Could not fetch the orders.',
                            name: 'order-fetch-error',
                            type: 'danger'
                        });
                    })
                .then(function () {
                    self._poll();
                });
        }, 500);

        this._pollTimer = timeout;
    }
});

export default Component.extend({
    tag: 'shuttle-orders',
    view,
    ViewModel
});
