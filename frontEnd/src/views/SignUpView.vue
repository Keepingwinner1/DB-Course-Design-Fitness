<template>
    <div class="background">
        <div class="background-image"></div>
        <div class="login-container">
            <div class="gradient-background"></div>
            <div class="content">
                <div class="title-img"></div>
                <el-card class="login-card">
                    <h2 class="login-title">用户注册</h2>
                    <el-form :model="SignUpForm" :rules="rules" label-position="left" label-width="80px">
                        <el-form-item label="用户昵称" prop="name">
                            <el-input v-model="SignUpForm.name"></el-input>
                        </el-form-item>
                        <el-form-item label="用户类型" prop="userType">
                            <el-radio-group v-model="SignUpForm.userType">
                                <el-radio label="user">普通用户</el-radio>
                                <el-radio label="coach">教练</el-radio>
                            </el-radio-group>
                        </el-form-item>
                        <el-form-item label="密码" prop="password">
                            <el-input v-model="SignUpForm.password" show-password></el-input>
                        </el-form-item>
                        <el-form-item label="确认密码" prop="verifyPwd">
                            <el-input v-model="SignUpForm.verifyPwd" show-password></el-input>
                        </el-form-item>
                        <el-form-item label="邮箱" prop="email">
                            <el-input v-model="SignUpForm.email"></el-input>
                        </el-form-item>
                        <el-form-item label="验证码" prop="verifyCode">
                            <el-row gutter="20">
                                <el-col :span="12">
                                    <el-input v-model="SignUpForm.verifyCode"></el-input>
                                </el-col>
                                <el-col :span="10">
                                    <div class="code-button-container">
                                        <el-button v-show="codeShow" type="primary" class="code-button"
                                            @click="sendVerifyCode">获取验证码</el-button>
                                        <el-button v-show="!codeShow" disabled="disabled" class="code-button">{{ count
                                            }}s</el-button>
                                    </div>
                                </el-col>
                            </el-row>
                        </el-form-item>
                    </el-form>
                    <el-form-item>
                        <el-button type="primary" class="return-button" @click="returnLoginView">返回</el-button>
                        <el-button type="primary" class="login-button" @click="SignUp">完成注册</el-button>
                    </el-form-item>
                </el-card>
            </div>
        </div>
    </div>
</template>

<script>
import axios from "axios";
import { ElNotification } from "element-plus";

