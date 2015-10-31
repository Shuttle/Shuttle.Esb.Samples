Shuttle.ViewModels.Book = can.Map.extend({
    init: function (book) {
        this.attr('id', book.id);
        this.attr('title', book.title);
        this.attr('price', book.price);
    }
});

Shuttle.ViewModels.Books = can.Map.extend({
    init: function() {
        this.validatePresenceOf('customerName');
        this.validateLengthOf('customerName', 0, 65);
        this.validatePresenceOf('customerEMail');
        this.validateLengthOf('customerEMail', 0, 130);
        this.validateFormatOf(['customerEMail'], /^(([^<>()[\]\.,;:\s@\"]+(\.[^<>()[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()[\]\.,;:\s@\"]+\.)+[^<>()[\]\.,;:\s@\"]{2,})$/i, {
            message: 'invalid email'
        });
    }
},{
    books: new can.List(),
    total: 0,
    canOrder: false,
    hasMessage: false,

    invalidCustomerName: function() {
        return this.errors('customerName') != undefined;
    },

    customerNameErrors: function() {
        return this.invalidCustomerName()
            ? this.errors('customerName').customerName
            : [];
    },

    invalidCustomerEMail: function() {
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

        Shuttle.Services.apiService.getJson('products')
            .done(function(data) {
                can.each(data, function(item) {
                    self.books.push(new Shuttle.ViewModels.Book(item));
                });
            })
            .fail(function(xhr, textStatus, errorThrown) {
                self.showMessage(textStatus, 'Error fetching products.', 'danger');
            })
            .then(function() {
                self.attr('fetching', false);
            });
    },

    showMessage: function (title, message, type) {
        this.attr('hasMessage', true);
        this.attr('messageType', type);
        this.attr('messageTitle', title);
        this.attr('message', message);
    },

    hideMessage: function() {
        this.attr('hasMessage', false);
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

    canSubmit: function() {
        return this.errors() == undefined;
    },

    orderHandRolled: function () {
        var order = {
            productIds: [],
            targetSystem: 'HandRolled',
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

        Shuttle.Services.apiService.postJson('orders', { data: order })
            .done(function() {
                alert('done!');
            })
            .fail(function(xhr, textStatus, errorThrown) {
                self.showMessage('Error submitting order.', errorThrown, 'danger');
            });
    }
});

Shuttle.Components.Books = can.Component.extend({
    tag: 'shuttle-books',
    template: can.view('assets/js/components/books.stache'),
    viewModel: Shuttle.ViewModels.Books
});


