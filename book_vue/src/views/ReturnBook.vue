<template>
  <div class="return-container">
    <el-form :model="searchForm" ref="searchForm" label-width="100px" class="search-form">
      <el-form-item label="搜索方式">
        <el-radio-group v-model="searchType">
          <el-radio label="reader">按读者搜索</el-radio>
          <el-radio label="book">按图书搜索</el-radio>
        </el-radio-group>
      </el-form-item>
      <el-form-item :label="searchType === 'reader' ? '读者' : '图书'">
        <el-input v-model="searchForm.keyword">
          <template slot="append">
            <el-button @click="searchRecords">查找</el-button>
          </template>
        </el-input>
      </el-form-item>
    </el-form>

    <div class="records-list">
      <h3>借阅记录</h3>
      <el-table :data="records" border style="width: 100%">
        <el-table-column prop="id" label="借阅ID" width="80"></el-table-column>
        <el-table-column prop="readerName" label="读者姓名" width="120"></el-table-column>
        <el-table-column prop="bookName" label="图书名称"></el-table-column>
        <el-table-column prop="lendDate" label="借出时间" width="180">
          <template slot-scope="scope">
            {{ new Date(scope.row.lendDate).toLocaleString() }}
          </template>
        </el-table-column>
        <el-table-column prop="returnDate" label="归还时间" width="180">
          <template slot-scope="scope">
            {{ scope.row.returnDate ? new Date(scope.row.returnDate).toLocaleString() : '未归还' }}
          </template>
        </el-table-column>
        <el-table-column fixed="right" label="操作" width="120">
          <template slot-scope="scope">
            <el-button 
              @click="returnBook(scope.row)" 
              type="text" 
              size="small"
              v-if="!scope.row.returnDate"
              :class="{ 'overdue-return': isOverdue(scope.row.lendDate) }">
              {{ isOverdue(scope.row.lendDate) ? '超期归还' : '归还' }}
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </div>
  </div>
</template>

<script>
import axios from 'axios'
import { EventBus } from '../utils/eventBus'

export default {
  name: 'ReturnBook',
  data() {
    return {
      searchType: 'reader',
      searchForm: {
        keyword: ''
      },
      records: []
    }
  },
  methods: {
    async searchRecords() {
      if (!this.searchForm.keyword) {
        this.$message.warning('请输入搜索关键词');
        return;
      }

      try {
        let url = this.searchType === 'reader' 
          ? `http://localhost:9999/lendReturn/searchByReader?keyword=${this.searchForm.keyword}`
          : `http://localhost:9999/lendReturn/searchByBook?keyword=${this.searchForm.keyword}`;

        const response = await axios.get(url);
        this.records = response.data.filter(r => !r.returnDate);
        
        if (this.records.length === 0) {
          if (this.searchForm.keyword) {
            this.$message.warning('未找到未归还的借阅记录');
          }
        }
      } catch (error) {
        console.error('Search error:', error);
        this.$message.error('搜索失败');
      }
    },
    async returnBook(record) {
      try {
        const response = await axios.post(`http://localhost:9999/lendReturn/return/${record.id}`);
        if (response.data === 'success') {
          this.$message.success('还书成功');
          this.records = this.records.filter(r => r.id !== record.id);
          EventBus.$emit('book-returned');
        } else {
          this.$message.error('还书失败');
        }
      } catch (error) {
        console.error('Return book error:', error);
        this.$message.error('还书失败');
      }
    },
    isOverdue(lendDate) {
      const now = new Date();
      const lendTime = new Date(lendDate);
      const diffDays = Math.floor((now - lendTime) / (1000 * 60 * 60 * 24));
      return diffDays > 60;
    }
  }
}
</script>

<style scoped>
.return-container {
  padding: 20px;
}

.search-form {
  max-width: 600px;
  margin: 0 auto 20px;
}

.records-list {
  background: #fff;
  padding: 20px;
  border-radius: 4px;
  box-shadow: 0 2px 12px 0 rgba(0,0,0,0.1);
}

.records-list h3 {
  margin-top: 0;
  margin-bottom: 15px;
}

.overdue-return {
  color: #F56C6C !important;
}
</style> 