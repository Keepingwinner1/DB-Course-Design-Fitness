<template>
  <common-layout />
  <div class="aaa">
    <div aria-label="A complete example of page header">
      <div class="kkk">
        <el-card class="vitality-card" shadow="hover">
          <div class="vitality-container">
            <el-icon>
              <Coin />
            </el-icon>
            <span>活力币：</span>
            <span>{{ vitalityCoins }}</span>
          </div>
        </el-card>
        <CartSidebar
          :cartCourses="cartCourses"
          :isCartVisible="isCartVisible"
          :usercourses="usercourses"
          @update:isCartVisible="handleCartVisibility"
          @removeCourse="removeCourseFromCart"
          @update:usercourses="updateUserCourses"
          class="cart"
        />
      </div>
      <el-page-header @back="onBack">
        <template #breadcrumb>
          <el-breadcrumb separator="/">
            <el-breadcrumb-item :to="{ path: './page-header.html' }">
              FitFit
            </el-breadcrumb-item>
            <el-breadcrumb-item :to="{ name: 'course' }">
              健身课程
            </el-breadcrumb-item>
            <el-breadcrumb-item>我的课程</el-breadcrumb-item>
          </el-breadcrumb>
        </template>
        <template #content>
          <div class="flexitems-center">
            <el-avatar class="mr-3" :size="32" :src="userIcon" />
            <span>{{ userName }}&nbsp;&nbsp;</span>
            <span style="font-size: 16px; color: var(--el-text-color-regular)">{{ email }}&nbsp;</span>
            <el-tag>欢迎您~</el-tag>
          </div>
        </template>
        <div class="poem-display">
          <div class="poem-content">
            <span class="poem-text" v-if="selectedPoem">
              {{ selectedPoem.text }}
            </span>
          </div>
          <div class="author-container" v-if="selectedPoem">
            <span class="author-title">
              {{ selectedPoem.author }} - {{ selectedPoem.title }}
            </span>
          </div>
        </div>
      </el-page-header>
    </div>
    <div class="empty-row"></div>

    <div class="course-calender">
      <div class="my-course-title">我的课程</div>
      <div class="coursee-list">
        <template v-if="usercourses.length > 0">
          <CourseCard
            v-for="(usercourse, index) in usercourses"
            :key="index"
            :thecourse="usercourse"
          />
        </template>
        <template v-else>
          <div class="empty-course-message">
            您还没有课程哟，快去课程大厅预约后，点击我的购物车下单吧
          </div>
        </template>
      </div>
      <div class="course-list">
        <h2>您今日的课程列表</h2>
        <el-row
          v-for="(course, index) in personalCourseList"
          :key="index"
          class="course-item"
        >
          <div class="course-item">
            <div
              :class="['circle', circleColors[index % circleColors.length]]"
            ></div>
            <div class="course-name">
              {{ truncateCourseName(course.courseName) }}
            </div>
            <div class="course-time">{{ course.classTime }}</div>
            <div class="status-box">
              <el-tag :type="getStatusType(course)">
                {{ getStatusText(course) }}
              </el-tag>
            </div>
          </div>
        </el-row>
      </div>
    </div>

    <div class="chart-container">
      <h2>我的活跃度</h2>
      <div class="chart-content">
        <div id="line-chart" ref="lineChart" class="chart"></div>
        <div class="stage-info">
          <h3>称号：{{ currentStageName }}</h3>
          <p>{{ currentStageDescription }}</p>
          <p>总健身时间: {{ totalWorkoutHours }} 小时</p>
          <p>下一阶段需要: {{ nextStageUnlockHours }} 小时</p>
        </div>
      </div>
      <p class="encouragement">{{ encouragementMessage }}</p>
    </div>
  </div>
</template>

<script>
import * as echarts from "echarts";
import CourseCard from "../components/CourseCard.vue";
import CartSidebar from "../components/CartSidebar.vue";
import { ElMessage, ElMessageBox } from "element-plus";
import { mapGetters, mapActions } from "vuex";
import { mapState } from "vuex";
import axios from "axios";

