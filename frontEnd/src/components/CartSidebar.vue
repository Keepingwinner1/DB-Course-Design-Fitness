<template>
  <div>
    <!-- 显示购物车按钮 -->
    <el-button type="primary" @click="toggleCartSidebar" class="cart-button">
      课程购物车
    </el-button>

    <transition name="slide">
      <div v-if="isCartVisible" class="cart-sidebar">
        <!-- 收回箭头 -->
        <el-icon style="font-size: 20px"><ShoppingTrolley /></el-icon>
        <b>课程购物车</b>
        <div class="cart-header">
          收起
          <el-icon @click="toggleCartSidebar" style="font-size: 24px">
            <Right />
          </el-icon>
        </div>
        <div class="cart-titles">
          <span>课程名称</span>
          <span>课程单价</span>
          <span>课程时间</span>
          <span>操作</span>
        </div>
        <!-- 购物车内容 -->
        <div
          v-for="(course, index) in cartCourses"
          :key="index"
          class="cart-item"
        >
          <el-checkbox v-model="course.selected" />
          <img
            :src="course.coursePhotoUrl"
            alt="课程图片"
            class="course-image"
          />
          <span>{{ course.courseName }}</span>
          <span
            >{{ course.coursePrice }}<el-icon><Coin /></el-icon
          ></span>
          <span
            >{{ formatDate(course.courseStartTime) }}至{{
              formatDate(course.courseEndTime)
            }}</span
          >
          <span class="delete-text" @click="removeCourse(index)">删除</span>
        </div>
        <!-- 统计信息 -->
        <div class="cart-summary">
          <span>
            共{{ cartCourses.length }}个课程，已选择{{
              selectedCourses.length
            }}个，课程共计：
            <strong class="total-price"
              >{{ totalPrice }}<el-icon><Coin /></el-icon
            ></strong>
          </span>
          <el-button type="primary" class="checkout-button" @click="checkout"
            >下单结算</el-button
          >
        </div>
      </div>
    </transition>
  </div>
</template>

<script>
import axios from "axios";
import { ElMessage, ElMessageBox } from "element-plus";

export default {
  props: {
    cartCourses: {
      type: Array,
      required: true,
      default: () => [],
    },
    isCartVisible: {
      type: Boolean,
      required: true,
      default: true,
    },
    usercourses: {
      type: Array,
      required: true,
      default: () => [],
    },
  },
  data() {
    return {
      // 这里可以放置一些数据属性，如果需要的话
      vitalityCoins: 0,
    };
  },

  mounted() {
    this.getVigorTokenBalance();
  },

  computed: {
    selectedCourses() {
      return this.cartCourses.filter((course) => course.selected);
    },
    totalPrice() {
      return this.selectedCourses.reduce(
        (total, course) => total + course.coursePrice,
        0
      );
    },
  },
  methods: {
    formatDate(dateTimeString) {
      return dateTimeString.split("T")[0];
    },
    toggleCartSidebar() {
      this.$emit("update:isCartVisible", !this.isCartVisible);
    },
    removeCourse(index) {
      this.$emit("removeCourse", index);
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

    // 计算所选课程的总价格
    checkout() {
      const bookIDList = [];

      // 遍历选中的课程，并将它们的 bookID 添加到 bookIDList 数组中
      this.selectedCourses.forEach((course) => {
        bookIDList.push(course.bookID);
      });
      // 检查余额是否足够
      if (this.vitalityCoins >= this.totalPrice) {
        const token = localStorage.getItem("token");
        //调用支付接口(完结版)
        axios
          .post("http://localhost:8080/api/Course/PayCourseFare", {
            token: token,
            bookID: bookIDList,
            payMethod: "vigor",
          })
          .then((response) => {
            //  this.UPDATE_VITALITY_COINS( this.totalPrice);
            this.$store.commit("ADD_COURSES_TO_USER", this.selectedCourses);
            this.selectedCourses.forEach((course) => {
              const index = this.$store.state.cartCourses.indexOf(course);
              if (index !== -1) {
                this.removeCourse(index);
              }
            });
            ElMessageBox.alert(
              `下单成功！您剩余的活力币余额为：${this.vitalityCoins}`,
              "订单确认",
              {
                confirmButtonText: "确定",
                type: "success",
              }
            );
          });
      } else {
        // 如果余额不足，弹出错误提示
        ElMessage({
          message: "余额不足，无法完成结算。",
          type: "error",
        });
      }
    },
  },
  watch: {
    // 如果需要监听某些数据变化并做出响应，可以在这里添加 watch 选项
  },
};
</script>

<style scoped>
.cart-button {
  margin-bottom: 20px;
}

.cart-sidebar {
  position: fixed;
  top: 0;
  right: 0;
  width: 400px;
  height: 100%;
  background-color: #fff;
  box-shadow: -2px 0 5px rgba(0, 0, 0, 0.1);
  padding: 20px;
  z-index: 1000;
}

.cart-header {
  display: flex;
  justify-content: flex-end;
}

.cart-titles {
  margin-top: 20px;
  display: flex;
  justify-content: space-between;
  margin-bottom: 10px;
  font-weight: bold;
}

.cart-item {
  display: flex;
  align-items: center;
  margin-bottom: 10px;
}

.cart-item > * {
  margin-right: 20px; /* 调整子元素之间的间距 */
}

.course-image {
  width: 100px;
  height: 100px;
  margin-left: 15px;
}

.delete-text {
  color: red;
  cursor: pointer;
}

.delete-text:hover {
  text-decoration: underline;
}

.cart-summary {
  margin-top: 20px;
  font-size: 16px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.total-price {
  color: red;
  font-weight: bold;
}

.checkout-button {
  background-color: #409eff;
  color: #fff;
  margin-left: auto;
}
</style>
