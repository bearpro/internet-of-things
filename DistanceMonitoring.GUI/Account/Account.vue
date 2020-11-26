<template>
  <div class="container account">
      <div class="form-container">
          <authForm-component 
            v-if="authToggle"
            @login-change="login = $event"
            @password-change="password = $event"
            @signin-click="signIn"
          />
          <div 
            class="alert alert-warning" 
            v-if="error"
            >
              {{ errorText }}
          </div>
      </div>
  </div>
</template>

<script>
import AuthFormComponent from './AuthForm.vue';

export default {
    components: {
        'authForm-component': AuthFormComponent
    },
    data: function() {
        return {
            authToggle: true,
            error: false,
            errorText: ''
        }
    },
    methods: {
        signIn: async function(e) {

            if (e.login == 'admin' && e.password == 'admin') {
                this.$emit('auth-action', "admin");
            } else {
                this.error = true;
                this.errorText = "Неверный логин или пароль";
            }
        }
    }
}
</script>

<style scoped>
.alert {
    margin: 10px;
    text-align: center;
}

.account {

    display: block;
    margin: auto;
    width: 400px;
}

.form-container {
    padding: 30px 40px;
    margin: 0 100px 0 0;
    border-radius: 10px;
    background: rgb(238, 238, 238);
    box-shadow: 0px 0px 100px 0px rgba(0, 0, 0, 0.2);
}
</style>
