<template>
  <div
    class="course-card"
    :style="{ width: width + 'px', height: height + 'px' }"
    :class="{ hovered: hovered }"
    @mouseover="hovered = true"
    @mouseleave="hovered = false"
  >
    <div class="image-section">
      <img :src="homecourse.coursePhotoUrl" alt="Course Image" />
    </div>
    <div class="content-section">
      <h3 class="bigcoursename">
        <b>{{ homecourse.courseName }}</b>
      </h3>
      <div v-if="hovered" class="description">
        <p class="coursefeature">
          <b>{{ homecourse.courseName }}</b>
        </p>
        <p>{{ homecourse.courseDescription }}</p>
        <button @click="showModal = true">进入课程</button>
      </div>
    </div>
  </div>

  <!-- 课程模态框 ,1.有isbooked,2.有教练信息-->
  <CourseModal
    v-if="showModal"
    :isVisible="showModal"
    :thecourse="homecourse"
    @close="showModal = false"
  />
</template>

<script>
import { ref } from "vue";
import CourseModal from "../components/CourseModal.vue";

export default {
  name: "CourseHome",
  components: {
    CourseModal,
  },

  props: {
    //homecourse包含教练相关信息，包含isbooked字段
    homecourse: Object,
    width: {
      type: Number,
      default: 300,
    },
    height: {
      type: Number,
      default: 400,
    },
  },

  data() {
    return {
      showModal: false,
    };
  },

  setup(props, { emit }) {
    const hovered = ref(false);

    return {
      hovered,
    };
  },
};
</script>

<style scoped>
.bigcoursename {
  font-family: "ZCOOL QingKe HuangYou", cursive;
  font-size: 1.3rem;
}
.course-card {
  display: flex;
  flex-direction: column;
  border: 1.5px solid #fefefe;
  transition:
    transform 0.3s,
    box-shadow 0.3s;
  overflow: hidden;
  position: relative;
}

.course-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.3);
}

.image-section {
  flex: 1 1 50%;
  background-size: cover;
  background-position: center;
  position: relative;
  overflow: hidden;
}
.coursefeature {
  font-size: larger;
  margin-top: -20px;
}
.image-section img {
  width: 100%;
  height: 100%;
  transition: filter 0.3s;
}

.course-card.hovered .image-section img {
  filter: blur(5px);
}

.content-section {
  flex: 1;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  padding: 20px;
  background-color: rgb(251, 246, 246);
  transition: background-color 0.3s;
}

.course-card.hovered .content-section {
  background-color: red;
}

.description {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  height: 100%;
  color: white;
}

.description p {
  margin-bottom: 1rem; /* 设置段落之间的间距 */
}

/* 确保最后一个段落没有底部边距 */
.description p:last-child {
  margin-bottom: 0;
}

button {
  padding: 10px 20px;
  background-color: white;
  color: red;
  border: none;
  cursor: pointer;
  margin-top: 20px;
}

.course-card:hover .content-section h3 {
  visibility: hidden;
}
</style>
