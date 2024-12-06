<template>
  <div class="records-container">
    <div class="search-container">
      <el-input
        v-model="searchKeyword"
        placeholder="输入读者姓名或图书名称搜索"
        class="search-input"
        clearable
        @clear="resetSearch"
      >
        <el-button slot="append" icon="el-icon-search" @click="handleSearch"></el-button>
      </el-input>
    </div>

    <el-table :data="tableData" border style="width: 100%">
      <el-table-column fixed prop="id" label="ID" width="80"></el-table-column>
      <el-table-column prop="readerName" label="读者姓名" width="120"></el-table-column>
      <el-table-column prop="bookName" label="图书名称" min-width="150"></el-table-column>
      <el-table-column prop="lendDate" label="借出时间" width="200">
        <template slot-scope="scope">
          {{ new Date(scope.row.lendDate).toLocaleString() }}
        </template>
      </el-table-column>
      <el-table-column prop="returnDate" label="归还时间" width="200">
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
    <el-pagination
      style="padding-top:20px"
      background
      layout="prev, pager, next"
      :total="total"
      :page-size="pageSize"
      :current-page.sync="currentPage"
      @current-change="changePage">
    </el-pagination>
  </div>
</template>

<script>
import axios from 'axios'
import { EventBus } from '../utils/eventBus'

export default {
  name: 'LendRecords',
  data() {
    return {
      searchKeyword: '',
      pageSize: 8,
      total: 0,
      currentPage: 1,
      tableData: []
    }
  },
  methods: {
    async returnBook(row) {
      try {
        const response = await axios.post(`http://localhost:9999/lendReturn/return/${row.id}`);
        if (response.data === 'success') {
          this.$message.success('还书成功');
          this.fetchData(this.currentPage);
          EventBus.$emit('book-returned');
        } else {
          this.$message.error('还书失败');
        }
      } catch (error) {
        console.error('Return book error:', error);
        this.$message.error('还书失败: ' + (error.response?.data?.error || error.message));
      }
    },
    changePage(page) {
      this.currentPage = page;
      this.fetchData(page);
    },
    async fetchData(page) {
      try {
        console.log('Fetching page:', page);
        const response = await axios.get(`http://localhost:9999/lendReturn/findAll/${page - 1}/8`);
        console.log('Response:', response.data);
        
        if (response.data && response.data.content) {
          this.tableData = response.data.content;
          this.total = response.data.totalElements;
          this.pageSize = response.data.size;
        } else {
          console.error('Invalid response format:', response.data);
          this.$message.error('数据格式错误');
        }
      } catch (error) {
        console.error('Fetch data error:', error);
        this.$message.error('获取数据失败: ' + (error.response?.data?.error || error.message));
      }
    },
    isOverdue(lendDate) {
      const now = new Date();
      const lendTime = new Date(lendDate);
      const diffDays = Math.floor((now - lendTime) / (1000 * 60 * 60 * 24));
      return diffDays > 60;
    },
    async handleSearch() {
      try {
        if (!this.searchKeyword.trim()) {
          this.fetchData(1);
          return;
        }

        const [readerResponse, bookResponse] = await Promise.all([
          axios.get(`http://localhost:9999/lendReturn/searchByReader?keyword=${this.searchKeyword}`),
          axios.get(`http://localhost:9999/lendReturn/searchByBook?keyword=${this.searchKeyword}`)
        ]);

        const allRecords = [...readerResponse.data, ...bookResponse.data];
        const uniqueRecords = Array.from(new Map(allRecords.map(item => [item.id, item])).values());
        
        this.tableData = uniqueRecords;
        this.total = uniqueRecords.length;
      } catch (error) {
        console.error('Search error:', error);
        this.$message.error('搜索失败');
      }
    },
    resetSearch() {
      this.searchKeyword = '';
      this.fetchData(1);
    }
  },
  created() {
    const page = parseInt(this.$route.query.page) || 1;
    this.currentPage = page;
    this.fetchData(page);
  }
}
</script>

<style scoped>
.records-container {
  padding: 20px;
}

.search-container {
  margin-bottom: 20px;
}

.search-input {
  max-width: 400px;
}

.overdue-return {
  color: #F56C6C !important;
}
</style> 