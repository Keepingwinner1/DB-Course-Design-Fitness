<template>
  <div v-if="isVisible" class="course-modal">
    <div class="modal-background" :style="containerStyle">
      <div :style="backgroundStyle"></div>
      <div :style="coursePhotoStyle"></div>

      <div class="modal-content">
        <button class="close-button" @click="closeModal">X</button>
        <div class="course-info">
          <h1 class="course-title">{{ this.thecourse.courseName }}</h1>
          <p class="course-details">
            课程有效时间：<b class="boldd">{{
              this.thecourse.courseStartTime
            }}</b>
            -
            <b class="boldd">{{ this.thecourse.courseEndTime }}</b>
          </p>
          <p class="course-details">
            上课时间：每周
            <span v-for="(schedule, index) in thecourse.schedules" :key="index">
              <b class="boldd">{{ getDayOfWeek(schedule.dayOfWeek) }}</b>
              <b class="boldd">{{ schedule.classTime }}</b>
              <span v-if="index !== thecourse.schedules.length - 1">, </span>
            </span>
          </p>
          <div class="features">
            <div
              class="feature-item"
              v-for="(feature, index) in this.thecourse.features"
              :key="index"
            >
              <div class="parallelogram">
                <span>{{ feature }}</span>
              </div>
            </div>
          </div>
          <div class="instructor-info">
            <img
              :src="this.thecourse.iconURL"
              alt="Instructor"
              class="instructor-image"
            />
            <div class="duiqi">
              <div class="instructor-info">
                <h2 class="instructor-name">
                  <b class="boldd">{{ this.thecourse.coachName }}</b>
                </h2>
                <div class="instructor-tag">
                  <span class="tag">职业教练</span>
                  <el-icon class="trophy-icon"><GoldMedal /></el-icon>
                </div>
              </div>
              <p class="instructor-honors">
                {{ this.thecourse.instructorHonors }}
              </p>
            </div>
          </div>
          <p class="course-description">
            <b class="boldd">课程须知</b>：{{
              this.thecourse.courseDescription
            }}
          </p>

          <p class="nandu" :style="iconContainerStyle">
            <b class="boldd">课程难度</b>:
            <el-icon
              v-if="this.thecourse.courseGrade >= 1"
              style="margin-right: 10px"
            >
              <Flag />
            </el-icon>
            <el-icon
              v-if="this.thecourse.courseGrade >= 2"
              style="margin-right: 10px"
            >
              <Flag />
            </el-icon>
            <el-icon
              v-if="this.thecourse.courseGrade >= 3"
              style="margin-right: 10px"
            >
              <Flag />
            </el-icon>
            <el-icon
              v-if="this.thecourse.courseGrade >= 4"
              style="margin-right: 10px"
            >
              <Flag />
            </el-icon>
            <el-icon
              v-if="this.thecourse.courseGrade >= 5"
              style="margin-right: 10px"
            >
              <Flag />
            </el-icon>
          </p>
          <p class="nandu">
            <b class="boldd">课程剩余容量</b>：{{ this.thecourse.capacity }}
          </p>
          <p class="nandu">
            <b class="boldd">课程费用</b>：{{ this.thecourse.coursePrice
            }}<el-icon class="coinn"><Coin /></el-icon>
          </p>
        </div>
        <div>
          <div v-if="this.thecourse.isBooked == 0" class="yuyue">
            <el-icon style="font-size: 35px" class="ic"
              ><ShoppingTrolley
            /></el-icon>
            <button class="book-button" @click="addToCart">加入购物车</button>
          </div>
          <button
            v-else
            :class="buttonClass"
            @click="handleButtonClick"
            :disabled="isDisabled"
          >
            {{ buttonText }}
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ElMessage } from "element-plus";
import axios from "axios";

