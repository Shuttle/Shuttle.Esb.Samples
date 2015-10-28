Shuttle.Services.ApiService = can.Construct.extend({
    getJson: function (url, options) {
        var o = options || {};

        if (o.async == undefined) {
            o.async = true;
        }

        if (!o.cache) {
            o.cache = false;
        }

        return $.ajax({
            url: this.getApiUrl(url) + url,
            dataType: 'json',
            type: 'GET',
            async: o.async,
            cache: o.cache
        });
    },

    postJson: function (url, options) {
        var o = options || {};

        if (o.async == undefined) {
            o.async = true;
        }
        var postData = o.data || {};

        return $.ajax({
            url: this.getApiUrl(url) + url,
            type: 'POST',
            async: o.async,
            contentType: 'application/json',
            data: JSON.stringify(postData),
            beforeSend: o.beforeSend,
            timeout: o.timeout
        });
    },

    getApiUrl: function (suffix) {
        var url;

        if (suffix == undefined || suffix.length === 0) {
            throw new Error('Shuttle.Services.ApiService.getApiUrl: argument \'baseUrl\' may not be empty.');
        }

        url = Shuttle.configuration.api;

        if (suffix[0] !== '/' && url.indexOf('/', url.length - 1) !== -1) {
            url = url + '/';
        }

        return url;
    }
});

Shuttle.Services.apiService = new Shuttle.Services.ApiService();