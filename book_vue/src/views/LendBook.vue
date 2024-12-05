<template>
  <div class="lend-container">
    <el-form :model="lendForm" :rules="rules" ref="lendForm" label-width="100px" class="search-form">
      <el-form-item label="读者" prop="readerSearch">
        <el-input v-model="lendForm.readerSearch">
          <template slot="append">
            <el-button @click="searchReader">查找读者</el-button>
          </template>
        </el-input>
      </el-form-item>
      <el-form-item label="图书" prop="bookSearch">
        <el-input v-model="lendForm.bookSearch">
          <template slot="append">
            <el-button @click="searchBook">查找图书</el-button>
          </template>
        </el-input>
      </el-form-item>
    </el-form>

    <div class="tables-container">
      <div class="table-section">
        <h3>读者信息</h3>
        <el-table :data="readers" border style="width: 100%">
          <el-table-column prop="id" label="ID" width="80"></el-table-column>
          <el-table-column prop="name" label="姓名" width="120"></el-table-column>
          <el-table-column prop="sex" label="性别" width="80">
            <template slot-scope="scope">
              {{ scope.row.sex ? '男' : '女' }}
            </template>
          </el-table-column>
          <el-table-column prop="workAddress" label="工作地址"></el-table-column>
          <el-table-column fixed="right" label="操作" width="120">
            <template slot-scope="scope">
              <el-button @click="selectReader(scope.row)" type="text" size="small">
                选择
              </el-button>
            </template>
          </el-table-column>
        </el-table>
      </div>

      <div class="table-section">
        <h3>图书信息</h3>
        <el-table :data="books" border style="width: 100%">
          <el-table-column prop="id" label="ID" width="80"></el-table-column>
          <el-table-column prop="name" label="书名" width="180"></el-table-column>
          <el-table-column prop="author" label="作者" width="120"></el-table-column>
          <el-table-column prop="publish" label="出版社"></el-table-column>
          <el-table-column fixed="right" label="操作" width="120">
            <template slot-scope="scope">
              <el-button @click="selectBook(scope.row)" type="text" size="small">
                选择
              </el-button>
            </template>
          </el-table-column>
        </el-table>
      </div>

      <div class="selected-info" v-if="selectedReader || selectedBook">
        <h3>已选信息</h3>
        <p v-if="selectedReader">已选读者：{{ selectedReader.name }}</p>
        <p v-if="selectedBook">已选图书：{{ selectedBook.name }}</p>
        <el-button 
          type="primary" 
          @click="submitForm" 
          :disabled="!selectedReader || !selectedBook">
          确认借书
        </el-button>
        <el-button @click="resetForm">重置</el-button>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios'

export default {
  name: 'LendBook',
  data() {
    return {
      lendForm: {
        readerSearch: '',
        bookSearch: ''
      },
      readers: [],
      books: [],
      selectedReader: null,
      selectedBook: null,
      rules: {
        readerSearch: [
          { required: true, message: '请输入读者姓名或ID', trigger: 'blur' }
        ],
        bookSearch: [
          { required: true, message: '请输入图书名称或ID', trigger: 'blur' }
        ]
      }
    }
  },
  methods: {
    async searchReader() {
      if (!this.lendForm.readerSearch) {
        this.$message.warning('请输入读者姓名或ID');
        return;
      }
      try {
        // 假设后端提供了按名称搜索的API
        const response = await axios.get(`http://localhost:9999/reader/search?keyword=${this.lendForm.readerSearch}`);
        this.readers = response.data;
        if (this.readers.length === 0) {
          this.$message.warning('未找到相关读者');
        }
      } catch (error) {
        this.$message.error('搜索读者失败');
        this.readers = [];
      }
    },
    async searchBook() {
      if (!this.lendForm.bookSearch) {
        this.$message.warning('请输入图书名称或ID');
        return;
      }
      try {
        // 假设后端提供了按名称搜索的API
        const response = await axios.get(`http://localhost:9999/book/search?keyword=${this.lendForm.bookSearch}`);
        this.books = response.data;
        if (this.books.length === 0) {
          this.$message.warning('未找到相关图书');
        }
      } catch (error) {
        this.$message.error('搜索图书失败');
        this.books = [];
      }
    },
    selectReader(reader) {
      this.selectedReader = reader;
    },
    selectBook(book) {
      this.selectedBook = book;
    },
    submitForm() {
      if (!this.selectedReader || !this.selectedBook) {
        this.$message.warning('请选择读者和图书');
        return;
      }

      this.$refs.lendForm.validate(async (valid) => {
        if (valid) {
          try {
            const lendData = {
              readerId: this.selectedReader.id,
              bookId: this.selectedBook.id
            };

            const response = await axios.post('http://localhost:9999/lendReturn/lend', lendData);
            
            if (response.data === 'success') {
              this.$message.success('借书成功');
              this.resetForm();
            } else if (response.data === 'borrowed') {
              this.$message.warning('该图书已被借出');
            } else {
              this.$message.error('借书失败');
            }
          } catch (error) {
            console.error('Lend book error:', error);
            this.$message.error('借书失败: ' + (error.response?.data?.error || error.message));
          }
        }
      });
    },
    resetForm() {
      this.$refs.lendForm.resetFields();
      this.readers = [];
      this.books = [];
      this.selectedReader = null;
      this.selectedBook = null;
    }
  }
}
</script>

<style scoped>
.lend-container {
  padding: 20px;
}

.search-form {
  max-width: 600px;
  margin: 0 auto 20px;
}

.tables-container {
  display: flex;
  flex-direction: column;
  gap: 20px;
}

.table-section {
  background: #fff;
  padding: 20px;
  border-radius: 4px;
  box-shadow: 0 2px 12px 0 rgba(0,0,0,0.1);
}

.table-section h3 {
  margin-top: 0;
  margin-bottom: 15px;
}

.selected-info {
  background: #f5f7fa;
  padding: 20px;
  border-radius: 4px;
  margin-top: 20px;
  text-align: center;
}

.selected-info p {
  margin: 10px 0;
}

.selected-info .el-button {
  margin-top: 15px;
}
</style> 