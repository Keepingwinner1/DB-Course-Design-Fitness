//import { l } from 'vite/dist/node/types.d-aGj9QkWt';
import { createStore } from "vuex";
import axios from "axios";

// 在 store.js 中
export default createStore({
  state: {
    connection: null,
    role: localStorage.getItem("role") || "unAuthenticated",
    username: localStorage.getItem("username") || "",
    email: localStorage.getItem("email") || "",
    token: localStorage.getItem("token") || "",
    recipe: localStorage.getItem("recipe") || "",
    userID: localStorage.getItem("userID") || "",
    name: localStorage.getItem("name") || "",
    isPost: localStorage.getItem("isPost") || "",
    introduction: localStorage.getItem("introduction") || "",
    iconUrl:
      localStorage.getItem("iconUrl") || "../assets/images/default-avatar.png",
    // role: 'unAuthenticated',
    // username: '',
    // token: '',
    categories: [
      "全部帖子",
      "健身计划",
      "饮食营养",
      "健身打卡",
      "健身问答",
      "健身挑战",
      "设备器材",
      "健身分享",
      "活动赛事",
      "初学指南",
      "其他",
    ],
    unreadIDs: [], //未读ID

    targetInfomation: {
      id: "",
      img: "",
      name: "",
    },
    //用户列表
    userList: [], //只返回好友的ID，无其他字段，使用其他字段从userListInformation中获取
    //购物车数据
    cartCourses: [],
    //活力币
    vitalityCoins: 100,
    //存储用户课程表
    usercourses: [],
    //教练课程
    teachcourses: [],
    //用户列表的相关信息
    userListInformation: [],
    //聊天记录
    MessageList: [],
  },
  mutations: {
    //增加未读ID
    addUnreadID(state, id) {
      if (!state.unreadIDs.includes(id)) {
        state.unreadIDs.push(id);
      }
    },
    //购物车下单后添加课程到我的
    ADD_COURSES_TO_USER(state, newCourses) {
      state.usercourses = [...state.usercourses, ...newCourses];
    },
    //课程加入购物车
    ADD_COURSE_TO_CART(state, course) {
      state.cartCourses.push(course);
    },
    //购物车下单后从购物车除去
    REMOVE_COURSE_FROM_CART(state, index) {
      state.cartCourses.splice(index, 1);
    },
    //更新购物车
    UPDATE_CART(state, course) {
      state.cartCourses = course;
    },
    //更新用户课程表
    updateUserCourses(state, courses) {
      state.usercourses = courses;
    },
    //更新教练课程表
    updateTeachCourses(state, courses) {
      state.teachcourses = courses;
    },
    //删除教练课程
    DELETE_TEACH_COURSE(state, courseName) {
      state.teachcourses = state.teachcourses.filter(
        (course) => course.courseName !== courseName
      );
    },
    //增加教练课程
    ADD_TEACH_COURSE(state, newCourses) {
      state.teachcourses = [...state.teachcourses, ...newCourses];
      //state.teachcourses.push(newCourse);
    },
    setIsPost(state, isPost) {
      state.isPost = isPost;
      localStorage.setItem("isPost", isPost);
    },

    setUserID(state, userID) {
      state.userID = userID;
      localStorage.setItem("userID", userID);
    },
    setName(state, name) {
      state.name = name;
      localStorage.setItem("name", name);
    },
    setRole(state, role) {
      state.role = role;
      // 将数据保存到 LocalStorage
      localStorage.setItem("role", role);
    },
    setEmail(state, email) {
      state.email = email;
      localStorage.setItem("email", email);
    },
    setUsername(state, username) {
      state.username = username;
      localStorage.setItem("username", username);
    },
    setIconUrl(state, iconUrl) {
      state.iconUrl = iconUrl;
      localStorage.setItem("iconUrl", iconUrl);
    },
    setIntroduction(state, introduction) {
      state.introduction = introduction;
      localStorage.setItem("introduction", introduction);
    },
    setToken(state, token) {
      state.token = token;
      localStorage.setItem("token", token);
    },
    setRecipe(state, recipe) {
      state.recipe = recipe;
      localStorage.setItem("recipe", recipe);
    },
    removeRecipe(state, recipe) {
      state.recipe = undefined;
      localStorage.removeItem("recipe");
    },
    setTargetInformation(state, info) {
      state.targetInfomation = info;
      console.log("targetInfomation:", state.targetInfomation);
    },
    setUserList(state, userList) {
      state.userList = userList;
      localStorage.setItem("userList", userList);
      console.log("Updated Friend List:", state.userList);
    },
    setUserListInformation(state, userListInformation) {
      state.userListInformation = userListInformation;
      localStorage.setItem("userListInformation", userListInformation);
      console.log("User List Information Updated:", state.userListInformation);
    },
    addMessage(state, message) {
      const targetName = message.targetName;
      const targetId = message.targetId;
      const target = state.MessageList.find(
        (item) => item.targetName === targetName
      );

      if (target) {
        target.list.push({
          is_me: message.list.is_me,
          time: message.list.time,
          message: message.list.message,
          messageType: message.list.messageType,
        });
        console.log(message.list.message);
      } else {
        // 如果没有找到匹配的 targetName，可以选择添加新的对象
        if (targetName) {
          state.MessageList.push({
            targetName: targetName,
            list: [
              {
                is_me: message.list.is_me,
                time: message.list.time,
                message: message.list.message,
                messageType: message.list.messageType,
              },
            ],
          });
        }
      }
      localStorage.setItem("MessageList", state.MessageList);
      console.log("Updated Message List:", state.MessageList);
    },
  },
  actions: {
    //上传用户课程表
    updateUserCourses({ commit }, courses) {
      commit("updateUserCourses", courses);
    },
    //上传教练课程表
    updateTeachCourses({ commit }, courses) {
      commit("updateTeachCourses", courses);
    },
    //删除
    deleteTeachCourse({ commit }, courseName) {
      commit("DELETE_TEACH_COURSE", courseName);
    },
    //添加
    addTeachCourse({ commit }, newCourse) {
      commit("ADD_TEACH_COURSE", newCourse);
    },
    pollIsPost({ commit, state }) {
      setInterval(async () => {
        try {
          const response = await axios.get(
            `http://localhost:8080/api/User/GetPersonalProfile?token=${state.token}`
          );
          const newIsPost = response.data.isPost;
          console.log("isPost", newIsPost);
          if (newIsPost !== state.isPost) {
            commit("setIsPost", newIsPost);
          }
        } catch (error) {
          console.error("Error polling isPost status:", error);
        }
      }, 50000000); // 每5秒检查一次
    },
  },
  modules: {
    // 在这里可以添加模块pollIsPost
  },
  getters: {
    getUserCourses: (state) => state.usercourses,
    getTeachCourses: (state) => state.teachcourses,
  },
});
