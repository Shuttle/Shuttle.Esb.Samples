Shuttle.ViewModels.Book = can.Map.extend({
    init: function (book) {
        this.attr('id', book.id);
        this.attr('title', book.title);
        this.attr('price', book.price);
    },

    setStatus: function (status) {
        this.attr('status', status);
    },

    orderInProcess: function () {
        return this.attr('status') === 'orderInProcess';
    },

    displayStatus: function () {
        var status = this.attr('status') || '';

        switch (status) {
            case 'orderInProcess':
                {
                    return 'The order is in process.';
                }
            case 'error':
                {
                    return 'An error has occurred.';
                }
        }

        return '';
    }
});

Shuttle.ViewModels.Books = can.Map.extend({
    books: new can.List(),

    init: function () {
        var self = this;

        Shuttle.Services.apiService.getJson('products')
            .done(function (data) {
                can.each(data, function(item) {
                    self.books.push(new Shuttle.ViewModels.Book(item));
                });
            });
    },

    add: function(content) {
        content.attr('buying', true);
    },

    remove: function(content) {
        content.attr('buying', false);
    },

    orderHandRolled: function (content, element, event) {
        can.ajax({
            url: Shuttle.configuration.api + 'orders',
            type: 'POST',
            data: { title: content.attr('title') }
        })
            .done(function () {
                content.setStatus('orderInProcess');
            })
            .fail(function () {
                content.setStatus('error');
            });
    }
});

Shuttle.Components.Books = can.Component.extend({
    tag: 'shuttle-books',
    template: can.view('assets/js/components/books.stache'),
    viewModel: Shuttle.ViewModels.Books
});


