<template>
  <div class="icon-course-container">
    <div class="icon-container">
      <el-icon>
        <ChatSquare />
      </el-icon>
      <span class="hot-text"><b class="bolder">HOT</b></span>
    </div>

    <el-card class="my-card">
      <template #header>
        <div class="card-header">
          <div class="title-left">
            <b class="bolder">继续学习</b>
            <b class="title-right">{{ thecourse.courseName }}</b>
            <span class="learning-status">学习中</span>
          </div>
          <div class="icoin-container">
            <el-icon
              :style="{ fontSize: '24px', marginRight: '20px' }"
              @click="handleStarClick"
            >
              <Star />
            </el-icon>
            <el-icon :style="{ fontSize: '24px' }" @click="handleChatClick">
              <ChatDotRound />
            </el-icon>
          </div>
        </div>
      </template>
      <div class="card-firstrow">
        <el-icon>
          <CaretRight />
        </el-icon>
        <b class="bolder">学习进度</b>
        <div class="course-progress" :style="progressStyle">
          {{ nowprogress }}节课/{{ allprogress }}节课
        </div>
      </div>
      <div class="card-secondrow">
        <el-icon>
          <CaretRight />
        </el-icon>
        <b class="bolder">有效日期</b>
        <div class="course-time" :style="timeStyle">
          {{ courseStartTim }} - {{ courseEndTim }}
        </div>
      </div>
    </el-card>
    <el-card class="continue-learn">
      <template #header class="header2"> </template>
      <div class="continue-btn" @click="showModal = true">查看课程</div>
    </el-card>
  </div>

  <!-- 课程模态框,你这里isbooked就必须是1 -->
  <CourseModal
    v-if="showModal"
    :isVisible="showModal"
    :thecourse="thecourse"
    @close="showModal = false"
  />

  <!-- 课程结束前弹窗 -->
  <el-dialog v-model="showDialog" title="提示">
    <p>请课程结束再评价课程</p>
    <template #footer>
      <el-button @click="showDialog = false">知道了</el-button>
    </template>
  </el-dialog>

  <!-- 评分弹窗 -->
  <el-dialog v-model="showRateDialog" title="课程评分">
    <el-rate
      v-model="ratingValue"
      :texts="['oops', 'disappointed', 'normal', 'good', 'great']"
      show-text
    />
    <template #footer>
      <el-button @click="submitRating">提交</el-button>
      <el-button @click="showRateDialog = false">取消</el-button>
    </template>
  </el-dialog>

  <!-- 评论输入弹窗 -->
  <el-button v-if="isCommentVisible" @click="isCommentVisible = false"
    >收起评论</el-button
  >
  <Comment
    v-if="isCommentVisible"
    :myName="this.userName"
    :myHeader="this.userIcon"
    :comments="comments"
  />
</template>

<script>
import CourseModal from "../components/CourseModal.vue";
import Comment from "../components/Comment.vue";

