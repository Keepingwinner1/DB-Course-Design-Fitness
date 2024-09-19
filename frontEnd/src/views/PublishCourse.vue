<template>
  <div class="aaa">
    <div aria-label="A complete example of page header">
      <el-page-header @back="onBack">
        <template #breadcrumb>
          <el-breadcrumb separator="/">
            <el-breadcrumb-item :to="{ name: 'HomeView' }">
              FitFit
            </el-breadcrumb-item>
            <el-breadcrumb-item :to="{ name: 'course' }">
              健身课程
            </el-breadcrumb-item>
            <el-breadcrumb-item>我的教学</el-breadcrumb-item>
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

    <!-- teachcourses代表这个教师的所有课程,他的全局定义在store,变量的来源是每次页面初始的时候fetch这个API传进来的,之后用户的操作会通过对全局变量进行操作,来使得这里的展示发生变化 -->
    <div class="course-calender">
      <div class="my-course-title">我的课堂</div>
      <div class="coursee-list">
        <TeachCard
          v-for="(teachcourse, index) in teachcourses"
          :key="index"
          :editForm="teachcourse"
          @delete-teachcourse="removeCourse"
        />
        <!-- 发布课程的按钮 -->
        <el-button type="primary" @click="showModal = true" class="plus-button">
          <i class="el-icon-plus">
            <el-icon class="el-icon-my-circle-plus">
              <Plus />
            </el-icon>
          </i>
        </el-button>

        <!-- 弹出的表单 newCourse代表发布的新课程-->
        <el-dialog v-model="showModal" title="发布新课程" width="50%">
          <el-form :model="form">
            <el-form-item label="课程名称">
              <el-input
                v-model="newCourse.courseName"
                placeholder="例如:30到45分钟核心训练"
              ></el-input>
            </el-form-item>
            <el-form-item label="课程图片">
              <el-upload
                class="upload-demo"
                :file-list="fileList"
                :on-change="handleFileUpload"
                :before-upload="beforeUpload"
                :show-file-list="false"
              >
                <div class="image-upload-container">
                  <el-image
                    v-if="newCourse.coursePhotoUrl"
                    :src="newCourse.coursePhotoUrl"
                    fit="cover"
                  ></el-image>
                  <div v-else class="upload-placeholder">
                    <el-icon><plus /></el-icon>
                    <span>点击上传</span>
                  </div>
                </div>
              </el-upload>
            </el-form-item>
            <el-form-item label="课程描述">
              <el-input
                v-model="newCourse.courseDescription"
                type="textarea"
                placeholder="核心肌群是身体的中心力量，对于维持姿势、提高运动表现和预防受伤至关重要。"
              ></el-input>
            </el-form-item>
            <el-form-item label="课程容量">
              <el-input-number
                v-model="newCourse.capacity"
                :min="0"
                :max="100"
                :step="1"
                placeholder="请填入课程容量"
              />
            </el-form-item>
            <el-form-item label="课程开始时间">
              <el-date-picker
                v-model="newCourse.courseStartTime"
                type="date"
                placeholder="选择日期"
                @change="handleDateChange"
              />
            </el-form-item>
            <el-form-item label="课程结束时间">
              <el-date-picker
                v-model="newCourse.courseEndTime"
                type="date"
                placeholder="选择日期"
                @change="handleDateeChange"
              />
            </el-form-item>
            <!-- 每周上课时间段 -->
      <el-form-item label="每周上课时间段">
        <div v-for="(schedule, index) in schedules" :key="index" style="margin-bottom: 10px;">
          <el-select
            v-model="schedule.dayOfWeek"
            placeholder="选择星期几"
            style="width: 120px;"
          >
            <el-option v-for="(day, i) in weekDays" :key="i" :label="day.label" :value="day.value"></el-option>
          </el-select>
          <el-time-picker
            v-model="schedule.classTime"
            is-range
            range-separator="至"
            start-placeholder="开始时间"
            end-placeholder="结束时间"
            format="HH:mm"
            value-format="HH:mm"
            style="width: 220px; margin-left: 10px;"
          ></el-time-picker>
          <el-button type="danger" @click="removeSchedule(index)" style="margin-left: 10px;">删除</el-button>
        </div>
        <el-form-item>
          <el-select v-model="newSchedule.dayOfWeek" placeholder="选择星期几" style="width: 120px;">
            <el-option v-for="(day, i) in weekDays" :key="i" :label="day.label" :value="day.value"></el-option>
          </el-select>
          <el-time-picker
            v-model="newSchedule.classTime"
            is-range
            range-separator="至"
            start-placeholder="开始时间"
            end-placeholder="结束时间"
            format="HH:mm"
            value-format="HH:mm"
            style="width: 220px; margin-left: 10px;"
          ></el-time-picker>
          <el-button type="primary" @click="addSchedule" style="margin-left: 10px;">添加时间段</el-button>
        </el-form-item>
      </el-form-item>

            <el-form-item label="课程难度">
              <el-radio-group v-model="newCourse.courseGrade">
                <el-radio :label="1">1</el-radio>
                <el-radio :label="2">2</el-radio>
                <el-radio :label="3">3</el-radio>
                <el-radio :label="4">4</el-radio>
                <el-radio :label="5">5</el-radio>
              </el-radio-group>
            </el-form-item>
            <el-form-item label="课程价格">
              <el-input-number
                v-model="newCourse.coursePrice"
                :min="0"
                :max="3000"
                :step="1"
                placeholder="请填入一个数字"
              />
            </el-form-item>
            <el-form-item label="课程特征">
              <div>
                <el-tag
                  v-for="(feature, index) in newCourse.features"
                  :key="index"
                  closable
                  @close="removeFeature(index)"
                  style="margin-right: 8px"
                >
                  {{ feature }}
                </el-tag>
                <el-input
                  v-model="inputFeature"
                  placeholder="输入并按回车,如 [力量, 增肌]"
                  @keyup.enter.native="addFeature"
                  style="width: 200px"
                ></el-input>
              </div>
            </el-form-item>
            <el-form-item label="课程分类">
              <el-select
                v-model="newCourse.courseType"
                placeholder="请选择课程分类"
              >
                <el-option label="高强度间歇" value="高强度间歇"></el-option>
                <el-option label="儿童趣味课" value="儿童趣味课"></el-option>
                <el-option label="低强度塑形" value="低强度塑形"></el-option>
                <el-option label="有氧训练" value="有氧训练"></el-option>
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" @click="submitForm">提交</el-button>
              <el-button @click="showModal = false">取消</el-button>
            </el-form-item>
          </el-form>
          <template #footer>
            <el-button @click="showModal = false">取消</el-button>
            <el-button type="primary" @click="saveCourse">保存</el-button>
          </template>
        </el-dialog>
      </div>

      <!-- 每日教学任务列表 -->
      <div class="course-list">
        <h2>您今日的教学任务</h2>
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
            <div class="square" @click="handleClick(course)">
              <el-icon v-if="course.attended">
                <Select />
              </el-icon>
            </div>
          </div>
        </el-row>
      </div>
    </div>

    <!-- 报名的学生可视化 -->
    <div class="chart-container">
      <h2>我的课程报名人数</h2>
      <div class="chart-content">
        <div id="line-chart" ref="lineChart" class="chart"></div>
        <div class="stage-info">
          <h3>称号：{{ currentStageName }}</h3>
          <p>{{ currentStageDescription }}</p>
          <p>总学生: {{ totalWorkoutHours }} 位</p>
          <p>下一称号需要: {{ nextStageUnlockHours }} 学生</p>
        </div>
      </div>
      <p class="encouragement">{{ encouragementMessage }}</p>
    </div>
  </div>
