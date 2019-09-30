import React from 'react';
import Alert from 'react-bootstrap/Alert'
import state from './state';

export default class Alerts extends React.Component {
    render() {
        if (!state.alerts.messages) {
            return undefined;
        }

        return state.alerts.messages.map(message => (
            <Alert key={message.name} variant={alert.success} className={"alert-dismissible"} dismissable show onClose={() => state.alerts.remove(alert)}>
                {alert.message}
            </Alert>
        ));
    }
}