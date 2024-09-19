import './assets/main.css'
import { createApp } from 'vue'
import store from './store'
import App from './App.vue'
import router from './router'
import ElementPlus from 'element-plus'
import 'element-plus/dist/index.css'
import * as ElementPlusIconsVue from '@element-plus/icons-vue'
import zhCn from 'element-plus/es/locale/lang/zh-cn'
import NavigationBar from './components/NavigationBar.vue';
import CommonLayout from "./components/CommonLayout.vue";

const app = createApp(App)
app.component('NavigationBar', NavigationBar);
app.component('CommonLayout', CommonLayout);
app.use(router)
app.use(store)
app.use(ElementPlus, {
  locale: zhCn,
})
app.mount('#app')
for (const [key, component] of Object.entries(ElementPlusIconsVue)) {
  app.component(key, component)
}
