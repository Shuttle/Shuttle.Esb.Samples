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
    _pollTimer: undefined,

    init: function () {
        var self = this;

        this.attr('fetching', true);

        Shuttle.Services.apiService.getJson('orders')
            .done(function(data) {
                can.each(data, function(item) {
                    self.orders.push(new Shuttle.ViewModels.Order(item));
                });

                if (self.orders.length === 0) {
                    self.showMessage('Active orders', 'There are no active orders.', 'info');
                }
            })
            .fail(function (xhr, textStatus, errorThrown) {
                self.showMessage(textStatus, 'Error fetching orders.', 'danger');
            })
            .always(function () {
                self.attr('fetching', false);
                self._poll();
            });
    },

    showMessage: function (title, message, type) {
        this.attr('hasMessage', true);
        this.attr('messageType', type);
        this.attr('messageTitle', title);
        this.attr('message', message);
    },

    hideMessage: function () {
        this.attr('hasMessage', false);
    },

    cancel: function (content, element, event) {
        can.ajax({
            url: Shuttle.configuration.api + 'orders/' + content.attr('id'),
            type: 'DELETE',
            data: { title: content.attr('title') }
        })
            .done(function () {
                content.setStatus('orderInProcess');
            })
            .fail(function () {
                content.setStatus('error');
            });
    },

    _poll: function () {
        var found;
        var self = this;
        var timeout = this.get('_pollTimer');

        if (timeout) {
            clearTimeout(timeout);
        }

        timeout = setTimeout(function () {
            Shuttle.Services.apiService.getJson('orders')
                .done(function (data) {
                    found = false;

                    can.each(data, function (item) {
                        self.orders.forEach(function(element, index, list) {
                            if (element.attr('id') === item.id) {
                                element.attr('orderNumber', item.orderNumber);
                                element.attr('orderDate', item.orderDate);
                                element.attr('orderTotal', item.orderTotal);
                                element.attr('status', item.statu);

                                found = true;
                            }
                        });

                        if (!found) {
                            self.orders.push(new Shuttle.ViewModels.Order(item));
                        }
                    });
                })
                .fail(function(xhr, textStatus, errorThrown) {
                    self.showMessage(textStatus, 'Error fetching orders.', 'danger');
                })
                .always(function() {
                    self._poll();
                });
        }, 1000);

        this.set('_pollTimer', timeout);
    }
});

Shuttle.Components.Orders = can.Component.extend({
    tag: 'shuttle-orders',
    template: can.view('assets/js/components/orders.stache'),
    viewModel: Shuttle.ViewModels.Orders
});


