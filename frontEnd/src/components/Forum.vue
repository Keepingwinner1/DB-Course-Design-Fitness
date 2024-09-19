<template>
  <div class="forum-bg">
    <el-backtop class="forum-backtop-button" />
    <div class="forum-container">
      <!-- 帖子卡片 -->
      <el-card class="card">
        <!-- 站内公告 -->
        <div class="announcement-section">
          <el-row class=" row">
            <el-col :span="4">
              <icon-home />
            </el-col>
            <el-col :span="20">
              <el-text class="title">站内公告</el-text>
            </el-col>
          </el-row>
          <div class="announcement-content">
            <el-divider />
            <el-text>📢 全新打卡活动上线！快来参与，每日打卡赢取丰厚奖励！<br />🎉 社区迎来全新升级，更多精彩功能等你体验！</el-text>
            <el-divider />
          </div>
        </div>
        <el-divider />
        <!-- 打卡活动 -->
        <div class="activity-section">
          <el-row class="row">
            <el-col :span="4">
              <icon-calendar />
            </el-col>
            <el-col :span="20">
              <el-text class="title">打卡活动</el-text>
            </el-col>
          </el-row>
          <div class="activity-content">
            <el-divider />
            <el-text>💪 健身达人7天打卡挑战赛：完成7天连续打卡，赢取健身礼包！<br />🏃 每日晨跑打卡：坚持跑步，每日签到赢取健康积分！</el-text>
            <el-divider />
          </div>
        </div>
        <el-divider />
        <!-- 比赛活动通知 -->
        <div class="contest-section">
          <el-row class="row">
            <el-col :span="4">
              <icon-trophy />
            </el-col>
            <el-col :span="20">
              <el-text class="title">比赛活动</el-text>
            </el-col>
          </el-row>
          <div class="contest-content">
            <el-divider />
            <el-text>🏅 社区健身大赛：参与比赛，展现你的健身成果，赢取丰厚奖品！<br />🏆 全年健身挑战赛：累计积分最高者将赢得终极大奖！</el-text>
            <el-divider />
          </div>
        </div>
      </el-card>

      <div class="main-content">
        <!-- 导航栏部分 -->
        <nav class="navbar">
          <ul class="navbar-list">
            <li class="navbar-item" v-for="category in visibleCategories" :key="category"
                @click="filterByCategory(category)" :class="{ active: selectedCategory === category }">
              {{ category }}
              <span class="underline" v-if="selectedCategory === category"></span>
            </li>
          </ul>
          <button class="scroll-btn" @click="scrollRight">
            <icon-arrow-right />
          </button>
        </nav>
        <div>
          <el-input v-model="searchQuery" placeholder="搜索帖子..." class="search-box" @input="filterPosts"
                    clearable>
            <template #prefix>
              <el-icon>
                <search />
              </el-icon>
            </template>
          </el-input>
        </div>
        <EditArticle v-model:title="newPost.title" v-model:content="newPost.content"
                     v-model:category="newPost.category" v-model:imgUrl="newPost.imgUrl" @add-post="addPost" />

        <!-- 帖子列表部分 -->
        <div v-for="post in filteredPosts" :key="post.postID" class="post-item">
          <div class="post-content">
            <img v-if="post.isPinned" src="../assets/images/top-icon.png" alt="置顶" class="top-icon" />
            <h3 class="post-title" @click="viewPost(post.postID)">
              {{ post.postTitle }}
              <span class="category-tag">{{ post.postCategory }}</span>
            </h3>
            <div v-if="post.imgUrl != `null`" class="post-image">
              <img :src="post.imgUrl" alt="Post Image" class="image" />
            </div>
            <p class="post-snippet" v-html="renderContent(post.postContent)"></p>
          </div>
          <div class="post-footer">
            <span class="post-author">{{ post.userName }}</span>
            <span class="post-actions">
                            <span class="icon-with-text no-click">
                                <LikeOutlined />
                                <span>{{ post.likesCount }}</span>
                            </span>
                            <span class="icon-with-text no-click">
                                <MessageOutlined />
                                <span>{{ post.commentsCount }}</span>
                            </span>
                            <span class="icon-with-text no-click">
                                <ShareAltOutlined />
                                <span>{{ post.forwardCount }}</span>
                            </span>
                            <span v-if="isCurrentUser(post.userName)" class="icon-with-text-delete"
                                  @click="deletePost(post.postID, post.userID)">
                                <DeleteOutlined />
                            </span>
              <!-- 根据是否置顶显示不同图标 -->
                            <span v-if="isAdmin(post.userName)" class="icon-with-text-delete"
                                  @click="putTop(post.postID)">
                                <VerticalAlignTopOutlined v-if="!post.isPinned" />
                                <VerticalAlignBottomOutlined v-else />
                            </span>
                        </span>
          </div>
        </div>
      </div>

      <!-- 右侧栏：热帖展示区域 -->
      <div class="right-sidebar">
        <div class="hot-posts-section">
          <el-row class="row">
            <el-col :span="4">
              <icon-fire />
            </el-col>
            <el-col :span="20">
              <el-text class="title">热帖推荐</el-text>
            </el-col>
          </el-row>
          <div class="hot-posts-content">
            <el-divider />
            <el-text v-for="hotPost in hotPosts" :key="hotPost.postID" @click="viewPost(hotPost.postID)"
                     class="hot-post-title">
              <icon-fire class="icon-fire-small" /> {{ hotPost.postTitle }}
            </el-text>
            <el-divider />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>


