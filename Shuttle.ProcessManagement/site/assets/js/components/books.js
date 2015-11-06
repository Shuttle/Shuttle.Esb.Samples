Shuttle.ViewModels.Book = can.Map.extend({
    init: function (book) {
        this.attr('id', book.id);
        this.attr('title', book.title);
        this.attr('price', book.price);
    }
});

Shuttle.ViewModels.Books = Shuttle.ViewModel.extend({
    init: function () {
        this.validatePresenceOf('customerName');
        this.validateLengthOf('customerName', 0, 65);
        this.validatePresenceOf('customerEMail');
        this.validateLengthOf('customerEMail', 0, 130);
        this.validateFormatOf(['customerEMail'], /^(([^<>()[\]\.,;:\s@\"]+(\.[^<>()[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()[\]\.,;:\s@\"]+\.)+[^<>()[\]\.,;:\s@\"]{2,})$/i, {
            message: 'invalid email'
        });
    }
}, {
    books: new can.List(),
    total: 0,
    canOrder: false,

    invalidCustomerName: function () {
        return this.errors('customerName') != undefined;
    },

    customerNameErrors: function () {
        return this.invalidCustomerName()
            ? this.errors('customerName').customerName
            : [];
    },

    invalidCustomerEMail: function () {
        return this.errors('customerEMail') != undefined;
    },

    customerEMailErrors: function () {
        return this.invalidCustomerEMail()
            ? this.errors('customerEMail').customerEMail
            : [];
    },

    init: function () {
        var self = this;

        this.attr('fetching', true);

        Shuttle.Services.apiService.get('products')
            .done(function (data) {
                can.each(data, function (item) {
                    self.books.push(new Shuttle.ViewModels.Book(item));
                });
            })
            .fail(function (xhr, textStatus, errorThrown) {
                self.showMessage(textStatus, 'Error fetching products.', 'danger');
            })
            .then(function () {
                self.attr('fetching', false);
            });
    },

    calculateTotal: function () {
        var total = 0;

        can.each(this.books, function (book) {
            if (book.buying) {
                total = total + book.price;
            }
        });

        this.attr('canOrder', total > 0);
        this.attr('total', total);
    },

    add: function (content) {
        content.attr('buying', true);

        this.calculateTotal();
    },

    remove: function (content) {
        content.attr('buying', false);

        this.calculateTotal();
    },

    canSubmit: function () {
        return this.errors() == undefined;
    },

    cancel: function () {
        this._clearOrder();
    },

    _clearOrder: function () {
        this.books.each(function (book) {
            book.attr('buying', false);
        });

        this.calculateTotal();
    },

    _submitOrder: function(targetSystem) {
        var self = this;
        var order = {
            productIds: [],
            targetSystem: targetSystem,
            customerName: this.attr('customerName'),
            customerEMail: this.attr('customerEMail')
        };

        if (!this.canSubmit()) {
            return;
        }

        can.each(this.books, function (book) {
            if (book.buying) {
                order.productIds.push(book.id);
            }
        });

        Shuttle.Services.apiService.post('orders', { data: order })
            .done(function () {
                self._clearOrder();

                self.showModalMessage('Order', 'Your order has been sent for processing.');
            })
            .fail(function (xhr, textStatus, errorThrown) {
                self.showMessage('Error submitting order.', errorThrown, 'danger');
            });
    },

    orderCustom: function () {
        this._submitOrder('custom');
    },

    orderCustomEventSource: function() {
        this._submitOrder('custom / event-source');
    },

    orderEventSourceModule: function() {
        this._submitOrder('event-source / module');
    }
});

Shuttle.Components.Books = can.Component.extend({
    tag: 'shuttle-books',
    template: can.view('assets/js/components/books.stache'),
    viewModel: Shuttle.ViewModels.Books
});


