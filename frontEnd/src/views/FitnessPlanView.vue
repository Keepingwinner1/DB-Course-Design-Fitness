<template>
  <navigation-bar/>
  <common-layout />
  <body class="container">
  <div>
    <el-button @click="dialogFormVisible = true" class="bot" type="primary">
      填写我的体测表
    </el-button>
  </div>
  <el-dialog v-model="dialogFormVisible" title="体测信息" width="400">
    <el-form :model="form" :rules="rules"  ref="ruleFormRef">
      <el-form-item label="身高(cm)" :label-width="formLabelWidth" prop="height">
        <el-input-number v-model.number="form.height" autocomplete="off"  :precision="2" :controls="false"></el-input-number>
      </el-form-item>
      <el-form-item label="体重(kg)" :label-width="formLabelWidth" prop="weight">
        <el-input-number v-model.number="form.weight" autocomplete="off"  :precision="2" :controls="false"></el-input-number>
      </el-form-item>
      <el-form-item label="BMI" :label-width="formLabelWidth" prop="BMI">
        <el-input-number v-model.number="form.BMI" autocomplete="off"  :precision="1" :controls="false"></el-input-number>
      </el-form-item>
      <el-form-item label="体脂率" :label-width="formLabelWidth" prop="bodyFatRate">
        <el-input-number v-model.number="form.bodyFatRate" autocomplete="off"  :precision="1" :controls="false"></el-input-number>
      </el-form-item>
      <el-form-item label="俯卧撑个数" :label-width="formLabelWidth" prop="pushups">
        <el-input-number v-model.number="form.pushups" autocomplete="off"  :controls="false"></el-input-number>
      </el-form-item>
      <el-form-item label="深蹲个数" :label-width="formLabelWidth" prop="squats">
        <el-input-number v-model.number="form.squats" autocomplete="off"  :controls="false"></el-input-number>
      </el-form-item>
      <el-form-item label="仰卧起坐个数" :label-width="formLabelWidth" prop="situps">
        <el-input-number v-model.number="form.situps" autocomplete="off"  :controls="false"></el-input-number>
      </el-form-item>
      <el-form-item label="引体向上个数" :label-width="formLabelWidth" prop="pullup">
        <el-input-number v-model.number="form.pullup" autocomplete="off"  :controls="false"></el-input-number>
      </el-form-item>
      <el-form-item label="一千米时间(s)" :label-width="formLabelWidth" prop="longDistance">
        <el-input-number v-model.number="form.longDistance" autocomplete="off"  :controls="false"></el-input-number>
      </el-form-item>
      <el-form-item label="健身目标" :label-width="formLabelWidth" prop="goal">
        <el-select v-model="form.goal" placeholder="请选择一个健身目标">
          <el-option label="减脂" value="loseWeight" />
          <el-option label="增肌" value="buildMuscle" />
          <el-option label="塑型" value="bodySculpting" />
        </el-select>
      </el-form-item>




    </el-form>
    <template #footer>
      <div class="dialog-footer">
        <el-button @click="dialogFormVisible = false">取消</el-button>
        <el-button  type="primary" @click="submitForm(ruleFormRef)">
          创建
        </el-button>
      </div>
    </template>
  </el-dialog>


  <TimeThread></TimeThread>
  </body>
</template>

<script setup lang="ts">

