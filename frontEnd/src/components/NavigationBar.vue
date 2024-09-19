<template>
    <div :class="['head_con', navBarFixed ? 'navBarWrap' : '']">
        <div class="logo-container">
            <img src="../assets/images/logo.png" alt="FitFit" class="logo">
        </div>
        <div class="wrapper">
            <nav>
                <input type="radio" name="tab" id="home" :checked="$route.path === '/home' || $route.path === '/'">
                <input type="radio" name="tab" id="equipment" :checked="$route.path === '/equipment'">
                <input type="radio" name="tab" id="aifit" :checked="$route.path === '/aifit'">
                <input type="radio" name="tab" id="forum" :checked="$route.path === '/forum'">
                <input type="radio" name="tab" id="achievement" :checked="$route.path === '/achievements'">
                <!-- <input type="radio" name="tab" id="rank" :checked="$route.path === '/ranking-list'"> -->
                <input type="radio" name="tab" id="course" :checked="$route.path === '/course'">
                <input type="radio" name="tab" id="plan" :checked="$route.path === '/fitnessplan'">
                <input type="radio" name="tab" id="chat" :checked="$route.path === '/chat'">
                <input type="radio" name="tab" id="healthyDiet"
                    :checked="$route.path === '/healthyDiet' || $route.path === '/MealPlanner' || $route.path === '/MealRecord'">


                <label for="home" class="home" @click="delayedNavigation('/home')">
                    <router-link to="/home" style="font-weight: bold;">
                        <el-icon>
                            <House style="position: relative; top: 3px;" />
                        </el-icon>
                        首页
                    </router-link>
                </label>
                <label for="plan" class="plan" @click="delayedNavigation('/fitnessplan')">
                    <router-link to="/fitnessplan" style="font-weight: bold;">
                        <el-icon>
                            <Finished style="position: relative; top: 3px;" />
                        </el-icon>
                        健身计划
                    </router-link>
                </label>
                <label for="aifit" class="aifit" @click="delayedNavigation('/aifit')">
                    <router-link to="/aifit" style="font-weight: bold;">
                        <el-icon>
                            <Picture style="position: relative; top: 3px;" />
                        </el-icon>
                        健身指导
                    </router-link>
                </label>
                <label for="course" class="course" @click="delayedNavigation('/course')">
                    <router-link to="/course" style="font-weight: bold;">
                        <el-icon style="position: relative; top: 3px;">
                            <Notebook />
                        </el-icon>
                        健身课程
                    </router-link>
                </label>
                <label for="equipment" class="equipment" @click="delayedNavigation('/equipment')">
                    <router-link to="/equipment" style="font-weight: bold;">
                        <img src="../assets/images/equip.png"
                            style="width: 19px; height: 19px;vertical-align: middle" />
                        健身器材
                    </router-link>
                </label>
                <label for="healthyDiet" class="healthyDiet" @click="delayedNavigation('/healthyDiet')">
                    <router-link to="/healthyDiet" style="font-weight: bold;">
                        <el-icon style="position: relative; top: 3px;">
                            <Food />
                        </el-icon>
                        健康饮食
                    </router-link>
                </label>
                <label for="forum" class="forum" @click="delayedNavigation('/forum')">
                    <router-link to="/forum" style="font-weight: bold;">
                        <el-icon style="position: relative; top: 3px;">
                            <ChatDotRound />
                        </el-icon>
                        健身论坛
                    </router-link>
                </label>
                <div class="tab"></div>
            </nav>
        </div>
        <div class="avatar-container">
            <template v-if="token">
                <el-dropdown>
                    <img :src="iconUrl" alt="User" class="dropdownlink">
                    <template #dropdown>
                        <el-dropdown-menu>
                            <!-- <el-dropdown-item @click="navigateToMessageBox">
                                <el-icon><MessageBox /></el-icon>
                                账号消息
                            </el-dropdown-item> -->
                            <el-dropdown-item @click="navigateToUserProfile">
                                <el-icon>
                                    <Setting />
                                </el-icon>
                                个人资料
                            </el-dropdown-item>
                            <el-dropdown-item v-if="currentUser === 'admin'" @click="navigateToAdminPanel">
                                <el-icon>
                                    <Tools />
                                </el-icon>
                                管理界面
                            </el-dropdown-item>
                            <el-dropdown-item @click="navigateToLoginOut">
                                <el-icon>
                                    <Switch />
                                </el-icon>
                                退出登录
                            </el-dropdown-item>
                        </el-dropdown-menu>
                    </template>
                </el-dropdown>
            </template>
            <template v-else>
                <el-button type="primary" class="round-button" @click="navigateToLoginOut">登录/注册</el-button>
            </template>
        </div>
    </div>
</template>

<script>
import router from "../router/index.js";
import axios from "axios";

