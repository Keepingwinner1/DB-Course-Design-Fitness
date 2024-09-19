<template>
  <div>
    <!-- 勋章展示 -->
    <div class="video-container">
      <video class="responsive-video" autoplay loop muted>
        <source src="/src/assets/videos/medal.mp4" type="video/mp4" />
        您的浏览器不支持视频标签。
      </video>
    </div>
    <!-- 动态箭头 -->
    <div class="scroll-arrow" @click="scrollToBottom"></div>
    
    <div class="badge-wall">
      <!-- 已获得的勋章展示 -->
      <div class="achieved-badges">
        <h2>我的勋章墙</h2>
        <div class="badge-container">
          <!-- 循环展示已获得的勋章 -->
          <div
            v-for="badge in achievedBadges"
            :key="badge.achievementId"
            @click="navigateToRankingList(badge.achievementId)"
            class="badge"
          >
            <img :src="badge.imgSrc" alt="Achieved Badge" class="badge-icon achieved" />
          </div>
        </div>
      </div>

      <!-- 未获得的勋章展示 -->
      <div class="unachieved-badges">
        <h2>未获得的勋章</h2>
        <div class="badge-container">
          <!-- 循环展示未获得的勋章 -->
          <div
            v-for="badge in unachievedBadges"
            :key="badge.achievementId"
            @click="navigateToRankingList(badge.achievementId)"
            class="badge"
          >
            <img :src="badge.imgSrc" alt="Unachieved Badge" class="badge-icon unachieved" />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios';
import { useRouter } from 'vue-router';
import thumb from '../assets/badges/Achievement_2.png'; // 点赞
import comment from '../assets/badges/Achievement_5.png'; // 评论
import post from '../assets/badges/Achievement_6.png'; // 发帖数
import information from '../assets/badges/Achievement_1.png'; // 个人信息完成
import login from '../assets/badges/Achievement_2.png'; // 累计登录天数
import course from '../assets/badges/Achievement_3.png'; // 完成课程数量
import train from '../assets/badges/Achievement_8.png'; // 训练计划完成天数
import food from '../assets/badges/Achievement_7.png'; // 饮食计划完成天数

export default {
  name: 'BadgeWall',
  data() {
    return {
      achievedBadges: [], // 已获得的勋章
      unachievedBadges: [], // 未获得的勋章
      badgeImages: {
        1: information, // 个人信息完成度
        2: login, // 累计登录天数
        3: course, // 完成课程数量
        4: thumb, // 被点赞
        5: comment, // 被评论
        6: post, // 发帖数
        7: food, // 饮食计划完成天数
        8: train, // 训练计划完成天数
      }
    };
  },
  mounted() {
    this.fetchAchievements();
  },
  methods: {
    async fetchAchievements() {
      try {
        const response = await axios.get('http://localhost:8080/api/Achievement/GetAchievement', {
          params: { token: localStorage.getItem('token') }
        });
        const achievements = response.data.achievements;

        // 根据是否获得成就将其分为两类，并为每个成就添加图片路径
        this.achievedBadges = achievements
          .filter(achievement => achievement.isAchieved === 'true')
          .map(achievement => ({
            ...achievement,
            imgSrc: this.badgeImages[achievement.achievementId] // 修改为根据 achievementId 获取图片
          }));

        this.unachievedBadges = achievements
          .filter(achievement => achievement.isAchieved === 'false')
          .map(achievement => ({
            ...achievement,
            imgSrc: this.badgeImages[achievement.achievementId] // 修改为根据 achievementId 获取图片
          }));
      } catch (error) {
        console.error('获取成就数据失败:', error);
      }
    },

    navigateToRankingList(achievementId) {
      this.$router.push({ name: 'RankingList', query: { achievementId } });
    },
    scrollToBottom() {
      window.scrollTo({
        top: document.documentElement.scrollHeight,
        behavior: 'smooth'
      });
    }
  }
}

</script>

<style scoped>
/* 动态箭头样式 */
@keyframes fade {
  0% {
    opacity: 1;
  }
  50% {
    opacity: 0.5;
  }
  100% {
    opacity: 1;
  }
}