export default {
  components: {
    CourseCard,
    CartSidebar,
  },

  data() {
    return {
      personalCourseList: [], //存储今日课程数组
      userAllcourses: [], //储存所有用户参与课程
      userName: "", //用户名
      userIcon: "",
      email: "",
      vitalityCoins: 0,
      courseData: [
        { date: "2024-08-01", duration: 1.5 },
        { date: "2024-08-02", duration: 2 },
        { date: "2024-08-03", duration: 0.5 },
        { date: "2024-08-04", duration: 1 },
        { date: "2024-08-05", duration: 2.5 },
        { date: "2024-08-06", duration: 1.5 },
        { date: "2024-08-07", duration: 2 },
        { date: "2024-08-08", duration: 0.5 },
        { date: "2024-08-09", duration: 1 },
        { date: "2024-08-10", duration: 2.5 },
      ],
      encouragementMessage: "",
      totalWorkoutHours: 0,
      stages: [
        {
          name: "武韵新秀",
          unlockHours: 0,
          description: "入门之旅，强身健体的第一步。",
        },
        {
          name: "气功精进",
          unlockHours: 30,
          description: "初窥门径，气功渐入佳境。",
        },
        {
          name: "鹤舞长空",
          unlockHours: 80,
          description: "身轻如燕，身姿矫健如鹤。",
        },
        {
          name: "金刚不坏",
          unlockHours: 150,
          description: "强筋健骨，体魄如钢。",
        },
        {
          name: "神行百变",
          unlockHours: 250,
          description: "速度与敏捷的完美结合。",
        },
        {
          name: "易筋洗髓",
          unlockHours: 400,
          description: "内外兼修，境界升华。",
        },
        {
          name: "大宗师",
          unlockHours: 600,
          description: "登峰造极，出神入化。",
        },
      ],
      // 古诗数据
      poems: [
        {
          text: "蹴鞠屡过飞鸟上，秋千竞出垂杨里。 少年分日作遨游，不用清明兼上巳。",
          author: "王维",
          title: "《寒食城东即事》",
        },
        {
          text: "十月冰床遍九城，游人曳去一毛轻。风和日暖时端坐，疑在琉璃世界行。",
          author: "杨静亭",
          title: "《都门杂咏》",
        },
        {
          text: "少年骑马入咸阳，鹘似身轻蝶似狂。蹴鞠场边万人看，秋千旗下一春忙。",
          author: "陆游",
          title: "《晚春感事》",
        },
        {
          text: "黄沙连海路无尘，边草长枯不见春。日暮拂云堆下过，马前逢著射雕人。",
          author: "杜牧",
          title: "《游边》",
        },
        {
          text: "㸌如羿射九日落，矫如群帝骖龙翔。来如雷霆收震怒，罢如江海凝清光。",
          author: "杜甫",
          title: "《观公孙大娘弟子舞剑器行》",
        },
      ],
      selectedPoem: null,
      circleColors: ["blue", "yellow", "red", "orange", "green"],
      isCartVisible: false,
    };
  },

  mounted() {
    this.selectRandomPoem();
    this.initChart();
    this.generateEncouragementMessage();

    //页面启动的时候,需要调用的所有API
    this.fetchUserCourse();
    this.fetchTodayCourseList();
    this.getReserved();
    this.userName = localStorage.getItem("name");
    this.getVigorTokenBalance();
    this.email = localStorage.getItem("email");
    this.userIcon = localStorage.getItem("iconUrl");
  },

  methods: {
    //-------------------------------------- 页面静态函数-----------------------------------------------------
    //每日课程列表的名字长度问题
    truncateCourseName(name) {
      if (name.length > 5) {
        return name.slice(0, 5) + "...";
      } else {
        return name;
      }
    },

    //回退<-back
    onBack() {
      this.$router.go(-1);
    },

    //随机生成
    selectRandomPoem() {
      const index = Math.floor(Math.random() * this.poems.length);
      this.selectedPoem = this.poems[index];
    },

    //初始化图
    initChart() {
      const chart = echarts.init(this.$refs.lineChart);

      const option = {
        title: {
          text: "",
          left: "center",
        },
        xAxis: {
          type: "category",
          data: this.courseData.map((item) => item.date),
          boundaryGap: false,
        },
        yAxis: {
          type: "value",
          name: "每日课程在线时间 (小时)",
        },
        series: [
          {
            data: this.courseData.map((item) => item.duration),
            type: "line",
            smooth: true,
            areaStyle: {},
            lineStyle: {
              color: "#409EFF",
            },
            itemStyle: {
              color: "#409EFF",
            },
          },
        ],
      };
      chart.setOption(option);
    },

    //鼓励话语生成函数
    generateEncouragementMessage() {
      this.totalWorkoutHours = this.courseData.reduce(
        (sum, item) => sum + item.duration,
        0
      );
      if (this.totalWorkoutHours > 1000) {
        this.encouragementMessage =
          "太棒拉！你已经是一位业余健身的天花板了，期待你的自律与积极的精神去引领更多的人参与健身，继续加油！";
      } else if (this.totalWorkoutHours > 500) {
        this.encouragementMessage =
          "不错哦！转眼间，轻舟已过万重山，保持这个节奏，你会越来越棒！";
      } else {
        this.encouragementMessage =
          "开始锻炼总是最难的，坚持下去，你会看到改变！";
      }
    },

    //获取今日课程状态
    getStatusText(course) {
      const currentTime = new Date();
      const [startHour, startMinute] = course.classTime
        .split(" - ")[0]
        .split(":")
        .map(Number);
      const startTime = new Date();
      startTime.setHours(startHour, startMinute);
      const [endHour, endMinute] = course.classTime
        .split("-")[1]
        .split(":")
        .map(Number);
      const endTime = new Date();
      endTime.setHours(endHour, endMinute);
      if (currentTime < startTime) {
        return "记得上课哦";
      } else if (currentTime > endTime) {
        return "课程已结束";
      } else {
        return "课程进行中";
      }
      return "";
    },

    //获取状态的字体样式
    getStatusType(course) {
      const currentTime = new Date();
      const [startHour, startMinute] = "11:00-12:00"
        .split(" - ")[0]
        .split(":")
        .map(Number);
      const startTime = new Date();
      startTime.setHours(startHour, startMinute);

      if (currentTime < startTime) {
        return "warning";
      } else {
        return "success";
      }
    },

    //-------------------------------------- 全局变量处理函数-----------------------------------------------------
    //更新我的课程到store
    ...mapActions(["updateUserCourses"]),

    //移除购物车中的课程
    removeCourseFromCart(index) {
      this.$store.commit("REMOVE_COURSE_FROM_CART", index);
    },

    //添加 usercourses
    updateUserCourses(newCourses) {
      this.$store.commit("updateUserCourses", newCourses);
    },

    addUserCourses(newCourses) {
      this.$store.commit("ADD_COURSES_TO_USER", newCourses);
    },

    //更新活力币
    UPDATE_VITALITY_COINS(amount) {
      this.vitalityCoins -= amount;
    },

    //点击之后,右边栏出现
    handleCartVisibility(isVisible) {
      this.isCartVisible = isVisible;
    },

    //-------------------------------------- API接口函数-----------------------------------------------------
    //获取学生课程列表(完结版)
    fetchTodayCourseList() {
      const token = localStorage.getItem("token");
      axios
        .get(
          `http://localhost:8080/api/Course/GetTodayCoursesByUserID?token=${token}`
        )
        .then((response) => {
          console.log("学生获取今日课程列表成功:", response.data);
          this.personalCourseList = response.data;
        })
        .catch((error) => {
          console.error("学生获取今日课程列表失败:", error);
        });
    },

    //获取用户所有参与的课程(完结版)
    fetchUserCourse() {
      const token = localStorage.getItem("token");
      axios
        .get(
          `http://localhost:8080/api/Course/GetParticipatedCourseByUserID?token=${token}`
        )
        .then((response) => {
          console.log("学生获取所有课程列表成功:", response.data);
          this.updateUserCourses("");
          const initialCourses = response.data.map((item) => {
            if (item.features) {
              item.features = item.features.split("#");
            }
            return item;
          });
          this.addUserCourses(initialCourses);
        })
        .catch((error) => {
          console.error("用户获取课程列表错误:", error);
        });
    },

    // 获取活力币余额
    getVigorTokenBalance() {
      const token = localStorage.getItem("token");
      axios
        .get(`http://localhost:8080/api/User/GetVigorTokenBalance`, {
          params: {
            token: token,
            userID: localStorage.getItem("userID"),
          },
        })
        .then((response) => {
          this.vitalityCoins = response.data.balance;
        })
        .catch((error) => {
          this.vigorTokenBalance = 0;
          console.error("Error fetching vigorTokenBalance:", error);
        });
    },

    //获取预约课程（购物车内）API
    getReserved() {
      const token = localStorage.getItem("token");
      axios
        .get(
          `http://localhost:8080/api/Course/GetReservedCourseByUserID?token=${token}`
        )
        .then((response) => {
          console.log("学生获取预约课程列表成功:", response.data);
          this.$store.commit("UPDATE_CART", []);
          const bookCourseList = response.data;
          // 为每个课程项添加 selected 字段
          const updatedBookCourseList = bookCourseList.map((item) => ({
            ...item,
            selected: false,
          }));
          updatedBookCourseList.forEach((item) => {
            this.$store.commit("ADD_COURSE_TO_CART", item);
          });
        })
        .catch((error) => {
          console.error("用户获取预约课程列表错误:", error);
        });
    },

    // // 计算所选课程的总价格
    // handleCheckout(selectedCourses) {
    //   const totalPrice = selectedCourses.reduce(
    //     (total, course) => total + course.coursePrice,
    //     0
    //   );

    //   // 检查余额是否足够
    //   if (this.vitalityCoins >= totalPrice) {
    //     const token = localStorage.getItem("token");
    //     //调用支付接口(完结版)
    //     axios
    //       .post("http://localhost:8080/api/Course/PayCourseFare", {
    //         params: {
    //           token: token,
    //           bookID: bookIDList,
    //           amount: totalPrice,
    //           PayMethod: "vigor",
    //         },
    //       })
    //       .then((response) => {
    //         this.UPDATE_VITALITY_COINS(totalPrice);
    //         this.$store.commit("ADD_COURSES_TO_USER", selectedCourses);
    //         selectedCourses.forEach((course) => {
    //           const index = this.$store.state.cartCourses.indexOf(course);
    //           if (index !== -1) {
    //             this.removeCourseFromCart(index);
    //           }
    //         });
    //         ElMessageBox.alert(
    //           `下单成功！您剩余的活力币余额为：${this.vitalityCoins}`,
    //           "订单确认",
    //           {
    //             confirmButtonText: "确定",
    //             type: "success",
    //           }
    //         );
    //       });
    //   } else {
    //     // 如果余额不足，弹出错误提示
    //     ElMessage({
    //       message: "余额不足，无法完成结算。",
    //       type: "error",
    //     });
    //   }
    // },
  },

  computed: {
    // 从store获取用户课程数组
    usercourses() {
      return this.getUserCourses;
    },
    ...mapGetters(["getUserCourses"]),

    //获取用户购物车数组
    ...mapState({
      cartCourses: (state) => state.cartCourses,
    }),

    // 用户当前健身阶段
    currentStage() {
      return this.stages
        .slice()
        .reverse()
        .find((stage) => this.totalWorkoutHours >= stage.unlockHours);
    },

    //用户当前阶段称号与描述
    currentStageName() {
      return this.currentStage?.name || "";
    },
    currentStageDescription() {
      return this.currentStage?.description || "";
    },
    currentStageImage() {
      return this.currentStage?.image || "";
    },
    nextStageUnlockHours() {
      const nextStage = this.stages.find(
        (stage) => this.totalWorkoutHours < stage.unlockHours
      );
      return nextStage ? nextStage.unlockHours : "已达最高境界";
    },
  },
};
</script>

