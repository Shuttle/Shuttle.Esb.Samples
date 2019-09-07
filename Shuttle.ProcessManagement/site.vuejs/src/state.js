export class Alerts {
    constructor() {
        this._key = 1;
        this.messages = [];

        this._removeExpiredAlerts();
    }

    add(options) {
        if (!options || !options.message) {
            return;
        }
        if (options.key || options.name) {
            this.remove(options);
        }
        this._push(options);
    }

    clear() {
        this.messages = [];
    }

    remove(options) {
        if (!options || (!options.key && !options.name && !options.type)) {
            return;
        }
        this.messages = this.messages.filter(function (item) {
            var keep = true;
            if (options.key) {
                keep = item.key !== options.key;
            }
            else {
                if (options.name) {
                    keep = item.name !== options.name;
                }
                else {
                    if (options.type) {
                        keep = (item.type || 'info') !== options.type;
                    }
                }
            }
            return keep;
        });
    }

    _push(options, mode) {
        var key = this._key + 1;
        var expiryDate = new Date();
        if (!options || !options.message) {
            return;
        }
        var type = options.type || 'info';
        expiryDate.setSeconds(expiryDate.getSeconds() + 10);
        const message = {
            message: options.message,
            type: type,
            mode: mode,
            key: key,
            name: options.name,
            expiryDate: expiryDate
        };
        this.messages.push(message);
        this._key = key;
    }

    _removeExpiredAlerts() {
        var self = this;
        var date = new Date();

        if (!this.messages) {
            return;
        }

        this.messages = this.messages.filter(function (message) {
            return (message.expiryDate &&
                message.expiryDate < date) ? message : undefined;
        });

        setTimeout(function() {
            self._removeExpiredAlerts();
        }, 500);
    }
}

const state = {
    alerts: new Alerts()
};

export default state;