import 'element-plus/dist/index.css';

import ElementPlus from 'element-plus';
import {createApp} from 'vue'

import App from './App.vue'

const app = createApp(App);

app.config.devtools = true;
app.use(ElementPlus).mount('#app')
