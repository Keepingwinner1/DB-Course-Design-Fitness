<template>
    <navigation-bar />
    <CommonLayout />
    <!-- 悬浮的置顶和刷新按钮 -->
    <el-backtop class="backtop-button"/>
    <div class="floating-refresh-button" style="z-index: 2;">
        <el-button type="primary" @click="fetchAllEquipmentGuide" class="refresh-button">
            <el-icon><refresh /></el-icon>
        </el-button>
    </div>

    <div class="carousel-container-top" >
        <el-carousel indicator-position="outside">
            <el-carousel-item v-for="(item, index) in items" :key="index">
                <img :src="item.src" :alt="item.alt" class="carousel-image" />
            </el-carousel-item>
        </el-carousel>
    </div>

    <div v-if="showMask" class="mask"></div>
    <div class="card_1">
        <el-card class="custom-card" style="max-width: 1000px; flex: 65;" shadow="hover"
            @click="showNotification(equipmentList,1)">
            <img :src="equipmentList.value[0].imgUrl" class="hover-zoom"
                style="width: 100%; height: 300px; object-fit: cover;" />
            <div class="footer-content" style="background-color: #ffffff; text-align: left;">
                <br>
                <p style="font-size: 30px; margin-left: 25px; margin-right: 25px; font-weight: bold ;">
                    {{ equipmentList.value[0].equipmentName }}
                </p><br>
                <p style="font-size: 20px; margin-left: 25px; margin-right: 25px;">
                    {{ equipmentList.value[0].shortIntr }}
                </p><br><br><br>
            </div>
        </el-card>
        <el-card class="custom-card" style="max-width: 1000px; flex: 35;" shadow="hover"
            @click="showNotification(equipmentList,2)">
            <img :src="equipmentList.value[1].imgUrl" class="hover-zoom"
                style="width: 100%; height: 200px; object-fit: cover;" />
            <div class="footer-content" style="background-color: #33aee3; text-align: left;">
                <br>
                <p style="font-size: 30px; margin-left: 25px; margin-right: 25px; color: white; font-weight: bold ;">
                    {{ equipmentList.value[1].equipmentName }}
                </p><br>
                <p style="font-size: 20px; margin-left: 25px; margin-right: 25px; color: white;">
                    {{ equipmentList.value[1].shortIntr }}
                </p><br><br><br><br><br><br>
            </div>
        </el-card>
    </div>
    <div class="card_2">
        <el-card class="custom-card" style="max-width: 1000px; flex: 32;" shadow="hover"
            @click="showNotification(equipmentList,3)">
            <img :src="equipmentList.value[2].imgUrl" class="hover-zoom"
                style="width: 100%; height: 250px; object-fit: cover;" />
            <div class="footer-content" style="background-color: #3453dd; text-align: left;">
                <br>
                <p style="font-size: 30px; margin-left: 25px; margin-right: 25px; color: white; font-weight: bold ;">
                    {{ equipmentList.value[2].equipmentName }}
                </p><br>
                <p style="font-size: 20px; margin-left: 25px; margin-right: 25px; color: white;">
                    {{ equipmentList.value[2].shortIntr }}
                </p><br><br><br><br><br><br><br>
            </div>
        </el-card>
        <el-card class="custom-card" style="max-width: 1000px; flex: 32;" shadow="hover"
            @click="showNotification(equipmentList,4)">
            <img :src="equipmentList.value[3].imgUrl" class="hover-zoom"
                style="width: 100%; height: 250px; object-fit: cover;" />
            <div class="footer-content" style="background-color: #ffffff; text-align: left;">
                <br>
                <p style="font-size: 30px; margin-left: 25px; margin-right: 25px; font-weight: bold ;">
                    {{ equipmentList.value[3].equipmentName }}
                </p><br>
                <p style="font-size: 20px; margin-left: 25px; margin-right: 25px;">
                    {{ equipmentList.value[3].shortIntr }}
                </p><br><br><br><br><br><br><br>
            </div>
        </el-card>
        <el-card class="custom-card" style="max-width: 1000px; flex: 35;" shadow="hover"
            @click="showNotification(equipmentList,5)">
            <img :src="equipmentList.value[4].imgUrl" class="hover-zoom"
                style="width: 100%; height: 250px; object-fit: cover;" />
            <div class="footer-content" style="background-color: #ededed; text-align: left;">
                <br>
                <p
                    style="font-size: 30px; margin-left: 25px; margin-right: 25px; color: rgb(0, 0, 0); font-weight: bold ;">
                    {{ equipmentList.value[4].equipmentName }}
                </p>
                <br>
                <p style="font-size: 20px; margin-left: 25px; margin-right: 25px; color: rgb(0, 0, 0);">
                    {{ equipmentList.value[4].shortIntr }}
                </p><br><br><br><br><br><br><br>
            </div>
        </el-card>
    </div>
    <div class="card_3">
        <el-card class="custom-card" style="max-width: 1000px; flex: 65;" shadow="hover"
            @click="showNotification(equipmentList,6)">
            <img :src="equipmentList.value[5].imgUrl" class="hover-zoom"
                style="width: 100%; height: 300px; object-fit: cover;" />
            <div class="footer-content" style="background-color: #ffffff; text-align: left;">
                <br>
                <p style="font-size: 30px; margin-left: 25px; margin-right: 25px; font-weight: bold ;">
                    {{ equipmentList.value[5].equipmentName }}
                </p><br>
                <p style="font-size: 20px; margin-left: 25px; margin-right: 25px;">
                    {{ equipmentList.value[5].shortIntr }}
                </p><br><br><br>
            </div>
        </el-card>
        <el-card class="custom-card" style="max-width: 1000px; flex: 35;" shadow="hover"
            @click="showNotification(equipmentList,7)">
            <img :src="equipmentList.value[6].imgUrl" class="hover-zoom"
                style="width: 100%; height: 250px; object-fit: cover;" />
            <div class="footer-content" style="background-color: #3453dd; text-align: left;">
                <br>
                <p style="font-size: 30px; margin-left: 25px; margin-right: 25px; color: white; font-weight: bold ;">
                    {{ equipmentList.value[6].equipmentName }}
                </p><br>
                <p style="font-size: 20px; margin-left: 25px; margin-right: 25px; color: white;">
                    {{ equipmentList.value[6].shortIntr }}
                </p><br><br><br><br><br><br>
            </div>
        </el-card>
    </div>
