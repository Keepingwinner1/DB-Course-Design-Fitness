<template>
    <div>
        <el-row class="row-bg" justify="space-between">
            <el-col :span="6">
                <div :style="{ fontSize: '20px' }">当前共有器材 {{ allEquipment.length }} 件</div>
            </el-col>
            <el-col :span="9">
                <el-button style="width: auto" :style="{ fontSize: '18px' }" size="large" @click="addEquipment"
                    type="danger" text bg>
                    <el-icon>
                        <CirclePlusFilled />
                    </el-icon>
                    添加新器材
                </el-button>
            </el-col>
        </el-row>
        <div style="margin: 5px 0" />
        <!--展示器材-->
        <div class="posts-container">
            <div v-for="item in paginatedEquipments" :key="item.equipmentName">
                <el-row class="equipment-container">
                    <el-col :span="22">
                        <el-card>
                            <div class="card-content" @click="showEquipment(item)">
                                <img :src="item.imgUrl" class="card-image" />
                                <div class="card-text">
                                    <h3>{{ item.equipmentName }}</h3>
                                    <p>{{ getEquipmentInfo(item) }}</p>
                                </div>
                            </div>
                        </el-card>
                    </el-col>
                    <el-col :span="2" style="align-items: flex-end;">
                        <div style="margin: 50% 0" />
                        <el-button @click="showModiEquipment(item)" size="large" :style="{ fontSize: '18px' }"
                            type="primary" icon="Edit" circle />
                        <div style="margin: 10px 0" />
                        <el-button @click="deleteEquipment(item)" size="large" :style="{ fontSize: '18px' }"
                            type="danger" icon="Delete" circle />
                    </el-col>
                </el-row>
            </div>
        </div>
        <!--分页控件-->
        <div v-if="allEquipment.length > 0" class="pagination-container">
            <el-pagination :current-page="currentPage" :page-size="pageSize" :pager-count="11"
                layout="prev, pager, next" :total="allEquipment.length" @current-change="handlePageChange" />
        </div>
        <!--发布器材
        <el-card :style="{ fontSize: '20px' }" style="height: 100px; padding: 0;width: 550px;">
            <el-row style="margin: 0;padding: 0;">
                <el-col :span="20">
                    <el-image :src="equipmentBG2" style="width: 400px; padding: 0;" />
                </el-col>
                <el-col :span="4">
                    添加<br>新器材
                </el-col>
            </el-row>
        </el-card>
    -->
        <!--新建部分-->
        <el-dialog v-model="dialogVisible">
            <el-form ref="form">
                <h2>添加新器材</h2>
                <el-input v-model="newEquipment.equipmentName" size="large" style="width: 200px" autosize
                    type="textarea" placeholder="请输入器材名称" />
                <div style="margin: 15px 0" />
                <el-upload class="avatar-uploader" :show-file-list="false" :before-upload="beforeAvatarUpload">
                    <img v-if="imageUrl" :src="imageUrl" class="avatar" />
                    <el-icon v-else class="avatar-uploader-icon">
                        <Plus />
                    </el-icon>
                </el-upload>
                <div style="margin: 15px 0" />
                <el-input v-model="newEquipment.briefIntr" size="large" style="width: 100%"
                    :autosize="{ minRows: 2, maxRows: 12 }" type="textarea" placeholder="请输入简要介绍" />
                <div style="margin: 15px 0" />
                <el-input v-model="newEquipment.operationGuide" size="large" style="width: 100%"
                    :autosize="{ minRows: 2, maxRows: 12 }" type="textarea" placeholder="请输入操作指南" />
                <div style="margin: 15px 0" />
                <el-button type="primary" :style="{ fontSize: '18px' }" @click="saveNewEquipment">
                    <el-icon>
                        <UploadFilled />
                    </el-icon> 保存
                </el-button>
            </el-form>
        </el-dialog>
        <!--修改部分-->
        <el-dialog v-model="showRec">
            <el-form ref="form">
                <h2>修改器材信息</h2>
                <br>
                <el-input v-model="currentEquipment.equipmentName" size="large" style="width: 200px" autosize
                    type="textarea" placeholder="请输入器材名称" disabled />
                <div style="margin: 15px 0" />
                <el-upload class="avatar-uploader" :show-file-list="false" :before-upload="beforeAvatarUpload">
                    <img v-if="currentEquipment.imgUrl" :src="currentEquipment.imgUrl" class="avatar" />
                    <el-icon v-else class="avatar-uploader-icon">
                        <Plus />
                    </el-icon>
                </el-upload>
                <div style="margin: 15px 0" />
                <el-input v-model="currentEquipment.briefIntr" size="large" style="width: 100%"
                    :autosize="{ minRows: 2, maxRows: 12 }" type="textarea" placeholder="请输入简要介绍" />
                <div style="margin: 15px 0" />
                <el-input v-model="currentEquipment.operationGuide" size="large" style="width: 100%"
                    :autosize="{ minRows: 2, maxRows: 12 }" type="textarea" placeholder="请输入操作指南" />
                <div style="margin: 15px 0" />
                <el-button type="primary" :style="{ fontSize: '18px' }" @click="saveModiEquipment">
                    <el-icon>
                        <UploadFilled />
                    </el-icon> 保存修改
                </el-button>
            </el-form>
        </el-dialog>
        <!--展示部分-->
        <el-dialog v-model="showEquipmentContext">
            <h2 :style="{ fontSize: '30px' }">{{ this.currentEquipment.equipmentName }}</h2>
            <p>最后更新时间：{{ this.formatDate(this.currentEquipment.lastUpdateTime) }}</p>
            <img :src="this.currentEquipment.imgUrl" class="dialog-image" />
            <p :style="{ fontSize: '18px' }" v-html="formattedDescription()" class="left-align"></p>
        </el-dialog>
    </div>
