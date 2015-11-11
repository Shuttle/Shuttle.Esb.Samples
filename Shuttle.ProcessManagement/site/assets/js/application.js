Shuttle.applicationState = new can.Map({
});

$(function () {
    $('#application-container').html(can.view('#application-template', Shuttle.applicationState));
});