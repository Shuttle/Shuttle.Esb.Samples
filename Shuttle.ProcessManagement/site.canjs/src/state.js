import { DefineMap } from 'can';
import { Alerts } from 'shuttle-canstrap/alerts/';

const State = DefineMap.extend({
    alerts: {
        Default: Alerts
    }
});

const state = new State();

export default state;