.scroll-arrow {
  top: 180px;
  position: relative; /* 绝对定位 */
  bottom: -5px; /* 距离底部的距离 */
  left: 50%; /* 水平居中 */
  transform: translateX(-50%); /* 水平居中调整 */
  width: 50px; /* 箭头宽度 */
  height: 50px; /* 箭头高度 */
  background: url('../assets/images/箭头.png') no-repeat center center; /* 背景图标 */
  background-size: contain; /* 使背景图标适应容器 */
  cursor: pointer; /* 鼠标悬停时显示手形光标 */
  animation: fade 1.5s infinite; /* 渐变动画效果 */
}

.video-container {
  justify-content: center;
  position: relative; 
  top: 400px; /* 使视频从导航栏下方开始 */
  left: 0; /* 视频紧贴网页左侧 */
  width: 100vw; /* 视频宽度覆盖整个视口宽度 */
  height: 100vh; /* 高度减去导航栏的高度 */; /* 视频高度覆盖整个视口高度 */
  z-index: -1; /* 确保视频在其他内容的下方 */
  overflow: hidden; /* 防止视频内容超出容器 */
  margin-bottom: 80px; /* 调整底部间距 */
}

.badge-header {
  text-align: center;
  margin-top: 60px; /* 距离导航栏下方的距离，根据实际情况调整 */
  color: #333; /* 深灰色字体 */
  font-size: 3vw; /* 根据视口宽度自适应 */
  font-weight: bold; /* 设置字体加粗 */
}

.badge-header h2 {
  font-size: 3vw; /* 增大字体 */
  font-weight: bold; /* 字体加粗 */
  text-align: center; /* 居中对齐 */
  margin-top: 60px; /* 距离导航栏的距离 */
  color: #333; /* 深灰色字体 */
}

.responsive-video {
  margin-top: 15vh;
  width: 80vw; /* 视频宽度覆盖整个视口宽度 */
  height: 75vh; /* 视频高度覆盖整个视口高度 */
  object-fit: cover; /* 保持视频的宽高比，确保其覆盖整个容器 */
  border: none; /* 去掉边框 */
}

.achieved-badges,
.unachieved-badges {
  margin: 50px auto;
  text-align: auto; /* 将内部内容居中 */
}

.badge-container {
  display: flex;
  flex-wrap: wrap; /* 实现内容自动换行 */
  justify-content: center; /* 居中对齐内容 */
  gap: 10px; /* 添加徽章之间的间距 */
  margin-top: 0px; /* 根据需要调整值 */
}

.badge {
  margin: 0 10px; /* 间距 */
  flex-shrink: 0; /* 防止缩小 */
  display: flex;
  align-items: center;
  justify-content: center;
}
.badge-wall {
  top: 250px;
  position: relative; /* 相对定位 */
  background: url('../assets/images/bg5.jpg') no-repeat center center; /* 背景图标 */
  background-size: cover; /* 背景图标覆盖容器 */
  border: 2px solid #ddd; /* 边框 */
  border-radius: 15px; /* 圆角边框 */
  padding: 20px;
  width: 50vw; /* 宽度 */
  max-width: 1200px; /* 最大宽度 */
  margin: 20px auto; /* 自动水平居中 */
}
.badge-icon {
  width: 10vw; /* 根据视口宽度自适应 */
  height: 10vw; /* 根据视口宽度自适应 */
  max-width: 100px; /* 设置最大宽度 */
  max-height: 100px; /* 设置最大高度 */
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  background-color: #ccc;
  color: #333; /* 深灰色字体 */
}

.badge-icon.achieved {
  background-color: gold; /* 已获得的勋章颜色 */
}

.badge-icon.unachieved {
  background-color: gray; /* 未获得的勋章颜色 */
}

h2 {
  text-align: center;
  color: #333; /* 深灰色字体 */
  font-size: 2vw; /* 根据视口宽度自适应 */
}

@keyframes scroll {
  0% {
    transform: translateX(0);
  }
  100% {
    transform: translateX(-100%);
  }
}
</style>
