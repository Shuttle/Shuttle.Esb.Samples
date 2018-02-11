import $ from 'jquery';
import 'popper.js';
import 'bootstrap';

import 'bootstrap/dist/css/bootstrap.css';
import 'font-awesome/css/font-awesome.css';

import template from "./main.stache";

import 'shuttle-canstrap';
import {options} from 'shuttle-can-api';
import loader from '@loader';

import '~/books/';
import '~/orders/';

import {alerts} from 'shuttle-canstrap/alerts/';

options.wire({
    url: function(){
        return loader.serviceBaseURL;
    }
});

$.ajaxPrefilter(function (options, originalOptions) {
    options.error = function (xhr) {
        if (xhr.responseJSON) {
            alerts.show({message: xhr.responseJSON.message, type: 'danger', name: 'ajax-prefilter-error'});
        } else {
            alerts.show({
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

$('#application-container').html(template());
