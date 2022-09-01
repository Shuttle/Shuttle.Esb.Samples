<template>
    <div>
        <div class="header">Books</div>
        <v-alert v-if="message" closable :type="messageType">
            {{ message }}
        </v-alert>
        <v-table density="comfortable" class="mb-4">
            <thead>
                <tr>
                    <th class="text-left w-0">
                    </th>
                    <th class="text-left">
                        Title
                    </th>
                    <th class="text-right">
                        Price
                    </th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="book in books" :key="book.description">
                    <td>
                        <v-btn v-if="book.buying" density="comfortable" @click="toggle(book)" variant="outlined">Remove
                        </v-btn>
                        <v-btn v-else density="comfortable" @click="toggle(book)" variant="tonal">Buy</v-btn>
                    </td>
                    <td>{{ book.description }}</td>
                    <td class="text-right">{{ book.price }}</td>
                </tr>
            </tbody>
            <tfoot>
                <tr>
                    <td colspan="2"></td>
                    <td class="col-2 text-right info">{{ total }}</td>
                </tr>
            </tfoot>
        </v-table>
        <div v-if="canOrder" class="">
            <div class="header">Checkout</div>
            <v-form ref="form">
                <v-container fluid class="tw-border-zinc-500 tw-border-2 tw-border-solid">
                    <v-row>
                        <v-col>
                            <v-text-field v-model="state.customerName" label="Customer name"
                                :rules="validate('customerName')"></v-text-field>
                        </v-col>
                        <v-col>
                            <v-text-field v-model="state.customerEMail" label="Customer e-mail"
                                :rules="validate('customerEMail')"></v-text-field>
                        </v-col>
                    </v-row>
                    <v-row>
                        <v-col class="text-right">
                            <v-btn class="mr-2" @click="submitOrder('custom')">Order (Custom)</v-btn>
                            <v-btn class="mr-2" @click="submitOrder('custom / event-source')">Order (Custom -
                                EventSource)
                            </v-btn>
                            <v-btn @click="submitOrder('event-source / module')">Order (EventSource - Module)</v-btn>
                        </v-col>
                    </v-row>
                </v-container>
            </v-form>
        </div>
    </div>
</template>

<script setup>
import axios from "axios";
import { onMounted, reactive, ref } from "vue";
import configuration from "@/configuration";
import { computed } from "@vue/reactivity";
import useValidate from "@vuelidate/core";
import { required, email } from "@vuelidate/validators";

const form = ref();
const message = ref('');
const messageType = ref('');

const state = reactive({
    customerName: "",
    customerEMail: ""
});

const rules = {
    customerName: { required },
    customerEMail: { required, email }
};

const v$ = useValidate(rules, state);

const validate = field => {
    const result = v$.value[field].$errors.map(item => item.$message)?.[0];

    return !!result ? [result] : [];
}

const books = ref([]);

const toggle = book => {
    book.buying = !book.buying;
};

const total = computed(() => {
    return books.value.reduce(function (result, item) {
        return result + (item.buying ? item.price : 0);
    }, 0);
})

const canOrder = computed(() => {
    return total.value > 0;
});

const submitOrder = async targetSystem => {
    const self = this;

    const valid = await v$.value.$validate();

    if (!valid) {
        await form.value.validate();
        return false;
    }

    var order = {
        productIds: [],
        targetSystem: targetSystem,
        customerName: state.customerName,
        customerEMail: state.customerEMail
    };

    books.value.forEach(function (book) {
        if (book.buying) {
            order.productIds.push(book.id);
        }
    });

    axios
        .post(configuration.url + "/orders", order)
        .then(() => {
            books.value.forEach(function (book) {
                book.buying = false;
            });

            message.value = "Your order has been sent for processing.";
            messageType.value = "info";
        })
        .catch(error => {
            message.value = error.message;
            messageType.value = "error";
        });
};

onMounted(() => {
    axios.get(configuration.url + "/products").then(response => {
        books.value = response.data.data.map(book => {
            book.buying = false;
            return book;
        });
    });
})
</script>