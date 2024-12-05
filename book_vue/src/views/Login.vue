<template>
    <div class="login-container">
        <el-card class="login-card">
            <div class="title">图书管理系统</div>
            <el-form :model="loginForm" :rules="rules" ref="loginForm">
                <el-form-item prop="name">
                    <el-input v-model="loginForm.name" placeholder="用户名"></el-input>
                </el-form-item>
                <el-form-item prop="password">
                    <el-input v-model="loginForm.password" type="password" placeholder="密码"></el-input>
                </el-form-item>
                <el-form-item>
                    <el-button type="primary" @click="handleLogin" style="width: 100%">登录</el-button>
                </el-form-item>
                <el-form-item>
                    <el-button @click="goToRegister" style="width: 100%">注册新账号</el-button>
                </el-form-item>
            </el-form>
        </el-card>
    </div>
</template>

<script>
import axios from 'axios';

export default {
    name: 'Login',
    data() {
        return {
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
                        const response = await axios.post('http://localhost:9999/manager/login', this.loginForm);
                        if (response.data.status === 'success') {
                            localStorage.setItem('isLoggedIn', 'true');
                            localStorage.setItem('name', response.data.name);
                            this.$router.push('/home');
                        } else {
                            this.$message.error('用户名或密码错误');
                        }
                    } catch (error) {
                        this.$message.error('登录失败');
                    }
                }
            });
        },
        goToRegister() {
            this.$router.push('/register');
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
    background-color: #f3f3f3;
}

.login-card {
    width: 400px;
}

.title {
    text-align: center;
    font-size: 24px;
    margin-bottom: 30px;
}
</style>