</template>

<script setup>
import { ref, h } from 'vue'
import { nextTick } from 'vue';
import { onMounted, onBeforeMount } from 'vue'
import axios from 'axios'
import { ElNotification, ElMessageBox, ElMessage } from 'element-plus'
import NotificationContent from '../components/NotificationContent.vue'
import CommonLayout from "../components/CommonLayout.vue";
import {useRouter} from 'vue-router'


const items = ref([
    { src: new URL('../assets/images/top1.png', import.meta.url).href, alt: 'Image 1' },
    { src: new URL('../assets/images/top2.png', import.meta.url).href, alt: 'Image 2' },
    { src: new URL('../assets/images/top3.png', import.meta.url).href, alt: 'Image 3' }
])

// 表单数据和管理操作
const form = ref({
    equipmentName: '',
    imgUrl: '',
    operationGuide: '',
    shortfIntr: ''
})
// 伪造的假数据
const fakeData = [
    { equipmentName: '单车机', imgUrl: 'https://image-tongji-sse-db.oss-cn-shanghai.aliyuncs.com/%E5%8D%95%E8%BD%A6%E6%9C%BA.png', shortIntr: '提高心肺耐力的最佳选择' },
    { equipmentName: '单杠', imgUrl: 'https://image-tongji-sse-db.oss-cn-shanghai.aliyuncs.com/%E5%8D%95%E8%BD%A6%E6%9C%BA.png', shortIntr: '锻炼上肢力量的经典器械' },
    { equipmentName: '动感单车', imgUrl: 'https://image-tongji-sse-db.oss-cn-shanghai.aliyuncs.com/%E5%8D%95%E8%BD%A6%E6%9C%BA.png', shortIntr: '高效的有氧运动工具' },
    { equipmentName: '瑜伽垫', imgUrl: 'https://image-tongji-sse-db.oss-cn-shanghai.aliyuncs.com/%E5%8D%95%E8%BD%A6%E6%9C%BA.png', shortIntr: '舒适的柔韧性训练基础' },
    { equipmentName: '哑铃', imgUrl: 'https://image-tongji-sse-db.oss-cn-shanghai.aliyuncs.com/%E5%8D%95%E8%BD%A6%E6%9C%BA.png', shortIntr: '力量训练的万能选择' },
    { equipmentName: '绳索拉力器', imgUrl: 'https://image-tongji-sse-db.oss-cn-shanghai.aliyuncs.com/%E5%8D%95%E8%BD%A6%E6%9C%BA.png', shortIntr: '灵活的全身肌肉训练工具' },
    { equipmentName: '椭圆机', imgUrl: 'https://image-tongji-sse-db.oss-cn-shanghai.aliyuncs.com/%E5%8D%95%E8%BD%A6%E6%9C%BA.png', shortIntr: '低冲击力的有氧运动神器' }
]