export default {
    data() {
        return {
            SignUpForm: {
                name: '',
                password: '',
                verifyPwd: '',
                email: '',
                verifyCode: '',
                userType: 'user',
            },
            rules: {
                password: [
                    { required: true, message: '请输入密码', trigger: 'blur' },
                    { min: 6, max: 16, message: '密码长度为6-16位', trigger: 'blur' },
                ],
                verifyPwd: [
                    { required: true, message: '请确认密码', trigger: 'blur' },
                    {
                        trigger: 'blur',
                        validator: (rule, value, callback) => {
                            if (value === '') callback(new Error("请确认密码"));
                            else {
                                if (value !== this.SignUpForm.password) callback(new Error("密码不一致"));
                                else callback();
                            }
                        }
                    }
                ],
                email: [
                    { required: true, message: '请输入邮箱', trigger: 'blur' },
                    {
                        trigger: 'blur',
                        validator: (rule, value, callback) => {
                            const emailReg = /^([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+(.[a-zA-Z0-9_-])+/;
                            if (!value) callback();
                            else {
                                if (emailReg.test(value)) callback();
                                else callback(new Error("邮箱地址非法"));
                            }
                        }
                    }
                ],
                verifyCode: [
                    { required: true, message: '请输入验证码', trigger: 'blur' },
                    { min: 6, max: 6, message: '验证码长度为6位', trigger: 'blur' },
                ],
                name: [
                    { required: true, message: '请输入昵称', trigger: 'blur' },
                    { min: 0, max: 20, message: '昵称过长', trigger: 'blur' },
                ],
                userType: [
                    { required: true, message: '请选择用户类型', trigger: 'change' }
                ],
            },
            count: 60,
            timer: null,
            codeShow: true,
        }
    },

    methods: {
        sendVerifyCode() {
            const TIME_LIMIT = 60;
            const email = this.SignUpForm.email;
            axios.post(`http://localhost:8080/api/User/SendVerificationCode?email=${email}`).then(res => {
                console.log(res.data.message);
                if (!this.timer) {
                    this.count = TIME_LIMIT;
                    this.codeShow = false;
                    this.timer = setInterval(() => {
                        if (this.count > 0) {
                            this.count--;
                        } else {
                            this.codeShow = true;
                            clearInterval(this.timer);
                            this.timer = null;
                        }
                    }, 1000);
                }
            }).catch(error => {
                console.error('Error sending verification code:', error);
                ElNotification({
                    message: '验证码发送失败，请稍后再试',
                    type: 'error',
                    duration: 2000
                });
            });
        },

        async SignUp() {
            try {
                const requestData = {
                    email: this.SignUpForm.email,
                    password: this.SignUpForm.password,
                    accountName: this.SignUpForm.name,
                    role: this.SignUpForm.userType === 'user' ? 'user' : 'coach',
                    coachName: this.SignUpForm.name,
                    verificationCode: this.SignUpForm.verifyCode
                };

                console.log(requestData);

                const response = await axios.post(`http://localhost:8080/api/User/Register`, requestData);

                if (response.data.message === "成功注册") {
                    ElNotification({
                        message: "注册成功",
                        type: 'success',
                        duration: 2000
                    });
                    this.$router.push({ name: 'LoginView' });
                } else if (response.data.message === "验证码错误或已过期") {
                    ElNotification({
                        message: "注册失败：验证码错误或已过期",
                        type: 'error',
                        duration: 2000
                    });
                    this.resetForm();
                } else if (response.data.message === "注册失败：邮箱已存在") {
                    ElNotification({
                        message: "注册失败：邮箱已存在，请重新填写",
                        type: 'error',
                        duration: 2000
                    });
                    this.resetForm();
                }

            } catch (error) {
                ElNotification({
                    message: '注册失败，请稍后再试',
                    type: 'error',
                    duration: 2000
                });
                console.error('Error registering:', error);
            }
        },

        resetForm() {
            this.$refs['SignUpForm'].resetFields();
        },

        returnLoginView() {
            this.$router.go(-1);
        },
    }
}
</script>

<style scoped>
.background {
    display: flex;
    width: 100%;
    height: 100vh;
}

.background-image {
    background-image: url('../assets/images/background.jpg');
    background-size: cover;
    background-position: center;
    width: 70%;
    height: 100%;
}

.title-img {
    background-image: url('../assets/images/login_signup.jpg');
    background-size: contain;
    background-repeat: no-repeat;
    background-position: center;
    width: 100%;
    height: auto;
    aspect-ratio: 16 / 9;
}

.login-container {
    width: 40%;
    height: 100%;
    display: flex;
    justify-content: center;
    align-items: center;
    background-color: white;
}

.content {
    width: 80%;
    border-radius: 8px;
}

.title {
    margin-top: 20px;
    margin-bottom: 20px;
    text-align: center;
    font-size: 3.5rem;
    font-weight: bold;
    font-family: 'PingFang SC', sans-serif;
    color: rgb(44, 225, 44);
}

.login-card {
    border: none;
    box-shadow: none;
    padding: 20px;
    border-radius: 30px;
}

.login-title {
    text-align: center;
    margin-bottom: 20px;
    color: #333;
}

.tabs {
    margin-top: 20px;
}

.code-button-container {
    width: 100px;
    /* 确保容器宽度与父元素一致 */
    display: flex;
    justify-content: center;
}

.code-button {
    width: 100px;
    /* 确保按钮宽度与容器一致 */
}

.return-button {
    width: 16%;
    background-color: #409EFF;
    margin-left: 0;
    margin-right: 5px;
}

.login-button {
    width: 79%;
    background-color: #409EFF;
    color: #fff;
    border-radius: 5px;
    margin-right: 0;
}

.login-button:hover {
    background-color: #66b1ff;
}
</style>
