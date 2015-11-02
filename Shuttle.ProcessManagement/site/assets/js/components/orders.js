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
    }
});

Shuttle.ViewModels.Orders = can.Map.extend({
    orders: new can.List(),
    _pollTimer: undefined,

    init: function () {
        var self = this;

        this.attr('fetching', true);

        Shuttle.Services.apiService.get('orders')
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

    showModalMessage: function (title, message, type) {
        this.attr('modalTitle', title);
        this.attr('modalMessage', message);
        this.attr('modalTextType', type);

        $('#order-modal').modal({ show: true });
    },

    _removeOrder: function(id) {
        var removeIndex = undefined;

        this.orders.forEach(function(element, index) {
            if (removeIndex != undefined) {
                return;
            }

            if (element.attr('id') === id) {
                removeIndex = index;
            }
        });

        if (removeIndex == undefined) {
            return;
        }

        this.orders.splice(removeIndex, 1);
    },

    cancelOrder : function (content) {
        var self = this;
        var id = content.attr('id');

        Shuttle.Services.apiService.delete('orders/' + id)
            .done(function () {
                self._removeOrder(id);
            })
            .fail(function () {
                self.showModalMessage('Server Error', 'The selected order could not be deleted.', 'danger');
            });
    },

    _poll: function () {
        var found;
        var self = this;
        var timeout = this.attr('_pollTimer');
        var orderIdsToRemove = [];

        if (timeout) {
            clearTimeout(timeout);
        }

        timeout = setTimeout(function () {
            Shuttle.Services.apiService.get('orders')
                .done(function (data) {
                    can.each(data, function (order) {
                        found = false;

                        self.orders.each(function (element) {
                            if (element.attr('id') === order.id) {
                                element.attr('orderNumber', order.orderNumber);
                                element.attr('orderDate', order.orderDate);
                                element.attr('orderTotal', order.orderTotal);
                                element.attr('status', order.status);

                                found = true;
                            }
                        });

                        if (!found) {
                            self.orders.push(new Shuttle.ViewModels.Order(order));
                        }
                    });

                    self.orders.each(function (existingOrder) {
                        found = false;

                        can.each(data, function (order) {
                            if (found) {
                                return;
                            }

                            found = (order.id === existingOrder.attr('id'));
                        });

                        if (!found) {
                            orderIdsToRemove.push(existingOrder.attr('id'));
                        }
                    });

                    can.each(orderIdsToRemove, function(id) {
                        self._removeOrder(id);
                    });

                    self.hideMessage();
                })
                .fail(function(xhr, textStatus, errorThrown) {
                    self.showMessage(textStatus, 'Error fetching orders.', 'danger');
                })
                .always(function() {
                    self._poll();
                });
        }, 1000);

        this.attr('_pollTimer', timeout);
    }
});

Shuttle.Components.Orders = can.Component.extend({
    tag: 'shuttle-orders',
    template: can.view('assets/js/components/orders.stache'),
    viewModel: Shuttle.ViewModels.Orders
});