import { ElNotification } from "element-plus";
import { commonMixin } from '../mixins/checkLoginState';
export default {
    name: "NavigationBar",
    mixins: [commonMixin],
    data() {
        return {
            navBarFixed: false,
            token: localStorage.getItem('token'),  // 从 localStorage 获取 token
            iconUrl: this.$store.state.iconUrl,
            checkLoginInterval: null,  // 定时器ID
            currentUser: localStorage.getItem('role')
        };
    },
    props: {
        showMenu: {
            type: Boolean,
            default: true,
        }
    },
    methods: {
        navigateToAdminPanel() {
            this.router().push('/admin');
        },
        router() {
            return router;
        },
        delayedNavigation(target) {
            setTimeout(() => {
                this.router().push(target);
            }, 500);
        },
        navigateToUserProfile() {
            const userID = this.$store.state.userID;
            this.router().push(`/user/${userID}`);
        },
        navigateToLoginOut() {
            this.router().push(`/login`);
            localStorage.removeItem('iconUrl');
            localStorage.removeItem('token')
        },
        watchScroll() {
            var scrollTop = window.pageYOffset || document.documentElement.scrollTop || document.body.scrollTop;
            //console.log(scrollTop);
            if (scrollTop > 90) {
                this.navBarFixed = true;
            } else {
                this.navBarFixed = false;
            }
        },
        checkLoginStatus() {
            // 获取当前路径
            const currentPath = this.$route.path;
            // // 如果当前路径是 '/home' 或 '/'，则不进行检查
            if (currentPath === '/home' || currentPath === '/') {
                return;
            }
            this.checkAvailable()
        }
    },
    mounted() {
        window.addEventListener("scroll", this.watchScroll);
        // 每隔20秒检查一次登录状态
        this.checkLoginInterval = setInterval(this.checkLoginStatus, 20000);
    },
    beforeUnmount() {
        // 清理定时器
        if (this.checkLoginInterval) {
            clearInterval(this.checkLoginInterval);
        }
        window.removeEventListener("scroll", this.watchScroll);
    }
};
</script>

<style>
* {
    margin: 0;
    padding: 0;
    box-sizing: border-box;
}

body {
    height: 100vh;
    display: flex;
    justify-content: center;
    align-items: flex-start;
    text-align: center;
    background: #fff;
}

.head_con {
    display: flex;
    align-items: center;
    position: absolute;
    top: 3%;
    left: 0;
    width: 100%;
    padding-left: 5%;
    padding-right: 5%;
}

.logo-container {
    margin-right: 0px;
}

.logo {
    height: 50px;
    display: block;
}

.wrapper,
.avatar-container {
    height: 50px;
    display: flex;
    align-items: center;
    margin-left: auto;
}

.wrapper {
    width: 60vw;
    line-height: 50px;
    background-color: white;
    box-shadow: 0px 5px 15px rgba(0, 0, 0, 0.25);
    border-radius: 50px;
}

.wrapper nav {
    display: flex;
    flex: 1;
    position: relative;
}

.wrapper nav label {
    flex: 1;
    width: 100%;
    position: relative;
    z-index: 1;
    cursor: pointer;
}

.wrapper nav label a {
    position: relative;
    z-index: -1;
    color: #333;
    font-size: 1vw;
    font-weight: 500;
    text-decoration: none;
}

.wrapper nav input {
    display: none;
}

.wrapper nav .tab {
    position: absolute;
    height: 100%;
    width: 14.3%;
    left: 0px;
    bottom: 0px;
    background: linear-gradient(to right, #f09819, #ff5858);
    border-radius: 50px;
    transition: 0.6s cubic-bezier(0.68, -0.55, 0.265, 1.55);
}

.wrapper nav #home:checked~label.home a,
.wrapper nav #equipment:checked~label.equipment a,
.wrapper nav #aifit:checked~label.aifit a,
.wrapper nav #forum:checked~label.forum a,
/*.wrapper nav #achievement:checked~label.achievement a,
/* .wrapper nav #rank:checked~label.rank a, */
.wrapper nav #course:checked~label.course a,
.wrapper nav #plan:checked~label.plan a,
/*.wrapper nav #chat:checked~label.chat a,*/
.wrapper nav #healthyDiet:checked~label.healthyDiet a {
    color: #fff;
    transition: 0.6s;
}

.wrapper nav #equipment:checked~.tab {
    left: 57.5%;
}

.wrapper nav #aifit:checked~.tab {
    left: 28.5%;
}

.wrapper nav #forum:checked~.tab {
    left: 85.7%;
}

.wrapper nav #course:checked~.tab {
    left: 42.9%;
}

.wrapper nav #plan:checked~.tab {
    left: 14.3%;
}


.wrapper nav #healthyDiet:checked~.tab {
    left: 71.6%;
}

.wrapper nav #mealPlanner:checked~.tab {
    left: 71.6%;
}

.wrapper nav #mealRecord:checked~.tab {
    left: 71.6%;
}

.avatar-container {
    position: relative;
    display: flex;
    align-items: center;
    height: 50px;
    border: none;
}

.dropdownlink {
    width: 50px;
    height: 50px;
    border-radius: 50%;
    cursor: pointer;
    border: 5px solid #fff;
    box-shadow: 0 0 10px rgba(0, 0, 0, 0.20);
}

.example-showcase .el-dropdown-link {
    cursor: pointer;
    color: var(--el-color-primary);
    display: flex;
    align-items: center;
}

@media (min-width: 1024px) {
    #app {
        display: flex;
        justify-content: center;
        /* 水平居中 */
        align-items: center;
        /* 垂直居中 */
        height: 100vh;
        /* 父容器的高度占满整个视口 */
    }
}

.navBarWrap {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    z-index: 999;
    background-color: white;
    /* 添加背景色防止滚动时内容透过导航栏 */
    box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
    /* 添加阴影以区别于内容 */
}

.round-button {
    width: 100px;
    height: 50px;
    padding: 0;
    display: flex;
    justify-content: center;
    align-items: center;
    font-size: 16px;
    /* 根据需要调整字体大小 */
    color: #fff;
    border: none;
    /* 去除默认边框 */
    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
    /* 添加轻微的阴影效果 */
    cursor: pointer;
}
</style>
