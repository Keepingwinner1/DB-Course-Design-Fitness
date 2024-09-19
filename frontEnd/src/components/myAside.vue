<template>
  <div style="height: 550px; overflow-y: auto;">
    <el-row style="height:40px">
      <el-input  v-model="query"  placeholder="搜索好友"  @keyup.enter="search"  clearable>
        <template #append>
          <el-button  @click="dialogVisible = true">
            <template #icon>
              <Plus />
            </template> 
          </el-button>
        </template>
      </el-input>
    </el-row>
    
    <!-- 搜索弹窗 -->
    <el-dialog
      v-model="dialogVisible"
      width="50%"
      :modal="false"
      @close="handleClose">
      
      <!-- 搜索框 -->
      <el-input
        v-model="searchQuery"
        placeholder="请输入用户名并按下回车键进行搜索"
        @keyup.enter="performSearch">
        <template #prefix>
          <el-icon><Search /></el-icon>
        </template>
      </el-input>

      <el-row >
        <el-table  :data="searchResults" :show-header="false"  style="width: 100%;">
          <el-table-column > 
            <template v-slot="scope">
              <div style="display: flex; align-items: center;">
                <el-avatar :src=scope.row.iconURL />
                <span style="margin-left: 8px;">{{ scope.row.userName }}</span>
              </div>
            </template>
          </el-table-column>
          <!-- 添加按钮的列 -->
          <el-table-column align="right" >
            <template v-slot="scope">
              <el-button type="primary" @click="handleAdd(scope.row)">添加</el-button>
            </template>
          </el-table-column>
        </el-table>
      </el-row>

      <!-- 底部按钮 -->
      <span slot="footer" class="dialog-footer">
        <el-button @click="handleCancel">取消</el-button>
      </span>
    </el-dialog>

    <el-row >
      <el-table @row-click="handleClick" :data="filteredtest" :show-header="false"   highlight-current-row  style="width: 100%;">
        <el-table-column > 
          <template v-slot="scope">
            <div style="display: flex; align-items: center;">                
              <el-avatar :src="scope.row.img" />
              <span v-if="hasUnreadMessages(scope.row.id)"
                    style="width: 6px; height: 6px; background-color: red; border-radius: 50%; margin-left: 4px; margin-top: -2px;">
              </span>

              <span style="margin-left: 8px;">{{ scope.row.name }}</span>
              
            </div>
          </template>
        </el-table-column>
      </el-table>
    </el-row>
  </div>
</template>


<script>
import { Position, Search } from '@element-plus/icons-vue'
import { ElTable, ElTableColumn } from 'element-plus'
import store from "../store/index.js";
import { Plus } from '@element-plus/icons-vue';
import axios from 'axios';
import { ElNotification } from 'element-plus';

// import userImage from '@/assets/logo.png';