import { reactive, ref, onMounted } from 'vue'
import type { ComponentSize, FormInstance, FormRules } from 'element-plus'
import type { Action } from 'element-plus'
import TimeThread from "../components/TimeThread.vue";
import img from 'src/assets/images/background.jpg';
import {useRouter} from 'vue-router'
import axios from "axios";
import {ElNotification} from "element-plus";
const dialogFormVisible = ref(false)
const formLabelWidth = '110px'
const imagePath = 'src/assets/images/background.jpg';
interface RuleForm {
  height: number,
  weight: number,
  BMI: number,
  bodyFatRate:  number,
  pushups:  number,
  squats: number,
  situps:  number,
  pullup:  number,
  longDistance: number,
  goal: string,
}
const ruleFormRef = ref<FormInstance>()
const form = reactive<RuleForm>({
  height: null,
  weight: null,
  BMI: null,
  bodyFatRate:null,
  pushups: null,
  squats: null,
  situps: null,
  pullup: null,
  longDistance:null,
  goal:'',
})
const rules = reactive<FormRules<RuleForm>>({
  height: [
    { required: true, message: '请输入身高', trigger: 'blur' },
    { type: 'number', message: '请输入数字', trigger: 'blur' },
  ],
  weight: [
    { required: true, message: '请输入体重', trigger: 'blur' },
    { type: 'number', message: '请输入数字', trigger: 'blur' },
  ],
  BMI: [
    { required: true, message: '请输入BMI', trigger: 'blur' },
    { type: 'number', message: '请输入数字', trigger: 'blur' },
  ],
  bodyFatRate: [
    { required: true, message: '请输入体脂率', trigger: 'blur' },
    { type: 'number', message: '请输入数字', trigger: 'blur' },
  ],
  pushups: [
    { required: true, message: '请输入俯卧撑个数', trigger: 'blur' },
    { type: 'integer', message: '请输入整数', trigger: 'blur' },
  ],
  squats: [
    { required: true, message: '请输入深蹲个数', trigger: 'blur' },
    { type: 'integer', message: '请输入整数', trigger: 'blur' },
  ],
  situps: [
    { required: true, message: '请输入仰卧起坐个数', trigger: 'blur' },
    { type: 'integer', message: '请输入整数', trigger: 'blur' },
  ],
  pullup: [
    { required: true, message: '请输入引体向上个数', trigger: 'blur' },
    { type: 'integer', message: '请输入整数', trigger: 'blur' },
  ],
  longDistance: [
    { required: true, message: '请输入一千米时间', trigger: 'blur' },
    { type: 'integer', message: '请输入整数', trigger: 'blur' },
  ],
  goal: [
    { required: true, message: '请选择健身目标', trigger: 'blur' },
  ],

})
const backgroundStyle = {
  'background': `${imagePath} no-repeat center/cover`,
  'height': '100vh' // 设置高度为视窗高度
};
const loading = ref(true);
const submitForm = async (formEl: FormInstance | undefined) => {
  if (!formEl) return
  await formEl.validate((valid, fields) => {
    if (valid) {
      console.log('submit!')
      dialogFormVisible.value = !dialogFormVisible.value;
      submit()
    } else {
      console.log('error submit!', fields)
    }
  })


}
function submit() {
  loading.value = true;
  console.log(form.pullup)
  axios.get('http://localhost:8080/api/FitnessPlan/PostFitness',  {
    params: {
      token: localStorage.getItem('token'),
      height: form.height,
      weight: form.weight,
      BMI: form.BMI,
      bodyFatRate: form.bodyFatRate
    }}).then(response => {
    console.log(response.data.message);
  });
  axios.get('http://localhost:8080/api/FitnessPlan/PostPhysicalTest',  {
    params: {
      token: localStorage.getItem('token'),
      pushups: form.pushups,
      squats: form.squats,
      situps: form.situps,
      pullup: form.pullup,
      longDistance: form.longDistance
    }}).then(response => {
    console.log(response.data.message);
  });
  axios.get('http://localhost:8080/api/FitnessPlan/SetGoal',  {
    params: {
      token: localStorage.getItem('token'),
      goal: form.goal,
      duration:4
    }}).then(response => {
    console.log(response.data.message);
  });
}
const activeName = ref(1);
function checkAvailable(){
    let token = localStorage.getItem('token');
    if (token == null) {
      ElNotification({
        title: '提示',
        message: '请先登录',
        type: 'warning',
        duration: 2000
      })
      const router = useRouter()
      router.push('/login')
      return;
    };
    axios.get(`http://localhost:8080/api/User/GetTokenInvalidateRes`, {
                  params: {
                      token: token,
                  }
              }).then(response => {
                      console.log("登录状态:",response.data);
                      if(!response.data) {
                        ElNotification({
                          title: '提示',
                          message: '登录已过期，请重新登录',
                          type: 'warning',
                          duration: 2000
                        });
                        localStorage.removeItem('token');
                        this.router().push('/login');
                      }
                  }).catch(error => {
                      ElNotification({
                          title: '错误',
                          message: '获取用户信息失败',
                          type: 'error',
                      });
                  });

}

onMounted(() => {
  checkAvailable()
});
</script>



<style scoped>
/* 未解决的背景问题 */
.container {
  background-image: url();
  background-size: cover;
  background-position: center;
  width: 100%;
  position: absolute;
  background-attachment: fixed;
  top: 12vh;
  min-height: 100vh;
  left: 0;
}
.bot{
  position:absolute;
  left: 20%;
  top: 12px;
  width: 150px;
  height: 40px;
}

.title {
  font-size: 18px;
  font-weight: bold;
  color: #303133;
  padding-bottom: 17px;
  padding-top: 17px;
}
/* 自定义 el-collapse-item 的样式 */
>>>.custom-collapse-item .el-collapse-item__header {
  font-size: 18px;
  font-weight: bold;
  color: #1f2d3d;
  display: flex;
  align-items: center;
  cursor: pointer;
  padding: 10px;
  border-bottom: 1px solid #ebeef5;
  transition: all 0.3s ease;
  background-color: transparent;
}

/* 为标题添加图标 */
>>>.custom-collapse-item .el-collapse-item__header::before {
  content: '📅';
  margin-right: 10px;
}

/* 鼠标悬停时标题的样式 */
>>>.custom-collapse-item .el-collapse-item__header:hover {
  background-color: #e6f7ff;
  color: #409eff;
}

</style>

