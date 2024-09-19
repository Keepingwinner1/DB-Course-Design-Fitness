<template>
    <div>
        <el-row class="row-bg" justify="space-between">
            <el-col :span="6">
                <div :style="{ fontSize: '21px' }">当前共有帖子 {{ allRecipe.length }} 条</div>
            </el-col>
            <el-col :span="9">
                <el-button style="width: auto" :style="{ fontSize: '18px' }" size="large" @click="addRecipe"
                    type="danger" text bg>
                    <el-icon>
                        <CirclePlusFilled />
                    </el-icon>
                    发布新帖
                </el-button>
            </el-col>
        </el-row>
        <div style="margin: 5px 0" />
        <!--展示帖子-->
        <div class="posts-container">
            <div v-for="item in paginatedRecipes" :key="item.recipeID">
                <el-row class="recipe-container">
                    <el-col :span="22">
                        <el-card>
                            <div class="card-content" @click="showDiet(item)">
                                <img :src="item.imgUrl" class="card-image" />
                                <div class="card-text">
                                    <h3>{{ item.title }}</h3>
                                    <p>{{ getDietInto(item) }}</p>
                                </div>
                            </div>
                        </el-card>
                    </el-col>
                    <el-col :span="2" style="align-items: flex-end;">
                        <div style="margin: 100% 0" />
                        <el-button @click="showModi(item)" size="large" :style="{ fontSize: '18px' }" type="primary"
                            icon="Edit" circle />
                        <div style="margin: 10px 0" />
                        <el-button @click="deleteItem(item)" size="large" :style="{ fontSize: '18px' }" type="danger"
                            icon="Delete" circle />
                    </el-col>
                </el-row>
            </div>
        </div>
        <!--分页控件-->
        <div v-if="allRecipe.length > 0" class="pagination-container">
            <el-pagination :current-page="currentPage" :page-size="pageSize" :pager-count="11"
                layout="prev, pager, next" :total="allRecipe.length" @current-change="handlePageChange" />
        </div>
        <!--发布饮食贴-->
        <div style="display: flex; justify-content: center;margin-top: 5px">
            <el-card :style="{ fontSize: '20px' }" style="height: 100px; padding: 0; width: auto;">
                <el-row style="margin: 12px; padding: 0;">
                    <el-col :span="20">
                        <el-image :src="foodBG2" style="width: auto; padding: 0px;" />
                    </el-col>
                    <el-col :span="4">
                        发布<br>饮食贴
                    </el-col>
                </el-row>
            </el-card>
        </div>
        <!--新建部分-->
        <el-dialog v-model="dialogVisible">
            <el-form ref="form">
                <h2>发布新帖</h2>
                <el-input v-model="recipe.title" size="large" style="width: 200px" autosize type="textarea"
                    placeholder="请输入标题" />
                <div style="margin: 15px 0" />
                <el-upload class="avatar-uploader" :show-file-list="false" :before-upload="beforeAvatarUpload">
                    <img v-if="imageUrl" :src="imageUrl" class="avatar" />
                    <el-icon v-else class="avatar-uploader-icon">
                        <Plus />
                    </el-icon>
                </el-upload>
                <div style="margin: 15px 0" />
                <el-input v-model="recipe.content" size="large" style="width: 100%"
                    :autosize="{ minRows: 2, maxRows: 12 }" type="textarea" placeholder="请输入内容" />
                <div style="margin: 15px 0" />
                <el-button type="primary" :style="{ fontSize: '18px' }" @click="saveEvent">
                    <el-icon>
                        <UploadFilled />
                    </el-icon> 保存
                </el-button>
            </el-form>
        </el-dialog>
        <!--修改部分-->
        <el-dialog v-model="showRec">
            <el-form ref="form">
                <h2>修改帖子</h2>
                <br>
                <el-input v-model="currentRecipe.title" size="large" style="width: 200px" autosize type="textarea"
                    placeholder="请输入标题" />
                <div style="margin: 15px 0" />
                <el-upload class="avatar-uploader" :show-file-list="false" :before-upload="beforeAvatarUpload">
                    <img v-if="currentRecipe.imgUrl" :src="currentRecipe.imgUrl" class="avatar" />
                    <el-icon v-else class="avatar-uploader-icon">
                        <Plus />
                    </el-icon>
                </el-upload>
                <div style="margin: 15px 0" />
                <el-input v-model="currentRecipe.content" size="large" style="width: 100%"
                    :autosize="{ minRows: 2, maxRows: 12 }" type="textarea" placeholder="请输入内容" />
                <div style="margin: 15px 0" />
                <el-button type="primary" :style="{ fontSize: '18px' }" @click="saveModi">
                    <el-icon>
                        <UploadFilled />
                    </el-icon> 保存修改
                </el-button>
            </el-form>
        </el-dialog>
        <!--展示部分-->
        <el-dialog v-model="showDietContext">
            <h2 :style="{ fontSize: '30px' }">{{ this.currentRecipe.title }}</h2>
            <p>发布时间：{{ this.formatDate(this.currentRecipe.releaseTime) }}</p>
            <img :src="this.currentRecipe.imgUrl" class="dialog-image" />
            <p :style="{ fontSize: '18px' }" v-html="formattedDescription()" class="left-align"></p>
        </el-dialog>
    </div>
</template>

<script>
import foodBG2 from '../assets/images/foodBG2.jpg';
import axios from 'axios';
import { ElNotification } from 'element-plus';