const equipmentList = ref([]) // 默认情况下先使用fakeData
equipmentList.value = ref(fakeData)


function fetchAllEquipmentGuide() {
    axios.get('http://localhost:8080/api/AIGuide/GetRandomEquipmentGuide')
        .then(function (response) {
            const guides = response.data.guides;
            const tmp = guides.map(item => ({
                equipmentName: item.equipmentName,
                imgUrl: item.imgUrl,
                shortIntr: item.briefIntr
            }));
            equipmentList.value = ref(tmp);
        })
        .catch(function (error) {
            console.error('Failed to fetch equipment guide:', error);
            equipmentList.value = ref([]); // Set an empty array or default data
        });
}

onMounted(() => {
    fetchAllEquipmentGuide();
})

const showMask = ref(false)
let notificationInstance = null;


function showNotification(equipmentList, i) {
    showMask.value = true; // 显示遮罩层
    document.documentElement.classList.add('blur-active'); // 添加模糊效果

    // 使用 Vue 组件作为通知内容
    notificationInstance = ElNotification({
        title: equipmentList.value[i-1].equipmentName,
        message: h(NotificationContent, { equipmentName: equipmentList.value[i-1].equipmentName }),
        duration: 0,
        position: (i <= 3) ? 'top-left' : 'top-right',
        customClass: 'custom-notification',
        dangerouslyUseHTMLString: true,
        onClose: closeNotification
    });

    // 延迟1秒后添加点击事件监听器
    setTimeout(() => {
        window.addEventListener('click', handleOutsideClick);
    }, 1000); // 1秒延迟
}

function handleOutsideClick(event) {
    const notificationElement = document.querySelector('.el-notification');
    // 确保点击的不是卡片本身，才触发关闭
    if (notificationElement && !notificationElement.contains(event.target)) {
        closeNotification();
    }
}

function closeNotification() {
    showMask.value = false; // 隐藏遮罩层
    document.documentElement.classList.remove('blur-active'); // 移除模糊效果
    notificationInstance && notificationInstance.close();
    window.removeEventListener('click', handleOutsideClick); // 移除点击事件监听器
}

function openInNewTab(url) {
    var win = window.open(url, '_blank');
    win.focus();
}

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

<style>

.carousel-container-top {
    width: 80vw;
    height: 80vh;
    position: absolute;
    justify-content: center;
    z-index: 1;
    white-space: nowrap; /* 防止子元素换行 */
}

.carousel-image {
    width: 100%;
    height: 600px;
    object-fit: cover;
}

.el-carousel__item:nth-child(2n) {
    background-color: #99a9bf;
}

.el-carousel__item:nth-child(2n + 1) {
    background-color: #d3dce6;
}

.el-card .el-card__body {
    padding: 0px;
}

.card_1,
.card_2,
.card_3 {
    width: 80vw;
    display: flex;
    justify-content: space-between;
    gap: 20px;
    height: 500px;
    z-index: 1;

    /* 确保内容处于最底层 */
}

.card_1 {
    position: absolute;
    top: 440px;
}

.card_2 {
    position: absolute;
    top: 970px;
}

.card_3 {
    position: absolute;
    top: 1500px;
}

.custom-card {
    display: flex;
    flex-direction: column;
    transition: box-shadow 0.1s, transform 0.1s;
}

.custom-card:hover {
    box-shadow: 0 30px 50px rgba(0, 0, 0, 0.5);
    transform: translateY(-18px);
    cursor: pointer;
}

.footer-content {
    flex-grow: 1;
}

.footer-content p {
    margin: 0;
    font-size: 16px;
}

.hover-zoom {
    transition: transform 0.3s;
}

.hover-zoom:hover {
    transform: scale(1.1);
}

.mask {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.5);
    z-index: 2;
    backdrop-filter: blur(5px);
    pointer-events: none;
}

.custom-notification {
    position: fixed;
    z-index: 3;
    background-color: #ffffff !important;
    color: #000 !important;
    border-radius: 10px !important;
    padding: 20px !important;
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2) !important;
    width: 65% !important;
    max-height: calc(100vh - 120px) !important;
    /* 设置最大高度 */
    overflow-y: auto !important;
    /* 添加垂直滚动条 */
    transition: transform 1s ease-out !important;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
    text-align: center;
    font-family: Arial, sans-serif;
    margin-top: 5%;
    margin-bottom: 5%;
}






