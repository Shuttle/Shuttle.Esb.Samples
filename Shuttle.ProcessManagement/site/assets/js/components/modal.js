Shuttle.ViewModels.Modal = can.Map.extend({
    modalType: 'fade',
    dismissText: 'Ok',
    textType: 'primary'
});

Shuttle.Components.Modal = can.Component.extend({
    tag: 'shuttle-modal',
    template: can.view('assets/js/components/modal.stache'),
    viewModel: Shuttle.ViewModels.Modal
});


