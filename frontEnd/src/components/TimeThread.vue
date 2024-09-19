<template>
  <el-button  @click="loadWeeksData" class="button" type="primary">
    生成健身计划
  </el-button>


  <el-collapse class="list" v-model="activeName">
    <el-collapse-item
      v-for="(week, index) in weeks.plan"
      :key="index"
      :title="titles[index]"
      :name="index"
      class="custom-collapse-item"
    >
      <el-timeline class="line">
        <el-timeline-item
          v-for="(item, index2) in week"
          :key="index2"
          :timestamp="item.timestamp"
          class="custom-timeline-item"
          placement="top"
        >
          <el-container>
            <el-popover placement="right" :width="500" trigger="click">
              <template #reference>
                <el-card class="card" shadow="hover">
                  <img :src="item.coverUrl" alt="Event Image" class="event-image" />
                  <template #footer>{{ item.workoutName }}</template>
                </el-card>
              </template>
              <div style="padding-left: 0">
                <el-table :data="item.exercises" :row-style="{height:50+'px'}" stripe class="table">

                  <el-table-column prop="exerciseName" label="动作" width="150" />
                  <el-table-column prop="count" label="组数" />
                  <el-table-column prop="time" label="时间" />
                  <el-table-column label="演示">
                    <template v-slot="{ row }">
                      <div>
                        <img ref="imageRef" @click="open(row)" :src="row.coverUrl" alt="Click to open the Message Box" class="gif" />
                      </div>
                    </template>
                  </el-table-column>
                </el-table>
              </div>
              <el-button @click="finish(index,index2)" class="over" type="primary" :disabled="isButtonDisabled(index,index2) || buttonDisabled[index*7+index2]"> {{(isButtonDisabled(index,index2)|| buttonDisabled[index*7+index2])?'今日计划已完成':'完成计划'}}</el-button>
            </el-popover>
          </el-container>
        </el-timeline-item>
      </el-timeline>
    </el-collapse-item>
  </el-collapse>
  <el-popover placement="bottom" :width="400" trigger="click" >
    <template #reference>
      <el-button style="margin-right: 16px" class="map" type="primary">查看我的健身进度</el-button>
    </template>
    <el-calendar :range="[startDate, endDate]" >
      <!-- 自定义 date-cell 插槽 -->
      <template #date-cell="{ data }">
        <div :class="getCellStyle(data.day)"></div>
      </template>
    </el-calendar>
    <p>本月计划已完成<span class="bold-text">{{ count }}</span>天</p>
  </el-popover>


</template>

<style scoped>
.bold-text {
  font-size: 30px;
  font-weight: bold; /* 字体加粗 */
  color:red;
}
.button{
  position: absolute;
  left: 32%;
  top: 12px;
  width: 100px;
  height: 40px;
}
.map{
  position: absolute;
  left: 70%;
  top: 12px;
  width: 130px;
  height: 40px;
}
.list{
  position:absolute;
  left: 20%;
  top: 60px;
  width: 60%;
}
.line{
  position: relative;
  left: 10px;
  top: 10px;
}
.over{
  position: relative;
  margin-top: 10px;
  left: 38%;
}
/* 自定义 el-timeline-item 的时间戳样式 */
>>>.custom-timeline-item .el-timeline-item__timestamp {

  font-size: 16px;
  font-weight: bold;
  color: black; /* 设置时间戳的颜色 */
  padding: 5px 10px;
  border-radius: 5px;
  text-align: left;
}
>>>.custom-timeline-item .el-timeline-item__tail{
  border-color: red;
}
/* 自定义 el-timeline-item 的节点样式 */
>>>.custom-timeline-item .el-timeline-item__node {
  background-color: rgb(72, 147, 255); /* 设置节点的边框颜色 */

}
>>>.el-card:hover{
  box-shadow:5px 10px 16px #0ff !important;
}

>>>.el-card:active{
  box-shadow:5px 10px 16px #ff5858 !important;
}
>>>.el-calendar__header {
  display: none;
}
>>>.el-calendar-table thead th {
  display: none ;
}

>>> .el-calendar-table .el-calendar-day:hover {
  background-color: inherit;
}
p {
  font-size: 20px;
}
.card {
  max-width: 300px;
  height: 200px; /* 设定卡片的高度 */
}

.event-image {
  width: 100%; /* 图片宽度占满容器 */
  height: auto;
}
>>>.el-card__body{
  height: 166px;
}
>>>.el-card .el-card__footer{
  padding-top:0;
  color:white;
  font-size: 1.2em; /* 设置字体大小 */
  font-weight: bold; /* 设置字体加粗 */
  background-color: darkblue;
}

.table{
  width: 1500px;
  height:325px;
}
.gif {
  width: 50px; /* 设置图片的宽度 */
  height: 50px; /* 设置图片的高度 */
  object-fit: cover; /* 保持图片的宽高比，裁剪超出部分 */
}
.gif:hover {
  content: url(../assets/images/strength.png); /* 鼠标悬停时显示的图片路径 */
}
>>>.el-calendar-table .el-calendar-day{
  width: 60px;
  height: 40px;
}
>>>.el-calendar {
  width: 350px;
  height: 200px;
  padding-bottom: 0;
}

.calendar-day {
  height: 30px;
  width: 30px;
  /*background-color:blue;*/ 
  background-image: url('../assets/images/complete.png');
  background-size: contain;
  background-repeat: no-repeat;
}
.dialog-video{
  height:65%;
}
.el-container {
  width: 300px; /* 与卡片宽度一致 */
  height: 200px; /* 与卡片高度一致 */
}
</style>

