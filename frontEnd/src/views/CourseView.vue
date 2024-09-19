<template>
  <navigation-bar />
  <common-layout />
  <div class="trans">
    <router-link v-if="userRole === 'user'" to="/userhome">
      <el-button type="primary">我的主页</el-button>
    </router-link>
    <router-link v-else-if="userRole === 'coach'" to="/publishcourse">
      <el-button type="primary">我的主页</el-button>
    </router-link>
  </div>
  <div id="app">
    <div class="hero-section-wrapper">
      <div class="hero-section">
        <div class="bottom-bar">
          <img
            class="left-image"
            src="https://tse4-mm.cn.bing.net/th/id/OIP-C.sfCxPFaiDHtJbebl8q6mOAHaHa?w=192&h=193&c=7&r=0&o=5&pid=1.7"
            alt="左侧图片"
          />
          <span
            class="all-courses"
            :class="{ active: isActive }"
            @click="handleAllCoursesClick"
          >
            全部课程
          </span>
          <div class="search-container">
            <!-- 搜索框 -->
            <el-input
              v-model="searchKeyword"
              placeholder="请输入关键词"
              @input="onSearch"
              class="search-input"
              clearable
            >
              <template #prefix>
                <el-icon><Search /></el-icon>
              </template>
              <template #suffix>
                <el-button
                  type="primary"
                  size="small"
                  @click="handleSomeCoursesClick"
                  style="background-color: blue; color: white"
                >
                  搜索
                </el-button>
              </template>
            </el-input>

            <!-- 筛选区域 -->
            <div class="filters">
              <!-- 课程类别 -->
              <el-dropdown
                @command="onCourseTypeChange"
                v-model:visible="courseTypeVisible"
                class="dropdown"
                hide-on-click="false"
              >
                <span class="el-dropdown-link">
                  课程类别
                  <el-icon><CaretBottom /></el-icon>
                  <el-icon v-if="selectedCourseType"><SuccessFilled /></el-icon>
                </span>
                <template #dropdown>
                  <el-dropdown-menu>
                    <el-dropdown-item
                      v-for="type in courseTypes"
                      :key="type"
                      :command="type"
                      :class="{ selected: selectedCourseType === type }"
                    >
                      {{ type }}
                      <el-icon v-if="selectedCourseType === type"
                        ><Check
                      /></el-icon>
                    </el-dropdown-item>
                  </el-dropdown-menu>
                </template>
              </el-dropdown>

              <!-- 价格区间 -->
              <el-dropdown
                @command="onPriceRangeChange"
                v-model:visible="priceRangeVisible"
                class="dropdown"
                hide-on-click="false"
              >
                <span class="el-dropdown-link">
                  价格区间
                  <el-icon><CaretBottom /></el-icon>
                  <el-icon v-if="selectedPriceRange"><SuccessFilled /></el-icon>
                </span>
                <template #dropdown>
                  <el-dropdown-menu>
                    <el-dropdown-item
                      v-for="range in priceRanges"
                      :key="range.label"
                      :command="range.label"
                      :class="{ selected: selectedPriceRange === range.label }"
                    >
                      {{ range.label }}
                      <el-icon v-if="selectedPriceRange === range.label"
                        ><Check
                      /></el-icon>
                    </el-dropdown-item>
                  </el-dropdown-menu>
                </template>
              </el-dropdown>
            </div>
          </div>
          <div class="red-box" @click="showRecommendation">
            想知道什么课程适合我
          </div>
        </div>
      </div>
    </div>

    <!-- 课程内容展示区域,包含isbooked的,而且是包含教练信息的-->
    <!-- 搜索功能与全部课程功能的操作对象都是courses -->
    <div class="course-grid">
      <CourseHome
        v-for="(course, index) in courses"
        :key="index"
        :homecourse="course"
        :width="300"
        :height="400"
      />
    </div>

    <!-- 弹窗 -->
    <el-dialog v-model="isDialogVisible" title="提示">
      <p>{{ dialogMessage }}</p>
      <span slot="footer" class="dialog-footer">
        <el-button @click="isDialogVisible = false">确定</el-button>
      </span>
    </el-dialog>
  </div>
