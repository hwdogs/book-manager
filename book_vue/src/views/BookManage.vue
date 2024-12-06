<template>
  <div class="manage-container">
    <div class="search-container">
      <el-input
        v-model="searchKeyword"
        placeholder="输入书名、作者或出版社搜索"
        class="search-input"
        clearable
        @clear="resetSearch"
      >
        <el-button slot="append" icon="el-icon-search" @click="handleSearch"></el-button>
      </el-input>
    </div>

    <el-table :data="tableData" border style="width: 100%;">
      <el-table-column fixed prop="id" label="ID"></el-table-column>
      <el-table-column prop="name" label="书名"></el-table-column>
      <el-table-column prop="author" label="作者"></el-table-column>
      <el-table-column prop="publish" label="出版社"></el-table-column>
      <el-table-column fixed="right" label="操作">
        <template slot-scope="scope">
          <el-button @click="update(scope.row)" type="text" size="small">修改</el-button>
          <el-button @click="deleteBook(scope.row)" type="text" size="small">删除</el-button>
        </template>
      </el-table-column>
    </el-table>
    <el-pagination style="padding-top:20px" background layout="prev, pager, next" :total="total" :page-size="pageSize"
      :current-page.sync="currentPage" @current-change="changePage">
    </el-pagination>
  </div>
</template>

<script>
import axios from 'axios'

export default {
  methods: {
    deleteBook(row) {
      this.$confirm('确定要删除该图书吗?', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        axios.delete("http://localhost:9999/book/delete/" + row.id).then(ret => {
          this.$message.success("《" + row.name + "》已删除");
          
          // 计算删除后的总页数
          const newTotal = this.total - 1;
          const newTotalPages = Math.ceil(newTotal / this.pageSize);
          
          // 如果当前页大于新的总页数，就跳转到最后一页
          if (this.currentPage > newTotalPages) {
            this.currentPage = Math.max(1, newTotalPages);
          }
          
          // 重新获取当前页数据
          this.fetchData(this.currentPage);
        }).catch(() => {
          this.$message.error('删除失败');
        });
      }).catch(() => {
        // 取消删除操作
      });
    },
    update(row) {
      this.$router.push({
        path: "/BookUpdate",
        query: {
          id: row.id,
          page: this.currentPage
        }
      });
    },
    changePage(page) {
      this.currentPage = page;
      axios
        .get(
          "http://localhost:9999/book/findAll/" +
          (page - 1) +
          "/" +
          this.pageSize,
          {
            params: {}
          }
        )
        .then(ret => {
          console.log(ret.data);
          this.tableData = ret.data.content;
          this.total = ret.data.totalElements;
          this.pageSize = ret.data.size;
        });
    },
    async handleSearch() {
      try {
        if (!this.searchKeyword.trim()) {
          this.fetchData(1);
          return;
        }
        const response = await axios.get(`http://localhost:9999/book/search?keyword=${this.searchKeyword}`);
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
        const response = await axios.get(`http://localhost:9999/book/findAll/${page - 1}/8`);
        this.tableData = response.data.content;
        this.total = response.data.totalElements;
        this.pageSize = response.data.size;
      } catch (error) {
        this.$message.error('获取数据失败');
      }
    }
  },
  data() {
    return {
      searchKeyword: '',
      pageSize: 8,
      total: null,
      currentPage: 1,
      tableData: []
    };
  },
  created() {
    const page = parseInt(this.$route.query.page) || 1;
    this.currentPage = page;

    axios
      .get(`http://localhost:9999/book/findAll/${page - 1}/8`, {
        params: {}
      })
      .then(ret => {
        this.tableData = ret.data.content;
        this.total = ret.data.totalElements;
        this.pageSize = ret.data.size;
      });
  },
  // 添加路由监听
  watch: {
    '$route.query.page': {
      handler(newPage) {
        if (newPage) {
          this.currentPage = parseInt(newPage);
        }
      },
      immediate: true
    }
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
