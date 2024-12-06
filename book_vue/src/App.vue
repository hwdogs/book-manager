<template>
  <div id="app">
    <template v-if="$route.path === '/login' || $route.path === '/register'">
      <router-view></router-view>
    </template>
    <template v-else>
      <el-container direction="vertical">
        <el-header>
          <div class="header-content">
            <div class="logo">
              <span class="logo-text">花江图书</span>
              <span class="logo-divider">·</span>
              <span class="logo-sub">图书管理系统</span>
            </div>
            <div class="header-right">
              <el-dropdown @command="handleOverdueCommand" trigger="click">
                <div class="overdue-badge-wrapper">
                  <i class="el-icon-warning-outline"></i>
                  <span class="overdue-text">超期图书</span>
                  <el-badge 
                    :value="overdueCount" 
                    :hidden="overdueCount === 0"
                    :max="99"
                    class="overdue-badge">
                  </el-badge>
                  <i class="el-icon-arrow-down el-icon--right"></i>
                </div>
                <el-dropdown-menu slot="dropdown">
                  <el-dropdown-item command="return">
                    <div class="dropdown-item-content">
                      <i class="el-icon-time"></i>
                      <span>前往还书</span>
                      <el-badge 
                        v-if="overdueCount > 0"
                        :value="overdueCount"
                        class="dropdown-badge">
                      </el-badge>
                    </div>
                  </el-dropdown-item>
                </el-dropdown-menu>
              </el-dropdown>
              <el-dropdown @command="handleCommand" trigger="click">
                <el-button type="text" class="user-button">
                  <i class="el-icon-s-custom" style="margin-right: 5px; font-size: 16px;"></i>
                  {{ username }}
                  <i class="el-icon-arrow-down el-icon--right"></i>
                </el-button>
                <el-dropdown-menu slot="dropdown">
                  <el-dropdown-item command="profile">
                    <div class="dropdown-item-content">
                      <i class="el-icon-user"></i>
                      <span>个人信息</span>
                    </div>
                  </el-dropdown-item>
                  <el-dropdown-item command="logout" divided>
                    <div class="dropdown-item-content">
                      <i class="el-icon-switch-button"></i>
                      <span>退出登录</span>
                    </div>
                  </el-dropdown-item>
                </el-dropdown-menu>
              </el-dropdown>
            </div>
          </div>
        </el-header>
        <el-container style="border: 1px solid #eee">
          <el-aside width="230px" style="background-color: rgb(238, 241, 246)">
            <el-menu 
              router 
              :default-active="$route.path" 
              :unique-opened="false"
              :collapse-transition="false"
            >
              <!-- 首页菜单项 -->
              <el-menu-item index="/home">
                <i class="el-icon-s-data"></i>
                <span slot="title">数据统计</span>
              </el-menu-item>

              <!-- 图书管理系统 -->
              <el-submenu index="1">
                <template slot="title">
                  <i class="el-icon-reading"></i>
                  图书展示系统
                </template>
                <el-menu-item index="/BookManage">查询图书</el-menu-item>
                <el-menu-item index="/AddBook">添加图书</el-menu-item>
              </el-submenu>

              <!-- 读者系统 -->
              <el-submenu index="2">
                <template slot="title">
                  <i class="el-icon-user-solid"></i>
                  读者系统
                </template>
                <el-menu-item index="/ReaderManage">查询读者卡</el-menu-item>
                <el-menu-item index="/AddReader">添加读者卡</el-menu-item>
              </el-submenu>

              <!-- 借阅归还系统 -->
              <el-submenu index="3">
                <template slot="title">
                  <i class="el-icon-notebook-2"></i>
                  借阅归还系统
                </template>
                <el-menu-item index="/LendBook">借书</el-menu-item>
                <el-menu-item index="/ReturnBook">还书</el-menu-item>
                <el-menu-item index="/LendRecords">借阅记录</el-menu-item>
              </el-submenu>

              <!-- 数据管理 -->
              <el-menu-item index="/backup">
                <i class="el-icon-folder-opened"></i>
                <span slot="title">数据管理</span>
              </el-menu-item>
            </el-menu>
          </el-aside>
          <el-container>
            <el-main>
              <router-view></router-view>
            </el-main>
            <el-footer>
              <a href="https://github.com/hwdogs/book-manager">基于.NET core的图书管理系统 by 2200330119黄万首</a>
            </el-footer>
          </el-container>
        </el-container>
      </el-container>
    </template>
  </div>
</template>

<script>
import axios from 'axios'
import { EventBus } from './utils/eventBus'

