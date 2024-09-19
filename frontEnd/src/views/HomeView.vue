<template>
  <navigation-bar />
  <div class="home-page">
    <video ref="videoPlayer" class="background-image" autoplay loop muted>
      <source src="../assets/videos/HomeVideo.mp4" type="video/mp4">
      Your browser does not support the video tag.
    </video>
    <div class="contant">
      <!--<h2 style="margin-top: 3%;margin-bottom: 3%;margin-left: 15%;text-align: left;"><el-image :src="contact" :style="{ width: '15%' ,verticalAlign: 'middle' }"></el-image>联系我们:</h2>-->
      <el-button circle>
        <el-link :underline="false" :href="githubUrl" target="_blank" class="custom-link">
          <span class="github-icon"></span>
        </el-link>
      </el-button>
      <el-tooltip content="QQ号: 2465973463" placement="bottom-end" effect="light">
        <el-button circle>
          <span class="QQ-icon"></span>
        </el-button>
      </el-tooltip>
      <el-tooltip content="微信号: zwc2465973463" placement="bottom" effect="light">
        <el-button circle>
          <span class="Wechat-icon"></span>
        </el-button>
      </el-tooltip>
      <el-tooltip content="邮箱：wcheng@tongji.edu.cn" placement="bottom-start" effect="light">
        <el-button circle>
          <span class="Telephone-icon"></span>
        </el-button>
      </el-tooltip>
    </div>
  </div>

</template>

<script>
import { ref } from 'vue'
import contact from '../assets/images/contact.png';

export default {
  components: {},
  data() {
    return {
      githubUrl: ref('https://github.com/Zwc2003/DB-Fitness'),
      contact
    }
  }
};
</script>

<style scoped>
#app {
  overflow: hidden;
}

.avatar {
  width: 100%;
  height: auto;
}

.contant {
  background-color: rgba(255, 255, 255, 0.7);
  vertical-align: middle;
  border-radius: 20px;
  width: auto;
  height: auto;
  padding: 5px;
  position: fixed;
  bottom: 1vw;
  right: 1vw;
}


.github-icon {
  display: inline-block;
  width: 33px;
  height: 34px;
  background-image: url('../assets/images/github.png');
  background-size: contain;
  background-repeat: no-repeat;
  vertical-align: middle;
  margin: 1vw;
}

.QQ-icon {
  display: inline-block;
  width: 35px;
  height: 35px;
  background-image: url('../assets/images/QQ.png');
  background-size: contain;
  background-repeat: no-repeat;
  vertical-align: middle;
  margin: 1vw;
}

.Wechat-icon {
  display: inline-block;
  width: 35px;
  height: 35px;
  background-image: url('../assets/images/微信.png');
  background-size: contain;
  background-repeat: no-repeat;
  vertical-align: middle;
  margin: 1vw;
}

.Telephone-icon {
  display: inline-block;
  width: 40px;
  height: 39px;
  background-image: url('../assets/images/邮箱.png');
  background-size: contain;
  background-repeat: no-repeat;
  vertical-align: middle;
  margin: 1vw;
}

.custom-link {
  width: 33px;
  height: 34px;
  border-radius: 15px;
}

.member-list {
  display: flex;
  justify-content: space-around;
  flex-wrap: wrap;
  margin-top: 1100px;
  margin-left: -35%;
}

.text {
  color: #0b2d47;
  font-weight: bold;
  font-size: 100px;
}

.background-image {
  overflow-y: hidden;
  position: absolute;
  top: 0px;
  left: 0;
  width: 100%;
  height: 100%;
  /* 使图片覆盖整个背景区域 */
  object-fit: fill;
  z-index: -1;
  /* 使背景图片位于最底层 */
}

.overlay-content {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  pointer-events: none;
  /* 使得点击事件不会影响到背景 */
}

.overlay-image {
  width: 500px;
  height: auto;
  /* 你可以添加更多样式来调整图片位置和大小 */
}
</style>
