<template>
    <el-upload
      class="upload-demo"
      action="https://jsonplaceholder.typicode.com/posts/"
      :on-change="handleChange"
      :before-upload="beforeUpload"
    >
      <el-icon style="font-size: 20px;"><PictureFilled /></el-icon>
    </el-upload>
  </template>
  
  <script>
  import {PictureFilled} from '@element-plus/icons-vue';
  import store from "../store/index.js";
 
  export default {
    name: 'ImageUpload',
    data() {
      return {
        hasChanged: false // 标志位，初始为false
      };
    },
    components: {
      PictureFilled,
    },
    methods: {
      
      async handleChange(file, fileList) {
        console.log('用户确认选择该图片', file);
        this.hasChanged = !this.hasChanged;
        if (this.hasChanged) return;

        const token = store.state.token; // 假设 token 存储在 Vuex 的 store 中,在测试的时候直接拿一个字符串
        try {
          // 使用 FileReader 将图片文件转换为 Base64 格式
          const reader = new FileReader();
          reader.readAsDataURL(file.raw);

          reader.onload = async () => {
            const base64Image = reader.result;

            // 获取当前时间
            const currentTime = new Date().toLocaleString();

            // 创建前端渲染所需的图片URL
            const imageURL = URL.createObjectURL(file.raw);

            // 构建消息对象并提交到 store
            const msg = {
              targetName: store.state.targetInfomation.name,
              targetId: store.state.targetInfomation.targetId,
              list: {
                is_me: true, // 用来判断是聊天对象发送的消息还是我发送的消息
                time: currentTime, // 发送信息的时间
                message:imageURL ,
                messageType: 'image', // 指定消息类型为图片
              }
            };
            store.commit('addMessage', msg);

            // 构建请求体
            const messageData = {
              receiverID: store.state.targetInfomation.id,
              senderID: store.state.myInformation.id,
              time: currentTime,
              messageType: 'image', // 指定消息类型为图片
              content: base64Image, // 空字符串，因为图片是通过 `image` 字段发送的
            };

            // 发送 POST 请求到后端 API
            const response = await axios.post(`http://localhost:8080/api/Message/SendMessage?token=${token}`, messageData);
            console.log('Message with image sent successfully:', response.data);
          };

          reader.onerror = (error) => {
            console.error('Error reading file:', error);
          };
        } catch (error) {
          console.error('Error handling image upload:', error);
        }
      },
      beforeUpload(file) {
        const isImage = file.type.startsWith('image/');
        if (!isImage) {
          this.$message.error('只能上传图片格式的文件');
        }
        return isImage;
      }
    }
  }
  </script>
  
  <style scoped>
.upload-demo {
  display: inline-block; /* 使上传组件适应内容 */
  width :13px;
  height:24px;
}
.upload-button{
  width :13px;
  height:24px;
}

  </style>
  