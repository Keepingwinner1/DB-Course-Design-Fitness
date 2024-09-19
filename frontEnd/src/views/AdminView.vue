<template>
  <div class="background">
    <div class="common-layout">
      <el-container>
        <el-header>
          <img src="../assets/images/logo.png" alt="FitFit" class="logo" />
          <span>FitFit</span>
          <div class="user">
            <el-dropdown>
              <img :src="iconUrl" alt="User" class="dropdownlink" />
              <template #dropdown>
                <el-dropdown-menu>
                  <el-dropdown-item @click="goPage('/home')">
                    <el-icon>
                      <House />
                    </el-icon>
                    首页
                  </el-dropdown-item>
                  <el-dropdown-item @click="openUser(userID)">
                    <el-icon>
                      <UserFilled />
                    </el-icon>
                    个人资料
                  </el-dropdown-item>
                  <el-dropdown-item @click="navigateToLoginOut">
                    <el-icon>
                      <SwitchButton />
                    </el-icon>
                    退出登录
                  </el-dropdown-item>
                </el-dropdown-menu>
              </template>
            </el-dropdown>
          </div>
        </el-header>
        <el-container>
          <el-aside width="200px" :style="{ height: 'calc(100vh - 50px)' }">
            <el-menu default-active="1" class="el-menu-vertical-demo" @open="handleOpen"
                     @close="handleClose">
              <el-menu-item index="1" @click="active = 1">
                <el-icon>
                  <IconMenu />
                </el-icon>
                <span>用户管理</span>
              </el-menu-item>
              <el-menu-item index="2" @click="active = 2">
                <el-icon>
                  <Setting />
                </el-icon>
                <span>内容管理</span>
              </el-menu-item>
              <el-menu-item index="3" @click="active = 3">
                <el-icon>
                  <Dish />
                </el-icon>
                <span>饮食管理</span>
              </el-menu-item>
              <el-menu-item index="4" @click="active = 4">
                <el-icon>
                  <Basketball />
                </el-icon>
                <span>器械管理</span>
              </el-menu-item>
            </el-menu>
          </el-aside>
          <el-main :style="{ height: 'calc(100vh - 50px)' }">
            <!-- 用户管理界面 -->
            <div v-if="active == 1">
              <el-input v-model="searchQuery" placeholder="搜索用户ID或用户名" clearable class="search-box" />
              <el-table :data="filteredUsers" style="width: 100%">
                <el-table-column prop="email" label="邮箱地址"></el-table-column>
                <el-table-column prop="userName" label="用户名"></el-table-column>
                <el-table-column prop="userID" label="用户ID"></el-table-column>
                <el-table-column prop="registrationTime" label="注册时间">
                  <template #default="{ row }">
                    {{ formatDate(row.registrationTime) }}
                  </template>
                </el-table-column>
                <el-table-column prop="isMember" label="是否会员">
                  <template #default="{ row }">
                    {{ row.isMember === 1 ? '是' : '否' }}
                  </template>
                </el-table-column>

                <el-table-column prop="status" label="状态">
                  <template #default="{ row }">
                    <el-tag :type="getTagType(row)" :style="getTagStyle(row)">{{ row.status
                      }}</el-tag>
                  </template>
                </el-table-column>
                <el-table-column fixed="right" label="操作" width="250">
                  <template #default="{ row }">
                    <el-button size="small" :type="row.status === '禁言' ? 'primary' : 'danger'"
                               @click="row.status === '正常' ? restrictUser(row) : cancelBanUser(row)"
                               :disabled="row.status === '删除'">
                      {{ row.status === '禁言' ? '取消禁言' : '限制言论' }}
                    </el-button>
                    <el-button size="small" type="warning" @click="deactivateUser(row)"
                               :disabled="row.status === '删除'">
                      删除用户
                    </el-button>
                  </template>
                </el-table-column>

              </el-table>
            </div>


            <!-- 内容管理界面 -->
            <div v-if="active == 2">
              <el-table :data="contentList" style="width: 100%">
                <el-table-column prop="postTitle" label="标题"></el-table-column>
                <el-table-column prop="userName" label="作者"></el-table-column>
                <el-table-column prop="postCategory" label="类型">
                  <template #default="{ row }">
                    <el-tag :type="row.postID ? 'info' : 'warning'">{{ row.postID ? '帖子' : '评论' }}</el-tag>
                  </template>
                </el-table-column>
                <el-table-column prop="postTime" label="发布时间">
                  <template #default="{ row }">
                    {{ formatDate(row.postTime) }}
                  </template>
                </el-table-column>
                <el-table-column prop="status" label="状态">
                  <template #default="{ row }">
                    <el-tag :type="getTagType(row)" :style="getTagStyle(row)">{{ row.status }}</el-tag>
                  </template>
                </el-table-column>
                <el-table-column fixed="right" label="操作" width="250">
                  <template #default="{ row }">
                    <el-button size="small" type="danger" @click="deleteContent(row)"
                               :disabled="row.status === '删除'">删除</el-button>
                    <el-button size="small" @click="navigateToPost(row)"
                               :disabled="row.status === '删除'">查看评论</el-button>
                    <el-button size="small" type="primary" @click="cancelReport(row)"
                               :disabled="row.isReported === 0 || row.status === '删除'">取消举报</el-button>
                  </template>
                </el-table-column>
              </el-table>
            </div>


            <!--饮食管理界面-->
            <div v-if="active === 3">
              <el-row class="container" justify="space-between">
                <el-col :span="10">
                  <AddFood />
                </el-col>
                <el-col :span="12">
                  <AddDiet />
                </el-col>
              </el-row>
            </div>
            <div v-if="active === 4">
              <adminEquipment />
            </div>
          </el-main>
        </el-container>
      </el-container>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import store from '../store/index.js';
