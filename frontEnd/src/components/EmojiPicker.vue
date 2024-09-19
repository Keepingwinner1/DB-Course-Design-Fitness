<template>
    <el-popover
      placement="top"
      width="200"
      trigger="click"
      v-model="popoverVisible"
    >
      <div class="emoji-container">
        <span v-for="emoji in emojis" :key="emoji" @click="selectEmoji(emoji)">
          {{ emoji }}
        </span>
      </div>
      <template #reference>
        <img 
          src="../assets/images/emoji.jpg"
          alt="Emoji" 
          class="emoji-image" 
          @click="togglePopover"
        />
      </template>
    </el-popover>
  </template>
  
  <script>
  import { ElPopover, ElButton, ElIcon } from 'element-plus';
  import { Plus } from '@element-plus/icons-vue';
  
  export default {
    name: 'EmojiPicker',
    components: {
      ElPopover,
      ElButton,
      ElIcon,
      // SmileIcon,
    },
    data() {
      return {
        popoverVisible: false,
        emojis: ['😀', '😂', '😍', '🥳', '😎', '👍', '❤️', '🎉'],
      };
    },
    methods: {
      togglePopover() {
        this.popoverVisible = !this.popoverVisible;
      },
      selectEmoji(emoji) {
        this.popoverVisible = false;
        this.$emit('select-emoji', emoji);
      }
    }
  }
  </script>
  
  <style scoped>
  .emoji-container {
    display: flex;
    flex-wrap: wrap;
    gap: 10px;
  }
  
  .emoji-container span {
    font-size: 24px;
    cursor: pointer;
  }
  
  .emoji-container span:hover {
    transform: scale(1.2);
  }
  
  .emoji-button {
    padding: 0; /* 去除内边距 */
    background-color: transparent; /* 背景透明 */
    border: none; /* 无边框 */
    box-shadow: none; /* 移除阴影 */
    min-width: 0; /* 去掉按钮默认最小宽度 */
  }
  
  .emoji-button .el-icon {
    font-size: 20px; /* 根据需要调整图标大小 */
    line-height: 1; /* 确保图标垂直居中 */
  }
  
  .emoji-image {
    width: 16px; /* 根据需要调整图片尺寸 */
    height: 16px;
  }
  </style>
  