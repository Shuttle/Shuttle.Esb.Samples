<template>
  <div v-if="books.length > 0">
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
        <tr v-for="book in books" v-bind:key="book.id" class="row" v-bind:class="{ 'table-info' : buying }">
          <td class="col-1">
            <button
              v-if="book.buying"
              text="'Remove'"
              click:from="@toggle"
              elementClass:from="'btn-default btn-danger btn-sm'"
            />
            <button
              v-else
              text:from="'Add'"
              click:from="@toggle"
              elementClass:from="'btn-default btn-success btn-sm'"
            />
          </td>
          <td class="col">{{description}}</td>
          <td class="col-2 text-right">{{price}}</td>
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
</template>

<script>
export default {
  name: "shuttle-books",
  data() {
    return {
      books: []
    };
  }
};
</script>