</template>

<script>
import CourseHome from "../components/CourseHome.vue";
import axios from "axios";
import {
  Search,
  CaretBottom,
  Check,
  SuccessFilled,
} from "@element-plus/icons-vue";

export default {
  components: {
    CourseHome,
    [Search.name]: Search,
    [CaretBottom.name]: CaretBottom,
    [Check.name]: Check,
    [SuccessFilled.name]: SuccessFilled,
  },
  data() {
    return {
      searchKeyword: "", //搜索的关键词
      selectedCourseType: "", //选定的课程类型
      selectedPriceRange: "", //选定的价格区间
      min: 0,
      max: 0,
      isActive: false,
      courseTypes: ["高强度间歇", "低强度塑形", "儿童趣味课", "有氧训练"],
      priceRanges: [
        { label: "0-40 活力币", min: 0, max: 40 },
        { label: "40-80 活力币", min: 40, max: 80 },
        { label: "80 以上 活力币", min: 80, max: null },
      ],
      userRole: "普通用户",
      courses: [],
      isDialogVisible: false,
      dialogMessage: "",
      hoveredOption: null,
      selectedOption: null,
    };
  },

  mounted() {
    this.fetchCourses();
    this.userRole = localStorage.getItem("role");
  },

  methods: {
    //点击全部课程触发
    handleAllCoursesClick() {
      this.isActive = !this.isActive;
      this.searchKeyword = "";
      this.selectedCourseType = "";
      this.selectedPriceRange = "";
      (this.min = 0), (this.max = 0), console.log("全部课程按钮被点击了！");
      this.fetchCourses();
    },

    handleSomeCoursesClick() {
      this.searchCourses();
    },

    //提交搜索词
    onSearch() {
      this.isActive = false;
      console.log("搜索关键词:", this.searchKeyword);
    },

    //提交课程类型
    onCourseTypeChange(courseType) {
      this.isActive = false;
      this.selectedCourseType = courseType;
      console.log("选中的课程类别:", courseType);
    },

    //提交价格区间
    onPriceRangeChange(priceRange) {
      this.isActive = false;
      this.selectedPriceRange = priceRange;
      const range = this.priceRanges.find((r) => r.label === priceRange);
      console.log("选中的价格区间:", range.min, range.max);
    },

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

    showRecommendation() {
      // 弹出推荐课程的模态框逻辑
      console.log("显示推荐课程模态框");
    },
    submitInput() {
      // 将输入框的内容发送给后端
      console.log(`提交的内容: ${this.inputValue}`);
      // 发送请求到后端
      // axios.post('/api/filter', { [this.selectedOption]: this.inputValue })
      //   .then(response => {
      //     // 处理后端返回的筛选结果
      //   });
    },

    showRecommendation() {
      // 判断用户是否有上过课程
      if (this.courses.length === 0) {
        this.dialogMessage = "您无课程记录，请先选择课程，才能根据您的喜好推荐";
      } else {
        // 根据用户喜好推荐课程的逻辑
        this.dialogMessage = "根据您的课程记录，推荐的课程如下：...";
        // 更新 filteredCourses，显示推荐的课程
      }
      this.isDialogVisible = true;
    },

    //-------------------------------------- API接口------------------------------------------------------
    //大厅获取课程API
    fetchCourses() {
      const token = localStorage.getItem("token");
      return axios
        .get(`http://localhost:8080/api/Course/GetAllCourse?token=${token}`)
        .then((response) => {
          console.log("获取课程成功:", response.data);

          this.courses = response.data.map((item) => {
            if (item.features) {
              item.features = item.features.split("#");
            }
            return item;
          });
          console.log(this.courses);
        })
        .catch((error) => {
          console.error("Error fetching courses:", error);
          ElNotification({
            title: "错误",
            message: "获取所有课程时发生错误，请稍后再试。",
            type: "error",
          });
          throw error;
        });
    },

    // 搜索课程(已完结)
    searchCourses() {
      const token = localStorage.getItem("token");
      axios
        .get(`http://localhost:8080/api/Course/SearchCourse`, {
          params: {
            token: token,
            keyword: this.searchKeyword, //需要注意，keyword不能为空！！
            typeID: this.getCourseValue(this.selectedCourseType),
            minPrice: this.min,
            maxPrice: this.max,
          },
        })
        .then((response) => {
          console.log("筛选课程成功:", response.data);
          this.courses = [];
          this.courses = response.data.map((item) => {
            if (item.features) {
              item.features = item.features.split("#");
            }
            return item;
          });
          console.log(this.courses);
        })
        .catch((error) => {
          console.error("筛选课程失败:", error);
          ElNotification({
            title: "错误",
            message: "获取所有课程时发生错误，请稍后再试。",
            type: "error",
          });
          throw error;
        });
    },
  },
};
</script>

