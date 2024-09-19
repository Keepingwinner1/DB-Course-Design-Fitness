<template>
  <div class="container">
    <!-- 左侧排行榜 -->
    <div class="ranking-container">
      <!-- 显示排行榜用户信息 -->
      <div
        v-for="(item, index) in leaderboard"
        :key="index"
        class="ranking-item"
      >
        <div class="ranking-details">
          <div class="rank-number">{{ item.rank }}</div>
          <img :src="item.avatar" alt="Avatar" class="avatar" />
          <div class="details">
            <p class="name">{{ item.name }}</p>
            <p class="score">
              进度:
              <span class="progress-bar-wrapper">
                <span
                  class="progress-bar"
                  :style="{ width: item.score + '%' }"
                ></span>
              </span>
              <span class="score-text"> {{ item.score }}/{{ target }} </span>
              <el-progress
                v-if="target !== null"
                type="line"
                :percentage="
                  Math.min(100, ((item.score / target) * 100).toFixed(2))
                "
                class="progress-bar"
              ></el-progress>
            </p>
          </div>
          <!-- 在右侧放置图片，根据排名显示不同图片 -->
          <div class="ranking-image">
            <img
              v-if="parseInt(item.rank, 10) === 1"
              :src="gold"
              alt="第一名"
              class="ranking-medal"
            />
            <img
              v-else-if="parseInt(item.rank, 10) === 2"
              :src="silver"
              alt="第二名"
              class="ranking-medal"
            />
            <img
              v-else-if="parseInt(item.rank, 10) === 3"
              :src="copper"
              alt="第三名"
              class="ranking-medal"
            />
          </div>
        </div>
      </div>
    </div>

    <!-- 右侧信息栏 -->
    <div class="info-box">
      <!-- 徽章和排行榜标题 -->

      <!-- 显示对应的徽章 -->
      <img
        :src="badgeImage"
        alt="Achievement Badge"
        class="achievement-badge"
      />
      <!-- 显示正确的排行榜名称 -->
      <h2>{{ leaderboardTitle }}</h2>
      <el-divider><i class="el-icon-mobile-phone"></i></el-divider>
      <!-- 其他信息展示 -->
      <p>
        你的排名是：<br />
        <img
          :src="rankingImage"
          alt="排名图片"
          v-if="myRanking >= 1 && myRanking <= 10"
          style="width: 70px; height: auto"
        />
        <span v-else
          >很抱歉，你未上榜
          <img :src="lose" alt="未上榜图片" style="width: 70px; height: auto" />
        </span>
      </p>

      <!-- 显示用户的排名信息 -->
      <div class="ranking-info">
        <p v-if="myRanking === 1" style="color: #555">
          <img :src="num1" alt="第一图片" style="width: 280px; height: auto" />
        </p>
        <p v-else-if="myRanking > 1 && myRanking <= 10">
          <img
            :src="num2_10"
            alt="第2-10图片"
            style="width: 280px; height: auto"
          />
        </p>
        <p v-else>
          <img
            :src="num11"
            alt="未上榜图片"
            style="width: 280px; height: auto"
          />
        </p>
      </div>
      <el-divider><i class="el-icon-mobile-phone"></i></el-divider>
      <!-- 进度条 -->
      <div class="progress-section">
        <p>你的进度:</p>
        <el-progress
          v-if="target !== null"
          type="line"
          :percentage="Math.min(100, ((myProgress / target) * 100).toFixed(2))"
        ></el-progress>

        <!-- 进度消息 -->
        <p v-if="target !== null">
          <span v-if="(myProgress / target) * 100 >= 100">
            恭喜你，徽章轻松拿下
          </span>
          <span v-else>
            还差
            <strong class="jumping-text">
              {{ Math.max(0, 100 - (myProgress / target) * 100).toFixed(2) }}
              % </strong
            >就能拿下徽章啦！
          </span>
        </p>
      </div>
    </div>
  </div>
</template>


