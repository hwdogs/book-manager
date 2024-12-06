<template>
  <div class="login-container">
    <div class="login-box">
      <div class="login-header">
        <h2>图书管理系统</h2>
        <p>Welcome Back</p>
      </div>
      <el-form :model="loginForm" :rules="rules" ref="loginForm" class="login-form" autocomplete="off">
        <el-form-item prop="name">
          <el-input 
            v-model="loginForm.name" 
            prefix-icon="el-icon-user" 
            placeholder="用户名"
            autocomplete="off">
          </el-input>
        </el-form-item>
        <el-form-item prop="password">
          <el-input 
            v-model="loginForm.password" 
            prefix-icon="el-icon-lock" 
            type="password" 
            placeholder="密码"
            autocomplete="new-password"
            @keyup.enter.native="handleLogin">
          </el-input>
        </el-form-item>
        <el-form-item>
          <el-button 
            type="primary" 
            @click="handleLogin" 
            class="login-button"
            :loading="loading">
            登录
          </el-button>
        </el-form-item>
        <div class="register-link">
          <span>还没有账号？</span>
          <el-button type="text" @click="goToRegister">立即注册</el-button>
        </div>
      </el-form>
    </div>
  </div>
</template>

<script>
import axios from 'axios';
import { EventBus } from '../utils/eventBus';

export default {
  name: 'Login',
  data() {
    return {
      loading: false,
      loginForm: {
        name: '',
        password: ''
      },
      rules: {
        name: [
          { required: true, message: '请输入用户名', trigger: 'blur' }
        ],
        password: [
          { required: true, message: '请输入密码', trigger: 'blur' }
        ]
      }
    };
  },
  methods: {
    handleLogin() {
      this.$refs.loginForm.validate(async (valid) => {
        if (valid) {
          try {
            this.loading = true;
            const response = await axios.post('http://localhost:9999/manager/login', this.loginForm);
            console.log('Login response:', response.data);
            if (response.data.status === 'success') {
              localStorage.setItem('isLoggedIn', 'true');
              localStorage.setItem('name', response.data.name);
              console.log('Saved name to localStorage:', response.data.name);
              this.$message.success('登录成功！');
              
              // 登录成功后先跳转到首页
              await this.$router.push('/home');
              
              // 然后通过EventBus触发检查超期图书
              EventBus.$emit('book-returned');
            } else {
              this.$message.error('用户名或密码错误');
            }
          } catch (error) {
            console.error('Login error:', error);
            this.$message.error('登录失败');
          } finally {
            this.loading = false;
          }
        }
      });
    },
    goToRegister() {
      this.loginForm.name = '';
      this.loginForm.password = '';
      this.$router.push({
        path: '/register',
        query: {}
      });
    }
  },
  created() {
    // 获取路由参数中的账号密码
    const { name, password } = this.$route.query;
    if (name && password) {
      this.loginForm.name = name;
      this.loginForm.password = password;
    }
  }
};
</script>

<style scoped>
.login-container {
  height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background: linear-gradient(135deg, #e6f3ff 0%, #f0f7ff 100%);
}

.login-box {
  width: 400px;
  padding: 40px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
}

.login-header {
  text-align: center;
  margin-bottom: 40px;
}

.login-header h2 {
  margin: 0;
  font-size: 28px;
  color: #303133;
}

.login-header p {
  margin: 10px 0 0;
  font-size: 16px;
  color: #909399;
}

.login-form {
  margin-top: 30px;
}

.login-button {
  width: 100%;
  padding: 12px 0;
}

.register-link {
  text-align: center;
  margin-top: 20px;
  color: #606266;
}

.register-link .el-button {
  padding-left: 5px;
  padding-right: 5px;
}

/* 输入框样式优化 */
.el-input >>> .el-input__inner {
  padding-left: 40px;
  height: 45px;
  line-height: 45px;
}

.el-input >>> .el-input__prefix {
  left: 10px;
  font-size: 18px;
  color: #909399;
}
</style>