export default {
  name: 'App',
  data() {
    return {
      overdueCount: 0,
      username: ''
    }
  },
  methods: {
    handleCommand(command) {
      switch (command) {
        case 'logout':
          this.logout();
          break;
        case 'profile':
          this.goToProfile();
          break;
      }
    },
    logout() {
      localStorage.removeItem('isLoggedIn');
      localStorage.removeItem('name');
      this.$router.push('/login');
    },
    goToProfile() {
      this.$router.push('/profile');
    },
    goToReturn() {
      this.$router.push('/ReturnBook');
    },
    async checkOverdueBooks() {
      try {
        const response = await axios.get('http://localhost:9999/lendReturn/findAll/0/1000');
        if (response.data && response.data.content) {
          const now = new Date();
          this.overdueCount = response.data.content.filter(record => {
            if (!record.returnDate) {
              const lendDate = new Date(record.lendDate);
              const diffDays = Math.floor((now - lendDate) / (1000 * 60 * 60 * 24));
              return diffDays > 60;
            }
            return false;
          }).length;
        }
      } catch (error) {
        console.error('Check overdue books error:', error);
      }
    },
    handleOverdueCommand(command) {
      if (command === 'return') {
        this.$router.push({
          path: '/ReturnBook',
          query: { from: 'overdue' }
        });
      }
    },
    updateUsername() {
      const savedName = localStorage.getItem('name');
      console.log('Updating username from localStorage:', savedName);
      this.username = savedName || '管理员';
    }
  },
  created() {
    if (localStorage.getItem('isLoggedIn') === 'true') {
      this.updateUsername();
      this.checkOverdueBooks();
      setInterval(this.checkOverdueBooks, 60000);
      EventBus.$on('book-returned', this.checkOverdueBooks);
    }
  },
  watch: {
    '$route': {
      handler() {
        if (localStorage.getItem('isLoggedIn') === 'true') {
          this.updateUsername();
        }
      },
      immediate: true
    }
  },
  beforeDestroy() {
    // 组件销毁前移除事件监听
    EventBus.$off('book-returned', this.checkOverdueBooks);
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
  background: linear-gradient(to right, #ffffff, #f5f7fa);
  box-shadow: 0 2px 12px 0 rgba(0,0,0,0.05);
  color: #333;
  line-height: 60px;
}

.header-content {
  height: 100%;
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

.header-right {
  display: flex;
  align-items: center;
  gap: 20px;
}

.overdue-badge-wrapper {
  position: relative;
  display: flex;
  align-items: center;
  gap: 5px;
  cursor: pointer;
  padding: 5px 10px;
  border-radius: 4px;
  transition: background-color 0.3s;
  height: 32px;
}

.overdue-badge-wrapper:hover {
  background-color: rgba(255, 255, 255, 0.1);
}

.overdue-badge-wrapper .el-icon--right {
  margin-left: 5px;
  font-size: 12px;
}

.dropdown-badge {
  margin-left: 8px;
}

.dropdown-badge >>> .el-badge__content {
  height: 16px;
  padding: 0 4px;
  border: none;
  font-size: 12px;
  line-height: 16px;
  background-color: #F56C6C;
}

.el-dropdown-menu .dropdown-item-content {
  justify-content: space-between;
}

.el-dropdown-menu .dropdown-item-content i.el-icon-time {
  color: #F56C6C;
}

.overdue-text {
  font-size: 14px;
  color: #333;
  margin-left: 5px;
}

.header-right .el-button {
  height: 32px;
  line-height: 32px;
  padding: 0 10px;
}

.user-button {
  display: flex;
  align-items: center;
  font-size: 14px;
  color: #333;
}

.user-button i {
  margin-left: 5px;
  font-size: 12px;
}

.el-dropdown-menu {
  padding: 5px 0;
}

.el-dropdown-menu__item {
  padding: 0;
}

.dropdown-item-content {
  display: flex;
  align-items: center;
  padding: 8px 20px;
  width: 100%;
}

.dropdown-item-content i {
  margin-right: 8px;
  font-size: 16px;
  width: 16px;
  text-align: center;
}

.dropdown-item-content span {
  flex: 1;
}

.el-dropdown-menu__item.is-divided {
  border-top: 1px solid #ebeef5;
  margin: 5px 0;
}

.el-dropdown-menu {
  min-width: 120px;
}

.logo {
  display: flex;
  align-items: center;
  font-family: "PingFang SC", "Microsoft YaHei", sans-serif;
}

.logo-text {
  font-size: 24px;
  font-weight: 600;
  color: #2c3e50;
  letter-spacing: 2px;
}

.logo-divider {
  margin: 0 12px;
  color: #909399;
  font-weight: 300;
}

.logo-sub {
  font-size: 16px;
  color: #606266;
  font-weight: 400;
}

/* 菜单样式优化 */
.el-menu {
  border-right: none;  /* 移除默认右边框 */
}

.el-menu-item, .el-submenu__title {
  margin: 4px 8px;  /* 添加外边距 */
  border-radius: 4px;  /* 圆角边框 */
  height: 46px;  /* 稍微增加高度 */
  line-height: 46px;
}

/* 选中状态的样式 */
.el-menu-item.is-active {
  background-color: #ecf5ff !important;  /* 选中项的背景色 */
  border-right: 3px solid #409EFF;  /* 右侧边框指示器 */
}

/* 鼠标悬停状态的样式 */
.el-menu-item:hover, .el-submenu__title:hover {
  background-color: #f5f7fa !important;
}

/* 子菜单样式 */
.el-submenu .el-menu-item {
  margin: 2px 8px;  /* 子菜单项的外边距稍小 */
  height: 40px;  /* 子菜单项稍矮 */
  line-height: 40px;
  min-width: auto;  /* 移除最小宽度限制 */
}

/* 菜单图标样式 */
.el-menu-item i, .el-submenu__title i {
  margin-right: 8px;  /* 图标右边距 */
  font-size: 18px;  /* 图标大小 */
  color: #606266;  /* 图标颜色 */
}

/* 选中状态的图标颜色 */
.el-menu-item.is-active i {
  color: #409EFF;
}

/* 侧边栏背景色 */
.el-aside {
  background-color: #fff !important;  /* 改为白色背景 */
  border-right: 1px solid #e6e6e6;  /* 添加右侧边框 */
  box-shadow: 2px 0 8px rgba(0,0,0,0.05);  /* 添加阴影效果 */
}
</style>