<script>
import axios from "axios";
import Achievement_1 from "../assets/badges/Achievement_1.png";
import Achievement_2 from "../assets/badges/Achievement_2.png";
import Achievement_3 from "../assets/badges/Achievement_3.png";
import Achievement_4 from "../assets/badges/Achievement_4.png";
import Achievement_5 from "../assets/badges/Achievement_5.png";
import Achievement_6 from "../assets/badges/Achievement_6.png";
import Achievement_7 from "../assets/badges/Achievement_7.png";
import Achievement_8 from "../assets/badges/Achievement_8.png";
import rank_1 from "../assets/images/numbers/数字-1.png";
import rank_2 from "../assets/images/numbers/数字-2.png";
import rank_3 from "../assets/images/numbers/数字-3.png";
import rank_4 from "../assets/images/numbers/数字-4.png";
import rank_5 from "../assets/images/numbers/数字-5.png";
import rank_6 from "../assets/images/numbers/数字-6.png";
import rank_7 from "../assets/images/numbers/数字-7.png";
import rank_8 from "../assets/images/numbers/数字-8.png";
import rank_9 from "../assets/images/numbers/数字-9.png";
import rank_10 from "../assets/images/numbers/数字-10.png";
import lose from "../assets/images/numbers/fail.png";
import num1 from "../assets/images/numbers/第一.png";
import num2_10 from "../assets/images/numbers/加油.png";
import num11 from "../assets/images/numbers/未上榜.png";
import gold from "../assets/images/numbers/金牌.png";
import silver from "../assets/images/numbers/银牌.png";
import copper from "../assets/images/numbers/铜牌.png";
export default {
  props: {
    visible: Boolean,
    achievementId: Number, // 接收传递过来的 achievementId
    currentTarget: Number, // 接收 currentTarget
  },
  data() {
    return {
      achievementBadge: null,
      leaderboardTitle: "",
      leaderboard: [],
      myRanking: null,
      num1,
      num2_10,
      num11,
      lose,
      gold,
      silver,
      copper,
      rankingImages: {
        1: rank_1,
        2: rank_2,
        3: rank_3,
        4: rank_4,
        5: rank_5,
        6: rank_6,
        7: rank_7,
        8: rank_8,
        9: rank_9,
        10: rank_10,
      },
      achievementId: parseInt(this.achievementId, 10),
      badges: {
        1: Achievement_1,
        2: Achievement_2,
        3: Achievement_3,
        4: Achievement_4,
        5: Achievement_5,
        6: Achievement_6,
        7: Achievement_7,
        8: Achievement_8,
      },
      myProgress: null, // 我的进度
      target: null, // 目标值
      leaderboard: [], // 排行榜用户
    };
  },
  computed: {
    badgeImage() {
      return this.badges[this.achievementId] || this.badges[1];
    },
    rankingImage() {
      return this.rankingImages[this.myRanking] || this.rankingImages.default;
    },
  },
  mounted() {
    this.fetchAchievementTarget(); // 获取目标值
    this.fetchAchievementRanking();
    this.setAchievementDetails();
    console.log("Current Target:", this.currentTarget); // 可以在这里使用 currentTarget
  },
  methods: {
    async fetchAchievementRanking() {
      const token = localStorage.getItem("token"); // 从localStorage获取token
      const achievementId = parseInt(this.achievementId, 10);
      try {
        const response = await axios.get(
          "http://localhost:8080/api/Achievement/GetAchievementRanking",
          {
            params: {
              token: token,
              achievementId: achievementId,
            },
          }
        );

        const data = response.data;
        this.myRanking = data.myRanking;
        this.myProgress = data.progress;
        console.log(`Received myProgress: ${this.myProgress}`); // 记录目标值
        console.log(`Received myProgress: ${this.myProgress}`); // 记录进度值
        console.log(`Received target: ${this.target}`); // 记录目标值
        this.leaderboard = data.rankingUsers.map((user) => ({
          name: user.userName,
          score: user.progress,
          avatar: user.userIcon,
          rank: user.userRank, // 确保包含排名
        }));
      } catch (error) {
        console.error("获取成就排行榜失败:", error);
      }
    },
    setAchievementDetails() {
      const achievementId = parseInt(this.achievementId, 10);

      switch (achievementId) {
        case 1:
          this.achievementBadge = Achievement_1;
          this.leaderboardTitle = "信息满分徽章";
          break;
        case 2:
          this.achievementBadge = Achievement_2;
          this.leaderboardTitle = "百日之星徽章";
          break;
        case 3:
          this.achievementBadge = Achievement_3;
          this.leaderboardTitle = "学霸养成徽章";
          break;
        case 4:
          this.achievementBadge = Achievement_4;
          this.leaderboardTitle = "百赞先锋徽章";
          break;
        case 5:
          this.achievementBadge = Achievement_5;
          this.leaderboardTitle = "热评焦点徽章";
          break;
        case 6:
          this.achievementBadge = Achievement_6;
          this.leaderboardTitle = "发帖狂魔徽章";
          break;
        case 7:
          this.achievementBadge = Achievement_7;
          this.leaderboardTitle = "饮食达人徽章";
          break;
        case 8:
          this.achievementBadge = Achievement_8;
          this.leaderboardTitle = "健身王者徽章";
          break;
        default:
          this.achievementBadge = null;
          this.leaderboardTitle = "未知成就";
      }
    },
    async fetchAchievementTarget() {
      const token = localStorage.getItem("token"); // 从localStorage获取token
      const achievementId = parseInt(this.achievementId, 10);

      const predefinedTargets = {
        1: 80,
        2: 7,
        3: 5,
        4: 100,
        5: 20,
        6: 10,
        7: 5,
        8: 5,
      };

      // 如果achievementId为1到8，使用预定义的target值
      if (predefinedTargets[achievementId] !== undefined) {
        this.target = predefinedTargets[achievementId];
        console.log(
          `Using predefined target for achievementId ${achievementId}: ${this.target}`
        );
      } else {
        console.error(`未找到id为 ${achievementId} 的预定义成就目标`);
      }
    },
    calculateProgress() {
      if (this.target) {
        this.progressPercentage = Math.min(
          (this.myProgress / this.target) * 100,
          100
        );
      }
    },
  },
};
</script>