<script>
import axios from 'axios';
import { mapState } from 'vuex';
import EditArticle from '../components/EditArticle.vue';
import { ElNotification } from 'element-plus';
import { IconCalendar, IconTrophy, IconArrowRight, IconFire, IconHome } from '@arco-design/web-vue/es/icon';
import { postMixin } from '../mixins/postMixin.js';
import { LikeOutlined, MessageOutlined, ShareAltOutlined, DeleteOutlined, VerticalAlignTopOutlined, VerticalAlignBottomOutlined } from '@ant-design/icons-vue';
import store from '../store/index.js';
import { Search } from '@element-plus/icons-vue';
import { commonMixin } from '../mixins/checkLoginState';

export default {
  mixins: [postMixin, commonMixin],
  components: {
    EditArticle,
    IconHome,
    IconCalendar,
    IconTrophy,
    IconArrowRight,
    IconFire,
    LikeOutlined,
    MessageOutlined,
    ShareAltOutlined,
    DeleteOutlined,
    VerticalAlignTopOutlined,
    VerticalAlignBottomOutlined,
    Search,
  },
  data() {
    return {
      searchQuery: '',
      newPost: {
        postID: null,
        userID: null,
        userName: '',
        postTitle: '',
        postContent: '',
        postCategory: '',
        postTime: '',
        likesCount: null,
        forwardCount: null,
        commentsCount: null,
        refrencePostID: null,
        imgUrl: 'null'
      },
      currentUser: localStorage.getItem('name'),
      allPosts: [],
      filteredPosts: [],
      hotPosts: [],
      selectedCategory: "全部帖子",
      currentIndex: 0,
    };
  },
  computed: {
    ...mapState(["categories"]),
    visibleCategories() {
      const doubledCategories = [...this.categories, ...this.categories];
      const startIndex = this.currentIndex % this.categories.length;
      return doubledCategories.slice(startIndex, startIndex + 6);
    },
  },
  created() {
    this.checkAvailable();
    this.fetchAllPosts();
    store.dispatch('pollIsPost');
  },
  methods: {
    isAdmin(userName) {
      return this.$store.state.role === 'admin';
    },
    deletePost(postID, userID) {
      const token = localStorage.getItem('token');
      axios.delete(`http://localhost:8080/api/Post/DeletePostByPostID`, {
        params: {
          token: token,
          postID: postID,
          postOwnerID: userID
        }
      })
          .then(response => {
            console.log(response.data);
            if (response.data.message === '删除帖子成功') {
              ElNotification({
                title: '成功',
                message: '帖子删除成功',
                type: 'success',
              });
              this.fetchAllPosts();
            } else {
              ElNotification({
                title: '错误',
                message: '删除帖子失败',
                type: 'error',
              });
            }
          })
          .catch(error => {
            ElNotification({
              title: '错误',
              message: '删除帖子时发生错误',
              type: 'error',
            });
          });
    },
    isCurrentUser(userName) {
      return this.currentUser === userName || this.$store.state.role === 'admin';
    },
    scrollRight() {
      this.currentIndex = (this.currentIndex + 1) % this.categories.length;
    },

    fetchAllPosts() {
      const token = localStorage.getItem('token');
      axios.get(`http://localhost:8080/api/Post/GetAllPost?token=${token}`)
          .then(response => {
            this.allPosts = response.data
                .filter(post => post.isReported === 0) // 过滤掉被举报的帖子
                .sort((a, b) => new Date(b.postTime) - new Date(a.postTime));
            this.filteredPosts = this.allPosts;
            this.updateHotPosts(); // 更新热帖
            this.updatePostsOrder(); // 更新帖子顺序
          })
          .catch(error => {
            ElNotification({
              title: '错误',
              message: '获取帖子时发生错误，请稍后再试。',
              type: 'error',
            });
          });
    },

    getAllPosts(token) {
      return axios.get(`http://localhost:8080/api/Post/GetAllPost?token=${token}`)
          .then(response => {
            this.allPosts = response.data.sort((a, b) => new Date(b.postTime) - new Date(a.postTime));
            this.filteredPosts = this.allPosts;
            console.log(this.allPosts);
            this.updateHotPosts();
            this.updatePostsOrder();
            return response;
          })
          .catch(error => {
            ElNotification({
              title: '错误',
              message: '获取所有帖子时发生错误，请稍后再试。',
              type: 'error',
            });
            throw error;
          });
    },

    putTop(postID) {
      const token = localStorage.getItem('token');
      const postIndex = this.allPosts.findIndex(post => post.postID === postID);
      if (postIndex !== -1) {
        const post = this.allPosts[postIndex];
        if (!post.isPinned) {
          // 调用置顶接口
          axios.get('http://localhost:8080/api/Post/PinPost', {
            params: { token, postID }
          }).then(response => {
            if (response.data.message === '成功置顶') {
              post.isPinned = true;
              this.updatePostsOrder(); // 重新排序
              ElNotification({
                title: '成功',
                message: '帖子已置顶',

                type: 'success',

              });

            }
          })
              .catch(error => {
                ElNotification({
                  title: '错误',
                  message: '置顶帖子时发生错误',
                  type: 'error',
                });
              });
        } else {
          // 调用取消置顶接口
          axios.get('http://localhost:8080/api/Post/CanclePinPost', {
            params: { token, postID }
          })
              .then(response => {
                if (response.data.message === '成功取消置顶') {
                  post.isPinned = false;
                  this.updatePostsOrder(); // 重新排序
                  ElNotification({
                    title: '成功',
                    message: '帖子已取消置顶',
                    type: 'success',
                  });
                }
              })
              .catch(error => {
                ElNotification({
                  title: '错误',
                  message: '取消置顶时发生错误',
                  type: 'error',
                });
              });
        }
      }
    },

    updatePostsOrder() {
      // 将置顶的帖子放在最前面，其余的按时间排序
      const pinnedPosts = this.allPosts.filter(post => post.isPinned);
      const unpinnedPosts = this.allPosts.filter(post => !post.isPinned)
          .sort((a, b) => new Date(b.postTime) - new Date(a.postTime));
      // 置顶帖子放在前面，未置顶的放在后面
      this.allPosts = [...pinnedPosts, ...unpinnedPosts];
      this.filteredPosts = [...this.allPosts];
    },


    filterByCategory(category) {
      this.selectedCategory = category;
      this.filterPosts();
    },

    filterPosts() {
      const query = this.searchQuery.toLowerCase();
      this.filteredPosts = this.allPosts.filter(post => {
        const matchesTitle = post.postTitle.toLowerCase().includes(query);
        const matchesContent = post.postContent.toLowerCase().includes(query);
        const matchesUser = post.userName.toLowerCase().includes(query);
        const matchesCategory = this.selectedCategory === '全部帖子' || post.postCategory === this.selectedCategory;
        return (matchesTitle || matchesContent || matchesUser) && matchesCategory;
      });
    },

    addPost() {
      if (this.$store.state.isPost === 0) {
        ElNotification({
          title: '警告',
          message: '您已被禁言，无法发帖。',
          type: 'warning',
        });
        return;
      }

      const token = this.$store.state.token;
      const name = localStorage.getItem('name');

      if (this.newPost.title && this.newPost.content && this.newPost.category) {
        const cleanedContent = this.cleanHtml(this.newPost.content);
        const newPost = {
          postID: -1,
          userID: localStorage.getItem('userID'),
          userName: name,
          postTitle: this.newPost.title,
          postContent: cleanedContent,
          postCategory: this.newPost.category,
          postTime: new Date().toISOString(),
          likesCount: 0,
          forwardCount: 0,
          commentsCount: 0,
          refrencepostID: -1,
          imgUrl: this.newPost.imgUrl === 'null' ? 'null' : this.newPost.imgUrl
        };
        axios.post(`http://localhost:8080/api/Post/PublishPost?token=${token}`, newPost)
            .then(response => {
              console.log(newPost);
              newPost.postID = response.data.postID;
              this.allPosts.unshift(newPost);
              this.filterPosts();
              this.updateHotPosts();
              this.resetNewPostForm();

              ElNotification({
                title: '成功',
                message: '帖子发布成功！',
                type: 'success',
              });
            })
            .catch(error => {
              ElNotification({
                title: '错误',
                message: '发布帖子时发生错误，请稍后再试。',
                type: 'error',
              });
            });
      } else {
        ElNotification({
          title: '警告',
          message: '请填写所有字段！',
          type: 'warning',
        });
      }
    },

    cleanHtml(content) {
      let cleanedContent = content.replace(/<br\s*\/?>/gi, '<br/>');
      cleanedContent = cleanedContent.replace(/<span\s+style="font-family:\s*([^;]+);?">/gi, (match, fontFamily) => {
        return `<span style="font-family:${fontFamily};">`;
      });
      cleanedContent = cleanedContent.replace(/<\/?span[^>]*>/gi, '');
      return cleanedContent;
    },

    renderContent(content) {
      const plainText = content.replace(/<[^>]+>/g, '');
      return plainText.length > 40 ? plainText.slice(0, 40) + '...' : plainText;
    },

    resetNewPostForm() {
      this.newPost = {
        title: '',
        content: '',
        category: '',
        imgUrl: 'null'
      };
      this.$forceUpdate();
    },

    updateHotPosts() {
      this.hotPosts = this.allPosts
          .slice()
          .sort((a, b) => (b.likesCount + b.commentsCount) - (a.likesCount + a.commentsCount))
          .slice(0, 10);
    },

    truncatedContent(content) {
      return content.length > 30 ? content.slice(0, 30) + '...' : content;
    },
  },
};
</script>