<script lang="ts" setup>
import { ref, onMounted ,onBeforeMount,computed} from 'vue';
import {ElCalendar,ElMessageBox, ElNotification} from 'element-plus';
import axios from 'axios';
import dayjs from 'dayjs';

const buttonDisabled=ref([false, false, false, false, false, false, false,
  false, false, false, false, false, false, false,
  false, false, false, false, false, false, false,
  false, false, false, false, false, false, false,]);
const buttonText=ref(['完成计划','完成计划','完成计划','完成计划','完成计划','完成计划','完成计划',
  '完成计划','完成计划','完成计划','完成计划','完成计划','完成计划','完成计划',
  '完成计划','完成计划','完成计划','完成计划','完成计划','完成计划','完成计划',
  '完成计划','完成计划','完成计划','完成计划','完成计划','完成计划','完成计划',])
const DoneDates=ref(['2024-09-01','2024-09-02','2024-09-03','2024-09-04','2024-09-05','2024-09-06','2024-09-07',
  '2024-09-08','2024-09-09','2024-09-10','2024-09-11','2024-09-12','2024-09-13','2024-09-14',
  '2024-09-15','2024-09-16','2024-09-17','2024-09-18','2024-09-19','2024-09-20','2024-09-21',
  '2024-09-22','2024-09-23','2024-09-24','2024-09-25','2024-09-26','2024-09-27','2024-09-28',])
// 设置特定的开始日期，比如 2024年9月1日
const startDate = ref(dayjs('2024-09-01').toDate());
// 设置结束日期为28天后的日期
const endDate = ref(dayjs(startDate.value).add(27, 'day').toDate());
const Get = ref(false)
// 定义需要背景变蓝色的日期数组
/// 定义需要背景变蓝的日期数组
const count = ref(0)

function getCellStyle(day) {
  if (Get.value==false){
    Get.value=true
    axios.get('http://localhost:8080/api/FitnessPlan/GetPlan', {
      params: {
        token: localStorage.getItem('token')
      }
    }).then(response => {
      weeks.value = response.data;
      if(response.data.message == "fail"){
        return '';
      }
      else{
        for(let i=0;i<4;i++){
        for(let j=0;j<7;j++){
          if(weeks.value.plan[i][j].isCompleted=="true"){
            count.value=count.value+1
            DoneDates.value[i*7+j]='1';
          }
        }
      }
        console.log(DoneDates.value)}

    });
  }
  if(DoneDates.value.includes(day)){
    return '';}
  else{
    return 'calendar-day';
  }
}
// 判断是否是需要变蓝的日期


// 自定义日期单元格
const isSpecialDate = (date: Date) => {
  return date.getDate() === 15; // 假设每月的 15 日为特殊日期
};


const weeks = ref([]);
const titles = ["第一周", "第二周", "第三周", "第四周"];
const loading = ref(true);
const activeName = ref([0]);
const isDisable=ref(true);
function finish(index,index2){
  buttonDisabled.value[index*7+index2]=true;
  buttonText.value[index*7+index2]='今日计划已完成';
  count.value=count.value+1
  DoneDates.value[index*7+index2]='1';
  isButtonDisabled(index,index2);
  axios.get('http://localhost:8080/api/Achievement/UpdateFitnessPlanAchievement', {
    params: {
      token: localStorage.getItem('token'),
      workoutIndex:index*7+index2
    }
  })
}
function loadWeeksData() {
  loading.value = true;
  axios.get('http://localhost:8080/api/FitnessPlan/GetPlan', {
    params: {
      token: localStorage.getItem('token')
    }
  }).then(response => {
    weeks.value = response.data; // 假设数据结构中 weeks 在顶层
    if (response.data.message !== "fail") {
      ElNotification({
      message: "健身计划成功生成！",
      type: 'success',
      duration: 2000
    });
    }
    else{
      ElNotification({
        message: "请先填写体测表！",
        type: 'error',
        duration: 2000
      });
      return
    }
    //localStorage.setItem('savedFitnessPlan', JSON.stringify(weeks.value)); // 保存数据到本地存储
    loading.value = false;

  }).catch(error => {
    console.error('Failed to fetch data:', error);
    loading.value = false;
  });
}

function loadSavedPlan() {
    loadWeeksData();
}

const open = (row) => {
  ElMessageBox.alert(
    '<div class="video-div"><video controls autoplay width="90%" src="' + row.gifUrl + '" ></video></div>',
    row.explanation,
    {
      dangerouslyUseHTMLString: true,
      confirmButtonText: 'OK',
      customStyle: {
        'max-width': '45%',
        height: '90%'
      }
    }
  );
}
function isButtonDisabled(index,index2) {
  // 这里可以根据逻辑条件返回 true 或 false
  // 比如根据某个条件禁用按钮
  if(weeks.value.plan[index][index2].isCompleted=="true"){
    return true;
  }
  else{
    return false;
  }
}
onBeforeMount(() => {
  axios.get('http://localhost:8080/api/FitnessPlan/GetPlan', {
  params: {
    token: localStorage.getItem('token')
  }
}).then(response => {
  weeks.value = response.data;
  for(let i=0;i<4;i++){
    for(let j=0;j<7;j++){
      if(weeks.value.plan[i][j].isCompleted=="true"){
        console.log(111111)
        DoneDates[i*7+j]='1';
      }
    }
  }
});

  });
</script>