import { useRouter } from "vue-router";
import axios from 'axios';
import { ElNotification } from 'element-plus';
import { House, UserFilled, Setting, SwitchButton } from '@element-plus/icons-vue';
import { IconMenu } from '@arco-design/web-vue/es/icon';
import AddFood from '../components/AddFood.vue';
import AddDiet from '../components/AddDiet.vue';
import adminEquipment from "../components/adminEquipment.vue"
import create from '@ant-design/icons-vue/lib/components/IconFont';

const router = useRouter();
let active = ref(1);
let searchQuery = ref('');
let dialogVisible = ref(false);
let article = ref({
  content: '',
  title: '',
  comments: []
});
checkAvailable()
// 用户信息数据结构
const users = ref([]);
const iconUrl = store.state.iconUrl
// 帖子数据结构
const contentList = ref([]);

// 过滤用户数据
const filteredUsers = computed(() => {
  if (searchQuery.value === '') {
    return users.value;
  }
  return users.value.filter(user => user.userName.includes(searchQuery.value) || String(user.userID).includes(searchQuery.value));
});

// 获取用户信息
async function fetchUsers() {
  try {
    const token = localStorage.getItem('token');
    const response = await axios.get(`http://localhost:8080/api/User/GetAllUser?token=${token}`);
    users.value = response.data;
    users.value.sort((a, b) => new Date(a.registrationTime) - new Date(b.registrationTime));

    users.value.forEach(user => {
      if (user.isDelete === 1) {
        user.status = '删除';
      } else if (user.isPost === 0) {
        user.status = '禁言';
      } else {
        user.status = '正常';
      }
    });
  } catch (error) {
    ElNotification({
      title: '错误',
      message: '获取用户信息失败，请稍后再试。',
      type: 'error',
    });
  }
}

