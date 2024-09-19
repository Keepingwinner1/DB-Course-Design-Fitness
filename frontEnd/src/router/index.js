import { createRouter, createWebHistory } from "vue-router";
import HomeView from "../views/HomeView.vue";
import LoginView from "../views/LoginView.vue";
import SignUpView from "../views/SignUpView.vue";
import CourseView from "../views/CourseView.vue";
import TweetsView from "../views/TweetsView.vue";
// import FitnessChart from './components/dataVis.vue'
import adminEquipment from "../components/adminEquipment.vue";
import AddDiet from "../components/AddDiet.vue";
import FitnessGuide from "../views/FitnessGuideView.vue";
import AdminEquipmentView from "../views/AdminEquipmentView.vue";
import MealRecordView from "../views/MealRecordView.vue";
import MealPlannerView from "../views/MealPlannerView.vue";
import HealthyDiet from "../views/HealthyDiet.vue";
import AddFoodView from "../views/AddFoodView.vue";
import Achievements from "../views/AchievementsView.vue";
import FitnessPlan from "../views/FitnessPlanView.vue";

import chatRoom from "../views/chatRoom.vue";
import ForumView from "../views/ForumView.vue";
import PostDetail from "../views/PostDetail.vue";
import UserProfile from "../views/UserProfile.vue";
import AdminView from "../views/AdminView.vue";
import UserHomePage from "../views/UserHomePage.vue";
import PublishCourse from "../views/PublishCourse.vue";
import RankingList from "../components/RankingList.vue";

import Try from "../views/Try.vue";

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: "/try",
      name: "try",
      component: Try,
    },
    //首页
    {
      //path: '/',
      path: "/",
      name: "no_login_home",
      component: HomeView,
      meta: { requiresAuth: false },
    },
    {
      //path: '/',
      path: "/home",
      name: "home",
      component: HomeView,
      meta: { requiresAuth: false },
    },
    //课程报名
    {
      path: "/course",
      name: "course",
      component: CourseView,
      meta: { requiresAuth: false },
    },
    //用户课程中心
    {
      path: "/userhome",
      name: "userhome",
      component: UserHomePage,
    },
    {
      path: "/publishcourse",
      name: "publishcourse",
      component: PublishCourse,
    },
    //健身器材
    {
      path: "/equipment",
      name: "tweets",
      component: TweetsView,
    },
    //ai健身
    {
      path: "/aifit",
      name: "FitnessGuide",
      component: FitnessGuide,
      meta: { requiresAuth: false },
    },
    //聊天室页面
    {
      path: "/chat",
      name: "chatRoom",
      component: chatRoom,
    },

    // // 用户信息界面
    // {
    //   path: '/user/:userID',
    //   //path: '/',
    //   name: 'UserProfile',
    //   component: UserProfile,
    //   props: true
    // },
    //成就页面
    {
      path: "/achievements",
      name: "achievements",
      component: Achievements,
      meta: { requiresAuth: false },
    },
    // 健身计划
    {
      path: "/fitnessPlan",
      name: "fitnessPlan",
      component: FitnessPlan,
      meta: { requiresAuth: false },
    },
    //饮食计划页面
    {
      path: "/mealPlanner",
      name: "MealPlanner",
      component: MealPlannerView,
      meta: { requiresAuth: false },
    },
    //饮食记录页面
    {
      path: "/mealRecord",
      name: "MealRecord",
      component: MealRecordView,
      meta: { requiresAuth: false },
    },
    //健康饮食页面
    {
      path: "/healthyDiet",
      name: "healtyhDiet",
      component: HealthyDiet,
      meta: { requiresAuth: false },
    },
    //增加食物部分————管理员功能
    {
      path: "/addFood",
      name: "AddFood",
      component: AddFoodView,
      meta: { requiresAuth: false },
    },
    // 登录界面
    {
      //path: '/login',
      path: "/login",
      name: "LoginView",
      component: LoginView,
    },
    // 注册界面
    {
      path: "/signup",
      name: "SignUpView",
      component: SignUpView,
    },
    // 论坛界面
    {
      path: "/forum",
      name: "ForumView",
      component: ForumView,
    },
    // 帖子细节界面
    {
      path: "/post/:postID",
      //path: '/',
      name: "PostDetail",
      component: PostDetail,
      props: true,
    },
    // 用户信息界面
    {
      path: "/user/:userID",
      //path: '/',
      name: "UserProfile",
      component: UserProfile,
      props: true,
    },
    // 管理员界面
    {
      path: "/admin",
      //path: '/',
      name: "AdminView",
      component: AdminView,
      props: true,
    },
    // 排行榜界面
    {
      path: "/ranking-list",
      name: "RankingList",
      component: RankingList,
      props: true,
    },

  ],
});

export default router;
