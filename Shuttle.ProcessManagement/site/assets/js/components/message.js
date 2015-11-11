Shuttle.Components.Message = can.Component.extend({
    tag: 'shuttle-message',
    template: can.view('assets/js/components/message.stache'),
    viewModel: new can.Map({
        _messageType: function() {
            return this.attr('type')
                ? this.attr('type')
                : 'primary';
        }
    })
});


