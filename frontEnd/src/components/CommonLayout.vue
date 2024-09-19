<template>
  <div>
    <div class="chat-button" @click="toggleChatWindow">
      💬
    </div>
    <div class="overlay" @click="toggleChatWindow"></div> <!-- 遮罩层 -->
    <div class="custom-common-layout">
      <el-container style="height: 100vh; ">
        <el-header class="custom-header">
          {{ target.name }}
        </el-header> 
        <el-container>
          <el-aside width="200px">
            <MyAside />
          </el-aside>
          <el-container style="height: 80vh;">
            <el-main>
              <MyMain />
            </el-main>
            <el-footer>
              <div>
                <el-input v-model="input" placeholder="请输入内容" @keyup.enter="sendMessage">
                  <template #suffix>
                    <div class="input-suffix">
                      <img src="../assets/images/emoji.jpg" class="emoji-image" ref="emojiButton"
                        @click="toggleEmojiPicker" />
                      <image-upload />
                    </div>
                  </template>
                  <template #append>
                    <el-button @click="sendMessage">发送</el-button>
                  </template>
                </el-input>
              </div>
            </el-footer>
          </el-container>
        </el-container>
      </el-container>
    </div>
  </div>
</template>

<script>

import MyAside from '../components/myAside.vue';
//import MyFooter from './myFooter.vue';
import MyMain from '../components/myMain.vue';
import store from "../store/index.js";
import axios from 'axios';
import * as signalR from '@microsoft/signalr';
import ImageUpload from '../components/ImageUpload.vue';
import { EmojiButton } from '@joeattardi/emoji-button';
import { ElNotification } from 'element-plus';


