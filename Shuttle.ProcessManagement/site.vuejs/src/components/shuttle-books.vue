<template>
  <div>
    <div v-if="books.length">
      <h4>Available titles</h4>
      <table class="table table-sm">
        <thead class="thead-light">
          <tr class="row">
            <th class="col-1"></th>
            <th class="col">Title</th>
            <th class="col-2 text-right">Price</th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="book in books"
            v-bind:key="book.description"
            class="row"
            v-bind:class="{ 'table-info' : book.buying }"
          >
            <td class="col-1">
              <button
                v-if="book.buying"
                v-on:click="toggle(book)"
                class="btn btn-default btn-danger btn-sm"
              >Remove</button>
              <button
                v-else
                v-on:click="toggle(book)"
                class="btn btn-default btn-success btn-sm"
              >Add</button>
            </td>
            <td class="col">{{book.description}}</td>
            <td class="col-2 text-right">{{book.price}}</td>
          </tr>
        </tbody>
        <tfoot>
          <tr class="row">
            <td colspan="2" class="col"></td>
            <td class="col-2 text-right info">{{total}}</td>
          </tr>
        </tfoot>
      </table>
      <!-- <cs-form title:raw="Checkout">
            <cs-form-input label:raw="Name" required:raw="true" value:bind="customerName"
                        vm:errors:from="errors()" errorAttribute:raw="customerName"/>
            <cs-form-input label:raw="e-mail" required:raw="true" value:bind="customerEMail"
                        vm:errors:from="errors()" errorAttribute:raw="customerEMail"
                        placeholder:raw="abc.xyz@example.com"/>
            <br/>
            <cs-button text:raw="Cancel" click:from="@cancel" elementClass:raw="btn-secondary"/>
            <cs-button actions:from="actions" text:raw="Order" elementClass:raw="btn-primary"/>
        </cs-form>
      <br/>-->
    </div>
    <div v-else>no books</div>
  </div>
</template>

<script>
import axios from "axios";
import configuration from "@/configuration.js";

export default {
  name: "shuttle-books",
  data() {
    return {
      books: []
    };
  },
  computed: {
    total() {
      return this.books.reduce(function(result, item) {
        return result + (item.buying ? item.price : 0);
      }, 0);
    }
  },
  methods: {
    toggle(book) {
      book.buying = !book.buying;
    }
  },
  mounted() {
    axios.get(configuration.url + "/products").then(response => {
      this.books = response.data.data.map(function(book) {
        book.buying = false;
        return book;
      });
    });
  }
};
</script>
