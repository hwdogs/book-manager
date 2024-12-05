<template>
  <div class="add-container">
    <el-form :model="readerForm" :rules="rules" ref="readerForm" label-width="100px">
      <el-form-item label="姓名" prop="name">
        <el-input v-model="readerForm.name"></el-input>
      </el-form-item>
      <el-form-item label="性别" prop="sex">
        <el-radio-group v-model="readerForm.sex">
          <el-radio :label="true">男</el-radio>
          <el-radio :label="false">女</el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item label="工作地址" prop="workAddress">
        <el-input v-model="readerForm.workAddress"></el-input>
      </el-form-item>
      <el-form-item label="家庭地址" prop="homeAddress">
        <el-input v-model="readerForm.homeAddress"></el-input>
      </el-form-item>
      <el-form-item label="邮箱" prop="email">
        <el-input v-model="readerForm.email"></el-input>
      </el-form-item>
      <el-form-item label="备注" prop="notes">
        <el-input type="textarea" v-model="readerForm.notes"></el-input>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="submitForm">立即创建</el-button>
        <el-button @click="resetForm">重置</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import axios from 'axios'

export default {
  name: 'AddReader',
  data() {
    return {
      readerForm: {
        name: '',
        sex: true,
        workAddress: '',
        homeAddress: '',
        email: '',
        notes: ''
      },
      rules: {
        name: [
          { required: true, message: '请输入姓名', trigger: 'blur' }
        ],
        workAddress: [
          { required: true, message: '请输入工作地址', trigger: 'blur' }
        ],
        homeAddress: [
          { required: true, message: '请输入家庭地址', trigger: 'blur' }
        ],
        email: [
          { type: 'email', message: '请输入正确的邮箱地址', trigger: 'blur' }
        ]
      }
    }
  },
  methods: {
    submitForm() {
      this.$refs.readerForm.validate((valid) => {
        if (valid) {
          axios.post('http://localhost:9999/reader/save', this.readerForm)
            .then(ret => {
              if (ret.data === 'success') {
                this.$message.success('添加成功');
                this.$refs.readerForm.resetFields();
              } else {
                this.$message.error('添加失败');
              }
            })
            .catch(() => {
              this.$message.error('添加失败');
            });
        }
      });
    },
    resetForm() {
      this.$refs.readerForm.resetFields();
    }
  }
}
</script>

<style scoped>
.add-container {
  padding: 20px;
  max-width: 600px;
  margin: 0 auto;
}
</style> 