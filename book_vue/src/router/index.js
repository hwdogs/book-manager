import Vue from 'vue'
import VueRouter from 'vue-router'
import Login from '../views/Login.vue'
import Register from '../views/Register.vue'
import BookManage from '../views/BookManage.vue'
import AddBook from '../views/AddBook.vue'
import BookUpdate from '../views/BookUpdate.vue'
import Index from '../Index.vue'
import ReaderManage from '../views/ReaderManage.vue'
import AddReader from '../views/AddReader.vue'
import ReaderUpdate from '../views/ReaderUpdate.vue'
import LendBook from '../views/LendBook.vue'
import ReturnBook from '../views/ReturnBook.vue'
import LendRecords from '../views/LendRecords.vue'
import Profile from '../views/Profile.vue'
import Home from '../views/Home.vue'
import DataBackup from '../views/DataBackup.vue'

Vue.use(VueRouter)

const routes = [
  {
    path: '/login',
    name: 'Login',
    component: Login
  },
  {
    path: '/register',
    name: 'Register',
    component: Register
  },
  {
    path: '/home',
    name: '数据统计',
    meta: {
      title: '数据统计',
      requiresAuth: true
    },
    show: true,
    icon: 'el-icon-s-data',
    component: Home
  },
  {
    path: '/BookManage',
    name: '图书展示系统',
    meta: {
      title: '图书列表信息',
      requiresAuth: true
    },
    show: true,
    component: BookManage
  },
  {
    path: '/AddBook',
    name: '添加图书',
    meta: {
      title: '添加图书',
      requiresAuth: true
    },
    show: true,
    component: AddBook
  },
  {
    path: '/ReaderManage',
    name: '读者系统',
    meta: {
      title: '读者列表信息',
      requiresAuth: true
    },
    show: true,
    component: ReaderManage
  },
  {
    path: '/AddReader',
    name: '添加读者卡',
    meta: {
      title: '添加读者',
      requiresAuth: true
    },
    show: true,
    component: AddReader
  },
  {
    path: '/ReaderUpdate',
    component: ReaderUpdate,
    name: '修改读者信息',
    show: false,
    meta: {
      requiresAuth: true
    }
  },
  {
    path: '/LendBook',
    name: '借阅归还系统',
    meta: {
      title: '借书',
      requiresAuth: true
    },
    show: true,
    component: LendBook
  },
  {
    path: '/ReturnBook',
    name: '还书',
    meta: {
      title: '还书',
      requiresAuth: true
    },
    show: true,
    component: ReturnBook
  },
  {
    path: '/LendRecords',
    name: '借阅记录',
    meta: {
      title: '借阅记录',
      requiresAuth: true
    },
    show: true,
    component: LendRecords
  },
  {
    path: '/profile',
    name: '个人信息',
    component: Profile,
    meta: {
      requiresAuth: true
    },
    show: false
  },
  {
    path: '/backup',
    name: '数据管理',
    component: DataBackup,
    meta: {
      title: '数据备份',
      requiresAuth: true
    },
    show: true,
    icon: 'el-icon-document-copy'
  },
  {
    path: '/BookUpdate',
    component: BookUpdate,
    name: '修改图书信息',
    show: false,
    meta: {
      requiresAuth: true
    }
  },
  {
    path: '/',
    redirect: '/home'
  }
]

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes
})

// 导航守卫
router.beforeEach((to, from, next) => {
  const isLoggedIn = localStorage.getItem('isLoggedIn') === 'true'

  if (to.matched.some(record => record.meta.requiresAuth)) {
    if (!isLoggedIn) {
      next('/login')
    } else {
      next()
    }
  } else {
    next()
  }

  /* 路由发生变化修改页面title */
  if (to.meta.title) {
    document.title = to.meta.title
  }
})

export default router