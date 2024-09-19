<template>
  <el-icon
    :style="{ fontSize: '24px', marginRight: '20px' }"
    @click="showModal = true"
  >
    <Edit />
  </el-icon>

  <!-- 编辑课程模态框 -->
  <el-dialog v-model="showModal" title="编辑课程" width="50%">
    <el-form :model="form">
      <el-form-item label="课程名称">
        <el-input
          v-model="eeditForm.courseName"
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
              v-if="eeditForm.coursePhotoUrl"
              :src="eeditForm.coursePhotoUrl"
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
          v-model="eeditForm.courseDescription"
          placeholder="核心肌群是身体的中心力量，对于维持姿势、提高运动表现和预防受伤至关重要。"
        ></el-input>
      </el-form-item>
      <el-form-item label="课程容量">
        <el-input
          v-model="eeditForm.capacity"
          placeholder="请填入课程容量"
        ></el-input>
      </el-form-item>
      <el-form-item label="课程开始时间">
        <el-date-picker
          v-model="eeditForm.courseStartTime"
          type="date"
          placeholder="选择日期"
        />
      </el-form-item>
      <el-form-item label="课程结束时间">
        <el-date-picker
          v-model="eeditForm.courseEndTime"
          type="date"
          placeholder="选择日期"
        />
      </el-form-item>
      <el-form-item label="课程难度">
        <el-radio-group v-model="eeditForm.courseGrade">
          <el-radio :label="1">1</el-radio>
          <el-radio :label="2">2</el-radio>
          <el-radio :label="3">3</el-radio>
          <el-radio :label="4">4</el-radio>
          <el-radio :label="5">5</el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item label="课程价格">
        <el-input-number
          v-model="eeditForm.coursePrice"
          :min="0"
          :max="3000"
          :step="1"
          placeholder="请填入一个数字"
        />
      </el-form-item>
      <el-form-item label="课程特征">
        <div>
          <el-tag
            v-for="(feature, index) in eeditForm.features"
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
        <el-select v-model="eeditForm.typeID" placeholder="请选择课程分类">
          <el-option label="高强度间歇" value="1"></el-option>
          <el-option label="儿童趣味课" value="3"></el-option>
          <el-option label="低强度塑形" value="2"></el-option>
          <el-option label="有氧训练" value="4"></el-option>
        </el-select>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="submitForm">提交</el-button>
        <el-button @click="showModal = false">取消</el-button>
      </el-form-item>
    </el-form>
    <template #footer>
      <el-button @click="showModal = false">取消</el-button>
      <el-button type="primary" @click="submitEdit">保存修改</el-button>
    </template>
  </el-dialog>
</template>

<script>
import { ref } from "vue";

export default {
  data() {
    return {
      aaa: "",
    };
  },
};
</script>

<style scoped></style>
