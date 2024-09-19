<template>
  <navigation-bar />
  <CommonLayout style="margin-top: 0vh;" />
</template>

<script>

import {defineComponent} from "vue";
import { RouterView } from 'vue-router';
import CommonLayout from '../components/CommonLayout.vue';
export default defineComponent({
  components: { CommonLayout}
})

</script>