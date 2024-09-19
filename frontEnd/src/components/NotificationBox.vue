<template>
  <div>
    <div class="box-button" @click="toggleBoxWindow">
      ✉️
    </div>
    <div class="box-overlay" @click="toggleBoxWindow"></div> <!-- 遮罩层 -->
    <div class="box-common-layout">
      <div class="notifications-container">
        <div class="notifications-header">
          <h3>🔔 通知</h3>
          <el-divider></el-divider> <!-- 分割线 -->
        </div>
        <ul class="notification-list">
          <li v-for="(notification, index) in notifications" :key="index" class="notification-item" @click="toggleRead(index)">
              <div class="notification-content">
                <el-badge :value="notification.isRead  ? '' : '未读'" :class="{'is-read': notification.isRead}">
                </el-badge>
                <span class="notification-title">{{ notification.title }}</span>
                <span class="notification-message">{{ notification.message }}</span>
              </div>
          </li>
        </ul>
        <div class="notifications-footer">
          <el-button type="warning" @click="clickAllRead">一键已读</el-button>
        </div>
        </div>
    </div>
  </div>
</template>

<script>

import axios from 'axios';

export default {
  data() {
    return {
      isChatWindowOpen: false,
      notifications: [
      ],
    };
  },
  mounted() {
    this.fetchNotifications();
  },
  methods: {
    clickAllRead(){
      this.notifications.forEach(async notification => {
        if (notification.isRead === 0) {
          notification.isRead = 1;  
          const response = await axios.get(`http://localhost:8080/api/Message/MarkedMessagesAsRead?messageID=${notification.messageID}`);
          console.log(response.data);
        }
      });
    },

    async fetchNotifications(){
      try {
        const token = localStorage.getItem('token');
        const response = await axios.get(`http://localhost:8080/api/Message/GetMessages?token=${token}`);
        const data=response.data;
        data.forEach(async element => {
          const response = await axios.get(`http://localhost:8080/api/User/GetName?userID=${element.senderID}`);
          const userName= response.data;
          console.log(userName);

          let title;
          switch (element.messageType) {
            case '点赞帖子':
              title = `${userName} 点赞了你的帖子`;
              break;
            case '点赞评论':
              title = `${userName} 点赞了你的评论`;
              break;
            case '转发':
              title = `${userName} 转发了你的帖子`;
              break;
            case '评论':
              title = `${userName} 评论了你的帖子`;
              break;
            case '回复':
              title = `${userName} 回复了你的评论`;
              break;
            default:
              title = `${userName} 有了新的互动`;
              break;
          }
          console.log(element.isRead);
          this.notifications.push({
            messageID:element.messageID,
            title:title,
            message: element.content,
            isRead: element.isRead
          });
        });
      } catch (error) {
        console.error('Error fetching notifications:', error);
      }
      
    },
    async toggleRead(index){
      try {
        const message =  this.notifications[index];
        const response = await axios.get(`http://localhost:8080/api/Message/MarkedMessagesAsRead?messageID=${message.messageID}`);
        console.log(response.data);
        this.notifications[index].isRead=1;
      } catch (error) {
        console.error('Error transmission of read:', error);
      }
    },
    toggleBoxWindow() {
      const chatButton = document.querySelector('.box-button');
      const chatWindow = document.querySelector('.box-common-layout');
      const overlay = document.querySelector('.box-overlay');
      const isVisible = chatWindow.style.display === 'block';

      if (isVisible) {
        chatWindow.classList.remove('open');
        setTimeout(() => {
          chatWindow.style.display = 'none';
          overlay.style.display = 'none';
        }, 400);
      } else {
        // 获取按钮的位置
        const rect = chatButton.getBoundingClientRect();
        chatWindow.style.setProperty('--start-top', `${rect.top + rect.height / 2}px`);
        chatWindow.style.setProperty('--start-left', `${rect.left + rect.width / 2}px`);

        chatWindow.style.display = 'block';
        overlay.style.display = 'block';
        setTimeout(() => {
          chatWindow.classList.add('open');
        }, 10);
      }
    },

  }
}
</script>

<style scoped>
.box-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100vw;
  height: 100vh;
  background-color: rgba(0, 0, 0, 0.5);
  /* 半透明黑色背景 */
  z-index: 100000;
  /* 确保遮罩层在聊天窗口下方但在其他元素上方 */
  display: none;
  /* 默认隐藏 */
}

.box-common-layout {
  position: fixed;
  top: var(--start-top, 50%);
  /* 动态设置起始点 */
  left: var(--start-left, 50%);
  /* 动态设置起始点 */
  transform: translate(-50%, -50%) scale(0);
  /* 初始状态缩小到按钮大小 */
  height: 70vh;
  width: 30vw;
  background-color: rgba(255, 255, 255, 0.8);
  backdrop-filter: blur(10px);
  border-radius: 20px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1),
    0 6px 20px rgba(0, 0, 0, 0.1);
  z-index: 10000001;
  padding: 10px;
  transition: transform 0.4s ease, top 0.4s ease, left 0.4s ease;
  /* 过渡效果 */
  display: none;
  transform-origin: center center;
  /* 使动画从中心展开 */
}

.box-common-layout.open {
  top: 50%;
  /* 最终位置为屏幕中央 */
  left: 50%;
  transform: translate(-50%, -50%) scale(1);
  /* 展开至全屏中央 */
}

.box-button {
  position: fixed;
  bottom: 100px;
  right: 20px;
  width: 60px;
  height: 60px;
  background-color: #a8bc37;
  /* 基本蓝色背景 */
  border-radius: 50%;
  color: white;
  font-size: 24px;
  text-align: center;
  line-height: 60px;
  cursor: pointer;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  z-index: 1001;
  transition: transform 0.3s ease, background-color 0.3s ease;
  background: linear-gradient(135deg, #aecc53, #aecc53);
  /* 蓝色系渐变背景 */
  animation: bounce 2s infinite;
}

/* 悬停效果 */
.box-button:hover {
  transform: scale(1.1);
  /* 放大效果 */
  background: linear-gradient(135deg, #aecc53, #aecc53);
  /* 浅蓝色系渐变背景 */
}

/* 点击时的效果 */
.box-button:active {
  transform: scale(0.9);
  /* 轻微缩小效果 */
}

/* 轻微弹跳动画 */
@keyframes bounce {

  0%,
  20%,
  50%,
  80%,
  100% {
    transform: translateY(0);
  }

  40% {
    transform: translateY(-10px);
  }

  60% {
    transform: translateY(-5px);
  }
}

.notifications-container {
  max-height: 68vh; /* 根据需要设置最大高度 */
  overflow-y: auto; /* 当内容超出容器时显示滚动条 */
  padding-right: 10px; /* 为了给滚动条留出空间 */
}


.notifications-header {
  text-align: center;
  margin-bottom: 0;
}

.notifications-header h3 {
  font-size: 24px;
  font-weight: bold;
  margin: 0;
  padding: 0;
}

.notification-list {
  list-style: none;
  padding: 0;
  margin: 0;
}

.notification-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px;
  border-bottom: 1px solid #eee;
}

.notification-content {
  flex: 1;
  text-align: middle;
}

.notification-title {
  text-align: middle;
  font-weight: bold;
  font-size: 16px;
  color: #333;
}

.notification-message {
  display: block;
  color: #666;
  margin-top: 5px;
  text-align:middle;
}

.notification-time {
  color: #999;
  font-size: 12px;
  margin-left: 10px;
  white-space: nowrap;
}

.notification-item.is-read .el-badge__content {
  background-color: #f0f0f0;
  color: #999;
}

</style>