.custom-notification .el-notification__title {
    font-weight: bold !important;
    font-size: 30px !important;
    margin-bottom: 10px !important;
    color: #333333;
    margin-top: 20px;
    /* 设置标题颜色 */
}

.custom-notification .el-notification__content {
    font-size: 18px !important;
    /* 设置内容字体大小 */
    color: #666666 !important;
    /* 设置内容颜色 */
    line-height: 1.5 !important;
    /* 设置行高 */
    margin: 0 20px !important;
    /* 设置内容内边距 */
    text-align: justify !important;
    /* 设置内容两端对齐 */
}

.el-notification-fade-enter-active,
.el-notification-fade-leave-active {
    transition: all 0.4s ease-out !important;
}

.el-notification-fade-enter,
.el-notification-fade-leave-to {
    transform: translateX(-100%) !important;
}

/* 禁止点击底层内容 */
.blur-active .carousel-container-top,
.blur-active .card_1,
.blur-active .card_2,
.blur-active .card_3 {
    pointer-events: none;
}

.management-container {
    padding: 20px;
    background-color: #f5f5f5;
    margin-bottom: 20px;
    margin-top: 120px;
    width: 80%;
}

.background-container {
    width: 99vw;
    background-color: #252525;
    /* 设置黑色背景 */
    padding: 40px 40px;
    /* 添加上下内边距，移除左右内边距 */
    box-sizing: border-box;
    /* 确保内边距不会影响总宽度 */
    position: relative;
    margin-top: 1325px;
    margin-left: -18%;
    /* 调整顶部间距 */
}

.image-wrapper {
    display: flex;
    justify-content: space-between;
}

.image-container {
    position: relative;
    width: calc(50% - 15px);
    /* 每个图片容器的宽度为整体的50%，并减去间距的一半 */
    height: 300px;
    /* 调整为图片的实际高度 */
    overflow: hidden;
    cursor: pointer;
    /* 鼠标变为手指 */
}

.normal-image,
.hover-image {
    position: absolute;
    width: 100%;
    height: 100%;
    top: 0;
    left: 0;
    transition: opacity 0.4s ease, transform 0.4s ease;
    object-fit: cover;
    /* 保持比例填充 */
}

.image-container:hover .normal-image,
.image-container:hover .hover-image {
    transform: scale(1.1);
    /* 放大效果 */
}

.hover-image {
    opacity: 0;
}

.image-container:hover .hover-image {
    opacity: 1;
}

.image-container:hover .normal-image {
    opacity: 0;
}

.text-overlay {
    position: absolute;
    width: 100%;
    height: 100%;
    top: 0;
    left: 0;
    display: flex;
    justify-content: center;
    align-items: center;
    color: white;
    font-size: 2em;
    background: rgba(0, 0, 0, 0.2);
    /* 半透明背景 */
    transition: background 0.4s ease;
}

.image-container:hover .text-overlay {
    background: rgba(0, 47, 255, 0.7);
    /* 悬停时背景变为蓝色 */
}

.backtop-button
{
    position: fixed;
    bottom: 200px !important;
    right: 25px !important;
    z-index: 2;
    width: 60px !important; /* 增加按钮的宽度 */
    height: 60px !important; /* 增加按钮的高度 */
    display: flex;
    justify-content: center;
    align-items: center;
    transition: transform 0.3s ease; /* 添加缩放的过渡效果 */
}
.floating-refresh-button {
    position: fixed;
    bottom: 120px;
    right: 23px;
    z-index: 2;
    width: 60px; /* 增加按钮的宽度 */
    height: 60px; /* 增加按钮的高度 */
    display: flex;
    justify-content: center;
    align-items: center;
    transition: transform 0.3s ease; /* 添加缩放的过渡效果 */
    border-radius: 50% !important; /* 将按钮变成圆形 */
}

.floating-refresh-button:hover {
    transform: scale(1.2); /* 悬停时按钮变大 */
}

.refresh-button {
    width: 60px;
    height: 60px;
    background-color: #409EFF;
    box-shadow: 0 2px 12px rgba(0, 0, 0, 0.1); /* 添加阴影效果 */
    padding: 0;
    display: flex;
    justify-content: center;
    align-items: center;
    border-radius: 50% !important; /* 将按钮变成圆形 */

}

.refresh-button .el-icon {
    font-size: 30px;
    color: white;

}
</style>
