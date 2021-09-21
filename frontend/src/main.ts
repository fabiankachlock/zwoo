import { Router } from "./routes";
import { createApp } from "vue";
import App from "./App.vue";

const app = createApp(App);

app.use(Router);

app.mount("#app");
