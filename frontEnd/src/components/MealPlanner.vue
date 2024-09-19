<template>
    <div>
        <el-calendar ref="calendarRef" v-model="value" class="custom-calendar">
            <template #date-cell="{ data }">
                <div class="calendar-cell" @click="handleCellClick(data.date)">
                    <div class="date">
                        <div class="date-top">
                            {{ data.day.split('-').slice(1).join('-') }}
                        </div>
                        <div :class="['plan', getMealTypeClass(item.mealType)]"
                            v-for="(item, index) in getItemsForDate(data.date)" :key="index"
                            @click.stop="showPlan(getItemForDate(data.date, index))">
                            <span v-html="Complete[(item.state == true ? 1 : 0)]" @click.stop="setCompleted(item)"
                                class="mark"></span>
                            <span class="mealType">{{ MealType[item.mealType] }}</span>
                            <span :key="index" class="food-item">
                                {{ getTopFoodItems(item) }}
                            </span>
                            <span v-if="item.foods.length > 2">...</span>
                        </div>
                    </div>
                </div>
            </template>
        </el-calendar>
        <div style="margin: 10px 0" />
        <el-dialog v-model="dialogVisible">
            <el-form ref="form" :model="currentFormData" class="form-content">
                <h2>{{ getDate() }} 饮食计划安排</h2>
                <el-divider content-position="left">
                    {{ getDate() }}
                </el-divider>
                <div class="mb-2 flex items-center text-sm">
                    <el-radio-group v-model="currentFormData.mealType" class="ml-4">
                        <el-radio-button :value="0" size="large"
                            :disabled="isMealTypeExists(getDate(), 0)">早餐</el-radio-button>
                        <el-radio-button :value="1" size="large"
                            :disabled="isMealTypeExists(getDate(), 1)">午餐</el-radio-button>
                        <el-radio-button :value="2" size="large"
                            :disabled="isMealTypeExists(getDate(), 2)">晚餐</el-radio-button>
                    </el-radio-group>
                </div>
                <br>
                <div class="flex gap-2">
                    <div class="tags-container">
                        <el-tag v-for="tag in dynamicTags" class="tag-block" :key="tag" closable
                            :disable-transitions="false" size="large" @close="handleClose(tag)">
                            {{ tag }}
                            <el-input-number v-model="tagQuantities[tag]" :min=0 :step=50 class="numInput" />
                            <span> g</span>
                        </el-tag>
                    </div>
                    <div class="custom-select">
                      <el-select
                          v-if="inputVisible"
                          ref="selectRef"
                          v-model="inputValue"
                          @change="handleInputConfirm"
                          @blur="handleInputConfirm"
                          placeholder="输入/选择食物"
                          filterable
                          allow-create
                          default-first-option
                          :style="{ width: '140px', height: '40px' }"
                      >
                          <el-option
                              v-for="item in food"
                              :key="item.value"
                              :label="item.label"
                              :value="item.value"
                              :disabled="isOptionDisabled(item.value)"
                          ></el-option>
                      </el-select>
                      <el-button
                          v-else
                          class="button-new-tag"
                          :disabled="!canAdd"
                          @click="showInput"
                      >
                          + 点击添加食物
                      </el-button>
                  </div>
                </div>
            </el-form>
            <div class="center-button-container">
                <el-form-item>
                    <el-button type="primary" :style="{ fontSize: '18px' }" @click="saveEvent">保存</el-button>
                </el-form-item>
            </div>
        </el-dialog>
        <el-dialog v-model="showCurrentPlan" :close-on-click-modal=false :close-on-press-escape=false
            :show-close="false">
            <el-form class="center-content">
                <h2>{{ getDate() }} 饮食计划</h2>
                <el-divider content-position="left">
                    {{ getDate() }}
                </el-divider>
                <h3 :style="{ fontSize: '20px !important' }">
                    <el-icon v-if="this.currentFormData.mealType == 0">
                        <CoffeeCup />
                    </el-icon>
                    <el-icon v-else-if="this.currentFormData.mealType == 1">
                        <Food />
                    </el-icon>
                    <el-icon v-else-if="this.currentFormData.mealType == 2">
                        <DishDot />
                    </el-icon>
                    {{ MealType[this.currentFormData.mealType] }}
                </h3>
                <div style="margin: 5px 0" />
                <div class="flex gap-2">
                    <div class="tags-container">
                        <el-tag v-for="tag in this.selectedFoods" class="tag-block" :key="tag" closable size="large"
                            :disable-transitions="false" @close="handleClose(tag)">
                            {{ tag }}
                            <el-input-number v-model="tagQuantities[tag]" :min=0 :step=50 class="numInput" />
                            <span> g</span>
                        </el-tag>
                    </div>
                    <div class="custom-select">
                      <el-select
                          v-if="inputVisible"
                          ref="selectRef"
                          v-model="inputValue"
                          @change="handleInputConfirm"
                          @blur="handleInputConfirm"
                          placeholder="输入/选择食物"
                          filterable
                          allow-create
                          default-first-option
                          :style="{ width: '140px', height: '40px' }"
                      >
                          <el-option
                              v-for="item in food"
                              :key="item.value"
                              :label="item.label"
                              :value="item.value"
                              :disabled="isOptionDisabled(item.value)"
                          ></el-option>
                      </el-select>
                      <el-button
                          v-else
                          class="button-new-tag"
                          :disabled="!canAdd"
                          @click="showInput"
                      >
                          + 点击添加食物
                      </el-button>
                  </div>
                </div>
            </el-form>
            <div class="center-button-container">
                <el-form-item>
                    <el-button type="primary" @click="saveModi" :style="{ fontSize: '18px' }">保存修改</el-button>
                    <el-button type="danger" @click="deletePlan" :style="{ fontSize: '18px' }">删除计划</el-button>
                </el-form-item>
            </div>
        </el-dialog>
    </div>
    <el-carousel :interval="4000" type="card" height="151px" indicator-position="outside">
        <el-carousel-item v-for="item in recipes" :key="item.id">
            <el-card>
                <div class="card-content" @click="showDiet(item)">
                    <img :src="item.imgUrl" class="card-image" />
                    <div class="card-text">
                        <h3>{{ item.title }}</h3>
                        <p>{{ getDietInto(item) }}</p>
                    </div>
                </div>
            </el-card>
        </el-carousel-item>
    </el-carousel>
    <el-dialog v-model="showDietContext">
        <h2>{{ this.selectedItem.title }}</h2>
        <p>{{ this.selectedItem.time }}</p>
        <img :src="this.selectedItem.imgUrl" class="dialog-image" />
        <p v-html="formattedDescription()" class="left-align"></p>
    </el-dialog>
