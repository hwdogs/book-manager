<template>
  <div class="profile-container">
    <h2>个人信息</h2>
    <el-form :model="profileForm" :rules="rules" ref="profileForm" label-width="100px" class="profile-form">
      <el-form-item label="用户名">
        <el-input 
          v-model="profileForm.username" 
          disabled
          placeholder="用户名"
          :readonly="true"
          class="readonly-input">
        </el-input>
      </el-form-item>
      <el-form-item label="旧密码" prop="oldPassword">
        <el-input type="password" v-model="profileForm.oldPassword" show-password></el-input>
      </el-form-item>
      <el-form-item label="新密码" prop="newPassword">
        <el-input type="password" v-model="profileForm.newPassword" show-password></el-input>
      </el-form-item>
      <el-form-item label="确认密码" prop="confirmPassword">
        <el-input type="password" v-model="profileForm.confirmPassword" show-password></el-input>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="submitForm" :loading="loading">保存修改</el-button>
        <el-button @click="resetForm">重置</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import axios from 'axios'

export default {
  name: 'Profile',
  data() {
    const validatePass = (rule, value, callback) => {
      if (value === '') {
        callback(new Error('请输入新密码'));
      } else if (value === this.profileForm.oldPassword) {
        callback(new Error('新密码不能与旧密码相同'));
      } else if (value.length < 6) {
        callback(new Error('密码长度不能小于6个字符'));
      } else {
        if (this.profileForm.confirmPassword !== '') {
          this.$refs.profileForm.validateField('confirmPassword');
        }
        callback();
      }
    };
    const validatePass2 = (rule, value, callback) => {
      if (value === '') {
        callback(new Error('请再次输入密码'));
      } else if (value !== this.profileForm.newPassword) {
        callback(new Error('两次输入密码不一致!'));
      } else {
        callback();
      }
    };
    return {
      loading: false,
      profileForm: {
        username: localStorage.getItem('name') || '',
        oldPassword: '',
        newPassword: '',
        confirmPassword: ''
      },
      rules: {
        oldPassword: [
          { required: true, message: '请输入旧密码', trigger: 'blur' }
        ],
        newPassword: [
          { required: true, message: '请输入新密码', trigger: 'blur' },
          { min: 6, message: '密码长度不能小于6个字符', trigger: 'blur' },
          { validator: validatePass, trigger: 'blur' }
        ],
        confirmPassword: [
          { required: true, message: '请再次输入密码', trigger: 'blur' },
          { validator: validatePass2, trigger: 'blur' }
        ]
      }
    }
  },
  created() {
    this.profileForm.username = localStorage.getItem('name') || '';
  },
  methods: {
    submitForm() {
      this.$refs.profileForm.validate(async (valid) => {
        if (valid) {
          try {
            this.loading = true;
            const response = await axios.put('http://localhost:9999/manager/updateProfile', {
              username: this.profileForm.username,
              oldPassword: this.profileForm.oldPassword,
              newPassword: this.profileForm.newPassword
            });

            switch (response.data) {
              case 'success':
                this.$message.success('修改成功，请重新登录');
                localStorage.removeItem('isLoggedIn');
                localStorage.removeItem('name');
                this.$router.push('/login');
                break;
              case 'notfound':
                this.$message.error('用户不存在');
                break;
              case 'wrongpassword':
                this.$message.error('旧密码错误');
                break;
              default:
                this.$message.error('修改失败');
            }
          } catch (error) {
            console.error('Update profile error:', error);
            this.$message.error('修改失败: ' + (error.response?.data?.error || error.message));
          } finally {
            this.loading = false;
          }
        }
      });
    },
    resetForm() {
      this.$refs.profileForm.resetFields();
    }
  }
}
</script>

<style scoped>
.profile-container {
  padding: 20px;
}

.profile-form {
  max-width: 500px;
  margin: 20px auto;
  background: #fff;
  padding: 30px;
  border-radius: 6px;
  box-shadow: 0 2px 12px 0 rgba(0,0,0,0.1);
}

h2 {
  text-align: center;
  margin-bottom: 30px;
  color: #333;
}

.readonly-input >>> .el-input__inner {
  background-color: #f5f7fa;
  color: #606266;
  cursor: not-allowed;
}

.readonly-input >>> .el-input__inner::placeholder {
  color: #606266;
}
</style> 