<template>
    <div class="container">
      <common-layout />
      <div class="navigation-bar">
         <navigation-bar />
      </div>
      <div class="title">
        <span>请选择您要使用的功能</span>
      </div>
      <div class="content">
        <el-button class="button1" type="info" circle @click="goToMealPlanner">
          <div class="btn-icon">
            <i class="el-icon-camera"></i>
          </div>
          <div class="btn-title">
            <span>饮食计划</span>
          </div>
          <div class="btn-content">
            <p>完善您的饮食计划</p>
            <p>查看我们给出的食谱</p>
          </div>
        </el-button>
        <el-button class="button2" type="info" circle @click="goToMealRecord">
          <div class="btn-icon">
            <i class="el-icon-video-camera"></i>
          </div>
          <div class="btn-title">
            <span>饮食记录</span>
          </div>
          <div class="btn-content">
            <p>记录您的每日饮食</p>
            <p>获得AI饮食建议</p>
          </div>
        </el-button>
      </div>
    </div>
  </template>
  
  <script>
  import {ElNotification} from "element-plus";
  import {useRouter} from "vue-router";
  import axios from "axios";
  import { commonMixin } from '../mixins/checkLoginState';
  export default {
    name: 'Tracking',
    components: {
    },
    mixins: [commonMixin],
    methods: {
      // 跳转到 MealPlannerView
      goToMealPlanner() {
        this.$router.push({ path: '/MealPlanner' });
      },
      // 跳转到 MealRecordView
      goToMealRecord() {
        this.$router.push({ path: '/MealRecord' });
      }
    },
    created() {
      this.checkAvailable()
    }
  }
  </script>
  
  <style scoped>
  .container {
    margin-top: 100px;
    width:150vh;
    height:85vh;
    background-color:#f2eadb;
    border-radius: 10px;
  }
  
  .container::after {
    z-index: -1;
    position: absolute;
    content: '';
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    opacity: 0.3;
  }
  
  .logo {
    margin-left: 3%;
    padding-top: 3%;
    width: 8%;
  }
  
  .logo img {
    width: 100%;
  }
  
  .title {
    margin-top: 1%;
    font-size: 3vw;
    color: #E86546;
    margin-left: 50px;
  }
  
  .title span {
    letter-spacing: 1vw;
  }
  
  .content {
    margin-top: 1%;
  }
  
  .content button {
    position: relative;
    width: 20%;
    height: 60vh;
    border-radius: 40px;
    transition: all 0.4s;
    margin: 1% 1%;
  }
  
  .button1 {
    background: url("../assets/images/healthyFoodBG1.png");
    background-size: 320px 630px;
  }
  
  .button2 {
    background: url("../assets/images/healthyFoodBG2.jpg");
    background-size: 320px 630px;
  }
  
  .content button:hover {
    width: 22%;
    height: 64vh;
    margin: 0 1%;
    transition: all 0.4s;
  }
  
  .content .btn-title {
    width: 100%;
    position: absolute;
    top: 10%;
    left: 0;
    height: 15%;
    font-size: 3.5vh;
    color:#000;
    text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.4); 
  }
  
  .content .btn-title span {
    letter-spacing: .5vw;
  }
  
  .content .btn-content {
    width: 100%;
    position: absolute;
    top: 20%;
    left: 0;
    font-size: 2vh;
    color:#F7F4ED;
    text-shadow: 2px 2px 4px rgba(0, 0, 0, 0.6); 
  }
  
  .content .btn-content p {
    margin-top: 5%;
  }
  </style>
  