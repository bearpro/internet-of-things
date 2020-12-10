<template>
  <div>
      <account-component 
        :user="user" 
        @auth-action="user = $event" 
        v-if="user == ''"
      />

      <admin-component 
        v-if="user == 'admin'"
      />
  </div>
</template>

<script>
import AccountComponent from './Account/Account.vue';
import AdminComponent from './Admin/Admin.vue';

export default {
  components: {
    'account-component': AccountComponent,
    'admin-component': AdminComponent
  },
  data: function() {
        return {
            user: 'admin'
        }
    },
}
</script>

<style>

</style>
