import Vue from 'vue'
import VueRouter from 'vue-router'
import Login from '../views/Login.vue'
import Register from '../views/Register.vue'
import BookManage from '../views/BookManage.vue'
import AddBook from '../views/AddBook.vue'
import BookUpdate from '../views/BookUpdate.vue'
import Index from '../Index.vue'

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
    path: '/',
    name: '图书展示系统',
    redirect: '/BookManage',
    meta: {
      title: '欢迎来到图书管理系统',
      requiresAuth: true
    },
    show: true,
    component: Index,
    children: [
      {
        path: '/BookManage',
        name: '查询图书',
        show: true,
        meta: {
          title: '图书列表信息',
          requiresAuth: true
        },
        component: BookManage
      },
      {
        path: '/AddBook',
        name: '添加图书',
        show: true,
        meta: {
          title: '添加图书',
          requiresAuth: true
        },
        component: AddBook
      }
    ]
  },
  {
    path: '/BookUpdate',
    component: BookUpdate,
    name: '修改图书信息',
    show: false,
    meta: {
      title: '修改图书信息',
      requiresAuth: true
    }
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