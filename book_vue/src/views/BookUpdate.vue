<template>
  <div class="update-container">
    <el-form :model="updateForm" :rules="rules" ref="updateForm" label-width="80px">
      <el-form-item label="书名" prop="name">
        <el-input v-model="updateForm.name"></el-input>
      </el-form-item>
      <el-form-item label="作者" prop="author">
        <el-input v-model="updateForm.author"></el-input>
      </el-form-item>
      <el-form-item label="出版社" prop="publish">
        <el-input v-model="updateForm.publish"></el-input>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="handleUpdate">更新</el-button>
        <el-button @click="goBack">返回</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
import axios from 'axios'

export default {
  name: 'BookUpdate',
  data() {
    return {
      updateForm: {
        id: '',
        name: '',
        author: '',
        publish: ''
      },
      currentPage: 1,
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
    handleUpdate() {
      this.$refs.updateForm.validate(async (valid) => {
        if (valid) {
          try {
            const response = await axios.put('http://localhost:9999/book/update', this.updateForm);
            if (response.data === 'success') {
              this.$message.success('更新成功');
              this.$router.push({
                path: '/BookManage',
                query: { page: this.currentPage }
              });
            } else {
              this.$message.error('更新失败');
            }
          } catch (error) {
            this.$message.error('更新失败');
          }
        }
      });
    },
    goBack() {
      this.$router.push({
        path: '/BookManage',
        query: { page: this.currentPage }
      });
    },
    async fetchBookData() {
      try {
        const id = this.$route.query.id;
        this.currentPage = this.$route.query.page || 1;
        const response = await axios.get(`http://localhost:9999/book/findById/${id}`);
        this.updateForm = response.data;
      } catch (error) {
        this.$message.error('获取图书信息失败');
      }
    }
  },
  created() {
    this.fetchBookData();
  }
}
</script>

<style scoped>
.update-container {
  padding: 20px;
  max-width: 500px;
  margin: 0 auto;
}
</style>