<style scoped>
/* 添加置顶图标的样式 */
.top-icon {
  position: absolute;
  top: 10px;
  right: 10px;
  width: 30px;
  /* 根据实际图标大小调整 */
  height: 30px;
  z-index: 1;
}

/* 新增搜索框样式 */
.search-box {
  height: 50px;
  margin-bottom: 20px;
  width: 100%;
  font-size: 15px;
}

/* 其他样式保持不变 */
body {
  margin: 0;
  padding: 0;
  font-family: Arial, sans-serif;
}

.forum-bg {

  min-height: 100vh;
  position: absolute;
  top: 10vh;
  left: 0;
  background-image: url('../assets/images/forum-bg.jpg');
  background-size: cover;
  background-position: center;
  background-attachment: fixed;
  width: 100vw;
  justify-content: center;
}

.announcement-section {
  margin-top: 30px;
}

.announcement-content,
.activity-content,
.contest-content {
  text-align: left;
  margin: 20px;
}

/* 导航栏样式 */
.navbar {
  border-radius: 30px;
  margin-top: 2vh;
  background: transparent;
  background-color: rgba(255, 255, 255, 0.6);
  padding: 10px 0;
  position: absolute;
  width: 51.6%;
  /*z-index: 100;*/
  top: 0;
  transition: background-color 0.3s ease;
  border-bottom: 2px solid #ccc;
  /* 添加下方的横线 */
  display: flex;
  justify-content: space-between;
  /* 保证列表和按钮分布均匀 */
  align-items: center;
  padding-left: 20px;
}

