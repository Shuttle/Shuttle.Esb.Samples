Shuttle.ViewModels.Order = can.Map.extend({
    init: function (order) {
        this.attr('id', order.id);
        this.attr('customerName', order.customerName);
        this.attr('orderNumber', order.orderNumber);
        this.attr('orderDate', order.orderDate);
        this.attr('orderTotal', order.orderTotal);
        this.attr('status', order.status);
    },

    setStatus: function (status) {
        this.attr('status', status);
    },

    displayStatus: function () {
        switch (status) {
            case 'orderInProcess':
                {
                    return 'The order is in process.';
                }
        }

        return '';
    }
});

Shuttle.ViewModels.Orders = can.Map.extend({
    orders: new can.List(),

    init: function () {
        var self = this;

        Shuttle.Services.apiService.getJson('orders')
            .done(function (data) {
                can.each(data, function(item) {
                    self.orders.push(new Shuttle.ViewModels.Order(item));
                });
            });
    },

    cancel: function (content, element, event) {
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

Shuttle.Components.Orders = can.Component.extend({
    tag: 'shuttle-orders',
    template: can.view('assets/js/components/orders.stache'),
    viewModel: Shuttle.ViewModels.Orders
});


