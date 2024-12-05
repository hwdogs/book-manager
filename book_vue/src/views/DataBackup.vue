<template>
  <div class="backup-container">
    <el-card class="backup-card">
      <div slot="header">
        <span><i class="el-icon-document-copy"></i> 数据备份与恢复</span>
        <el-button style="float: right; margin-left: 10px;" type="primary" @click="createBackup">
          <i class="el-icon-plus"></i> 创建备份
        </el-button>
        <el-upload
          class="upload-demo"
          style="float: right;"
          action="http://localhost:9999/backup/restore"
          :show-file-list="false"
          :on-success="handleRestoreSuccess"
          :on-error="handleRestoreError"
          accept=".sql">
          <el-button type="warning">
            <i class="el-icon-upload2"></i> 恢复备份
          </el-button>
        </el-upload>
      </div>
      
      <el-table :data="backupFiles" style="width: 100%">
        <el-table-column prop="name" label="文件名">
          <template slot="header" slot-scope="scope">
            <i class="el-icon-document"></i> 文件名
          </template>
        </el-table-column>
        <el-table-column prop="size" label="大小">
          <template slot="header" slot-scope="scope">
            <i class="el-icon-files"></i> 大小
          </template>
          <template slot-scope="scope">
            {{ formatFileSize(scope.row.size) }}
          </template>
        </el-table-column>
        <el-table-column prop="createTime" label="创建时间">
          <template slot="header" slot-scope="scope">
            <i class="el-icon-time"></i> 创建时间
          </template>
          <template slot-scope="scope">
            {{ new Date(scope.row.createTime).toLocaleString() }}
          </template>
        </el-table-column>
        <el-table-column label="操作" width="200">
          <template slot="header" slot-scope="scope">
            <i class="el-icon-setting"></i> 操作
          </template>
          <template slot-scope="scope">
            <el-button 
              type="text" 
              @click="downloadBackup(scope.row)">
              <i class="el-icon-download"></i> 下载
            </el-button>
            <el-button 
              type="text" 
              class="delete-button"
              @click="handleDelete(scope.row)">
              <i class="el-icon-delete"></i> 删除
            </el-button>
          </template>
        </el-table-column>
      </el-table>
    </el-card>
  </div>
</template>

<script>
import axios from 'axios'
import { EventBus } from '../utils/eventBus'  // 导入EventBus

export default {
  name: 'DataBackup',
  data() {
    return {
      backupFiles: []
    }
  },
  methods: {
    async fetchBackupList() {
      try {
        const response = await axios.get('http://localhost:9999/backup/list');
        if (response.data.status === 'success') {
          this.backupFiles = response.data.files;
        } else {
          this.$message.error('获取备份列表失败');
        }
      } catch (error) {
        this.$message.error('获取备份列表失败');
      }
    },
    async createBackup() {
      try {
        const response = await axios.post('http://localhost:9999/backup/create');
        if (response.data.status === 'success') {
          this.$message.success('备份创建成功');
          this.fetchBackupList();
        } else {
          this.$message.error('备份创建失败: ' + response.data.message);
        }
      } catch (error) {
        this.$message.error('备份创建失败');
      }
    },
    handleRestoreSuccess(response) {
      if (response.status === 'success') {
        this.$message.success('数据恢复成功');
        // 触发更新超期图书数量
        EventBus.$emit('book-returned');
        // 刷新备份列表
        this.fetchBackupList();
      } else {
        this.$message.error('数据恢复失败: ' + response.message);
      }
    },
    handleRestoreError() {
      this.$message.error('数据恢复失败');
    },
    formatFileSize(bytes) {
      if (bytes === 0) return '0 B';
      const k = 1024;
      const sizes = ['B', 'KB', 'MB', 'GB'];
      const i = Math.floor(Math.log(bytes) / Math.log(k));
      return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
    },
    async downloadBackup(file) {
      try {
        const response = await axios.get(
          `http://localhost:9999/backup/download/${file.name}`,
          {
            responseType: 'blob'
          }
        );
        
        const url = window.URL.createObjectURL(new Blob([response.data]));
        const link = document.createElement('a');
        link.href = url;
        link.download = file.name;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        window.URL.revokeObjectURL(url);
      } catch (error) {
        this.$message.error('下载失败');
        console.error('Download error:', error);
      }
    },
    async handleDelete(file) {
      try {
        await this.$confirm('此操作将永久删除该备份文件, 是否继续?', '提示', {
          confirmButtonText: '确定',
          cancelButtonText: '取消',
          type: 'warning'
        });
        
        const response = await axios.delete(`http://localhost:9999/backup/delete/${file.name}`);
        if (response.data.status === 'success') {
          this.$message.success('删除成功');
          this.fetchBackupList();
        } else {
          this.$message.error('删除失败: ' + response.data.message);
        }
      } catch (error) {
        if (error !== 'cancel') {
          this.$message.error('删除失败');
          console.error('Delete error:', error);
        }
      }
    }
  },
  created() {
    this.fetchBackupList();
  }
}
</script>

<style scoped>
.backup-container {
  padding: 20px;
}

.backup-card {
  margin-bottom: 20px;
}

.el-upload {
  display: inline-block;
}

.upload-demo {
  margin-right: 10px;
}

.delete-button {
  color: #F56C6C;
}

.delete-button:hover {
  color: #f78989;
}

/* 添加图标和标题的样式 */
.el-card__header span {
  font-size: 16px;
  font-weight: bold;
}

.el-card__header i {
  margin-right: 5px;
}

/* 表格图标样式 */
.el-table th i {
  margin-right: 5px;
  color: #909399;
}
</style> 