.navbar-list {
  list-style: none;
  display: flex;
  gap: 25px;
  margin: 0;
  padding: 0;
  align-items: center;
}

.navbar-item {
  width: 105px;
  position: relative;
  color: black;
  cursor: pointer;
  padding: 0 0;
  transition: color 0.3s ease, transform 0.3s ease;
  text-align: center;
  font-size: 16px;
}

.navbar-item::after {
  content: '';
  position: absolute;
  left: 0;
  bottom: -2px;
  width: 0;
  height: 2px;
  background-color: #2575fc;
  transition: width 0.3s ease;
}

.navbar-item:hover {
  font-weight: bold;
  color: blue;
  transform: scale(1.1);
}

.navbar-item:hover::after {
  width: 100%;
}

.navbar-item.active {
  font-weight: bold;
  color: blue;
}

.navbar-item.active::after {
  width: 100%;
  background-color: blue;
}

.scroll-btn {
  background: none;
  border: none;
  cursor: pointer;
  font-size: 20px;
  color: black;
  display: flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  transition: background-color 0.3s ease;
}

.scroll-btn:hover {
  background-color: rgba(0, 0, 0, 0.1);
}

.forum-container {

  display: flex;
  justify-content: space-between;
  padding-top: 60px;
  padding-right: 30px;
  max-width: 100%;
  margin-top: 5vh;
  /* 在顶部留出导航栏的空间 */
  overflow-y: scroll;
}