export default {
  name: 'myAside',

  components: {
  Search,
  ElTable,
  ElTableColumn,
},

  data() {
  return {
    dialogVisible: false, // 控制弹窗显示的状态
    query: '',//搜索已有好友
    searchQuery:'',//搜索聊天并进行添加
    searchResults: [], // 搜索结果列表
    // test: [
    //   {
    //     id:1,
    //     name: '同济',
    //     img: userImage
    //   },
    //   {
    //     id:2,
    //     name: '大学',
    //     img: userImage
    //   },
    //   {
    //     id:2,
    //     name: '大学',
    //     img: userImage
    //   },
    //   {
    //     id:3,
    //     name: '学院',
    //     img: userImage
    //   }
    // ]
  };
},

  computed: {
    users(){
      return store.state.userListInformation;//用户列表的信息
    },
    filteredtest() {
      const query = this.query.toLowerCase();
      const filtered = this.users.filter(data => !query || data.name.toLowerCase().includes(query));
      console.log('Filtered data count:', filtered.length);
      return filtered;
    }
  },

  methods: {
    
    hasUnreadMessages(id){
      if (!store.state.unreadIDs.includes(id)) {
          return false;
        }
        return true;
    },

    async performSearch() {
      // 检查 searchQuery 是否为空，为空则不需要调用后端API
      if (!this.searchQuery.trim()) {
        // searchQuery 为空，直接返回，退出函数
        console.log('searchQuery 为空，函数提前返回');
        return;
      }

      try {
        //调用后端搜索API，返回对应的搜索结果
        const token = localStorage.getItem('token');

        const response = await axios.get('http://localhost:8080/api/User/SearchUserByName', {
          params: {
            token: token, // 将 Token 作为查询参数 
            username: this.searchQuery //用户名称
          }
        });

        // 处理返回的搜索结果
        this.searchResults = response.data; 
        console.log(response);
        if (response.data === '') {
          ElNotification({
            title: '提示',
            message: '并未查找到指定用户',
            type: 'info',
            position:'top-left',
            zIndex:10000003
          });
        }
      } catch (error) {
        console.error('搜索请求失败:', error);
      }
    },

    async  getFriendInformation() {
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
            img: userInfo.iconURL , // 使用返回的 iconURL 或原始 img
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
        this.getChatHistory();
      } catch (error) {
        console.error('Error fetching user information:', error);
      }
    },

    async handleAdd(row) {
      this.dialogVisible = false; // 关闭弹窗
      // 清空 searchQuery 和 searchResults
      this.searchQuery = '';
      this.searchResults = [];

      //判断要添加的好友是否为自己的好友
      const foundUser = store.state.userList.find(user => user=== row.userID);
      // console.log(store.state.userList);
      // console.log(row.userID);
      if (foundUser) {
        ElNotification({
            title: '提示',
            message: `该用户已为您的好友`,
            type: 'info',
            position:'top-left',
            zIndex:10000003
        });
        return;
      }

      //调用后端添加好好友的API
      try {
        //调用后端搜索API，返回对应的搜索结果
        const token = localStorage.getItem('token');
        if (row.userID===parseInt(localStorage.getItem('userID'))){
          ElNotification({
            title: '失败',
            message: `无法添加自己为好友`,
            type: 'error',
            position:'top-left',
            zIndex:10000003
          });
          return ;
        }
        const response = await axios.get('http://localhost:8080/api/User/AddFriend', {
          params:{
            token: token, // 将 Token 和 friendID 作为请求体数据
            friendID: row.userID // 要添加朋友的ID
          }
        });

        console.log('返回响应为:',response.data);
        if (response.data === '添加好友成功') {
          // 添加成功
          ElNotification({
            title: '成功',
            message: '好友添加成功！',
            type: 'success',
            position:'top-left',
            zIndex:10000003
          });
        } else {
          // 处理其他状态码
          ElNotification({
            title: '失败',
            message: `添加好友失败`,
            type: 'error',
            position:'top-left',
            zIndex:10000003
          });
        }
        // 处理返回的搜索结果,重新调用API进行渲染好友列表
        this.getFriendInformation();

      } catch (error) {
        console.error('添加好友失败:', error);
        ElNotification({
          title: '失败',
          message: `添加好友失败`,
          type: 'error',
          position:'top-left',
          zIndex:10000003
        });
      }
    },
    handleCancel() {
      this.dialogVisible = false; // 关闭弹窗
      // 清空 searchQuery 和 searchResults
      this.searchQuery = '';
      this.searchResults = [];
    },
    handleClose() {
      // 清空 searchQuery 和 searchResults
      this.searchQuery = '';
      this.searchResults = [];
    },

    search() {
      console.log('搜索内容:', this.query);
      // 搜索逻辑
    },
    handleClick(row) {
      console.log('Row clicked:', row);
      // 在这里处理点击事件
      store.state.unreadIDs = store.state.unreadIDs.filter(unreadID => unreadID !== row.id);
      store.commit('setTargetInformation', { id:row.id,img: row.img, name: row.name});
    }
}
}
</script>

<style scoped>


.custom-content {
width: 150px; /* 设置固定宽度 */
max-width: 200px; /* 设置最大宽度 */
padding: 10px; /* 设置内边距 */
height:200px;
/* position: relative; 使子元素能够相对于此容器进行绝对定位 */
/* display: inline-block; 保持元素在同一行 */
}
.custom-avatar {
transform: translate(-120%, 0%);
width: 50px; /* el-avatar 的宽度 */
height: 50px; /* el-avatar 的高度 */

}
.name-label {
position: absolute; /* 绝对定位 */
top: 20px; /* 距离顶部 0 */
left: 60%; 
transform: translate(-50%, -50%);
white-space: nowrap; /* 防止文本换行 */
font-size: 18px; /* 设置字体大小 */
}
.ID-label {
position: absolute; /* 绝对定位 */
top: 50px; /* 距离顶部 0 */
left: 60%; 
transform: translate(-50%, -50%);
white-space: nowrap; 
font-size: 10px; /* 设置字体大小 */
}

</style>
