<template>
    <div>
        <navigation-bar />
        <Forum />
        <common-layout />
        <notification-box />

    </div>
</template>


<script>
import Forum from "../components/Forum.vue";
import {commonMixin} from '../mixins/checkLoginState';
import NotificationBox from '../components/NotificationBox.vue';

export default {
  mixins: [commonMixin],
  components: {Forum,NotificationBox},
  created() {
    this.checkAvailable()
  }
}
</script>