<style scoped>
.container {
  display: flex;
  width: auto;
  height: 100vh;
  overflow: hidden; /* 防止滚动条 */
  padding: 20px; /* 页面整体留白 */
  box-sizing: border-box; /* 确保padding和border包含在元素的总宽度和高度内 */
}
/* 样式保持不变 */
.ranking-title {
  text-align: center;
  margin-bottom: 20px;
  color: #333;
}

.ranking-container {
  width: 60%;

  padding: 40px;
  background-image: url("../assets/images/BG.jpg");
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
  border-radius: 15px; /* 圆角效果 */
  box-shadow: 0 0 0 15px #fff, 0 4px 8px rgba(0, 0, 0, 0.1); /* 白色圆角边框效果 */
  overflow-y: auto; /* 如果内容超出，允许垂直滚动 */
}

/* 自定义滚动条样式 */
.ranking-container::-webkit-scrollbar {
  width: 12px; /* 滚动条宽度 */
  margin-left: 20px; /* 调整右边距来增加与右侧窗口的距离 */
}

.ranking-container::-webkit-scrollbar-track {
  background: #f1f1f1; /* 滚动条轨道颜色 */
  border-radius: 10px; /* 轨道圆角 */
}

.ranking-container::-webkit-scrollbar-thumb {
  background: #888; /* 滚动条颜色 */
  border-radius: 10px; /* 滚动条圆角 */
}

.ranking-container::-webkit-scrollbar-thumb:hover {
  background: #555; /* 滚动条悬停颜色 */
}
.ranking-item {
  display: flex;
  align-items: center;
  font-size: 20px;
  margin-bottom: 10px; /* 添加下边距以分隔每个排行榜项 */
}

.rank-number {
  width: 50px; /* 设置宽度以适应名次数字 */
  text-align: center;
  font-size: 24px;
  font-weight: bold;
  color: #333;
  margin-right: 20px; /* 名次数字和其他内容之间的间距 */
}

.info-box {
  border-radius: 15px; /* 圆角效果 */
  margin-left: 20px;
  width: 35%;
  padding: 20px;
  background-color: #f9f9f9;
  border-left: 1px solid #ddd;
  box-shadow: 2px 0 5px rgba(109, 27, 27, 0.1);
  display: flex;
  flex-direction: column;
  align-items: center;
  /* justify-content: center; */
  color: #303030; /* 设置字体颜色为黑色 */
}

.ranking-item {
  font-size: 20px;
  margin-bottom: 10px; /* 添加下边距以分隔每个排行榜项 */
}

.ranking-details {
  display: flex;
  align-items: center;
  padding: 5px;
  background-color: rgba(255, 255, 255, 0.4);
  border-radius: 8px;
  width: 85%;
  color: #303030; /* 设置字体颜色为黑色 */
  animation: slideInFromLeft 0.5s ease-out forwards;
  /* 你可以调整动画的持续时间和效果 */
}

