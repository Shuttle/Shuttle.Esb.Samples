Shuttle.ViewModels.Book = can.Map.extend({
    init: function (title) {
        this.attr('title', title);
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
        this.addBook('Patterns of  Enterprise Application Architecture - Martin Fowler');
        this.addBook('Applying Domain-Driven Design Patterns - Jimmy Nilsso');
        this.addBook('Implementing Domain-Driven Design - Vaughn Vernon');
        this.addBook('Domain-Driven Design - Eric Evans');
        this.addBook('Refactoring - Martin Fowler, et. al.');
        this.addBook('Test Driven Development - Kent Beck');
    },

    addBook: function (title) {
        this.books.push(new Shuttle.ViewModels.Book(title));
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


