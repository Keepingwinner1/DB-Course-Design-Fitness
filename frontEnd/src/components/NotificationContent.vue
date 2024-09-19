<template>
  <div class="notification-content">
    <p>
      <img :src="imageSrc" alt="Example Image" class="notification-image" />
    </p>
    <p v-if="guide" style="text-indent: 2em; font-size:20px ">
      {{ guide }}
    </p>
    <p v-else style="text-align: center;">
      正在加载器械操作指南...
    </p>
    <!-- 修改按钮，用于开启或关闭对话 -->
    <el-button type="primary" @click="toggleConversation" style="font-size:20px">
      {{ conversationStarted ? '关闭对话' : '开启对话' }}
    </el-button>

    <!-- 对话组件 -->
    <div v-if="conversationStarted" class="conversation-container">
      <div class="home">
        <div class="home-right">
          <div class="right-version">
            <div class="llm-chat-demo">
              <span class="chat-demo" style="font-size:20px">AI健身教练</span>
            </div>
          </div>
          <div class="right-body" :class="messages.length === 0 ? 'nodata' : ''" ref="messageContainer">
            <div v-for="(message, index) in messages" class="main-message" :key="index"
              :class="{ 'user-message': message.sender === 'user', 'friend-message': message.sender === 'assistant' }">
              <div class="message-sender"
                :class="{ 'user-message': message.sender === 'user', 'friend-message': message.sender === 'assistant' }">
                <img v-if="message.sender === 'user'" src="../assets/images/我的.png" alt="User Icon">
                <img v-else-if="message.sender === 'assistant'" src="../assets/images/我的2.png" alt="Coach Icon">
                <span class="message-sender-name" :class="message.sender === 'user' ? 'user-color' : 'friend-color'">{{
        message.sender }}:</span>
              </div>
              <div v-if="message.sender === 'user'" class="user-message" v-html="message.content" ></div>
              <div v-else class="friend-message" v-html="md.render(message.content)"></div>
            </div>
          </div>
          <div class="right-input" @keyup.enter="handleSearch">
            <el-input v-model="queryKeyword" placeholder="给AI健身助手发送消息" class="input" style="font-size:15px"></el-input>
            <el-button v-if="!loading" type="primary" style="font-size:20px" @click="handleSearch">发送</el-button>
            <el-button v-if="loading" type="primary" style="font-size:20px" @click="closeEventSource">停止</el-button>
          </div>
          <div class="sec-notice" style="font-size:15px">AI健身教练可能会说错话哟，回答仅供参考，详细内容请斟酌
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, nextTick, onBeforeUnmount } from 'vue'
import axios from 'axios'
import MarkdownIt from 'markdown-it';
import markdownItFootnote from 'markdown-it-footnote';
import markdownItTaskLists from 'markdown-it-task-lists';
import markdownItAbbr from 'markdown-it-abbr';
import markdownItContainer from 'markdown-it-container';
import hljs from 'highlight.js';
import markdownItHighlightjs from 'markdown-it-highlightjs';

// 使用 defineProps 接收 props
const props = defineProps({
  equipmentName: {
    type: String,
    required: true
  }
})

const imageSrc = ref('')
const guide = ref(null)
const conversationStarted = ref(false)  // 跟踪对话是否已启动
const messages = ref([]);
const queryKeyword = ref('');
const loading = ref(false);
const eventSource = ref(null);
const messageContainer = ref(null);

// const md = new MarkdownIt()
//   .use(markdownItFootnote)
//   .use(markdownItTaskLists, { enabled: true })
//   .use(markdownItAbbr)
//   .use(markdownItContainer, 'warning')
//   .use(markdownItHighlightjs, { hljs });

const md = new MarkdownIt({
    breaks: false,   // 禁用自动将换行符转换为 <br>
    html: true,      // 允许内嵌 HTML
    linkify: true,   // 自动识别 URL 并转换为链接
    typographer: true // 启用替换“智能”引号等功能
});


const fetchGuide = async () => {
  try {
    const response = await axios.get('http://localhost:8080/api/AIGuide/GetEquipmentGuide', {
      params: { equipmentName: props.equipmentName } // 使用从 props 接收的 equipmentName

    })
    guide.value = response.data.operationGuide
    imageSrc.value = response.data.imgUrl
  } catch (error) {
    console.error('Failed to fetch guide:', error)
  }
}

onMounted(() => {
  fetchGuide()
})

// 新增的函数，用于切换对话状态
const toggleConversation = () => {
  conversationStarted.value = !conversationStarted.value;

  // 如果关闭对话，确保停止EventSource
  if (!conversationStarted.value) {
    closeEventSource();
  }
};