export default {
  components: {
    CourseModal,
    Comment,
  },
  name: "CourseCard",

  //传入的是GetParticipatedCourseByUserID API的数据
  props: {
    thecourse: Object,
  },

  data() {
    return {
      showModal: false, //课程视窗
      showDialog: false, //message弹窗
      showRateDialog: false,
      isCommentVisible: false, // 控制Comment组件的显示状态
      nnames: [
        "阿南",
        "阿贝",
        "小兰",
        "飞飞",
        "大强",
        "春花",
        "秋月",
        "天明",
        "晓晓",
        "冰冰",
        "灵儿",
        "小虎",
        "小龙",
        "美美",
        "大牛",
        "乐乐",
        "小剑",
        "冬梅",
        "云云",
        "浩天",
        "青青",
        "小冬",
        "晓丽",
        "明明",
        "小叶",
        "晶晶",
        "天宇",
        "小雪",
        "春雷",
        "小鹏",
        "兰兰",
        "小雨",
        "晨曦",
        "夏天",
        "阿凡",
        "梦瑶",
        "小文",
        "小芳",
        "阿磊",
        "小白",
        "小黑",
        "志强",
        "小豆",
        "阳阳",
        "小溪",
        "阿琪",
        "冰雨",
        "雪儿",
        "阿丽",
        "小辉",
        "小川",
        "小东",
        "大海",
        "小星",
        "阿宏",
        "小霞",
        "阿阳",
        "小薇",
        "天乐",
        "小江",
      ],
      hheadImgs: [
        "https://tse2-mm.cn.bing.net/th/id/OIP-C.9hA8vMz6W3sKhZWAg_5s0QHaHb?w=185&h=186&c=7&r=0&o=5&pid=1.7",
        "data:image/jpeg;base64,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",
        "https://tse1-mm.cn.bing.net/th/id/OIP-C.YFqyjRzW9wUC4queALK9WgHaHa?w=210&h=210&c=7&r=0&o=5&pid=1.7",
        "https://tse1-mm.cn.bing.net/th/id/OIP-C.Evn73SyuC6RhiBpl0lDBeQAAAA?w=138&h=184&c=7&r=0&o=5&pid=1.7",
        "https://tse2-mm.cn.bing.net/th/id/OIP-C.cdIyvvTCPwXmD9kB6zdXPwHaHO?w=177&h=192&c=7&r=0&o=5&pid=1.7",
        "https://tse4-mm.cn.bing.net/th/id/OIP-C.hD5eYQVF5UwJU-wAQrdUlQHaHZ?w=193&h=192&c=7&r=0&o=5&pid=1.7",
        "https://tse3-mm.cn.bing.net/th/id/OIP-C.qc3m4tw8MVM72Nz5VN3ajwHaHa?w=184&h=196&c=7&r=0&o=5&pid=1.7",
        "https://tse3-mm.cn.bing.net/th/id/OIP-C.vkjIV2D2-bqCohlChvyNGgHaHa?w=185&h=186&c=7&r=0&o=5&pid=1.7",
        "https://tse2-mm.cn.bing.net/th/id/OIP-C.auZ3qASEM3HxK4TabAbqGAHaHI?w=192&h=185&c=7&r=0&o=5&pid=1.7",
        "https://tse1-mm.cn.bing.net/th/id/OIP-C.lnSumWf9zD8u_isJRWhbWgHaHa?w=186&h=186&c=7&r=0&o=5&pid=1.7",
        "https://tse3-mm.cn.bing.net/th/id/OIP-C.BMTFucrvF20S3DOeZ8eujwHaHa?w=186&h=186&c=7&r=0&o=5&pid=1.7",
        "https://tse2-mm.cn.bing.net/th/id/OIP-C.ytEr-Lco76ADfuWCO9NmsAHaHa?w=211&h=211&c=7&r=0&o=5&pid=1.7",
        "https://tse1-mm.cn.bing.net/th/id/OIP-C.W3jHpBEe6ZYM1xqlOOC5cQAAAA?w=150&h=180&c=7&r=0&o=5&pid=1.7",
        "https://tse3-mm.cn.bing.net/th/id/OIP-C.9mIjyI67QA1nFqwYGnp15gHaHa?w=211&h=211&c=7&r=0&o=5&pid=1.7",
        "https://tse1-mm.cn.bing.net/th/id/OIP-C.wFdNNtCWCHeCCsT_C4xULQHaHa?w=169&h=193&c=7&r=0&o=5&pid=1.7",
        "https://tse1-mm.cn.bing.net/th/id/OIP-C.2HLKW1eECkOWntqY9Tii-AHaHa?w=185&h=185&c=7&r=0&o=5&pid=1.7",
        "https://tse3-mm.cn.bing.net/th/id/OIP-C.CFF9Y0b46hNRRj9f8RzPqwHaHa?w=211&h=211&c=7&r=0&o=5&pid=1.7",
        "https://tse2-mm.cn.bing.net/th/id/OIP-C.THAKWk5QeZBuad4c8Vl9sgHaHa?w=186&h=186&c=7&r=0&o=5&pid=1.7",
        "https://tse4-mm.cn.bing.net/th/id/OIP-C.iEYUcYhgVVnoXKCdN5nu8AHaHa?w=210&h=211&c=7&r=0&o=5&pid=1.7",
        "https://tse4-mm.cn.bing.net/th/id/OIP-C.8OLiHrGdFFQnl1eqn02QnAHaHa?w=211&h=211&c=7&r=0&o=5&pid=1.7",
        "https://tse1-mm.cn.bing.net/th/id/OIP-C.usb3nFn-3GGGyfmLX7dxTwHaHa?w=204&h=204&c=7&r=0&o=5&pid=1.7",
        "https://tse1-mm.cn.bing.net/th/id/OIP-C.yWBCfRFitRu8AC-23xzIwQHaHa?w=186&h=186&c=7&r=0&o=5&pid=1.7",
        "https://tse2-mm.cn.bing.net/th/id/OIP-C.EOWnCahIQxhCrry_pUL8wwHaHa?w=211&h=211&c=7&r=0&o=5&pid=1.7",
        "https://tse3-mm.cn.bing.net/th/id/OIP-C.qxHJWv4BlWVO5dZE5hKJYwHaHa?w=186&h=186&c=7&r=0&o=5&pid=1.7",
        "https://tse3-mm.cn.bing.net/th/id/OIP-C.nsdGFeDCEKCskqDcIz6hsAHaHW?w=212&h=211&c=7&r=0&o=5&pid=1.7",
        "https://tse1-mm.cn.bing.net/th/id/OIP-C.6---axJGJeWyxrwSzNVifAHaHa?w=185&h=185&c=7&r=0&o=5&pid=1.7",
        "https://tse1-mm.cn.bing.net/th/id/OIP-C.3MNrlfzuhmzuqVplZZkc9AHaHW?w=186&h=185&c=7&r=0&o=5&pid=1.7",
        "https://tse4-mm.cn.bing.net/th/id/OIP-C.fO9tdqLn2Cn9ZVz4x-8VxwHaHa?w=176&h=180&c=7&r=0&o=5&pid=1.7",
        "https://tse1-mm.cn.bing.net/th/id/OIP-C.JO9twPt3E4tIM4gBfxlTigAAAA?w=211&h=211&c=7&r=0&o=5&pid=1.7",
        "https://tse4-mm.cn.bing.net/th/id/OIP-C.sfMHa7vLf1KhkLRk5szVhQHaHa?w=211&h=211&c=7&r=0&o=5&pid=1.7",
      ],
      ccomments: [
        "这个课程内容丰富，老师讲解生动，收获颇丰！",
        "教学方法很新颖，课程非常吸引人！",
        "课程设计巧妙，老师讲解清晰，受益匪浅！",
        "老师专业知识深厚，课程内容实用，强烈推荐！",
        "课程节奏紧凑，老师热情洋溢，学习氛围极佳！",
        "课程内容深入浅出，老师讲解耐心，学习效果显著！",
        "授课风格幽默风趣，课程内容引人入胜！",
        "课程结构合理，老师讲解详细，学习体验极佳！",
        "老师的指导非常到位，课程内容让人印象深刻！",
        "课程内容全面，老师讲解透彻，学习过程愉快！",
        "老师的讲解非常专业，课程内容实用，值得学习！",
        "课程内容精彩，老师的授课方式让人耳目一新！",
        "老师的讲解条理清晰，课程内容丰富多彩，赞！",
        "课程内容扎实，老师的授课态度认真，学习效果好！",
        "老师的授课风格独特，课程内容新颖，学习体验棒！",
        "课程内容充实，老师的讲解方式让人容易理解！",
        "老师的授课技巧高超，课程内容精彩，学习收获大！",
        "课程内容翔实，老师的讲解生动，学习兴趣浓厚！",
        "老师的讲解深入浅出，课程内容引人入胜，学习体验好！",
        "这位老师的课堂充满活力，让人受益匪浅。",
        "课程内容深入浅出，老师的讲解让人茅塞顿开。",
        "老师的耐心指导，让复杂的知识点变得简单易懂。",
        "课程设计巧妙，每个环节都让人充满期待。",
        "老师的幽默风趣，让学习变得轻松愉快。",
        "课程内容丰富，老师的讲解细致入微。",
        "老师的专业知识令人敬佩，课程质量上乘。",
        "课程的互动环节设计得非常好，增强了学习体验。",
        "老师的讲解清晰，让人对知识点有了深刻的理解。",
        "课程的实践操作部分非常实用，提高了我的动手能力。",
        "老师的鼓励和支持，让我在学习中更加自信。",
        "课程的案例分析非常到位，让我对知识有了更深的认识。",
        "老师的授课方式新颖，让人耳目一新。",
        "课程的难度适中，既不会让人感到压力，也能学到东西。",
        "老师的讲解逻辑清晰，让人容易跟上思路。",
        "课程的组织结构合理，每个部分都紧密相连。",
        "老师的激情感染了每一位学生，课堂氛围极佳。",
        "每一次的汗水，都是成长的见证。",
        "坚持不懈的训练，带来难以言喻的满足感。",
        "痛苦只是暂时的，收获的喜悦却是永恒的。",
        "每一滴汗水，都是离目标更近一步的证明。",
        "身体的极限只是心理的起点，继续前进吧！",
        "健身的道路充满挑战，但成果永远不会背叛你。",
        "奋斗的过程总是伴随着疲惫，但成就感弥补一切。",
        "训练时的痛苦，会在成功的瞬间化为快乐的泪水。",
        "汗水和坚持，才是对梦想最好的回应。",
        "每一次的突破，都是对自己极限的挑战。",
        "这个老师喜欢在课程开始前讲个小故事。",
        "老师总是细致入微地关注每个学员的进步。",
        "他的教学风格非常幽默，课堂气氛轻松愉快。",
        "老师喜欢用真实案例来帮助我们理解动作背后的原理。",
        "她有一个习惯，总会鼓励我们多提问题。",
        "老师性格开朗，和学员的互动特别有趣。",
        "他非常严谨，每个动作的细节都会仔细纠正。",
      ],
      comments: [
        {
          name: "阿南",
          headImg:
            "data:image/jpeg;base64,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",
          comment: "这个课的老师特别认真，特别热情，能学到很多！",
          time: "2019年9月16日 18:43",
          commentNum: 2,
          like: 15,
          inputShow: false,
          reply: [
            {
              from: "飞飞",
              fromHeadImg:
                "https://ae01.alicdn.com/kf/H94c78935ffa64e7e977544d19ecebf06L.jpg",
              comment: "我也很喜欢这个课！！",
              time: "2019年9月16日 18:43",
              commentNum: 1,
              like: 15,
              inputShow: false,
            },
            {
              from: "Ariana Grande",
              fromHeadImg:
                "https://www.nd027.com/uploads/allimg/181229/1-1Q2291A155-50.jpg",
              comment: "谢谢你的支持",
              time: "2019年9月16日 18:43",
              commentNum: 0,
              like: 5,
              inputShow: false,
            },
          ],
        },
        {
          name: "不瘦10斤不改名",
          headImg:
            "https://tse4-mm.cn.bing.net/th/id/OIP-C.1vR3VjUC3xVg9iAIURF5PwHaHa?w=208&h=209&c=7&r=0&o=5&pid=1.7",
          comment: "永远记得最后一组结束的快乐",
          time: "2019年9月16日 18:43",
          commentNum: 1,
          like: 5,
          inputShow: false,
          reply: [
            {
              from: "小铭",
              fromHeadImg:
                "https://tse1-mm.cn.bing.net/th/id/OIP-C.jIURlW4dR3I4tVG1h3NcVAAAAA?w=198&h=198&c=7&r=0&o=5&pid=1.7",
              to: "Taylor Swift",
              comment: "最痛苦与最快乐相约而至",
              time: "2019年9月16日 18:43",
              commentNum: 25,
              like: 5,
              inputShow: false,
            },
          ],
        },
        {
          name: "li",
          headImg:
            "https://tse2-mm.cn.bing.net/th/id/OIP-C.O1dw-_dDk7WZCI1XK7lXiQAAAA?w=210&h=210&c=7&r=0&o=5&pid=1.7",
          comment: "That's Great!",
          time: "2019年9月16日 18:43",
          commentNum: 0,
          like: 5,
          inputShow: false,
          reply: [],
        },
      ],
      ratingValue: 0,
      inputText: "",
      nowprogress: 0,
      allprogress: 0,
      courseStartTim: new Date(this.thecourse.courseStartTime)
        .toISOString()
        .split("T")[0],
      courseEndTim: new Date(this.thecourse.courseEndTime)
        .toISOString()
        .split("T")[0],
    };
  },
  created() {
    this.generateRandomComments();
  },

  computed: {
    progressStyle() {
      return {
        color: "#337ecc",
        fontWeight: "bold",
        marginLeft: "10px",
      };
    },
    timeStyle() {
      return {
        color: "#337ecc",
        fontWeight: "bold",
        marginLeft: "10px",
      };
    },
  },

  mounted() {
    this.calculateProgress();
    this.userName = localStorage.getItem("name");
    this.email = localStorage.getItem("email");
    this.userIcon = localStorage.getItem("iconUrl");
  },

  methods: {
    generateRandomComments() {
      this.comments[0].name =
        this.nnames[Math.floor(Math.random() * this.nnames.length)];
      this.comments[1].name =
        this.nnames[Math.floor(Math.random() * this.nnames.length)];
      this.comments[2].name =
        this.nnames[Math.floor(Math.random() * this.nnames.length)];
      this.comments[0].headImg =
        this.hheadImgs[Math.floor(Math.random() * this.hheadImgs.length)];
      this.comments[1].headImg =
        this.hheadImgs[Math.floor(Math.random() * this.hheadImgs.length)];
      this.comments[2].headImg =
        this.hheadImgs[Math.floor(Math.random() * this.hheadImgs.length)];
      this.comments[0].comment =
        this.ccomments[Math.floor(Math.random() * this.ccomments.length)];
      this.comments[1].comment =
        this.ccomments[Math.floor(Math.random() * this.ccomments.length)];
      this.comments[2].comment =
        this.ccomments[Math.floor(Math.random() * this.ccomments.length)];
      this.comments[0].reply[0].from =
        this.nnames[Math.floor(Math.random() * this.nnames.length)];
      this.comments[0].reply[1].from =
        this.nnames[Math.floor(Math.random() * this.nnames.length)];
      this.comments[0].reply[0].fromHeadImg =
        this.hheadImgs[Math.floor(Math.random() * this.hheadImgs.length)];
      this.comments[0].reply[1].fromHeadImg =
        this.hheadImgs[Math.floor(Math.random() * this.hheadImgs.length)];
      this.comments[0].reply[0].comment =
        this.ccomments[Math.floor(Math.random() * this.ccomments.length)];
      this.comments[0].reply[1].comment =
        this.ccomments[Math.floor(Math.random() * this.ccomments.length)];
      this.comments[1].reply[0].from =
        this.nnames[Math.floor(Math.random() * this.nnames.length)];
      this.comments[1].reply[0].fromHeadImg =
        this.hheadImgs[Math.floor(Math.random() * this.hheadImgs.length)];
      this.comments[1].reply[0].comment =
        this.ccomments[Math.floor(Math.random() * this.ccomments.length)];
    },
    //评分
    handleStarClick() {
      const currentTime = new Date();
      const courseEndTime = new Date(this.thecourse.courseEndTime);
      if (currentTime < courseEndTime) {
        // 如果课程未结束，显示提示弹窗
        this.showDialog = true;
      } else {
        this.showRateDialog = true;
      }
    },
    //评论
    handleChatClick() {
      this.isCommentVisible = !this.isCommentVisible;
    },

    //提交评分
    submitRating() {
      gradeCourse();
      this.showRateDialog = false;
    },

    //提交评价
    submitComment() {
      commentCourse();
      this.showInputDialog = false;
    },

    //计算课程进度
    calculateProgress() {
      const courseStartTime = new Date(this.courseStartTim);
      const courseEndTime = new Date(this.courseEndTim);

      const today = new Date();

      // 计算allprogress，即课程总天数
      const timeDiff = courseEndTime.getTime() - courseStartTime.getTime();
      console.log(this.thecourse);
      this.allprogress = Math.floor(
        (Math.ceil(timeDiff / (1000 * 60 * 60 * 24)) / 7) *
          this.thecourse.schedules.length
      ); // 总天数

      // 计算nowprogress
      if (today < courseStartTime) {
        this.nowprogress = 0;
      } else if (today > courseEndTime) {
        this.nowprogress = this.allprogress;
      } else {
        const currentTimeDiff = today.getTime() - courseStartTime.getTime();
        this.nowprogress = Math.floor(
          (Math.ceil(currentTimeDiff / (1000 * 60 * 60 * 24)) / 7) *
            this.thecourse.schedules.length
        ); // 今天距离开始的天数
      }
    },
    //-------------------------------------- API接口函数-----------------------------------------------------
    //用户上传对课程的评分(完结版)
    gradeCourse() {
      const token = localStorage.getItem("token");
      const classID = thecourse.classID;
      const postData = {
        token: token,
        classID: classID,
        grade: this.ratingValue,
      };
      axios
        .post(`http://localhost:8080/api/Course/GradeCourse`, postData)
        .then((response) => {
          console.log("评分成功:", response.data);
        })
        .catch((error) => {
          console.error("评分失败:", error);
        });
    },
    commentCourse() {
      const token = localStorage.getItem("token");
      const classID = thecourse.classID;
      const postData = {
        token: token,
        classID: classID,
        comment: this.inputText,
      };
      axios
        .post("http://localhost:8080/api/Course/PublishComment", postData)
        .then((response) => {
          console.log("评论成功:", response.data);
        })
        .catch((error) => {
          console.error("评论失败:", error);
        });
    },
  },
};
</script>

