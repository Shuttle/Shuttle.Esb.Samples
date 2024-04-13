<template>
    <div class="mt-4">
        <div class="header">Orders</div>
        <v-alert v-if="message" closable :type="messageType">
            {{message}}
        </v-alert>
        <v-alert v-if="message.value" closable>
            {{message.value}}
        </v-alert>
        <v-table density="comfortable">
            <thead>
                <tr>
                    <th></th>
                    <th>Customer name</th>
                    <th>Order number</th>
                    <th>Order date</th>
                    <th>Status</th>
                    <th class="text-right">Total</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="order in orders" v-bind:key="order.orderNumber">
                    <td>
                        <v-btn v-if="order.canCancel" density="comfortable" @click="cancelOrder(order)"
                            variant="outlined">
                            Cancel</v-btn>
                        <v-btn v-if="order.canArchive" density="comfortable" @click="archiveOrder(order)"
                            variant="tonal">
                            Archive</v-btn>
                    </td>
                    <td>{{ order.customerName }}</td>
                    <td>{{ order.orderNumber }}</td>
                    <td>{{ order.orderDate }}</td>
                    <td>{{ order.status }}</td>
                    <td class="text-right">{{ order.orderTotal }}</td>
                </tr>
            </tbody>
        </v-table>
    </div>
</template>

<script setup>
import axios from "axios";
import { onBeforeUnmount, onMounted, ref } from "vue";
import configuration from "@/configuration";

const orders = ref([]);
const poll = ref(false);
const message = ref('');
const messageType = ref('');

const cancelOrder = order => {
    axios
        .delete(configuration.url + "/orders/" + order.id)
        .then(() => {
            message.value = "Your cancellation request has been sent for processing.";
            messageType.value = "info";
        })
        .catch(() => {
            message.value = "The selected order could not be cancelled.";
            messageType.value = "error";
        });
};

const archiveOrder = order => {
    axios
        .delete(configuration.url + "/orders/" + order.id + '/archive')
        .then(() => {
            message.value = "Your archive request has been sent for processing.";
            messageType.value = "info";
        });
};

const fetchOrders = () => {
    var found;
    const orderIdsToRemove = [];

    if (!poll.value) {
        return;
    }

    setTimeout(() => {
        axios
            .get(configuration.url + "/orders")
            .then(response => {
                self.orders = response.data;

                response.data.forEach(order => {
                    found = false;

                    orders.value.forEach(element => {
                        if (element.id === order.id) {
                            element.id = order.id;
                            element.customerName = order.customerName;
                            element.orderNumber = order.orderNumber;
                            element.orderDate = order.orderDate;
                            element.orderTotal = order.orderTotal;
                            element.status = order.status;
                            element.canCancel = order.canCancel;
                            element.canArchive = order.canArchive;

                            found = true;
                        }
                    });

                    if (!found) {
                        orders.value.push(order);
                    }
                });

                orders.value.forEach(existingOrder => {
                    found = false;

                    response.data.forEach(function (order) {
                        if (found) {
                            return;
                        }

                        found = order.id === existingOrder.id;
                    });

                    if (!found) {
                        orderIdsToRemove.push(existingOrder.id);
                    }
                });

                orders.value = orders.value.filter(item =>{
                    return !orderIdsToRemove.includes(item.id);
                });
            })
            .catch(() => {
                message.value = "Could not fetch the orders.";
                messageType.value = "error";
            })
            .then(() => {
                fetchOrders();
            });
    }, 500);
}

onMounted(() => {
    poll.value = true;
    fetchOrders();
})

onBeforeUnmount(() => {
    poll.value = false;
})
</script>