import DefineMap from 'can-define/map/';
import DefineList from 'can-define/list/';
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
    },
    toggle() {
        this.buying = !this.buying;
    },
});

const Books = DefineList.extend({
    '#': Book
})

const api = {
    products: new Api({
        endpoint: 'products',
        Map: Book
    }),
    orders: new Api('orders')
};

const ViewModel = DefineMap.extend({
    books: {
        Type: Books
    },
    actions: {
        Type: ActionList,
        default() {
            const self = this;

            return [
                {
                    text: 'Custom',
                    click: function () {
                        self.orderCustom()
                    }
                },
                {
                    text: 'Custom / EventSource',
                    click: function () {
                        self.orderCustomEventSource();
                    }
                },
                {
                    text: 'EventSource / Module',
                    click: function () {
                        self.orderEventSourceModule();
                    }
                },
            ]
        }
    },
    refreshTimestamp: {
        type: 'string'
    },
    get booksPromise() {
        const self = this;
        const refreshTimestamp = this.refreshTimestamp;
        return api.products.list()
            .then(function (books) {
                self.books = new Books(books);
            });
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
        get() {
            var result = 0;

            each(this.books, function (book) {
                if (book.buying) {
                    result = result + book.price;
                }
            });

            return result;
        }
    },
    canOrder: {
        get() {
            return this.total > 0;
        }
    },
    cancel: function () {
        this._clearOrder();
    },
    _clearOrder: function () {
        this.books.forEach(function (book) {
            book.buying = false;
        });
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

        api.orders.post(order)
            .then(function () {
                    self._clearOrder();

                    alerts.show({message: 'Your order has been sent for processing.', name: 'order-placed'});
                },
                function (xhr, textStatus, errorThrown) {
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