<style scoped>
.bolder {
  font-weight: bold;
}

.icon-course-container {
  display: flex;
  align-items: center;
}

.icon-container {
  position: relative;
  display: inline-flex;
  font-size: 30px;
  color: red;
  margin-right: -10px;
  margin-top: -60px;
}

.hot-text {
  position: absolute;
  top: 5px;
  left: 3px;
  font-size: 10px;
  color: red;
  font-weight: bold;
}

.my-card {
  width: 500px;
  height: 130px;
}

.card-header {
  display: flex;
  align-items: center;
  height: 10px;
  justify-content: space-between;
  width: 470px;
}

.title-left {
  display: flex;
  align-items: center;
  font-size: smaller;
  font-weight: bold;
}

.title-right {
  font-weight: bold;
  font-size: 1.2rem;
}

.title-left .bolder,
.title-left .title-right {
  margin-right: 10px;
}

.learning-status {
  padding: 2px 6px; /* 内边距 */
  background-color: white;
  color: orange; /* 文本颜色 */
  font-size: 12px; /* 字体大小 */
  border: 2px solid orange; /* 边框颜色 */
  border-radius: 3px; /* 边框圆角 */
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1); /* 阴影效果 */
}

.icoin-container {
  display: flex;
  align-items: center;
  cursor: pointer;
}

.card-firstrow {
  display: flex;
  align-items: center;
  margin-top: 5px;
  margin-bottom: 7px;
}

.card-secondrow {
  display: flex;
  align-items: center;
}

.continue-learn {
  margin-top: 1px;
  height: 134px;
  padding: 20px;
}

.continue-btn {
  display: block;
  text-align: center;
  padding: 5px;
  width: 120px;
  background-color: orange;
  color: white;
  font-weight: bold;
  cursor: pointer;
  user-select: none;
  border-radius: 3px;
  margin-top: -10px;
}
</style>
