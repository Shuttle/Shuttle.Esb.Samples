import DefineMap from 'can-define/map/';
import Component from 'can-component';
import view from './books.stache';
import each from 'can-util/js/each/';
import Api from 'shuttle-can-api';
import validator from 'can-define-validate-validatejs';
import {alerts} from 'shuttle-canstrap/alerts/';
import {ActionList} from 'shuttle-canstrap/button/';

const Book = DefineMap.extend({
    id: {
        type: 'string',
        default: ''
    },
    title: {
        type: 'string',
        default: ''
    },
    price: {
        type: 'number',
        default: 0
    },
    buying: {
        type: 'boolean',
        default: false
    }
});

const api = {
    books: new Api({
        endpoint: 'books',
        Map: Book
    }),
    orders: new Api('orders')
};

const ViewModel = DefineMap.extend({
    actions: {
        Type: ActionList,
        default: [
            {
                text: 'Custom',
                click: function () {
                    this.orderCustom()
                }
            },
            {
                text: 'Custom / EventSource',
                click: function () {
                    this.orderCustomEventSource();
                }
            },
            {
                text: 'EventSource / Module',
                click: function () {
                    this.orderEventSourceModule();
                }
            },
        ]
    },
    refreshTimestamp: {
        type: 'string'
    },
    get booksPromise() {
        const refreshTimestamp = this.refreshTimestamp;
        return api.books.list();
    },
    customerName: {
        type: 'string',
        default: '',
        validate: {
            presence: true,
            length: {
                maximum: 65
            }
        }
    },
    customerEMail: {
        type: 'string',
        default: '',
        validate: {
            presence: true,
            email: true,
            length: {
                maximum: 130
            }
        }
    },
    total: {
        type: 'number',
        default: 0
    },
    canOrder: {
        type: 'boolean',
        default: false
    },
    init: function () {
        var self = this;

        api.books.get('products');
    },
    calculateTotal: function () {
        var total = 0;

        each(this.books, function (book) {
            if (book.buying) {
                total = total + book.price;
            }
        });

        this.total = total;
    },
    canOrder() {
        this.total > 0;
    },
    toggleBuying: function (book) {
        book.buying = !book.buying;

        this.calculateTotal();
    },
    cancel: function () {
        this._clearOrder();
    },
    _clearOrder: function () {
        this.books.each(function (book) {
            book.buying = false;
        });

        this.calculateTotal();
    },
    _submitOrder: function (targetSystem) {
        var self = this;

        if (!!this.errors()) {
            return false;
        }

        var order = {
            productIds: [],
            targetSystem: targetSystem,
            customerName: this.customerName,
            customerEMail: this.customerEMail
        };

        each(this.books, function (book) {
            if (book.buying) {
                order.productIds.push(book.id);
            }
        });

        api.orders.post('orders', {data: order})
            .done(function () {
                self._clearOrder();

                alerts.show({message: 'Your order has been sent for processing.', name: 'order-placed'});
            })
            .fail(function (xhr, textStatus, errorThrown) {
                alerts.show({message: errorThrown, name: 'order-error', type: 'danger'});
            });
    },

    orderCustom: function () {
        this._submitOrder('custom');
    },

    orderCustomEventSource: function () {
        this._submitOrder('custom / event-source');
    },

    orderEventSourceModule: function () {
        this._submitOrder('event-source / module');
    }
});

validator(ViewModel);

export default Component.extend({
    tag: 'shuttle-books',
    view,
    ViewModel
});
