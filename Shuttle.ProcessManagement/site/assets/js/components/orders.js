Shuttle.ViewModels.Order = can.Map.extend({
    init: function (order) {
        this.applyValues(order);
    },

    applyValues: function(order) {
        this.attr('id', order.id);
        this.attr('customerName', order.customerName);
        this.attr('orderNumber', order.orderNumber);
        this.attr('orderDate', order.orderDate);
        this.attr('orderTotal', order.orderTotal);
        this.attr('status', order.status);
        this.attr('canCancel', order.canCancel);
        this.attr('canArchive', order.canArchive);
    }
});

Shuttle.ViewModels.Orders = Shuttle.ViewModel.extend({
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

                self._checkOrderCount();
            })
            .fail(function (xhr, textStatus, errorThrown) {
                self.showMessage(textStatus, 'Error fetching orders.', 'danger');
            })
            .always(function () {
                self.attr('fetching', false);
                self._poll();
            });
    },

    _checkOrderCount: function() {
        if (this.orders.length === 0) {
            this.showMessage('Active orders', 'There are no active orders.', 'info');
        } else {
            this.hideMessage();
        }
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
                self.showModalMessage('Order', 'Your cancellation request has been sent for processing.');
            })
            .fail(function () {
                self.showModalMessage('Server Error', 'The selected order could not be deleted.', 'danger');
            });
    },

    archiveOrder : function (content) {
        var self = this;
        var id = content.attr('id');

        Shuttle.Services.apiService.post('archivedorders/' + id)
            .done(function () {
                self.showModalMessage('Order Archiving', 'Your archive request has been sent for processing.');
            })
            .fail(function () {
                self.showModalMessage('Server Error', 'The selected order could not be archived.', 'danger');
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
                                element.applyValues(order);

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

                    self._checkOrderCount();
                })
                .fail(function(xhr, textStatus, errorThrown) {
                    self.showMessage(textStatus, 'Error fetching orders.', 'danger');
                })
                .always(function() {
                    self._poll();
                });
        }, 250);

        this.attr('_pollTimer', timeout);
    }
});

Shuttle.Components.Orders = can.Component.extend({
    tag: 'shuttle-orders',
    template: can.view('assets/js/components/orders.stache'),
    viewModel: Shuttle.ViewModels.Orders
});