const handleSearch = (event) => {
    event.stopPropagation();  // 阻止事件冒泡，防止触发关闭逻辑
    if (loading.value) {
        return;
    }

    loading.value = true;
    const keyword = queryKeyword.value;

    // 将用户的输入加入消息列表
    messages.value.push({
        content: md.render(keyword),
        sender: 'user'
    });

    nextTick(() => {
        scrollToBottom(); // 确保滚动到底部
    });

    queryKeyword.value = '';

    // 构建要发送的消息数组
    const messageArray = messages.value.map((message) => {
        return {
            role: message.sender === 'user' ? 'user' : 'assistant',
            content: message.content
        };
    });

    // 使用EventSource处理流式输出
    const encodedMessages = encodeURIComponent(JSON.stringify(messageArray));
    eventSource.value = new EventSource(`http://localhost:8080/api/AIGuide/LLM?equipmentName=${props.equipmentName}&messages=${encodedMessages}`);

    let assistantMessage = {
        content: '',
        sender: 'assistant'
    };

    let assistantRawMessage = {
        content: '',
        sender: 'assistant'
    };

    messages.value.push(assistantMessage); // 先添加一条空的消息

    eventSource.value.onmessage = function (event) {
        if (event.data === '[DONE]') {
            eventSource.value.close();
            loading.value = false;
            return;
        }

        if (event.data) {
            console.log("Received chunk:", event.data);

            var tmp = event.data.toString();
            tmp = tmp.replaceAll("xx ", "\n");
            assistantRawMessage.content += tmp;
            assistantMessage.content = assistantRawMessage.content;

            // 强制刷新消息列表
            messages.value = [...messages.value];

            nextTick(() => {
                scrollToBottom(); // 确保滚动到底部
            });
        }
    };

    eventSource.value.onerror = function (event) {
        console.error('EventSource failed:', event);
        eventSource.value.close();
        loading.value = false;
    };
};



function scrollToBottom() {
  const container = document.querySelector('.right-body');
  container.scrollTop = container.scrollHeight;
  const container_2 = document.querySelector('.custom-notification');
  container_2.scrollTop = container_2.scrollHeight;
}



const closeEventSource = (event) => {
    event?.stopPropagation();  // 阻止事件冒泡，防止触发关闭逻辑
    loading.value = false;
    if (eventSource.value) {
        eventSource.value.close();
        eventSource.value = null;  // 确保之后不会继续触发
    }
};


onBeforeUnmount(() => {
  if (eventSource.value) {
    eventSource.value.close();
  }
});

</script>

<style scoped>
.notification-content {
  text-align: left;
  padding: 20px;
  width: 800px;
}

.notification-image {
  width: 100%;
  max-height: 400px;
  object-fit: cover;
  margin: 20px auto;
  display: block;
  border-radius: 10px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.conversation-container {
  margin-top: 20px;
}

.home {
  height: 100%;
  width: 100%;
  display: flex;
}

.home-right {
  width: 100%;
}

.right-version {
  width: 100%;
  height: 5%;
  display: flex;
  justify-content: start;
  align-items: center;
  border-radius: 15px;
  margin-bottom: 12px;
}

.version {
  color: rgb(155, 155, 155);
}

.llm-chat-demo {
  width: 80%;
  margin: auto;
  font-family: Söhne, ui-sans-serif, system;
  font-variation-settings: normal;
  font-weight: 550;
  font-size: 18px;
  cursor: pointer;
  color-scheme: light;

}

.chat-demo {
  opacity: 0.65;

}

.right-body {
  max-height: 79vh;
  height: 85%;
  overflow-y: auto;
}

.user-color {
  color: #1296db;
}

.friend-color {
  color: #14a003;
}

.nodata {
  background-image: url("../assets/images/happy.png");
  background-repeat: no-repeat;
  background-size: 35%;
  background-position: center 50%;
}

.main-message {
  width: 80%;
  justify-content: center;
  margin-bottom: 100px;
  margin-left: 20px;
  overflow-y: auto;
}

.message-sender-name {
  margin-left: 10px;
  font-family: Söhne, ui-sans-serif, system;
  font-weight: 550;
  font-size: 18px;
}

.right-input {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 6.5%;
  position: relative;
}

.sec-notice {
  height: 3.5%;
  margin-top: 10px;
  font-size: 12px;
  font-family: Söhne, ui-sans-serif;
  color: rgb(155, 155, 155);
  display: flex;
  justify-content: center;
}

.input {
  width: 75%;
  margin-right: 5px;
}


::v-deep .el-button {
  padding: 5px 6px;
}

::v-deep .el-button--primary {
  position: relative;
  right: 0%;
  background-color: rgba(180, 180, 180, 0.3) !important;
  color: black !important;
  border-color: rgba(180, 180, 180, 0.3) !important;
}

.user-message {
  text-align: left;
  padding: 5px;
  padding-left: 15px;
  margin-bottom: 5px;
  border-radius: 15px;
  color: #0b2d47;
}

.friend-message {
  background-color: rgba(177, 227, 227, 0.2);
  text-align: left;
  padding: 5px;
  padding-left: 15px;
  margin-bottom: 5px;
  color: #0b2d47;
}

::v-deep .friend-message pre .hljs {
  border-radius: 10px !important;
  background-color: #FAF7F7;
}

::-webkit-scrollbar {
  width: 6px;
}

::-webkit-scrollbar-track {
  background: #f1f1f1;
}

::-webkit-scrollbar-thumb {
  background: #888;
  border-radius: 3px;
}

::-webkit-scrollbar-thumb:hover {
  background: #555;
}
</style>
