<template>
  <div class="add-container">
    <el-form :model="bookForm" :rules="rules" ref="bookForm" label-width="100px">
      <el-form-item label="书名" prop="name">
        <el-input v-model="bookForm.name"></el-input>
      </el-form-item>
      <el-form-item label="作者" prop="author">
        <el-input v-model="bookForm.author"></el-input>
      </el-form-item>
      <el-form-item label="出版社" prop="publish">
        <el-input v-model="bookForm.publish"></el-input>
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
  name: 'AddBook',
  data() {
    return {
      bookForm: {
        name: '',
        author: '',
        publish: ''
      },
      rules: {
        name: [
          { required: true, message: '请输入书名', trigger: 'blur' }
        ],
        author: [
          { required: true, message: '请输入作者', trigger: 'blur' }
        ],
        publish: [
          { required: true, message: '请输入出版社', trigger: 'blur' }
        ]
      }
    }
  },
  methods: {
    submitForm() {
      this.$refs.bookForm.validate((valid) => {
        if (valid) {
          axios.post('http://localhost:9999/book/save', this.bookForm)
            .then(ret => {
              if (ret.data === 'success') {
                this.$message.success('添加成功');
                this.$refs.bookForm.resetFields();
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
      this.$refs.bookForm.resetFields();
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