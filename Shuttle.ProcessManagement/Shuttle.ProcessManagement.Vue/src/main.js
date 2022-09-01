import 'vuetify/styles';
import '@/styles/styles.css';
import "@mdi/font/css/materialdesignicons.css";
import { createApp } from 'vue';
import { createVuetify } from 'vuetify';
import { aliases, mdi } from 'vuetify/iconsets/mdi'

import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'

import App from './App.vue';

const app = createApp(App);
const vuetify = createVuetify({
    components,
    directives,
    theme: {
        defaultTheme: 'dark'
    },
    icons: {
        defaultSet: 'mdi',
        aliases,
        sets: {
            mdi,
        }
    },
});

app.use(vuetify);

app.mount('#app')
