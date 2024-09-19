<template>
  <div class="popup-overlay" v-if="visible">
    <div class="popup-content">
      <button @click="closePopup" class="close-btn">×</button>
      <!-- 在这里展示弹窗内容 -->
      <!-- RankingList 自适应弹窗 -->
      <div class="ranking-list-wrapper">
        <RankingList
          :achievementId="achievementId"
          :currentTarget="currentTarget"
        />
      </div>
    </div>
  </div>
</template>
  
  <script>
import RankingList from "./RankingList.vue"; // 确保导入正确的组件

export default {
  components: { RankingList },
  mounted() {
    console.log("RankingList mounted. Current Target:", currentTarget);
  },
  props: {
    visible: Boolean,
    achievementId: Number,
    currentTarget: Number, // 新增 currentTarget 属性
  },
  methods: {
    closePopup() {
      this.$emit("update:visible", false);
      console.log("Current Target:", this.currentTarget); // 可以在这里使用 currentTarget
    },
  },
};
</script>
  
  <style scoped>
.popup-overlay {
  position: auto;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  align-items: center;
  justify-content: center;
  transform: scale(0.8); /* 初始缩放比例 */
}

.popup-content {
  background: white;
  padding: 20px;
  border-radius: 8px;
  position: relative;
  width: auto;
  display: block;
  height: auto;
  overflow: hidden; /* 避免内容溢出出现滚动条 */
  max-width: 80vw;
  max-height: 80vw;
}

.close-btn {
  position: absolute;
  top: 10px;
  right: 10px;
  font-size: 20px;
  border: none;
  background: #ff5c5c; /* 设置按钮的背景色为红色 */
  color: white; /* 按钮文字为白色 */
  width: 30px;
  height: 30px;
  border-radius: 50%; /* 让按钮成为圆形 */
  cursor: pointer; /* 鼠标悬停时变为手指样式 */
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2); /* 添加轻微阴影效果 */
  transition: background 0.3s ease, transform 0.3s ease; /* 使背景色和平移有过渡效果 */
}

.ranking-list {
  width: auto;
  height: auto; /* 确保组件高度适应父容器 */
  overflow-y: auto; /* 如果内容超出，允许滚动 */
  max-height: 100%; /* 确保内容在弹窗内不超出 */
}
</style>
  