</template>

<script>
import * as echarts from "echarts";
import TeachCard from "../components/TeachCard.vue";
import { mapGetters, mapActions } from "vuex";
import axios from "axios";
import { ElNotification } from "element-plus";

export default {
  components: {
    TeachCard,
  },

  data() {
    return {
      weekDays: [
        { label: '周日', value: 0 },
        { label: '周一', value: 1 },
        { label: '周二', value: 2 },
        { label: '周三', value: 3 },
        { label: '周四', value: 4 },
        { label: '周五', value: 5 },
        { label: '周六', value: 6 }
      ], // 星期几选项
      schedules: [
        { classID: -1, dayOfWeek: 0, classTime: ['08:00', '10:00'] } // 默认值
        ],
      newSchedule: { classID:-1,dayOfWeek: 0, classTime: [] }, // 新添加的时间段
      userName: "",
      userIcon: "",
      introduction: "",
      email: "",
      vitalityCoins: 0,
      showModal: false,
      newCourse: {
        classID: "",
        typeID: "",
        courseName: "",
        capacity: "",
        courseDescription: "",
        coursePrice: 0,
        courseStartTime: "",
        courseEndTime: "",
        courseLastDays: "",
        courseGrade: 0,
        coursePhotoUrl: "",
        courseVideoUrl: "",
        features: [],
        schedules: [], // 每周上课时间段列表
      },
      inputFeature: "",
      courseData: [
        { date: "2024-08-01", duration: 3 },
        { date: "2024-08-02", duration: 5 },
        { date: "2024-08-03", duration: 9 },
        { date: "2024-08-04", duration: 10 },
        { date: "2024-08-05", duration: 2 },
        { date: "2024-08-06", duration: 4 },
        { date: "2024-08-07", duration: 2 },
        { date: "2024-08-08", duration: 5 },
        { date: "2024-08-09", duration: 1 },
        { date: "2024-08-10", duration: 2 },
      ],
      encouragementMessage: "",
      totalWorkoutHours: 0,
      stages: [
        {
          name: "教练",
          unlockHours: 0,
          description: "初窥门径，教学渐入佳。",
        },
        {
          name: "师父",
          unlockHours: 10,
          description: "熟能生巧，技艺日臻完善。",
        },
        {
          name: "堂主",
          unlockHours: 30,
          description: "炉火纯青，传道授业解惑。",
        },
        {
          name: "教头",
          unlockHours: 60,
          description: "登峰造极，桃李满天下。",
        },
        {
          name: "宗师",
          unlockHours: 100,
          description: "返璞归真，大道至简。",
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

    //进入页面就要调用的接口
    this.fetchTodayCourseList();
    this.fetchUserCourse();

    //进入页面就要获取的个人信息
    this.userName = localStorage.getItem("name");
    this.getVigorTokenBalance();
    this.email = localStorage.getItem("email");
    this.userIcon = localStorage.getItem("iconUrl");
    this.introduction = localStorage.getItem("introduction");
  },
  methods: {
    formatClassTime(schedule) {
      //console.log(schedule);

      if (Array.isArray(schedule.classTime) && schedule.classTime.length === 2) {
        // 将 classTime 数组转换为 "08:00-10:00" 格式的字符串
        schedule.classTime = `${schedule.classTime[0]}-${schedule.classTime[1]}`;
      }
      return schedule;
    },
    addSchedule() {
      if (this.newSchedule.dayOfWeek !== null && this.newSchedule.classTime.length === 2) {
        this.schedules.push({ ...this.newSchedule });
        this.newSchedule = { classID:-1,dayOfWeek: 0, classTime:[] }; // 重置新添加的时间段
      } else {
        this.$message.error('请完整填写时间段');
      }
    },
    removeSchedule(index) {
      this.schedules.splice(index, 1);
    },
    //----------------------------------------------------所有的静态函数--------------------------------------------
    //回退<-back
    onBack() {
      this.$router.go(-1);
    },

    // 随机生成一首诗
    selectRandomPoem() {
      const index = Math.floor(Math.random() * this.poems.length);
      this.selectedPoem = this.poems[index];
    },

    //初始化折线图
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
          name: "每日新报名人数",
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
      if (this.totalWorkoutHours > 100) {
        this.encouragementMessage = "心手相应，以武入道！";
      } else if (this.totalWorkoutHours > 50) {
        this.encouragementMessage = "厚积薄发，功到自然成，你会越来越棒！";
      } else {
        this.encouragementMessage =
          "教学相长，砺志铸魂，开始总是最难的，坚持下去，你会看到改变！";
      }
    },

    //---------------------------------- 发布课程功能函数-------------------------------------------------
    //发布课程---上传照片模块函数
    handleFileUpload(file) {
      this.newCourse.coursePhotoUrl = URL.createObjectURL(file.raw);
    },
    beforeUpload(file) {
      this.imagePreview = "";
      const isJPGorPNG =
        file.type === "image/jpeg" || file.type === "image/png";
      const isLt2M = file.size / 1024 / 1024 < 2;

      if (!isJPGorPNG) {
        this.$message.error("上传头像图片只能是 JPG 或 PNG 格式!");
        return false;
      }
      if (!isLt2M) {
        this.$message.error("上传头像图片大小不能超过 2MB!");
        return false;
      }
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => {
        this.imagePreview = reader.result;
        this.newCourse.coursePhotoUrl = this.imagePreview;
      };
      return false;
    },

    //发布课程---课程特征模块函数
    addFeature() {
      const feature = this.inputFeature.trim();
      if (feature && !this.newCourse.features.includes(feature)) {
        this.newCourse.features.push(feature);
      }
      this.inputFeature = ""; // 清空输入框
    },
    removeFeature(index) {
      this.newCourse.features.splice(index, 1);
    },

    //发布课程---课程类别模块函数
    getCourseValue(courseName) {
      switch (courseName) {
        case "高强度间歇":
          return 1;
        case "低强度塑形":
          return 2;
        case "儿童趣味课":
          return 3;
        case "有氧训练":
          return 4;
        default:
          return 0; // 如果输入的课程名称不在列表中，返回0或其他默认值
      }
    },
    //发布新课程的前端全局逻辑
    ...mapActions(["addTeachCourse"]),
    openModal() {
      console.log("Opening modal");
      this.showModal = true;
    },

    handleDateChange(value) {
      const date = new Date(value);
      const year = date.getFullYear();
      const month = String(date.getMonth() + 1).padStart(2, "0");
      const day = String(date.getDate()).padStart(2, "0");
      this.newCourse.courseStartTime = `${year}-${month}-${day}`;
    },

    handleDateeChange(value) {
      const date = new Date(value);
      const year = date.getFullYear();
      const month = String(date.getMonth() + 1).padStart(2, "0");
      const day = String(date.getDate()).padStart(2, "0");
      this.newCourse.courseEndTime = `${year}-${month}-${day}`;
    },

    //---------------------------------------更新与删除课程的全局函数,具体实现在TeachCard-----------------------------------------------------
    //更新我的课程到store
    ...mapActions(["updateTeachCourses"]),

    //删除教练课程
    ...mapActions(["deleteTeachCourse"]),
    removeCourse(courseId) {
      this.deleteTeachCourse(courseId);
    },

    //---------------------------------- 每日教学列表功能函数-------------------------------------------------
    //每日教学列表的名字长度问题
    truncateCourseName(name) {
      if (name.length > 5) {
        return name.slice(0, 5) + "...";
      } else {
        return name;
      }
    },

    //确定attend的函数
    handleClick(course) {
      course.attended = course.attended ? 0 : 1;
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
        .split(" - ")[1]
        .split(":")
        .map(Number);
      const endTime = new Date();
      endTime.setHours(endHour, endMinute);

      if (currentTime < startTime) {
        return "提醒我";
      } else if (currentTime > endTime && course.attended) {
        return "已打卡";
      } else if (currentTime > endTime && !course.attended) {
        return "未打卡";
      } else {
        return "进行中";
      }
      return "";
    },

    //获取状态的字体样式
    getStatusType(course) {
      const currentTime = new Date();
      const [startHour, startMinute] = course.classTime
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
    addTeachCourse(newCourses) {
      this.$store.commit("ADD_TEACH_COURSE", newCourses);
    },
    //-------------------------------------- API接口函数-----------------------------------------------------
    //获取教授的所有课程(完结版)
    fetchUserCourse() {
      const token = localStorage.getItem("token");
      axios
        .get(
          `http://localhost:8080/api/Course/GetCoachParticipatedCourseByUserID?token=${token}`
        )
        .then((response) => {
          this.$store.commit("updateTeachCourses", []);
          //把classTime字段转为['';'']形式
          const initialCourses = response.data.map((item) => {
            if (item.features) {
              item.features = item.features.split("#");
            };
            if(item.courseType){
              item.courseType = this.getCourseValue(item.courseType).toString();
            }
            if (item.schedules) {
              item.schedules = item.schedules.map((schedule) => ({
              ...schedule,
                classTime: schedule.classTime.split("-").map((time) => time.trim()),
              }));
            }
            return item;
          });
          console.log("教师获取教学课程成功:", initialCourses);
          this.addTeachCourse(initialCourses);
        })
        .catch((error) => {
          console.error("教师获取教学课程失败:", error);
        });
    },

    //获取教师教学列表(完结版)
    fetchTodayCourseList() {
      const token = localStorage.getItem("token");
      axios
        .get(
          `http://localhost:8080/api/Course/GetCoachTodayCoursesByUserID?token=${token}`
        )
        .then((response) => {
          console.log("教师获取今日教学列表成功:", response.data);
          this.personalCourseList = response.data;
        })
        .catch((error) => {
          console.error("教师获取今日教学列表失败:", error);
        });
    },

    // 获取活力币余额
    getVigorTokenBalance() {
      const token = localStorage.getItem("token");
      axios
        .get(
          `http://localhost:8080/api/User/GetVigorTokenBalance`,{
            params:{
              token: token,
              userID: localStorage.getItem("userID"),
            }
          }
        )
        .then((response) => {
          this.vitalityCoins = response.data.balance;
        })
        .catch((error) => {
          this.vigorTokenBalance = 0;
          console.error("Error fetching vigorTokenBalance:", error);
        });
    },

    //发布新课程的接口
    submitForm() {
      this.newCourse.instructorHonors = this.intoduction;
      this.newCourse.instructorName = this.username;
      this.newCourse.instructorImage = this.userIcon;
      this.addTeachCourse([this.newCourse]);
      // 转换日期字符串为 Date 对象
      var startDate = new Date(this.newCourse.courseStartTime);
      var endDate = new Date(this.newCourse.courseEndTime);
      // 计算时间差（以毫秒为单位）
      var timeDiff = endDate - startDate;
      // 将时间差转换为天数
      var daysDiff = timeDiff / (1000 * 3600 * 24);
      // 应用转换函数到每个元素
      let schedules=this.schedules;
      console.log("转换前的课程时间为:",schedules);
      schedules = schedules.map(this.formatClassTime);
      schedules.forEach(element => {
        console.log("转换后的课程时间为:",element);
      });

      const postData = {
        course: {
          classID: -1,
          typeID: this.getCourseValue(this.newCourse.courseType),
          courseName: this.newCourse.courseName,
          capacity: this.newCourse.capacity,
          courseDescription: this.newCourse.courseDescription,
          coursePrice: this.newCourse.coursePrice,
          courseStartTime: this.newCourse.courseStartTime,
          courseEndTime: this.newCourse.courseEndTime,
          courseLastDays: daysDiff,
          courseGrade: this.newCourse.courseGrade,
          coursePhotoUrl: this.newCourse.coursePhotoUrl,
          courseVideoUrl: "null",
          features: this.newCourse.features.join("#") + "#",
        },
        courseSchedules:schedules
      };
      console.log("上传数据为:",postData);
      axios
        .post(
          `http://localhost:8080/api/Course/PublishCourse?token=${localStorage.getItem("token")}`,
          postData
        )
        .then((response) => {
          console.log("Course added successfully:", response.data);
          this.showModal = false;
          this.newCourse = {
            classID: "",
            typeID: "",
            courseName: "",
            capacity: 0,
            courseDescription: "",
            coursePrice: 0,
            courseStartTime: "",
            courseEndTime: "",
            courseLastDays: "",
            courseGrade: 0,
            coursePhotoUrl: "",
            courseVideoUrl: "",
            features: [],
          };
          this.schedules = [
          { classID: -1, dayOfWeek: 0, classTime: ['08:00', '10:00'] } // 默认值
          ];

          ElNotification({
            title: "成功",
            message: "课程发布成功！",
            type: "success",
          });
        })
        .catch((error) => {
          console.log("Error adding course:", error);
          ElNotification({
            title: "错误",
            message: "发布课程时发生错误，请稍后再试。",
            type: "error",
          });
        });
    },
  },

  computed: {
    // 从store获取教练课程数组
    teachcourses() {
      return this.getTeachCourses;
    },
    ...mapGetters(["getTeachCourses"]),

    // 用户当前教练阶段
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
.upload-demo {
  width: 150px;
  height: 150px;
  border: 1px dashed #d9d9d9;
  border-radius: 4px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  overflow: hidden;
}

.image-upload-container {
  width: 100%;
  height: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
}

.upload-placeholder {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: #909399;
}

.upload-placeholder span {
  margin-top: 8px;
}

.el-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.aaa {
  position: absolute;
  display: flex;
  flex-direction: column;
  top: 5%;
  left: 8%;
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
  margin-top: 20px;
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

.course-card {
  width: 300px;
  height: 150px;
}

.plus-button {
  background-color: orange;
  color: white;
  font-size: 24px;
  width: 100px;
  height: 40px;
  margin-left: 50%;
  border-radius: 5px;
  display: flex;
  justify-content: center;
  align-items: center;
}

.plus-button .el-icon-plus {
  font-weight: bold;
}

.el-icon-my-circle-plus {
  font-weight: bold;
  font-size: larger;
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
  margin-right: 20px; /* 控制圆形与课程名称的间隔 */
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

.square {
  margin-left: 5px;
  font-size: 1.5rem;
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