.avatar {
  width: 100px;
  height: 100px;
  border-radius: 50%;
  margin-right: 20px;
}
.ranking-image {
  justify-content: flex-end; /* 将内容右对齐 */
  align-items: center; /* 垂直居中对齐 */
  margin-left: auto; /* 将 .ranking-image 推到右侧 */
}

.ranking-medal {
  width: 100px;
  height: auto; /* 保持图片的纵横比 */
  margin-left: 10px; /* 如果需要，添加一些左边距以分隔图片 */
  justify-content: flex-end; /* 将内容右对齐 */
}
.details {
  font-size: 20px;
}

.name {
  font-weight: bold;
  font-size: 24px;
  margin: 0;
}

.score {
  color: #555;
  font-size: 20px;
  margin: 5px 0 0;
}

.back-button {
  display: block;
  width: 150px;
  margin: 20px auto;
  background-color: #ccc;
  color: #333;
  border: none;
  border-radius: 4px;
  padding: 10px;
  text-align: center;
  cursor: pointer;
}
.achievement-badge {
  width: 150px; /* 控制徽章的大小 */
  height: 150px;
  border-radius: 50%; /* 圆形 */
  border: 4px solid #b8b8b8; /* 金属边框 */
  background: linear-gradient(145deg, #dcdcdc, #f7f7f7); /* 金属光泽的渐变 */
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.5),
    /* 主阴影 */ inset 0 2px 4px rgba(255, 255, 255, 0.6),
    /* 内部高光 */ inset 0 -2px 4px rgba(0, 0, 0, 0.2); /* 内部阴影 */
  object-fit: cover; /* 确保图片以圆形裁剪 */
  animation: rotateBadge 6s infinite linear; /* 添加旋转动画 */
  transition: transform 0.3s ease-in-out; /* 鼠标悬停时的动态效果 */
}

.achievement-badge:hover {
  transform: scale(1.1); /* 鼠标悬停时徽章放大 */
}

@keyframes rotateBadge {
  0% {
    transform: rotate(0deg);
  }
  100% {
    transform: rotate(360deg); /* 徽章旋转360度 */
  }
}
.leaderboard-title {
  color: #555;
  color: #00070e !important; /* 设置排行榜标题的颜色 */
}

.my-ranking {
  color: #333333 !important; /* 设置"你的排名是"文本的颜色 */
}

.rank-message {
  color: #ff6600 !important; /* 设置排名信息提示文本的颜色 */
}

.achievement-badge {
  width: 150px;
  height: auto;
  margin-bottom: 20px;
}
.highlighted-progress {
  color: red;
  font-weight: bold;
  transition: transform 0.5s ease-in-out;
  animation: pulse 1.5s infinite;
}

@keyframes pulse {
  0%,
  100% {
    transform: scale(1);
  }
  50% {
    transform: scale(1.2);
  }
}
.jumping-text {
  color: rgb(241, 77, 77);
  font-weight: bold;
  display: inline-block;
  animation: jump 0.5s infinite;
}

@keyframes jump {
  0%,
  100% {
    transform: translateY(0);
  }
  50% {
    transform: translateY(-10px);
  }
}
.score-text {
  margin-right: 10px; /* 给文本与进度条之间添加间距 */
}

.progress-bar-container {
  display: inline-block;
  width: 100px; /* 根据实际需要调整宽度 */
  height: 10px; /* 根据实际需要调整高度 */
  background-color: #e0e0e0; /* 进度条的背景颜色 */
  border-radius: 5px; /* 可选：圆角 */
}

.progress-bar-filled {
  height: 100%;
  background-color: #76c7c0; /* 进度条的填充颜色 */
  border-radius: 5px; /* 可选：圆角 */
  transition: width 0.3s ease; /* 动画效果，平滑过渡 */
}
.score-text {
  font-size: 14px; /* 根据需求调整字体大小 */
  color: #333; /* 根据需求调整字体颜色 */
  margin-right: 10px; /* 设置文本和进度条之间的间距 */
}
.progress-bar {
  flex: 1; /* 使进度条占用剩余的空间 */
  width: 200px; /* 设置最大宽度 */
}
@keyframes slideInFromLeft {
  0% {
    transform: translateX(-100%);
    opacity: 0;
  }
  100% {
    transform: translateX(0);
    opacity: 1;
  }
}
</style>