// 获取帖子信息
async function fetchPosts() {
  try {
    const token = localStorage.getItem('token');
    const response = await axios.get(`http://localhost:8080/api/Post/GetAllPost?token=${token}`);
    contentList.value = response.data;
    console.log(contentList.value);
    // 判断帖子是否被举报
    contentList.value.forEach(post => {
      console.log(post.isReported)
      if (post.isReported === 1) {
        post.status = '被举报';
      } else {
        post.status = '正常';
      }
    });

    // 按照发布时间从小到大排序
    contentList.value.sort((a, b) => new Date(a.postTime) - new Date(b.postTime));
  } catch (error) {
    ElNotification({
      title: '错误',
      message: '获取帖子信息失败，请稍后再试。',
      type: 'error',
    });
  }
}

// 取消举报操作
async function cancelReport(post) {
  try {
    const response = await axios.get('http://localhost:8080/api/Post/CancleReportPost', {
      params: {
        token: localStorage.getItem('token'),
        postID: post.postID,
      }
    });
    if (response.data.message === '成功取消举报') {
      post.isReported = 0;
      post.status = '正常';
      ElNotification({
        title: '成功',
        message: `已成功取消举报该帖子：${post.postTitle}`,
        type: 'success',
      });
    }
  } catch (error) {
    ElNotification({
      title: '错误',
      message: '取消举报失败，请稍后再试。',
      type: 'error',
    });
  }
}



// 获取标签样式
function getTagStyle(row) {
  if (row.status === '正常') return { backgroundColor: 'green', color: 'white' };
  if (row.status === '被举报') return { backgroundColor: 'orange', color: 'white' };
  if (row.status === '删除') return { backgroundColor: 'red', color: 'white' };
  return {};
}

// 获取标签类型
function getTagType(row) {
  if (row.status === '正常') return 'success';
  if (row.status === '被举报') return 'warning';
  if (row.status === '删除') return 'danger';
  return 'info';
}


function checkAvailable() {
  let token = localStorage.getItem('token');
  if (token == null) {
    ElNotification({
      title: '提示',
      message: '请先登录',
      type: 'warning',
      duration: 2000
    })
    const router = useRouter()
    router.push('/login')
    return;
  };
  axios.get(`http://localhost:8080/api/User/GetTokenInvalidateRes`, {
    params: {
      token: token,
    }
  }).then(response => {
    console.log("登录状态:", response.data);
    if (!response.data) {
      ElNotification({
        title: '提示',
        message: '登录已过期，请重新登录',
        type: 'warning',
        duration: 2000
      });
      localStorage.removeItem('token');
      this.router().push('/login');
    }
  }).catch(error => {
    ElNotification({
      title: '错误',
      message: '获取用户信息失败',
      type: 'error',
    });
  });
}

// 页面加载时获取数据
onMounted(() => {
  fetchUsers();
  fetchPosts();
});

// 格式化日期
function formatDate(date) {
  const d = new Date(date);
  const year = d.getFullYear();
  const month = String(d.getMonth() + 1).padStart(2, '0');
  const day = String(d.getDate()).padStart(2, '0');
  const hours = String(d.getHours()).padStart(2, '0');
  const minutes = String(d.getMinutes()).padStart(2, '0');
  const seconds = String(d.getSeconds()).padStart(2, '0');
  return `${year}-${month}-${day} ${hours}:${minutes}:${seconds}`;
}

// 用户管理操作
async function restrictUser(user) {
  try {
    const response = await axios.get('http://localhost:8080/api/User/BanUser', {
      params: {
        token: localStorage.getItem('token'),
        userID: user.userID,
      }
    });
    if (response.data === '禁言成功') {
      user.status = '禁言';
      ElNotification({
        title: '成功',
        message: `用户 ${user.userName} 已成功禁言。`,
        type: 'success',
      });
    }
  } catch (error) {
    ElNotification({
      title: '错误',
      message: '限制用户言论失败，请稍后再试。',
      type: 'error',
    });
  }
}