export default {
  name: 'CommonLayout',
  data() {
    return {
      // connection: null,
      input: '',
      message: {
        messageID: null,
        senderID: null,
        receiverID: null,
        messageType: 'text',
        content: '',
        sendTime: null
      },

    };
  },
  mounted() {
    this.emojiPicker = new EmojiButton({
      position: 'bottom-start',
      zIndex: 100000001,
    });
    this.emojiPicker.on('emoji', selection => {
      this.input += selection.emoji;
    });

  },
  computed: {
    target() {
      console.log("计算属性target被调用");
      return store.state.targetInfomation;
    }
  },
  components: {
    MyAside, MyMain, EmojiButton,
    ImageUpload
  },
  created() {
    this.setupSignalRConnection();
    this.getFriendInformation();
  },
  async beforeDestroy() {
    // 确保在组件销毁时断开连接
    if (store.state.connection) {
      await store.state.connection.stop();
    }
  },

  methods: {
    toggleChatWindow() {
      const chatButton = document.querySelector('.chat-button');
      const chatWindow = document.querySelector('.custom-common-layout');
      const overlay = document.querySelector('.overlay');
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

    toggleEmojiPicker() {
      document.body.style.overflow = this.emojiPicker.isOpen ? '' : 'hidden';
      this.emojiPicker.togglePicker(this.$refs.emojiButton);
    },

    async setupSignalRConnection() {
      if (store.state.connection!=null){
        return ;
      }
      store.state.connection = new signalR.HubConnectionBuilder()
        .withAutomaticReconnect() // 自动重连
        .configureLogging(signalR.LogLevel.Information) // 启用日志记录，帮助调试
        .withHubProtocol(new signalR.JsonHubProtocol()) // 使用JSON作为协议
        .withUrl('http://localhost:8080/chathub', {
        })
        .build();

      //定义接收消息的方法并监听接收消息的事件
      store.state.connection.on('ReceiveMessage', (messageID, senderID, receiverID, messageType, Content, sendTime) => {
        console.log(parseInt(localStorage.getItem('userID')));
        if (receiverID === parseInt(localStorage.getItem('userID'))) {
          console.log('Received message:', messageID, senderID, receiverID, messageType, Content, sendTime);
          const target = store.state.userListInformation.find(user => user.id === senderID);
          var msg = {
            targetName: target.name,
            targetId: senderID,
            list: {
              is_me: false, // 判断是聊天对象发送的消息还是我发送的消息
              time: sendTime, // 发送信息的时间
              message: Content,
              messageType: messageType
            }
          };
          ElNotification({
            title: '信息：',
            message: '你收到了新的信息',
            type: 'info',
            zIndex:10000003
          });
          store.commit('addUnreadID', senderID);
          store.commit('addMessage', msg);
        }
      });

      // 开始连接
      await store.state.connection
        .start()
        .then(() => {
          console.log("SignalR connected.");
        })
        .catch((err) => console.error("Error connecting to SignalR", err));

    },

    sendMessage() {
      //控制台打印用户输入信息
      console.log('用户输入的信息为：', this.input);

      // 获取当前时间
      const currentTime = new Date().toLocaleString();
      if (!store.state.targetInfomation.id) {
        // 如果无发送用户
        ElNotification({
          title: '提示',
          message: '请选择你要发送的用户',
          type: 'info',
          position: 'bottom-right', // 将通知显示在右下角
          zIndex:10000003
        });
        return ;
      }
      if (this.input) {
        // 构建消息对象并提交到 store
        var msg = {
          targetName: store.state.targetInfomation.name,
          targetId: store.state.targetInfomation.id,
          list: {
            is_me: true,//用来判断是聊天对象发送的消息还是我发送的消息
            time: currentTime,//发送信息的时间
            message: this.input,
            messageType: '', // 这里为空字符串
          }
        };
        // 构建消息请求体
        const message = {
          messageID: -1,
          senderID: parseInt(localStorage.getItem('userID')),
          receiverID: store.state.targetInfomation.id,
          messageType: 'text',
          Content: this.input,
          sendTime: currentTime,
        };
        console.log(localStorage.getItem('userID'));

        // this.connection.invoke("Sgn")
        //       .then(() => {
        //           console.log("Sgn method invoked successfully.");
        //       })
        //       .catch((err) => {
        //           console.error("Error invoking Sgn method:", err);
        //       });
        store.state.connection
          .invoke("Send", message.messageID, message.senderID, message.receiverID, message.messageType, message.Content, message.sendTime)
          .then(() => {
            console.log("Message sent.");
            this.input = '';//需要将输入框进行清空
            console.log('清空输入框,输入框内容为：', this.input);
            store.commit('addMessage', msg);
          })
          .catch((err) => {
            console.error("Error sending message", err);
            ElNotification({
              title: '失败',
              message: `网络连接出错，无法发送信息`,
              type: 'error',
              position:'top-left',
              zIndex:10000003
        });
          });
      }
      else {
        ElNotification({
          title: '提示',
          message: '输入不能为空',
          type: 'info',
          position: 'bottom-right', // 将通知显示在右下角
          zIndex:10000003
        });
      }
    },
    addEmoji(emoji) {
      this.input += emoji;
    },
    async getFriendInformation() {
      try {
        const token = localStorage.getItem('token');
        const response = await axios.get(`http://localhost:8080/api/User/GetFriendList?token=${token}`);
        const userList = response.data;  // 获取 userList
        const userListInformation = []; // 用于存储用户详细信息
        for (const user of userList) {
          const response = await axios.get('http://localhost:8080/api/User/GetProfileByUserID', {
            params: {
              token: token, // 将 Token 作为查询参数 
              userID: user //用户ID   
            }
          });
          const userInfo = response.data;
          // 构建新的用户信息对象
          const userInformation = {
            id: userInfo.userID,
            name: userInfo.userName,
            img: userInfo.iconURL, // 使用返回的 iconURL 或原始 img
            age: userInfo.age,
            gender: userInfo.gender,
            tags: userInfo.tags,
            introduction: userInfo.introduction,
            goalType: userInfo.goalType,
            goalWeight: userInfo.goalWeight
          };
          // 将用户信息推入 userListInformation 数组
          userListInformation.push(userInformation);
        }
        // 将新的用户详细信息列表提交到 Vuex store
        store.commit('setUserListInformation', userListInformation);
        store.commit('setUserList', userList);
        console.log('Updated User List Information:', store.state.userListInformation);
        //this.getChatHistory();
      } catch (error) {
        console.error('Error fetching user information:', error);
      }
    },

    async getChatHistory() {
      try {
        //console.log("1",userList);
        const userList = localStorage.getItem('userList');
        //console.log("2",userList);
        const token = localStorage.getItem('token');
        //console.log("3",token);

        for (const user of userList) {
          const response = await axios.get(`http://localhost:8080/api/Message/GetChatHistory?userID=${user}`);

          // 获取返回的数据
          const data = response.data;

          const userListInformation = localStorage.getItem('userListInformation');
          // 遍历每条消息
          data.forEach(message => {
            // 根据 messageType 判断并处理消息
            if (message.senderID === user) { //发送方是好友
              var target_user = userListInformation.find(user => user.id === message.senderID);
              var msg = {
                targetName: target_user.name,
                targetId: target_user.id,
                list: {
                  is_me: false,//用来判断是聊天对象发送的消息还是我发送的消息
                  time: message.sendTime,//发送信息的时间
                  message: message.content,
                  messageType: message.messageType,
                }
              }
              store.commit('addMessage', msg);
            } else {
              var target_user = userListInformation.find(user => user.id === message.receiverID);
              var msg = {
                targetName: target_user.name,
                targetId: target_user.id,
                list: {
                  is_me: true,//用来判断是聊天对象发送的消息还是我发送的消息
                  time: message.sendTime,//发送信息的时间
                  message: message.content,
                  messageType: message.messageType,
                }
              }
              store.commit('addMessage', msg);
            }
          });
          // 打印处理后的聊天记录
          console.log('Chat History:', data);
        }
      } catch (error) {
        // 处理请求错误
        console.error('Error fetching chat history:', error);
      }
    }
  }
}
</script>

<style scoped>
.overlay {
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

.custom-common-layout {
  position: fixed;
  top: var(--start-top, 50%);
  /* 动态设置起始点 */
  left: var(--start-left, 50%);
  /* 动态设置起始点 */
  transform: translate(-50%, -50%) scale(0);
  /* 初始状态缩小到按钮大小 */
  height: 90vh;
  width: 60vw;
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

.custom-common-layout.open {
  top: 50%;
  /* 最终位置为屏幕中央 */
  left: 50%;
  transform: translate(-50%, -50%) scale(1);
  /* 展开至全屏中央 */
}

.input-suffix {
  display: flex;
  align-items: center;
  /* 确保子元素在垂直方向上居中对齐 */
  gap: 10px;
  /* 控制两个组件之间的间距 */
}

.emoji-image {
  width: 16px;
  /* 根据需要调整图片尺寸 */
  height: 16px;
}

.common-layout {

  height: 550px;
  width: 958.4px;
}

.el-container {
  height: 592px;
}

.el-header {
  border-bottom: 1px solid #ccc;
  /* 下边框 */

}

.el-footer {
  /* background-color:white; */
  color: #333;
  text-align: center;
  line-height: 60px;
  border-top: 1px solid #ccc;
  /* 上边框 */
  border-right: 1px solid #ccc;
  /* 右边框 */
  border-bottom: 1px solid #ccc;
  /* 下边框 */
  border-left: 1px solid #ccc;
  /* 左边框 */
}

.el-aside {

  /* background-color: white; */
  color: #333;
  text-align: center;
  height: 80vh;
  border-top: 1px solid #ccc;
  /* 上边框 */
  border-right: 1px solid #ccc;
  /* 右边框 */
  border-bottom: 1px solid #ccc;
  /* 下边框 */
  border-left: 1px solid #ccc;
  /* 左边框 */
}

.el-main {
  /* background-color: white; */
  color: #333;
  height: 532px;
  border-top: 1px solid #ccc;
  /* 上边框 */
  border-right: 1px solid #ccc;
  /* 右边框 */
  border-bottom: 1px solid #ccc;
  /* 下边框 */
  border-left: 1px solid #ccc;
  /* 左边框 */
}

.custom-header {

  line-height: 60px;
  /* 使文本垂直居中，对应你的header高度 */
  font-size: 24px;
  /* 可选：调整字体大小 */
}

.chat-button {
  position: fixed;
  bottom: 20px;
  right: 20px;
  width: 60px;
  height: 60px;
  background-color: #3686d7;
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
  background: linear-gradient(135deg, #1E90FF, #00BFFF);
  /* 蓝色系渐变背景 */
  animation: bounce 2s infinite;
}

/* 悬停效果 */
.chat-button:hover {
  transform: scale(1.1);
  /* 放大效果 */
  background: linear-gradient(135deg, #00BFFF, #87CEFA);
  /* 浅蓝色系渐变背景 */
}

/* 点击时的效果 */
.chat-button:active {
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
</style>
