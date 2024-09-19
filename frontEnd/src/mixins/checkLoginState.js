// src/mixins/commonMixin.js
import {ElNotification} from "element-plus";
import {useRouter} from "vue-router";
import axios from "axios";
import router from "../router/index.js";
export const commonMixin = {
  methods: {
    checkAvailable(){
        let token = localStorage.getItem('token');
        if (token == null) {
          ElNotification({
            title: '提示',
            message: '请先登录',
            type: 'warning',
            duration: 2000
          })
          localStorage.removeItem('iconUrl');
          this.$router.push({ path: '/login' });
          //this.router().push(`/login`);
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
                            localStorage.removeItem('iconUrl');
                            this.$router.push({ path: '/login' });
                          }
                      }).catch(error => {
                          ElNotification({
                              title: '错误',
                              message: '获取用户信息失败',
                              type: 'error',
                          });
                      });
        }
  }
};
