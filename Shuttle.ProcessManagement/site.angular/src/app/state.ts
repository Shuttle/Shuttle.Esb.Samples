class Message {
    readonly key: number;
    readonly name: string;
    readonly message: string;
    readonly type: string;
    readonly expiryDate: Date;
}

class Alerts {
    private key: number;
    private messages: Message[];

    constructor(
    ) {
        this.key = 1;
        this.messages = [];

        this.removeExpiredAlerts = this.removeExpiredAlerts.bind(this);

        this.removeExpiredAlerts();
    }

    add(options) {
        if (!options || !options.message) {
            return;
        }
        if (options.key || options.name) {
            this.remove(options);
        }
        this.push(options);
    }

    clear() {
        this.messages = [];
    }

    remove(options) {
        if (!options || (!options.key && !options.name && !options.type)) {
            return;
        }
        this.messages = this.messages.filter(item => {
            let keep = true;

            if (options.key) {
                keep = item.key !== options.key;
            } else {
                if (options.name) {
                    keep = item.name !== options.name;
                } else {
                    if (options.type) {
                        keep = (item.type || 'info') !== options.type;
                    }
                }
            }
            return keep;
        });
    }

    private push(options) {
        const key = this.key + 1;
        const expiryDate = new Date();

        if (!options || !options.message) {
            return;
        }

        const type = options.type || 'info';

        expiryDate.setSeconds(expiryDate.getSeconds() + 10);

        const message = {
            message: options.message,
            type,
            key,
            name: options.name,
            expiryDate
        };

        this.messages.push(message);
        this.key = key;
    }

    private removeExpiredAlerts() {
        const date = new Date();

        if (!this.messages) {
            return;
        }

        this.messages = this.messages.filter(message => {
            return (message.expiryDate &&
                message.expiryDate < date) ? undefined : message;
        });

        setTimeout(() => {
            this.removeExpiredAlerts();
        }, 500);
    }
}

class State {
    readonly alerts: Alerts = new Alerts();
}

export default new State();