</template>

<script lang="ts">
import { defineComponent } from 'vue';
import { ElNotification } from 'element-plus';

/*import diet1 from '@/assets/diet1.jpg';
import diet2 from '@/assets/diet2.jpg';
import diet3 from '@/assets/diet3.jpg';*/
import axios from 'axios';

export default defineComponent({
    data() {
        return {
            inputValue: '',
            dynamicTags: [],
            inputVisible: false,
            calendarRef: null,
            value: new Date(),
            dialogVisible: false,
            showCurrentPlan: false,
            currentFormData: {
                userID: 0,
                foodPlanID: -1,
                mealType: -1,
                date: '',
                foods: [],
                state: false,
                numOfTypes: 0
            },
            formDataStore: {},
            food: [
                { value: 'Apple', label: 'Apple' },
                { value: 'Banana', label: 'Banana' },
                { value: 'Orange', label: 'Orange' },
            ],
            canAdd: true,
            MealType: ['早餐', '午餐', '晚餐'],
            Complete: ['&#9744', '&#9745'],
            tagQuantities: {},
            selectedFoods: [],
            recipes: [ ],
            showDietContext: false,
            selectItem: {},
        };
    },
    methods: {
        isMealTypeExists(date, mealType) {
            if (this.formDataStore[date]) {
                return this.formDataStore[date].some(plan => plan.mealType === mealType);
            }
            return false;
        },
        // 根据种类设定标签颜色
        getMealTypeClass(mealType) {
            switch (mealType) {
                case 0:
                    return 'breakfast';
                case 1:
                    return 'lunch';
                case 2:
                    return 'dinner';
                default:
                    return '';
            }
        },
        // 删除标签：添加时
        handleClose(tag) {
            this.dynamicTags.splice(this.dynamicTags.indexOf(tag), 1);
            this.currentFormData.foods.splice(this.currentFormData.foods.indexOf(tag), 1);
            // 删除标签
            if (this.tagQuantities.hasOwnProperty(tag)) {
                delete this.tagQuantities[tag];
            }
            this.currentFormData.numOfTypes--;
            if (this.currentFormData.numOfTypes < 5) {
                this.canAdd = true;
            }
            this.selectedFoods = this.selectedFoods.filter(food => food !== tag);//设定选择食物
            //console.log(this.tagQuantities);
        },
        // 
        showInput() {
            this.inputVisible = true;
        },
        // 标签输入确定：添加时
        handleInputConfirm() {
            if (this.inputValue && this.currentFormData.numOfTypes < 5) {
                // 检查输入值是否在已存在的选项中，如果不存在则添加新选项
                const existingItem = this.food.find(item => item.value === this.inputValue);
                if (!existingItem) {
                    this.food.push({ value: this.inputValue, label: this.inputValue });
                }

                this.currentFormData.numOfTypes++;
                this.dynamicTags.push(this.inputValue);
                this.tagQuantities[this.inputValue] = 0;
                this.selectedFoods.push(this.inputValue);

                this.inputVisible = false;
                this.inputValue = '';
                if (this.currentFormData.numOfTypes == 5) {
                    this.canAdd = false;
                }
            }
        },
        // 打开计划安排
        handleCellClick(date) {
            const formattedDate = new Date(date);
            this.dialogVisible = true;
            this.currentFormData = {
                userID: 0,
                foodPlanID: -1,
                date: formattedDate,
                mealType: -1,
                foods: [],
                state: false,
                numOfTypes: 0
            };
            this.dynamicTags = [];
            this.tagQuantities = {};
            this.selectedFoods = [];
            if(this.currentFormData.numOfTypes < 5){
            this.inputVisible=true;
            }
        },
        // 保存计划
        saveEvent() {
            if ((this.currentFormData.mealType == 0 || this.currentFormData.mealType == 1 || this.currentFormData.mealType == 2)
                && (this.currentFormData.numOfTypes != 0)) {
                const year = this.currentFormData.date.getFullYear();
                const month = String(this.currentFormData.date.getMonth() + 1).padStart(2, '0');
                const day = String(this.currentFormData.date.getDate()).padStart(2, '0');
                const dateString = `${year}-${month}-${day}`;
                if (!this.formDataStore[dateString]) {
                    this.formDataStore[dateString] = [];
                }
                let found = false;
                for (let i = 0; i < this.formDataStore[dateString].length; i++) {
                    if (this.formDataStore[dateString][i].mealType == this.currentFormData.mealType) {
                        this.formDataStore[dateString][i] = this.currentFormData;
                        found = true;
                        break;
                    }
                }
                for (let i = 0; i < this.selectedFoods.length; i++) {
                    if (this.tagQuantities[this.selectedFoods[i]] == 0) {
                        this.currentFormData.foods = [];
                        return;
                    }
                    const newFood = { foodName: this.selectedFoods[i], quantity: this.tagQuantities[this.selectedFoods[i]] };
                    this.currentFormData.foods.push(newFood);
                }
                if (!found) {
                    this.formDataStore[dateString].push({ ...this.currentFormData });
                    this.sendPlanToDB();
                }
                console.log(this.formDataStore);
                this.dialogVisible = false;
            }
            else{
                ElNotification({
            title: '警告',
            message: '必须选择餐次，计划内食物不可为空',
            type: 'warning',
            duration: 2000
        });
            }
        },
        // 得到前两个食物标签
        getTopFoodItems(item) {
            let result = item.foods.map(food => food.foodName).join(' ');
            if (result.length > 5) {
                result = result.slice(0, 4) + '...';
            }
            //console.log(result);
            return result;
            //return item.foods.slice(0, 2);
        },
        // 展示计划
        showPlan(plan) {
            this.currentFormData = plan;
            this.tagQuantities = {};
            this.selectedFoods = [];
            for (let i = 0; i < this.currentFormData.numOfTypes; i++) {
                this.tagQuantities[this.currentFormData.foods[i].foodName] = this.currentFormData.foods[i].quantity;
                this.selectedFoods.push(this.currentFormData.foods[i].foodName);
            }
            //console.log(this.selectedFoods, this.tagQuantities);
            this.showCurrentPlan = true;
        },
        // 更新完成状态
        setCompleted(set) {
            set.state = !set.state;
            this.updatePlanStateToDB(set.foodPlanID, set.state);
        },
        isOptionDisabled(value) {
            return this.selectedFoods.includes(value);
        },
        // 得到格式化日期显示
        getDate() {
            const year = this.currentFormData.date.getFullYear();
            const month = String(this.currentFormData.date.getMonth() + 1).padStart(2, '0');
            const day = String(this.currentFormData.date.getDate()).padStart(2, '0');
            return `${year}-${month}-${day}`;
        },
        formatLocalDate(date: Date): string {
            const year = date.getFullYear();
            const month = String(date.getMonth() + 1).padStart(2, '0');
            const day = String(date.getDate()).padStart(2, '0');
            return `${year}-${month}-${day}`;
        },

        // 根据日期获取对应的项
        getItemsForDate(date: Date) {
            const formattedDate = this.formatLocalDate(date);
            return this.formDataStore[formattedDate] || [];
        },
        // 根据日期和索引获取对应的项
        getItemForDate(date, index) {
            const formattedDate = this.formatLocalDate(date);
            return this.formDataStore[formattedDate][index] || [];
        },
        saveModi() {
            // 安全判断
            if (this.currentFormData.numOfTypes != 0) {
                const year = this.currentFormData.date.getFullYear();
                const month = String(this.currentFormData.date.getMonth() + 1).padStart(2, '0');
                const day = String(this.currentFormData.date.getDate()).padStart(2, '0');
                const dateString = `${year}-${month}-${day}`;
                if (!this.formDataStore[dateString]) {
                    this.formDataStore[dateString] = [];
                }
                //必然存在
                let i = -1;
                for (i = 0; i < this.formDataStore[dateString].length; i++) {
                    if (this.formDataStore[dateString][i].mealType == this.currentFormData.mealType) {
                        break;
                    }
                }
                this.currentFormData.foods = [];
                for (let i = 0; i < this.selectedFoods.length; i++) {
                    if (this.tagQuantities[this.selectedFoods[i]] == 0) {
                        this.currentFormData.foods = [];
                        return;
                    }
                    const newFood = { foodName: this.selectedFoods[i], quantity: this.tagQuantities[this.selectedFoods[i]] };
                    this.currentFormData.foods.push(newFood);
                }
                this.updatePlanToDB(this.currentFormData);
                this.formDataStore[dateString][i] = this.currentFormData;
                //console.log(this.currentFormData.foods);
                this.showCurrentPlan = false;
            }
            else{
                ElNotification({
            title: '警告',
            message: '计划内食物不可为空',
            type: 'warning',
            duration: 2000
        });
            }
        },
        deletePlan() {
            const year = this.currentFormData.date.getFullYear();
            const month = String(this.currentFormData.date.getMonth() + 1).padStart(2, '0');
            const day = String(this.currentFormData.date.getDate()).padStart(2, '0');
            const dateString = `${year}-${month}-${day}`;

            // 遍历查找匹配项并删除
            for (let i = 0; i < this.formDataStore[dateString].length; i++) {
                if (this.formDataStore[dateString][i].mealType == this.currentFormData.mealType) {
                    this.deletePlanInDB(this.formDataStore[dateString][i].foodPlanID);
                    // 使用 splice 方法删除匹配的元素
                    this.formDataStore[dateString].splice(i, 1);
                    i--; // 因为删除后数组长度减少，需要将索引回退
                    break;
                }
            }
            // 删除完毕后，检查数组是否为空，若为空则删除整个日期数据
            if (this.formDataStore[dateString].length === 0) {
                delete this.formDataStore[dateString];
            }
            this.showCurrentPlan = false;
        },
        // 得到简介
        getDietInto(item) {
            const truncatedText = item.content.slice(0, 100);
            // 如果文本长度超过100个字符，则添加省略号
            const hasMore = item.content.length > 100;
            return hasMore ? `${truncatedText}...` : truncatedText;
        },
        // 增加换行符
        formattedDescription() {
            return this.selectedItem.content.replace(/\n/g, '<br>');
        },
        showDiet(item) {
            this.selectedItem = item;
            this.showDietContext = true;
        },

        // 创建计划函数
        sendPlanToDB() {
            const requestData = {
                //userID: this.currentFormData.userID,
                date: this.currentFormData.date,  // 确保日期格式正确
                mealType: this.currentFormData.mealType,
                state: this.currentFormData.state,
                foods: this.currentFormData.foods.map(food => ({
                    foodName: food.foodName,
                    quantity: food.quantity  // 确保每个食物对象包含 quantity 字段
                }))
            };
            //console.log("发送", requestData);
            const token = localStorage.getItem('token');
            axios.post(`http://localhost:8080/api/MealPlans/Create?token=${token}`, requestData)
                .then(response => {
                    console.log(response.data.message);
                    // 显示通知
                    ElNotification({
                        message: response.data.message,
                        type: 'success',
                        duration: 2000
                    });
                    const year = this.currentFormData.date.getFullYear();
                    const month = String(this.currentFormData.date.getMonth() + 1).padStart(2, '0');
                    const day = String(this.currentFormData.date.getDate()).padStart(2, '0');
                    const dateString = `${year}-${month}-${day}`;
                    for (let i = 0; i < this.formDataStore[dateString].length; i++) {
                        if (this.formDataStore[dateString][i].mealType === this.currentFormData.mealType) {
                            this.formDataStore[dateString][i].foodPlanID = response.data.foodPlanID;
                            break;
                        }
                    }
                })
                .catch(error => {
                    console.error('Error:', error);
                });
        },

        // 得到食物函数
        getFoodFromDB() {
            //const token = localStorage.getItem('token');
            axios.get(`http://localhost:8080/api/MealPlans/GetFoodsInfo`)
                .then(response => {
                    console.log(response.data.foodsInfo);
                    this.food = response.data.foodsInfo.map(item => ({
                        value: item.foodName,
                        label: item.foodName
                    }))
                })
        },
        // 得到计划函数
        getPlanFromDB() {
            const token = localStorage.getItem('token');
            axios.get(`http://localhost:8080/api/MealPlans/GetAllDetails?token=${token}`)
                .then(response => {
                    this.formDataStore = {};
                    response.data.plans.forEach(item => {
                        const getUserID = item.userID;
                        const getfoodPlanID = item.foodPlanID;
                        const date = new Date(item.date);
                        const year = date.getFullYear();
                        const month = String(date.getMonth() + 1).padStart(2, '0');
                        const day = String(date.getDate()).padStart(2, '0');
                        const dateString = `${year}-${month}-${day}`;
                        if (!this.formDataStore[dateString]) {
                            this.formDataStore[dateString] = [];
                        }
                        const foods = item.foods.map(foodItem => ({
                            foodName: foodItem.foodName,
                            quantity: foodItem.quantity
                        }));
                        this.formDataStore[dateString].push({
                            userID: getUserID,
                            foodPlanID: getfoodPlanID,
                            mealType: item.mealType,
                            date: new Date(item.date),
                            foods: foods,
                            state: item.state,
                            numOfTypes: item.numOfTypes
                        });
                    });
                })
        },
        // 更新计划函数
        updatePlanToDB(planContent) {
            const requestData = {
                foodPlanID: planContent.foodPlanID,
                mealType: planContent.mealType,
                foods: planContent.foods.map(food => ({
                    foodName: food.foodName,
                    quantity: food.quantity  // 确保每个食物对象包含 quantity 字段
                }))
            };
            console.log("更新", requestData);
            axios.put(`http://localhost:8080/api/MealPlans/Update`, requestData)
                .then(response => {
                    console.log(response.data.message);
                    ElNotification({
                        message: response.data.message,
                        type: 'success',
                        duration: 2000
                    });
                })
        },
        // 删除计划函数
        deletePlanInDB(foodPlanID) {
            axios.delete(`http://localhost:8080/api/MealPlans/Delete`, {
                params: {
                    foodPlanID: foodPlanID
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
                .catch(error => {
                    console.error('Error deleting plan:', error);
                });
        },

        // 更新状态函数
        updatePlanStateToDB(foodPlanID, state) {
            const requestData = {
                foodPlanID: foodPlanID,
                state: state
            };
            console.log("更新", requestData);
            axios.put(`http://localhost:8080/api/MealPlans/UpdateState`, requestData)
                .then(response => {
                    console.log(response.data.message);
                    if (state) {
                        ElNotification({
                            message: `您已完成本计划`,
                            type: 'success',
                            duration: 2000
                        });
                    }
                    else {
                        ElNotification({
                            message: `取消完成本计划`,
                            type: 'success',
                            duration: 2000
                        });
                    }
                })
                .catch(error => {
                    ElNotification({
                        message: '计划状态更新失败',
                        type: 'error',
                        duration: 2000
                    });
                });
        },
        // 获取食谱函数
        getRecipeFromDB() {
            axios.get(`http://localhost:8080/api/MealPlans/GetAllRecipes`)
                .then(response => {
                    this.recipes = [];
                    response.data.recipes.forEach(item => {
                        const time = new Date(item.releaseTime);
                        this.recipes.push({
                            recipeID: item.recipeID,
                            title: item.title,
                            imgUrl: item.imgUrl,
                            content: item.content,
                            releaseTime: time
                        });
                    })
                })
        },
    },
    created() {//加载时触发
        this.getFoodFromDB();
        this.getPlanFromDB();
        this.getRecipeFromDB();
    }
})

</script>

<style scoped>
.custom-calendar {
    border-radius: 15px;
    font-size: 20px;
}

.calendar-cell {
    padding: 0;
    margin-top: 0;
    height: 150%;
}

.center-button-container {
    width: 100%;
    display: flex;
    justify-content: center;
    position: sticky;
    bottom: 0;
    background: white;
    padding: 10px 0;
    border-top: 1px solid #e0e0e0;
}

.form-content {
    height: 390px;
    justify-content: center;
    align-items: center;
}

.plan {
    display: block;
    margin-bottom: 1px;
    margin-left: 10%;
    padding: 0;
    border: 0.1px solid #F7EEDD;
    border-radius: 10px;
    line-height: 16px;
    width: 80%;
}

.date-top {
    margin-bottom: 0px;
    font-size: 14px;
    line-height: 14px;
}

.mealType {
    margin: 0px;
    margin-right: 3px;
    font-size: 12px;
}

.mark {
    margin: 0px;
    margin-right: 3px;
    font-size: 11px;
}

.food-item {
    margin: 0px;
    margin-right: 3px;
    font-size: 12px;
}

.tags-container {
    display: flex;
    align-items: center;
    flex-direction: column;
}

.tag-block {
    width: auto;
    min-width: 120px;
    margin-bottom: 5px;
}

.custom-select {
    width: auto;
    min-width: 120px;
    max-width: 120px;
    margin: auto;
}

.numInput {
    margin-left: 3px;
}

.center-content {
    align-items: center;
    justify-content: center;
    text-align: center;
    height: 390px;
}

.centered-form-item {
    width: 100%;
    display: flex;
    justify-content: center;
    bottom: 0;
    background: white;
    padding: 10px 0;
}

.breakfast {
    background-color: #FFF8F3;
    color: #C96000;
}

.lunch {
    background-color: #FFFBDA;
    color: #CA4716;
}

.dinner {
    background-color: #F8F6E3;
    color: #FF7801;
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

.tags-container {
    font-size: 18px;
}

.custom-select {
    font-size: 15px;
}

.numInput input {
    font-size: 15px;
}

.button-new-tag {
    font-size: 15px;
}

.tag-block {
    font-size: 15px;
}
</style>