<style scoped>
.boldd {
  font-weight: bolder;
}
.trans {
  position: absolute;
  top: 13%;
  left: 74%;
  z-index: 9999;
  width: 300px;
}

.trans .el-button {
  padding: 10px 20px;
  font-size: 16px;
  width: 130px;
  height: 40px;
}

.hero-section {
  position: relative;
  width: 160vh;
  height: 80vh;
  background-image: url("https://www.lesmills.com.cn/uploads/20230316/f3e86ab4e56683a1481b0c0b4562d799.png");
  background-size: cover;
  background-position: center;
  margin-top: 30px;
}

.hero-title {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  color: white;
  font-size: 3rem;
  font-weight: bold;
}

.bottom-bar {
  margin-bottom: -50px;
  margin-left: 10%;
  position: absolute;
  bottom: 0;
  width: 80%;
  padding: 10px 20px;
  background-color: white;
  border-bottom: 5px solid black; /* 控制黑色底边的厚度 */
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.left-image {
  width: 100px;
  height: auto;
}

.all-courses {
  cursor: pointer;
  color: black;
  text-decoration: none;
  margin-right: 100px;
  font-size: 1.2rem;
}

.all-courses.active {
  font-weight: bolder;
  color: red;
  position: relative;
}

.all-courses.active::after {
  content: "";
  position: absolute;
  left: 0;
  bottom: -5px;
  width: 100%;
  height: 2px;
  background-color: red;
}

.search-container span.active {
  color: red;
  border-bottom: 2px solid red;
}

.filters .el-dropdown-link {
  color: initial;
}

.filters .el-dropdown-menu .el-dropdown-item.selected {
  background-color: initial;
  color: initial;
}

.search-container {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.search-input {
  width: 300px;
}

.filters {
  display: flex;
  justify-content: space-between;
  gap: 20px;
}

.dropdown {
  display: flex;
  align-items: center;
}

.el-dropdown-link {
  cursor: pointer;
  display: flex;
  align-items: center;
  gap: 5px;
}

.selected {
  background-color: #f0f9eb;
  color: #67c23a;
}

.red-box {
  background-color: red;
  color: white;
  padding: 10px 20px;
  cursor: pointer;
}

.input-box {
  margin-left: 20px;
}

.input-box input {
  padding: 5px;
  margin-right: 10px;
}

.input-box button {
  padding: 5px 10px;
  background-color: blue;
  color: white;
  cursor: pointer;
}

.course-grid {
  position: absolute;
  top: calc(100% + 25px); /* 在 bottom-bar 正下方 20px */
  left: 0;
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  row-gap: 20px;
  column-gap: 50px;
  margin-left: 180px;
}
</style>