export default {
  //传进来的数据,用于展示UI需要的所有变量
  //传到这里来的数据,有三个来源,分别是getallcourse的API,GetParticipatedCourseByUserID的API,GetCoachParticipatedCourseByUserID的API
  //其中,getallcourse包括教练信息,
  //GetCoachParticipatedCourseByUserID传入的时候不包含教练信息,需要在PublishCourse的view界面增加教练自己的用户信息到thecourse的数组
  //GetParticipatedCourseByUserID我暂时没看到数据
  props: {
    isVisible: {
      type: Boolean,
      default: 1,
    },
    thecourse: Object,
    modalWidth: {
      type: String,
      default: "1000px", // 默认宽度
    },
    modalHeight: {
      type: String,
      default: "650px", // 默认高度
    },
    modalBackground: {
      type: String,
      default:
        "https://www.lesmills.com.cn/uploads/20231104/fbdee91e55d525de27a01e2e0a74040b.png",
    },
  },
  data() {
    return {
      currentState: "", // "remember" | "signIn" | "completed" | "missed"
    };
  },
  emits: ["close"],

  methods: {
    getDayOfWeek(dayOfWeek) {
      const days = ["周日", "周一", "周二", "周三", "周四", "周五", "周六"];
      return days[dayOfWeek];
    },

    //关闭课程模式
    closeModal() {
      this.$emit("close");
    },

    //点击课程按钮
    handleButtonClick() {
      console.log(this.currentTime);
      if (this.isDuringClass) {
        this.currentState = "signedIn";
      } else if (this.isAfterClass && this.currentState !== "signedIn") {
        ElMessage({
          message: "请联系教练补课",
          type: "warning",
        });
      }
    },

    addToCart() {
      // 创建课程对象
      const course = this.thecourse;
      const classID = course.schedules[0].classID;

      //补充调用后端的接口：ReserveCourse
      const postData = {
        token: localStorage.getItem("token"),
        classID: [classID],
        payMethod: "活力币",
      };
      console.log(postData);

      axios
        .post("http://localhost:8080/api/Course/ReserveCourse", postData)
        .then((response) => {
          console.log(response.data);
          //添加成功后的提示
          this.$message.success("课程已成功加入购物车！");
        })
        .catch((error) => {
          console.error(error);
          ElMessage({
            message: "课程加入购物车失败，请稍后重试！",
            type: "error",
          });
        });
    },

    //-------------------------------------- API接口------------------------------------------------------
    //1.发布评论
    //2.查看评论
  },

  computed: {
    //卡片样式
    containerStyle() {
      return {
        position: "relative",
        width: this.modalWidth,
        height: this.modalHeight,
        overflow: "hidden",
      };
    },
    backgroundStyle() {
      return {
        position: "absolute",
        top: 0,
        left: 0,
        width: "100%",
        height: "100%",
        backgroundImage: `url(${this.modalBackground})`,
        backgroundSize: "cover",
        backgroundPosition: "center",
      };
    },
    coursePhotoStyle() {
      return {
        position: "absolute",
        top: 0,
        left: 0,
        width: "50%",
        height: "100%",
        backgroundImage: `url(${this.thecourse.coursePhotoUrl})`,
        backgroundSize: "cover",
        backgroundPosition: "center",
      };
    },

    //获取当前时间
    currentTime() {
      return new Date();
    },
    //获取课程的开始时间与结束时间
    timeRange() {
      //const [start, end] = this.thecourse.schedules.classTime;
      const [start, end] = "10:00-12:00".split("-").map((time) => {
        const [hours, minutes] = time.split(":");
        const date = new Date();
        date.setHours(parseInt(hours), parseInt(minutes), 0, 0);
        return date;
      });
      return { start, end };
    },
    //判断是否在课程时间之前
    isBeforeClass() {
      return this.currentTime < this.timeRange.start;
    },
    //判断是否在课程进行中
    isDuringClass() {
      return (
        this.currentTime >= this.timeRange.start &&
        this.currentTime <= this.timeRange.end
      );
    },
    //判断是否在课程时间之后
    isAfterClass() {
      return this.currentTime > this.timeRange.end;
    },
    //按钮的字样
    buttonText() {
      if (this.isBeforeClass) {
        return "准时上课";
      } else if (this.isDuringClass) {
        return "正在上课";
      } else if (this.isAfterClass) {
        return "课程结束";
      }
    },
    //按钮的样式
    buttonClass() {
      if (
        this.isBeforeClass ||
        (this.isDuringClass && this.currentState === "signedIn")
      ) {
        return "book-button blue";
      } else if (this.isDuringClass) {
        return "book-button active";
      } else if (this.isAfterClass) {
        return "book-button dark";
      }
    },
    //课程开始前,已经签到,课程结束后,按钮都不能再被点击
    isDisabled() {
      return (
        this.isBeforeClass ||
        this.currentState === "signedIn" ||
        (this.isAfterClass && this.currentState === "signedIn")
      );
    },
  },
};
</script>

