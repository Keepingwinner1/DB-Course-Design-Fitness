<template>
  <div class="divMain">
     <!-- 先循环找到聊天对象 -->
    <div v-for="(list, index) in MessageList" :key="index">
      <!-- 判断是否有聊天记录 -->
      <div v-if="list.targetName === target.name">
        <!-- 循环显示聊天记录 -->
        <div v-for="(message, index) in list.list" :key="index" :class="{'message-item': true, 'right-aligned': message.is_me}">
          <el-avatar v-if="!message.is_me" :src="target.img" class="avatar" @click="showUserInfo(target)"></el-avatar>

          <!-- 弹出框 -->
          <el-dialog
            v-model="dialogVisible"
            width="40%"
            :modal="false"
            @close="handleClose"
          >
            <div class="user-info">
              <!-- 用户头像 -->
              <el-avatar :src="selectedUser.img" class="user-info-avatar" size="large"></el-avatar>

              <el-row :gutter="20" class="user-info-details">
                <el-col :span="24" class="info-item">
                  <p> {{ selectedUser.name }}</p>
                </el-col>
              
                <el-col :span="24" class="info-item">
                  <p><strong>标签:</strong> {{ selectedUser.tags }}</p>
                </el-col>
                <el-col :span="24" class="info-item">
                  <p><strong>简介:</strong> {{ selectedUser.introduction || '暂无简介' }}</p>
                </el-col>
                <el-col :span="24" class="info-item">
                  <p><strong>目标类型:</strong> {{ selectedUser.goalType || '未设置' }}</p>
                </el-col>
                <el-col :span="24" class="info-item">
                  <p><strong>目标体重:</strong> {{ selectedUser.goalWeight || '未设置' }}</p>
                </el-col>
              </el-row>
              
            </div>
            
            <span slot="footer" class="dialog-footer">
              <el-button @click="dialogVisible = false">关闭</el-button>
            </span>
          </el-dialog>

          <!-- 显示消息内容 -->
          <!-- 如果是 5 分钟的倍数，或者与上一条消息的时间差超过 5 分钟，显示时间 -->
          <div v-if="shouldShowTime(index,list.list)&&message.is_me" class="message-time">{{ message.time }}</div>

           <!-- 判断并渲染 -->
          <template v-if="isImage(message.messageType)">
            <el-image
              :src="message.message"
              :preview-src-list="[message.message]" 
              fit="cover"
              style="max-width: 200px; margin-left: 10px;"
            />
          </template>
          <template v-else>
            <span class="content">{{ message.message }}</span>
          </template>
        
          <div v-if="shouldShowTime(index,list.list)&&!message.is_me" class="message-time">{{ message.time }}</div>
          <el-avatar v-if="message.is_me" :src="iconUrl" class="avatar right-avatar"></el-avatar>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import store from "../store/index.js";

export default {
name: 'myMain',
data() {
  return {
    dialogVisible: false,
    selectedUser: {
      img: '',
      name: '',
      age: '',
      gender: '',
      tags: '',
      introduction: '',
      goalType: '',
      goalWeight: ''
    }
  };
},
computed: {
    users(){
      return store.state.userListInformation;//用户列表的信息
    },
    target(){
      return store.state.targetInfomation;
    },
    MessageList(){
      return store.state.MessageList;
    },
    iconUrl(){
      return store.state.iconUrl;
    },
  },
methods: {
  // 显示用户信息弹出框
  showUserInfo(target) {
    const userInformation = this.users.find(user => user.id === target.id);
    this.selectedUser = userInformation;
    this.dialogVisible = true;
    console.log(this.dialogVisible)
  },
  // 处理弹出框关闭事件
  handleClose() {
    this.dialogVisible = false;
  },
  shouldShowTime(index,messageList) {
    if (index === 0) return true; // 总是显示第一条消息的时间
    const currentMessage = messageList[index];
    const previousMessage = messageList[index - 1];
    const currentTime = new Date(currentMessage.time);
    const previousTime = new Date(previousMessage.time);
    const timeDifference = (currentTime - previousTime) / 1000 / 60; // 时间差，单位为分钟
    return timeDifference >= 5; // 如果时间差大于等于 5 分钟，显示时间
  },
  isImage(message) {
    console.log(message);
    return message === "image";
  }
}
}
</script>

<style scoped>
.message-time {
color: gray;
font-size: 0.8em;
margin-bottom: 10px;
}

.message-item {
display: flex;
align-items: center;
margin-bottom: 10px;
}

.right-aligned {
justify-content: flex-end;
}

.avatar {
width: 40px;
height: 40px;
margin-right:0px;
}

.right-avatar {
margin-left: 10px;
}

.content {
background-color: rgb(170, 232, 78);
padding: 10px;
border-radius: 10px;
font-weight: bold;
margin-left: 10px;
margin-right: 10px;
max-width: 60%;
line-height: 1.2; /* 调整行高 */
font-family: "Microsoft YaHei";

}

.user-info {
display: flex;
flex-direction: column;
align-items: center;
}

.user-info-avatar {
  width: 100px;   /* 通过 CSS 增加宽度 */
  height: 100px;  /* 通过 CSS 增加高度 */
margin-bottom: 20px;
}

/* .user-info-details {
width: 100%;
margin-top: 20px;
}

.user-info-details p {
margin: 5px 0;
} */

.user-info-details {
  font-family: 'Arial', sans-serif;
}

.info-item {
  margin-bottom: 15px;
}

p {
  margin: 0;
  font-size: 14px;
  color: #333;
}

strong {
  color: #555;
}

</style>
