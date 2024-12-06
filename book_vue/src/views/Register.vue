<template>
  <div class="register-container">
    <div class="register-box">
      <div class="register-header">
        <h2>注册新账号</h2>
        <p>Join Us</p>
      </div>
      <el-form :model="registerForm" :rules="rules" ref="registerForm" class="register-form" autocomplete="off">
        <el-form-item prop="name">
          <el-input 
            v-model="registerForm.name" 
            prefix-icon="el-icon-user" 
            placeholder="用户名"
            autocomplete="off">
          </el-input>
        </el-form-item>
        <el-form-item prop="password">
          <el-input 
            v-model="registerForm.password" 
            prefix-icon="el-icon-lock" 
            type="password" 
            placeholder="密码"
            autocomplete="new-password">
          </el-input>
        </el-form-item>
        <el-form-item prop="confirmPassword">
          <el-input 
            v-model="registerForm.confirmPassword" 
            prefix-icon="el-icon-key" 
            type="password" 
            placeholder="确认密码"
            autocomplete="new-password">
          </el-input>
        </el-form-item>
        <el-form-item>
          <el-button 
            type="primary" 
            @click="handleRegister" 
            class="register-button"
            :loading="loading">
            注册
          </el-button>
        </el-form-item>
        <div class="login-link">
          <span>已有账号？</span>
          <el-button type="text" @click="goToLogin">返回登录</el-button>
        </div>
      </el-form>
    </div>
  </div>
</template>

<script>
import axios from 'axios';

export default {
  name: 'Register',
  data() {
    const validatePass2 = (rule, value, callback) => {
      if (value !== this.registerForm.password) {
        callback(new Error('两次输入密码不一致!'));
      } else {
        callback();
      }
    };
    return {
      loading: false,
      registerForm: {
        name: '',
        password: '',
        confirmPassword: ''
      },
      rules: {
        name: [
          { required: true, message: '请输入用户名', trigger: 'blur' },
          { min: 3, max: 20, message: '长度在 3 到 20 个字符', trigger: 'blur' }
        ],
        password: [
          { required: true, message: '请输入密码', trigger: 'blur' },
          { min: 6, message: '密码长度不能小于6个字符', trigger: 'blur' }
        ],
        confirmPassword: [
          { required: true, message: '请再次输入密码', trigger: 'blur' },
          { validator: validatePass2, trigger: 'blur' }
        ]
      }
    };
  },
  methods: {
    handleRegister() {
      this.$refs.registerForm.validate(async (valid) => {
        if (valid) {
          try {
            this.loading = true;
            const response = await axios.post('http://localhost:9999/manager/register', {
              name: this.registerForm.name,
              password: this.registerForm.password
            });

            if (response.data === 'success') {
              this.$message.success('注册成功');
              this.$router.push({
                path: '/login',
                query: {
                  name: this.registerForm.name,
                  password: this.registerForm.password
                }
              });
            } else if (response.data === 'exists') {
              this.$message.error('用户名已存在');
            } else {
              this.$message.error('注册失败');
            }
          } catch (error) {
            this.$message.error('注册失败');
          } finally {
            this.loading = false;
          }
        }
      });
    },
    goToLogin() {
      this.registerForm.name = '';
      this.registerForm.password = '';
      this.registerForm.confirmPassword = '';
      this.$router.push({
        path: '/login',
        query: {}
      });
    }
  }
};
</script>

<style scoped>
.register-container {
  height: 100vh;
  display: flex;
  justify-content: center;
  align-items: center;
  background: linear-gradient(135deg, #e6f3ff 0%, #f0f7ff 100%);
}

.register-box {
  width: 400px;
  padding: 40px;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 12px 0 rgba(0, 0, 0, 0.1);
}

.register-header {
  text-align: center;
  margin-bottom: 40px;
}

.register-header h2 {
  margin: 0;
  font-size: 28px;
  color: #303133;
}

.register-header p {
  margin: 10px 0 0;
  font-size: 16px;
  color: #909399;
}

.register-form {
  margin-top: 30px;
}

.register-button {
  width: 100%;
  padding: 12px 0;
}

.login-link {
  text-align: center;
  margin-top: 20px;
  color: #606266;
}

.login-link .el-button {
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