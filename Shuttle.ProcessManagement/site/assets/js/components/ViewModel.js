Shuttle.ViewModel = can.Map.extend({}, {
    _options: undefined,

    init: function(options) {
        this.attr('_options', options || {});
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
        Shuttle.applicationState.attr('modalTitle', title);
        Shuttle.applicationState.attr('modalMessage', message);
        Shuttle.applicationState.attr('modalTextType', type);

        $('#modal-message').modal({ show: true });
    }
});