<style scoped>
.aaa {
  position: absolute;
  display: flex;
  flex-direction: column;
  top: 5%;
  left: 8%;
}

.kkk {
  display: flex;
  align-items: center;
  justify-content: flex-end;
  position: absolute;
  top: 2.5%;
  right: 2%;
  gap: 10px; /* 设置两个组件之间的间距 */
}

.vitality-card {
  background-color: yellow;
  padding: 4px;
  border-radius: 5px;
  margin-top: -8%;
}

.vitality-container {
  display: flex;
  align-items: center;
  font-size: 16px;
  font-weight: bold;
}

.vitality-container .el-icon {
  margin-right: 5px;
}

.el-page-header {
  width: 110%;
  margin-left: -5%;
}

.poem-entry {
  display: flex;
}

.poem-content {
  flex: 1;
  margin-left: -65%;
}

.poem-text {
  font-weight: bold;
}

.author-container {
  flex: 0 0 auto;
  display: flex;
  justify-content: flex-end;
}

.author-title {
  margin: 0;
  font-weight: bold;
}

.empty-row {
  margin-bottom: 20px;
}

.my-course-title {
  margin-left: -5%;
  color: #79bbff;
  font-weight: bold;
  font-size: 15px;
}

.course-calender {
  margin-top: 3%;
  display: flex;
  justify-content: space-between;
  align-items: flex-start;
}

