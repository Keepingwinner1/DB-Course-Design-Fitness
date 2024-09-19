<template>
    <div >
        <el-header class="header">
            <navigation-bar />
        </el-header>
        <el-row class="container" justify="space-between">
            <el-col :span="10">
                <AddFood />
            </el-col>
            <el-col :span="12">
                <AddDiet />
            </el-col>
        </el-row>
    </div>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import AddFood from '../components/AddFood.vue';
import AddDiet from '../components/AddDiet.vue';

export default defineComponent({
    components: {
        AddFood,
        AddDiet
    }
});
</script>

<style scoped>
.container {
    width: 150vh;
}

.header {
    height: 80px;
}

.el-row {
    margin-top: 20px;
}

.el-col {
    padding: 10px;
}
</style>
<!--
剩余部分：
路由部分
数据统计部分-->