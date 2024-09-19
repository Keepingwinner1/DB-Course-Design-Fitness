<template>
    <div class="container">
        <navigation-bar />
        <div class="achievements">
            <Achievements />
        </div>
        <CommonLayout />
    </div>
</template>

<style scoped>
.container {
    display: grid;
    grid-template-columns: 1fr;
    gap: 100px;
}




</style>

<script>
import Achievements from "../components/Achievements.vue";
import CommonLayout from "../components/CommonLayout.vue";

export default {
    components: {  Achievements,CommonLayout },
}
</script>