<style scoped>
.boldd {
  font-weight: bold;
}

.duiqi {
  text-align: left;
}

.close-button {
  position: absolute;
  top: 20px;
  right: 20px;
  padding: 10px 20px;
  background-color: #f44336;
  color: white;
  border: none;
  cursor: pointer;
}

.course-modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-color: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 999;
}

.modal-background {
  width: 100%;
  height: 100%;
  background-size: cover;
  background-position: center;
  position: relative;
}

.modal-content {
  position: absolute;
  right: 0;
  top: 0;
  width: 50%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  color: white;
  padding: 20px;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
}

.course-title {
  font-size: 2.5em;
  font-weight: bold;
  margin-bottom: 10px;
}

.course-details {
  font-size: 1.2em;
}

.features {
  margin-top: 20px;
  display: flex;
  flex-direction: row;
  align-items: center;
}

.feature-item {
  margin-right: 10px; /* 为每个特性项添加右边距 */
}

.parallelogram {
  width: 100px;
  background-color: #c45656;
  color: white;
  padding: 10px 20px;
  clip-path: polygon(10% 0, 100% 0, 90% 100%, 0% 100%);
  display: inline-block;
  margin: 18px;
  margin-bottom: -5px;
  margin-top: -8px;
}

.feature-item span {
  font-size: 1em;
  font-weight: bold;
  margin-right: 5px;
}

.instructor-info {
  display: flex;
  align-items: center;
  margin-top: 12px;
}

.instructor-image {
  width: 60px;
  height: 60px;
  border-radius: 50%;
  margin-right: 10px;
}

.instructor-name {
  margin-left: 2%;
  font-size: 1.4em;
}

.instructor-info {
  display: flex;
  align-items: center;
}

.instructor-name {
  margin-right: 10px; /* 调整名字和方框之间的间距 */
}

.instructor-tag {
  display: flex;
  align-items: center;
}

.tag {
  display: inline-block;
  padding: 4px 8px;
  background-color: #ffdd57;
  border-radius: 4px;
  margin-right: 8px; /* 调整方框和图标之间的间距 */
  font-size: 14px;
  font-weight: bold;
  color: #333;
}

.trophy-icon {
  font-size: 24px;
  color: gold;
  font-weight: bold;
  stroke-width: 2;
}

.nandu {
  margin-top: 20px;
  text-align: left;
  font-size: 1rem;
}

.coinn {
  margin-left: 5px;
  font-weight: bold;
  font-size: 14px;
  color: gold;
}

.instructor-honors {
  margin-left: 10px;
  font-size: 1.1em;
}

.course-description {
  font-size: 1rem;
  margin-top: 20px;
  text-align: left;
}

.yuyue {
  font-size: 1.2rem;
  margin-left: 300px;
  margin-bottom: 100px;
  display: flex;
  align-items: center;
}

.book-button {
  margin-left: 10px;
  font-size: 20px;
  cursor: pointer;
}

.book-button.blue {
  background-color: blue;
  color: white;

  padding: 10px 20px;
  margin-top: -10%;
}

.book-button.active {
  background-color: green;
  color: white;

  padding: 10px 20px;
  margin-top: -10%;
}

.book-button.dark {
  background-color: gray;
  color: white;

  padding: 10px 20px;
  margin-top: -10%;
}

.book-button[disabled] {
  cursor: not-allowed;
}
</style>