async function cancelBanUser(user) {
  try {
    const response = await axios.get('http://localhost:8080/api/User/CancelBanUser', {
      params: {
        token: localStorage.getItem('token'),
        userID: user.userID,
      }
    });
    if (response.data === '取消禁言成功') {
      user.status = '正常';
      ElNotification({
        title: '成功',
        message: `用户 ${user.userName} 已成功取消禁言。`,
        type: 'success',
      });
    }
  } catch (error) {
    ElNotification({
      title: '错误',
      message: '取消禁言失败，请稍后再试。',
      type: 'error',
    });
  }
}

async function deactivateUser(user) {
  try {
    const response = await axios.get('http://localhost:8080/api/User/RemoveUser', {
      params: {
        token: localStorage.getItem('token'),
        userID: user.userID,
      }
    });
    if (response.data === '删除成功') {
      user.status = '删除';
      ElNotification({
        title: '成功',
        message: `用户 ${user.userName} 已成功删除。`,
        type: 'success',
      });
    }
  } catch (error) {
    ElNotification({
      title: '错误',
      message: '删除用户失败，请稍后再试。',
      type: 'error',
    });
  }
}

function navigateToLoginOut() {
  goPage('/login')

  localStorage.removeItem('token')
}

// 内容管理操作
async function deleteContent(content) {
  try {
    let response;
    if (content.postID) {
      response = await axios.delete('http://localhost:8080/api/Post/DeletePostByPostID', {
        params: {
          token: localStorage.getItem('token'),
          postID: content.postID,
          postOwnerID: content.userID
        }
      });
      if (response.data.message === '删除帖子成功') {
        content.status = '删除';
        ElNotification({
          title: '成功',
          message: `帖子 ${content.postTitle} 已成功删除。`,
          type: 'success',
        });
      }
    } else if (content.commentID) {
      response = await axios.delete('http://localhost:8080/api/Comment/DeleteComment', {
        params: {
          token: localStorage.getItem('token'),
          commentID: content.commentID,
        }
      })
      if (response.data === '评论删除成功') {
        content.status = '删除';
        ElNotification({
          title: '成功',
          message: '评论已成功删除。',
          type: 'success',
        });
      }
    }
  } catch (error) {
    ElNotification({
      title: '错误',
      message: '删除内容失败，请稍后再试。',
      type: 'error',
    });
  }
}

// 跳转到帖子操作
function navigateToPost(content) {
  if (content.postID) {
    router.push({
      path: `/post/${content.postID}`,
    });
  }
}

// 导航操作
const handleOpen = (key, keyPath) => {
  console.log(key, keyPath);
};

const handleClose = (key, keyPath) => {
  console.log(key, keyPath);
};

const openUser = (userID) => {
  userID = userID || store.state.userID;
  router.push({
    path: `/user/${userID}`,
  });
};

const goPage = (path) => {
  router.push({
    path,
  });
};
</script>

<style>
#app {
  max-width: 100% !important;
  margin: 0;
  padding: 0;
}

.background {
  background-image: url('../assets/images/forum-bg.jpg');
  background-size: cover;
  background-position: center;
  width: 100%;
  min-height: 101%;
  position: absolute;
  top: 0;
  left: 0;
}

.el-container,
.el-menu {
  height: 90vh;
  margin: 0;
  padding: 0;
}

.el-header {
  position: relative;
  display: flex;
  justify-content: flex-start;
  align-items: center;
  box-shadow: 2px 2px 2px #ccc;
}

.el-header span {
  font-size: 24px;
  margin-left: 10px;
  font-weight: 700;
}

.logo {
  width: 60px;
  /* 调整logo尺寸 */
  height: auto;
}

.dropdownlink {
  width: 50px;
  /* 调整用户头像尺寸 */
  height: 50px;
  border-radius: 50%;
}

.user {
  position: absolute;
  right: 2%;
}

.search-box {
  margin-bottom: 10px;
}

.el-scrollbar__view {
  display: block !important;
}
</style>