.coursee-list,
.course-list {
  flex: 1;
}

.coursee-list {
  display: flex;
  flex-direction: column;
  gap: 50px;
  padding: 10px;
}

.empty-course-message {
  width: 100%;
  padding: 20px;
  margin-top: 20px;
  border: 1px solid #ccc;
  border-radius: 8px;
  text-align: center;
  color: #555;
  background-color: #f9f9f9;
}

.course-card {
  width: 300px;
  height: 150px;
}

.course-list {
  max-width: 35%;
  padding: 40px;
  background-color: #f9f9f9;
}

h2 {
  margin-bottom: 20px;
  font-weight: bold;
}

.course-item {
  display: flex;
  align-items: center;
  margin-bottom: 10px;
}

.circle {
  width: 30px;
  height: 30px;
  background-color: blue;
  border-radius: 50%;
  margin-right: 20px;
}

.course-name {
  flex: 1;
  margin-right: 20px;
  font-size: 1rem;
  width: 100px;
}

.course-time {
  flex: 2;
  margin-right: 20px;
}

.status-box {
  flex: 1;
}

.blue {
  background-color: #79bbff;
}

.yellow {
  background-color: #e6e8eb;
}

.red {
  background-color: #f89898;
}

.orange {
  background-color: #b88230;
}

.green {
  background-color: #95d475;
}

.chart-container {
  max-width: 1200px;
  margin-top: 30px;
}

.chart-container h2 {
  margin-left: -5%;
  color: #79bbff;
  font-weight: bold;
  font-size: 16px;
  text-align: left;
  margin-bottom: -2%;
}

.chart-content {
  max-width: 1200px;
  display: flex;
  justify-content: space-between;
  margin-top: 20px;
}

.chart {
  width: 1000px;
  height: 300px;
  margin-bottom: 20px;
}

.stage-info {
  font-family: "Noto Serif SC", serif;
  margin-top: 50px;
  width: 300px;
  margin-left: -50px;
  text-align: left;
  font-size: 1.2rem;
}

.stage-info p {
  line-height: 1.8;
  margin-bottom: 10px;
  color: #337ecc;
  font-weight: bond;
}

.stage-info h3 {
  color: blueviolet;
  line-height: 1.8;
  margin-bottom: 10px;
  font-weight: bold;
}

.encouragement {
  font-family: "ZCOOL XiaoWei", serif;
  margin-top: -5%;
  margin-left: -100px;
  font-size: 1.5em;
  color: #333;
  font-weight: bold;
}
</style>