</template>

<script>
import equipmentBG2 from '../assets/images/strength.png';
import axios from 'axios';
import { ElNotification } from "element-plus";

export default {
    name: 'equipmentGuide',
    data() {
        return {
            newEquipment: {
                equipmentName: "",
                imgUrl: "",
                briefIntr: "",
                operationGuide: "",
            },
            currentEquipment: {
                equipmentName: "",
                imgUrl: "",
                briefIntr: "",
                operationGuide: "",
                lastUpdateTime: "",
            },
            allEquipment: [],
            dialogVisible: false,
            showRec: false,
            imageUrl: "",
            showEquipmentContext: false,
            currentPage: 1,
            pageSize: 4,
            equipmentBG2 // Placeholder for image
        }
    },
    computed: {
        paginatedEquipments() {
            const start = (this.currentPage - 1) * this.pageSize;
            const end = this.currentPage * this.pageSize;
            return this.allEquipment.slice(start, end);
        }
    },
    methods: {
        formatDate(date) {
            if (!(date instanceof Date)) {
                return '';
            }
            const year = date.getFullYear();
            const month = String(date.getMonth() + 1).padStart(2, '0');
            const day = String(date.getDate()).padStart(2, '0');
            return `${year}-${month}-${day}`; // 格式化为 yyyy-mm-dd
        },
        beforeAvatarUpload(file) {
            this.imageUrl = '';
            const isJPGorPNG = file.type === 'image/jpeg' || file.type === 'image/png';
            const isLt2M = file.size / 1024 / 1024 < 2;

            if (!isJPGorPNG) {
                this.$message.error('上传图片只能是 JPG 或 PNG 格式!');
                return false;
            }
            if (!isLt2M) {
                this.$message.error('上传图片大小不能超过 2MB!');
                return false;
            }

            const reader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = () => {
                this.imageUrl = reader.result;
                this.newEquipment.imgUrl = this.imageUrl;
            };
            return false;
        },
        addEquipment() {
            this.dialogVisible = true;
        },
        saveNewEquipment() {
            // 检查是否有同名器材
            const duplicate = this.allEquipment.some(item => item.equipmentName === this.newEquipment.equipmentName);
            if (duplicate) {
                ElNotification({
                    message: '器材名称已存在，请输入不同的名称。',
                    type: 'error',
                    duration: 2000
                });
                return; // 阻止插入
            }
            if (this.newEquipment.equipmentName && this.newEquipment.imgUrl && this.newEquipment.briefIntr && this.newEquipment.operationGuide) {
                this.allEquipment.push({ ...this.newEquipment });
                this.sendEquipmentToDB();
                this.dialogVisible = false;
            }
            this.newEquipment = {
                equipmentName: "",
                imgUrl: "",
                briefIntr: "",
                operationGuide: "",
            };
            this.imageUrl = '';
        },
        getEquipmentInfo(item) {
            const truncatedText = item.briefIntr.slice(0, 100);
            const hasMore = item.briefIntr.length > 100;
            return hasMore ? `${truncatedText}...` : truncatedText;
        },
        showEquipment(item) {
            this.showEquipmentContext = true;
            this.currentEquipment = { ...item };
            this.imageUrl = this.currentEquipment.imgUrl;
        },
        showModiEquipment(item) {
            this.showRec = true;
            this.currentEquipment = { ...item };
        },
        deleteEquipment(item) {
            const index = this.allEquipment.findIndex(equipment => equipment.equipmentName === item.equipmentName);
            if (index !== -1) {
                const equipment = this.allEquipment[index];
                this.deleteEquipmentFromDB(equipment.equipmentName);
                this.allEquipment.splice(index, 1);
            }
        },
        saveModiEquipment() {
            const index = this.allEquipment.findIndex(equipment => equipment.equipmentName === this.currentEquipment.equipmentName);
            if (index !== -1) {
                this.allEquipment[index] = { ...this.currentEquipment };
                this.updateEquipmentInDB();
                this.showRec = false;
                this.imageUrl = '';
            }
        },
        formattedDescription() {
            return `${this.currentEquipment.briefIntr}<br><br>${this.currentEquipment.operationGuide}`.replace(/\n/g, '<br>');
        },
        handlePageChange(val) {
            this.currentPage = val;
        },
        getEquipmentGuide() {
            axios.get('http://localhost:8080/api/AIGuide/GetALLEquipmentGuide')
                .then(response => {
                    this.allEquipment = [];
                    response.data.guides.forEach(item => {
                        const time = new Date(item.lastUpdateTime);
                        this.allEquipment.push({
                            equipmentName: item.equipmentName,
                            imgUrl: item.imgUrl,
                            briefIntr: item.briefIntr,
                            operationGuide: item.operationGuide,
                            lastUpdateTime: time
                        })
                    })
                })
        },
        sendEquipmentToDB() {
            const requestData = {
                equipmentName: this.newEquipment.equipmentName,
                imgUrl: this.newEquipment.imgUrl,
                briefIntr: this.newEquipment.briefIntr,
                operationGuide: this.newEquipment.operationGuide
            }

            axios.post('http://localhost:8080/api/AIGuide/InsertEquipmentGuide', requestData)
                .then(response => {
                    this.newEquipment.lastUpdateTime = new Date(response.data.lastUpdateTime);
                    ElNotification({
                        message: response.data.message,
                        type: 'success',
                        duration: 2000
                    });
                })
        },
        updateEquipmentInDB() {
            const requestData = {
                equipmentName: this.currentEquipment.equipmentName,
                imgUrl: this.currentEquipment.imgUrl,
                briefIntr: this.currentEquipment.briefIntr,
                operationGuide: this.currentEquipment.operationGuide,
            }
            axios.put('http://localhost:8080/api/AIGuide/UpdateEquipmentGuide', requestData)
                .then(response => {
                    ElNotification({
                        message: response.data.message,
                        type: 'success',
                        duration: 2000
                    });
                })
        },
        deleteEquipmentFromDB(equipmentName) {
            axios.delete('http://localhost:8080/api/AIGuide/DeleteEquipmentGuide', {
                params: {
                    equipmentName: equipmentName
                }
            })
                .then(response => {
                    ElNotification({
                        message: response.data.message,
                        type: 'success',
                        duration: 2000
                    });
                })
        }
    },
    created() {
        this.getEquipmentGuide();
    }
}
</script>

<style scoped>
.avatar-uploader .avatar {
    width: 178px;
    height: 178px;
    display: block;
}

.avatar-uploader .el-upload {
    border: 1px dashed var(--el-border-color);
    border-radius: 6px;
    cursor: pointer;
    position: relative;
    overflow: hidden;
    transition: var(--el-transition-duration-fast);
}

.avatar-uploader .el-upload:hover {
    border-color: var(--el-color-primary);
}

.el-icon.avatar-uploader-icon {
    font-size: 28px;
    color: #8c939d;
    width: 178px;
    height: 178px;
    text-align: center;
}

.dialog-image {
    width: 60%;
    height: auto;
    object-fit: cover;
    margin-top: 10px;
}

.left-align {
    text-align: left;
    margin: 0;
    padding: 0;
}

.card-content {
    display: flex;
    align-items: center;
    height: 150px;
}

.card-image {
    width: 40%;
    height: 90%;
    margin-left: 10px;
    object-fit: cover;
}

.card-text {
    width: 60%;
    padding: 10px;
    text-align: left;
}

.posts-container {
    height: 760px;
}

.equipment-container {
    height: 190px;
}

.pagination-container {
    margin-top: 1px;
    display: flex;
    justify-content: center;
    width: 100%;
}
</style>
