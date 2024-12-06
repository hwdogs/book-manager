<template>
  <div class="manage-container">
    <div class="search-container">
      <el-input
        v-model="searchKeyword"
        placeholder="输入姓名或地址搜索"
        class="search-input"
        clearable
        @clear="resetSearch"
      >
        <el-button slot="append" icon="el-icon-search" @click="handleSearch"></el-button>
      </el-input>
    </div>

    <el-table :data="tableData" border style="width: 100%;">
      <el-table-column fixed prop="id" label="ID" width="70"></el-table-column>
      <el-table-column prop="name" label="姓名" width="120"></el-table-column>
      <el-table-column prop="sex" label="性别" width="80">
        <template slot-scope="scope">
          {{ scope.row.sex ? '男' : '女' }}
        </template>
      </el-table-column>
      <el-table-column prop="workAddress" label="工作地址"></el-table-column>
      <el-table-column prop="homeAddress" label="家庭地址"></el-table-column>
      <el-table-column prop="email" label="邮箱" width="180"></el-table-column>
      <el-table-column prop="createTime" label="创建时间" width="180">
        <template slot-scope="scope">
          {{ new Date(scope.row.createTime).toLocaleString() }}
        </template>
      </el-table-column>
      <el-table-column prop="notes" label="备注"></el-table-column>
      <el-table-column fixed="right" label="操作" width="150">
        <template slot-scope="scope">
          <el-button @click="update(scope.row)" type="text" size="small">修改</el-button>
          <el-button @click="deleteReader(scope.row)" type="text" size="small">删除</el-button>
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

export default {
  name: 'ReaderManage',
  data() {
    return {
      searchKeyword: '',
      pageSize: 8,
      total: 0,
      currentPage: 1,
      tableData: []
    };
  },
  methods: {
    deleteReader(row) {
      this.$confirm('确定要删除该读者吗？', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        axios.delete(`http://localhost:9999/reader/delete/${row.id}`).then(ret => {
          if (ret.data === 'success') {
            this.$message.success('删除成功');
            this.fetchData(this.currentPage);
          } else {
            this.$message.error('删除失败');
          }
        });
      }).catch(() => {});
    },
    update(row) {
      this.$router.push({
        path: "/ReaderUpdate",
        query: {
          id: row.id,
          page: this.currentPage
        }
      });
    },
    changePage(page) {
      this.currentPage = page;
      this.fetchData(page);
    },
    async handleSearch() {
      try {
        if (!this.searchKeyword.trim()) {
          this.fetchData(1);
          return;
        }
        const response = await axios.get(`http://localhost:9999/reader/search?keyword=${this.searchKeyword}`);
        this.tableData = response.data;
        this.total = response.data.length;
      } catch (error) {
        console.error('Search error:', error);
        this.$message.error('搜索失败');
      }
    },
    resetSearch() {
      this.searchKeyword = '';
      this.fetchData(1);
    },
    async fetchData(page) {
      try {
        const response = await axios.get(`http://localhost:9999/reader/findAll/${page - 1}/8`);
        this.tableData = response.data.content;
        this.total = response.data.totalElements;
        this.pageSize = response.data.size;
      } catch (error) {
        this.$message.error('获取数据失败');
      }
    }
  },
  created() {
    const page = parseInt(this.$route.query.page) || 1;
    this.currentPage = page;
    this.fetchData(page);
  }
};
</script>

<style scoped>
.manage-container {
  padding: 20px;
}

.search-container {
  margin-bottom: 20px;
}

.search-input {
  max-width: 400px;
}
</style> 