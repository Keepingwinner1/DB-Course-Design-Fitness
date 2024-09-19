<template>
    <navigation-bar />
    <div class="spacer"></div> <!-- 添加一个间隔元素 -->
    <AIGuide />
    <common-layout />
</template>

<script>

import { defineComponent } from "vue";
import {RouterView, useRouter} from 'vue-router';
import AIGuide from "../components/AIGuide.vue"
import adminEquipment from "../components/adminEquipment.vue"
import AddDiet from "../components/AddDiet.vue"
import {ElNotification} from "element-plus";
import axios from "axios";
import { commonMixin } from '../mixins/checkLoginState';


export default defineComponent({
    mixins: [commonMixin],
    components: { AIGuide, adminEquipment, AddDiet },
    created() {
      this.checkAvailable()
    },
})
</script>

<style scoped>
.spacer {
    margin-top: 20px;
    /* 根据需要调整间距 */
}
</style>
