<template>
    <v-app>
        <v-main>
            <v-app-bar title="Shuttle.Processmanagement" flat></v-app-bar>
            <div class="app-container">
                <Books />
                <Orders />
            </div>
        </v-main>
    </v-app>
</template>

<script>
import Books from './components/Books.vue'
import Orders from './components/Orders.vue'

export default {
    name: 'App',

    components: {
        Books,
        Orders,
    },

    data: () => ({
        //
    }),
}
</script>
