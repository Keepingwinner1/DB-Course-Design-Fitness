<template>
    <div class="main-container">
        <navigation-bar class="navbar-planner"/>
        <div class="page-content">
            <div class="back-button-container">
                <el-button @click="goBack" circle style="font-size: 24px; width: 50px; height: 50px;">
                    <el-icon>
                        <arrow-left />
                    </el-icon>
                </el-button>
            </div>
            <div class="meal-planner">
                <MealPlanner />
            </div>
            <common-layout />
        </div>
    </div>
</template>

<style scoped>

.main-container {
    display: flex;
    flex-direction: column;
    min-height: 100vh; /* 确保容器至少与视口高度一样高 */
}



/* 页面内容的容器 */
.page-content {
    flex: 1; /* 使内容区占据剩余空间 */
}

.meal-planner {
    /*margin-top: 200px;*/
    grid-column: 1;
    box-shadow: 2px 2px 4px rgba(0, 0, 0, 0.1);
    padding: 10px;
    border-radius: 15px;
    background-color: #f7fbff;
    width: 90vw;
    /*height: 800px;*/
    justify-content: center;
    position: absolute;
    left: 5vw;
    top: 17vh;
}



.back-button-container {
    position: absolute;
    top: 10vh; /* 调整为你需要的上边距 */
    left: 5vw; /* 调整为你需要的左边距 */
    z-index: 2; /* 确保按钮在日历表之上 */

}



</style>

<script>
import MealPlanner from "../components/MealPlanner.vue";
import { ArrowLeft } from '@element-plus/icons-vue'  // 引入ArrowLeft图标
import CommonLayout from "../components/CommonLayout.vue";
import {ElNotification} from "element-plus";
import {useRouter} from "vue-router";
import axios from "axios";
import { commonMixin } from '../mixins/checkLoginState';

export default {
    mixins: [commonMixin],
    components: {  MealPlanner,ArrowLeft, CommonLayout },
    methods: {
        goBack() {
            this.$router.back(); // 使用Vue Router的back方法返回上一页
        }
    },
    created() {
      this.checkAvailable()
    }
}
</script>