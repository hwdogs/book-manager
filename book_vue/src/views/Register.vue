<template>
    <div class="register-container">
        <el-card class="register-card">
            <div class="title">注册新账号</div>
            <el-form :model="registerForm" :rules="rules" ref="registerForm">
                <el-form-item prop="name">
                    <el-input v-model="registerForm.name" placeholder="用户名"></el-input>
                </el-form-item>
                <el-form-item prop="password">
                    <el-input v-model="registerForm.password" type="password" placeholder="密码"></el-input>
                </el-form-item>
                <el-form-item prop="confirmPassword">
                    <el-input v-model="registerForm.confirmPassword" type="password" placeholder="确认密码"></el-input>
                </el-form-item>
                <el-form-item>
                    <el-button type="primary" @click="handleRegister" style="width: 100%">注册</el-button>
                </el-form-item>
                <el-form-item>
                    <el-button @click="goToLogin" style="width: 100%">返回登录</el-button>
                </el-form-item>
            </el-form>
        </el-card>
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
            registerForm: {
                name: '',
                password: '',
                confirmPassword: ''
            },
            rules: {
                name: [
                    { required: true, message: '请输入用户名', trigger: 'blur' }
                ],
                password: [
                    { required: true, message: '请输入密码', trigger: 'blur' }
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
                        const response = await axios.post('http://localhost:9999/manager/register', {
                            name: this.registerForm.name,
                            password: this.registerForm.password
                        });

                        if (response.data === 'success') {
                            this.$message.success('注册成功');
                            this.$router.push('/login');
                        } else if (response.data === 'exists') {
                            this.$message.error('用户名已存在');
                        } else {
                            this.$message.error('注册失败');
                        }
                    } catch (error) {
                        this.$message.error('注册失败');
                    }
                }
            });
        },
        goToLogin() {
            this.$router.push('/login');
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
    background-color: #f3f3f3;
}

.register-card {
    width: 400px;
}

.title {
    text-align: center;
    font-size: 24px;
    margin-bottom: 30px;
}
</style>