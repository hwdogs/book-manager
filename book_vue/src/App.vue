<template>
  <div id="app">
    <template v-if="$route.path === '/login' || $route.path === '/register'">
      <router-view></router-view>
    </template>
    <template v-else>
      <el-container direction="vertical">
        <el-header>
          <div class="header-content">
            <span>欢迎来到心湖图书</span>
            <el-button type="text" @click="logout">退出登录</el-button>
          </div>
        </el-header>
        <el-container style="border: 1px solid #eee">
          <el-aside width="230px" style="background-color: rgb(238, 241, 246)">
            <el-menu router :default-openeds="['2']" :default-active="$route.path" :unique-opened="false">
              <el-submenu v-for="(item, index) in $router.options.routes" v-if="item.show" :key="index"
                :index="String(index)">
                <template slot="title">
                  <i class="el-icon-edit-outline"></i>
                  {{ item.name }}
                </template>
                <el-menu-item v-for="(item2, index2) in item.children" :key="index2" :index="item2.path">
                  <i class="el-icon-arrow-right"></i>
                  {{ item2.name }}
                </el-menu-item>
              </el-submenu>
            </el-menu>
          </el-aside>
          <el-container>
            <el-main>
              <router-view></router-view>
            </el-main>
            <el-footer>
              <a href="http://beian.miit.gov.cn/">粤 ICP备18132513号</a>
            </el-footer>
          </el-container>
        </el-container>
      </el-container>
    </template>
  </div>
</template>

<script>
export default {
  name: 'App',
  methods: {
    logout() {
      localStorage.removeItem('isLoggedIn');
      this.$router.push('/login');
    }
  }
}
</script>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

html,
body {
  height: 100%;
  width: 100%;
}

#app {
  font-family: "Helvetica Neue", Helvetica, "PingFang SC", "Hiragino Sans GB", "Microsoft YaHei", "微软雅黑", Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  height: 100%;
}

.el-header {
  background-color: #b3c0d1;
  color: #333;
  line-height: 60px;
}

.header-content {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0 20px;
}

.el-footer {
  text-align: center;
  background-color: #cbced1;
  line-height: 60px;
}

.el-aside {
  background-color: #d3dce6;
  color: #333;
  text-align: center;
}

a {
  text-decoration: none;
  color: rgb(37, 33, 51);
}

.el-container {
  height: 100%;
}

.el-menu {
  text-align: left;
}
</style>