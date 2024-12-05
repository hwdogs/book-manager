<template>
  <div class="update-container">
    <el-form :model="updateForm" :rules="rules" ref="updateForm" label-width="100px">
      <el-form-item label="姓名" prop="name">
        <el-input v-model="updateForm.name"></el-input>
      </el-form-item>
      <el-form-item label="性别" prop="sex">
        <el-radio-group v-model="updateForm.sex">
          <el-radio :label="true">男</el-radio>
          <el-radio :label="false">女</el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item label="工作地址" prop="workAddress">
        <el-input v-model="updateForm.workAddress"></el-input>
      </el-form-item>
      <el-form-item label="家庭地址" prop="homeAddress">
        <el-input v-model="updateForm.homeAddress"></el-input>
      </el-form-item>
      <el-form-item label="邮箱" prop="email">
        <el-input v-model="updateForm.email"></el-input>
      </el-form-item>
      <el-form-item label="备注" prop="notes">
        <el-input type="textarea" v-model="updateForm.notes"></el-input>
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
  name: 'ReaderUpdate',
  data() {
    return {
      updateForm: {
        id: '',
        name: '',
        sex: true,
        workAddress: '',
        homeAddress: '',
        email: '',
        notes: ''
      },
      currentPage: 1,
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
    handleUpdate() {
      this.$refs.updateForm.validate(async (valid) => {
        if (valid) {
          try {
            const response = await axios.put('http://localhost:9999/reader/update', this.updateForm);
            if (response.data === 'success') {
              this.$message.success('更新成功');
              this.$router.push({
                path: '/ReaderManage',
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
        path: '/ReaderManage',
        query: { page: this.currentPage }
      });
    },
    async fetchReaderData() {
      try {
        const id = this.$route.query.id;
        this.currentPage = this.$route.query.page || 1;
        const response = await axios.get(`http://localhost:9999/reader/findById/${id}`);
        this.updateForm = response.data;
      } catch (error) {
        this.$message.error('获取读者信息失败');
      }
    }
  },
  created() {
    this.fetchReaderData();
  }
}
</script>

<style scoped>
.update-container {
  padding: 20px;
  max-width: 600px;
  margin: 0 auto;
}
</style> 