export default {
    name: 'addDiet',
    data() {
        return {
            recipe: {
                recipeID: 0,
                title: "",
                imgUrl: "",
                content: "",
                releaseTime: "",
            },
            currentRecipe: {
                recipeID: 0,
                title: "",
                imgUrl: "",
                content: "",
                releaseTime: "",
            },
            allRecipe: [],
            dialogVisible: false,
            showRec: false,
            imageUrl: "",
            i: 0,
            showDietContext: false,
            currentPage: 1,
            pageSize: 3,
            foodBG2
        }
    },
    computed: {
        paginatedRecipes() {
            const start = (this.currentPage - 1) * this.pageSize;
            const end = this.currentPage * this.pageSize;
            return this.allRecipe.slice(start, end);
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
                this.$message.error('上传头像图片只能是 JPG 或 PNG 格式!');
                return false;
            }
            if (!isLt2M) {
                this.$message.error('上传头像图片大小不能超过 2MB!');
                return false;
            }

            const reader = new FileReader();
            reader.readAsDataURL(file);
            reader.onload = () => {
                this.imageUrl = reader.result;
                this.recipe.imgUrl = this.imageUrl;
            };
            return false;
        },
        addRecipe() {
            this.imageUrl = '';
            this.dialogVisible = true;
        },

        saveEvent() {
            if (this.recipe.title && this.recipe.imgUrl && this.recipe.content) {
                this.recipe.recipeID = this.i++;
                this.allRecipe.push({ ...this.recipe });
                this.sendRecipeToDB();
                this.dialogVisible = false;
            }
            else {
                console.log(12);
                ElNotification({
                    title: '警告',
                    message: '标题、内容或图片之一为空',
                    type: 'warning',
                    duration: 2000
                });
            }
            this.recipe = {
                recipeID: 0,
                title: "",
                imgUrl: "",
                content: "",
                releaseTime: "",
            };
            this.imageUrl = '';
        },
        getDietInto(item) {
            const truncatedText = item.content.slice(0, 100);
            const hasMore = item.content.length > 100;
            return hasMore ? `${truncatedText}...` : truncatedText;
        },
        showDiet(item) {
            this.showDietContext = true;
            this.currentRecipe = { ...item };
            this.imageUrl = this.currentRecipe.imgUrl;
        },
        showModi(item) {
            this.showRec = true;
            this.currentRecipe = { ...item };
        },
        deleteItem(item) {
            // 查找要删除的项的索引
            const index = this.allRecipe.findIndex(recipe => recipe.recipeID === item.recipeID);

            // 如果找到了索引，则删除该项
            if (index !== -1) {
                const recipe = this.allRecipe[index];
                this.deleteRecipe(recipe.recipeID);
                this.allRecipe.splice(index, 1);
            }
        },
        saveModi() {
            const index = this.allRecipe.findIndex(recipe => recipe.recipeID === this.currentRecipe.recipeID);
            if (index !== -1) {
                if (this.currentRecipe.title && this.currentRecipe.imgUrl && this.currentRecipe.content) {
                    this.allRecipe[index] = { ...this.currentRecipe };
                    this.UpdateRecipeToDB();
                    this.showRec = false;
                    this.imageUrl = '';
                }
                else {
                    ElNotification({
                        title: '警告',
                        message: '标题、内容或图片之一为空',
                        type: 'warning',
                        duration: 2000
                    });
                }
            }
        },
        // 增加换行符
        formattedDescription() {
            return this.currentRecipe.content.replace(/\n/g, '<br>');
        },
        handlePageChange(val) {
            this.currentPage = val;
        },

        // 得到食谱函数
        getRecipeFromDB() {
            axios.get('http://localhost:8080/api/MealPlans/GetAllRecipes')
                .then(response => {
                    this.allRecipe = [];
                    response.data.recipes.forEach(item => {
                        const time = new Date(item.releaseTime);
                        this.allRecipe.push({
                            recipeID: item.recipeID,
                            title: item.title,
                            imgUrl: item.imgUrl,
                            content: item.content,
                            releaseTime: time
                        })
                    })
                })
        },
        // 插入食物信息
        sendRecipeToDB() {
            const requestData = {
                title: this.recipe.title,
                imgUrl: this.recipe.imgUrl,
                content: this.recipe.content
            }
            console.log(requestData);
            axios.post('http://localhost:8080/api/MealPlans/InsertRecipe', requestData)
                .then(response => {
                    this.recipe.recipeID = response.data.recipeID;
                    this.recipe.releaseTime = new Date(response.data.releaseTime);
                    console.log(response.data.message);
                    // 显示通知
                    ElNotification({
                        message: response.data.message,
                        type: 'success',
                        duration: 2000
                    });
                })
        },
        // 更新食物信息
        UpdateRecipeToDB() {
            const requestData = {
                recipeID: this.recipeID,
                title: this.currentRecipe.title,
                imgUrl: this.currentRecipe.imgUrl,
                content: this.currentRecipe.content,
            }
            console.log(requestData);
            axios.put('http://localhost:8080/api/MealPlans/UpdateRecipe', requestData)
                .then(response => {
                    console.log(response.data.message);
                    // 显示通知
                    ElNotification({
                        message: response.data.message,
                        type: 'success',
                        duration: 2000
                    });
                })
        },
        // 删除食物
        deleteRecipe(recipeID) {
            axios.delete('http://localhost:8080/api/MealPlans/DeleteRecipe', {
                params: {
                    recipeID: recipeID
                }
            })
                .then(response => {
                    console.log(response.data.message);
                    // 显示通知
                    ElNotification({
                        message: response.data.message,
                        type: 'success',
                        duration: 2000
                    });
                })
        }
    },
    created() {//加载时触发
        this.getRecipeFromDB();
    }
}
</script>

<style>
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
    height: 580px;
}

.recipe-container {
    height: 200px;
}

.pagination-container {
    margin-top: 20px;
    display: flex;
    justify-content: center;
    /* 居中对齐分页控件 */
    width: 100%;
}
</style>
