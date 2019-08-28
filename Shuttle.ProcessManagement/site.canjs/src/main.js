import $ from 'jquery';
import 'popper.js';
import 'bootstrap';
import '@fortawesome/fontawesome-svg-core';

import 'bootstrap/dist/css/bootstrap.css';

import template from "./main.stache";

import 'shuttle-canstrap';
import {options} from 'shuttle-can-api';
import loader from '@loader';

import '~/books/';
import '~/orders/';

import state from '~/state';

options.url = loader.serviceBaseURL;

$.ajaxPrefilter(function (options, originalOptions) {
    options.error = function (xhr) {
        if (xhr.responseJSON) {
            state.alerts.add({message: xhr.responseJSON.message, type: 'danger', name: 'ajax-prefilter-error'});
        } else {
            state.alerts.add({
                message: xhr.status + ' / ' + xhr.statusText,
                type: 'danger',
                name: 'ajax-prefilter-error'
            });
        }

        if (originalOptions.error) {
            originalOptions.error(xhr);
        }
    };
});

$('#application-container').html(template(state));