.card {
  margin-left: 1%;
  width: 30%;
  height: fit-content;
  background-color: rgba(255, 255, 255, 0.6);
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
  margin-bottom: 10px;
}

.title {
  font-size: 20px;
  font-weight: bolder;
  color: #000;
  padding-left: 8px;
}

.text {
  font-size: 14px;
  color: #000;
  padding-left: 16px;
}

.tagLine {
  margin-left: 16px;
  margin-bottom: 10px;
}

.tag {
  margin-right: 5px;
  margin-bottom: 5px;
}

.row {
  display: flex;
  align-items: center;
  justify-content: center;
  margin-left: 20px;
}

.main-content {
  width: 1200px;
  padding-left: 20px;
  padding-right: 0;
}

.post-item {
  position: relative;
  background-color: rgba(255, 255, 255, 0.6);
  color: #000;
  padding: 20px;
  margin-bottom: 20px;
  border-radius: 5px;
  border: 2px solid #ddd;
}


.post-content {
  text-align: left;
}

.post-title {
  font-size: 18px;
  color: #007bff;
  margin-bottom: 10px;
  cursor: pointer;
}

.post-title .category-tag {
  background-color: #f0f0f0;
  border-radius: 50px;
  padding: 3px 8px;
  font-size: 12px;
  color: #555;
  margin-left: 10px;
}

.post-snippet {
  font-size: 16px;
  color: #666;
}

/*.post-footer {
      margin-top: 15px;
      display: flex;
      justify-content: space-between;
      align-items: center;
      font-size: 14px;
      color: #888;
  }*/
.post-footer {
  margin-top: 15px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  font-size: 14px;
  color: #888;
  gap: 20px;
}

.post-actions {
  color: blue;
  display: flex;
  gap: 15px;
  align-items: center;
}

.icon-with-text {
  display: flex;
  align-items: center;
  gap: 5px;
}

.icon-with-text.no-click {
  pointer-events: none;
  cursor: default;
}

.icon-with-text-delete {
  display: flex;
  align-items: center;
  gap: 5px;
  cursor: pointer;
}

.post-author {
  font-weight: bold;
}

.icon-fire-small {
  font-size: 15px !important;
  width: 15px !important;
  height: 15px !important;
  margin-right: 8px;
  display: inline-block;
  line-height: 1;
  /* 确保图标的高度不会因为行高影响 */
}

.right-sidebar {
  margin-left: 20px;
  width: 30%;
  height: fit-content;
  background-color: rgba(255, 255, 255, 0.6);
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.2);
  margin-bottom: 10px;
  padding-left: 0;
  display: flex;
  flex-direction: column;
  /*align-items: left;*/
}

.hot-posts-section {
  margin-top: 30px;
}

.hot-posts-content {
  padding-left: 16px;
  padding-right: 16px;
}

.hot-post-title {
  font-size: 14px;
  color: #007bff;
  cursor: pointer;
  display: block;
  margin-bottom: 8px;
  display: flex;
  text-align: left;
}

.hot-post-title:hover {
  text-decoration: underline;
}

.post-image {
  text-align: left;
  margin: 10px 0;
}

.post-image .image {
  width: 200px;
  /* 固定宽度 */
  height: auto;
  /* 高度自动调整以保持比例 */
  border-radius: 5px;
  display: inline-block;
}

.forum-backtop-button {
  position: fixed;
  bottom: 180px !important;
  right: 20px !important;
  z-index: 2;
  width: 60px !important;
  /* 增加按钮的宽度 */
  height: 60px !important;
  /* 增加按钮的高度 */
  display: flex;
  justify-content: center;
  align-items: center;
  transition: transform 0.5s ease;
  /* 添加缩放的过渡效果 */
}

</style>
