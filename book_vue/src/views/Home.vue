<template>
  <div class="home-container">
    <div class="chart-container">
      <h3>借阅趋势</h3>
      <div v-loading="loading" class="chart-wrapper">
        <div ref="trendChart" style="height: 300px"></div>
        <div v-if="!loading && (!trendData || trendData.length === 0)" class="no-data">
          暂无数据
        </div>
      </div>
    </div>

    <el-row :gutter="20" style="margin-top: 20px;">
      <el-col :span="12">
        <div class="chart-container">
          <h3>借阅量Top10图书</h3>
          <div v-loading="loading" class="chart-wrapper">
            <div ref="bookChart" style="height: 400px"></div>
            <div v-if="!loading && (!bookData || bookData.length === 0)" class="no-data">
              暂无数据
            </div>
          </div>
        </div>
      </el-col>
      <el-col :span="12">
        <div class="chart-container">
          <h3>借阅量Top10读者</h3>
          <div v-loading="loading" class="chart-wrapper">
            <div ref="readerChart" style="height: 400px"></div>
            <div v-if="!loading && (!readerData || readerData.length === 0)" class="no-data">
              暂无数据
            </div>
          </div>
        </div>
      </el-col>
    </el-row>
  </div>
</template>

<script>
import axios from 'axios'
import * as echarts from 'echarts'

export default {
  name: 'Home',
  data() {
    return {
      bookChart: null,
      readerChart: null,
      trendChart: null,
      loading: true,
      bookData: [],
      readerData: [],
      trendData: []
    }
  },
  methods: {
    async fetchData() {
      try {
        const [booksRes, readersRes, trendRes] = await Promise.all([
          axios.get('http://localhost:9999/statistics/topBooks'),
          axios.get('http://localhost:9999/statistics/topReaders'),
          axios.get('http://localhost:9999/statistics/lendTrend')
        ]);

        console.log('Books data:', booksRes.data);
        console.log('Readers data:', readersRes.data);

        if (booksRes.data && Array.isArray(booksRes.data)) {
          this.bookData = booksRes.data;
          this.initBookChart(booksRes.data);
        } else {
          console.error('Invalid books data format');
          this.$message.error('图书数据格式错误');
        }

        if (readersRes.data && Array.isArray(readersRes.data)) {
          this.readerData = readersRes.data;
          this.initReaderChart(readersRes.data);
        } else {
          console.error('Invalid readers data format');
          this.$message.error('读者数据格式错误');
        }

        if (trendRes.data && Array.isArray(trendRes.data)) {
          this.trendData = trendRes.data;
          this.initTrendChart(trendRes.data);
        }
      } catch (error) {
        console.error('Fetch statistics error:', error);
        if (error.response) {
          console.error('Error response:', error.response.data);
          this.$message.error('获取统计数据失败: ' + (error.response.data.error || '未知错误'));
        } else {
          this.$message.error('获取统计数据失败: ' + error.message);
        }
      } finally {
        this.loading = false;
      }
    },
    initBookChart(data) {
      if (!data || data.length === 0) {
        this.$message.warning('暂无图书借阅数据');
        return;
      }

      const chartDom = this.$refs.bookChart;
      if (!chartDom) {
        console.error('Book chart DOM not found');
        return;
      }

      try {
        this.bookChart = echarts.init(chartDom);
        
        const option = {
          tooltip: {
            trigger: 'item',
            formatter: '{b}: {c} ({d}%)'
          },
          legend: {
            orient: 'vertical',
            left: 'left',
            top: 'middle'
          },
          series: [
            {
              type: 'pie',
              radius: ['40%', '70%'],
              avoidLabelOverlap: false,
              itemStyle: {
                borderRadius: 10,
                borderColor: '#fff',
                borderWidth: 2
              },
              label: {
                show: false,
                position: 'center'
              },
              emphasis: {
                label: {
                  show: true,
                  fontSize: '20',
                  fontWeight: 'bold'
                }
              },
              labelLine: {
                show: false
              },
              data: data.map(item => ({
                name: item.bookName,
                value: item.count
              }))
            }
          ]
        };

        this.bookChart.setOption(option);
      } catch (error) {
        console.error('Init book chart error:', error);
        this.$message.error('初始化图书图表失败');
      }
    },
    initReaderChart(data) {
      if (!data || data.length === 0) {
        this.$message.warning('暂无读者借阅数据');
        return;
      }

      const chartDom = this.$refs.readerChart;
      if (!chartDom) {
        console.error('Reader chart DOM not found');
        return;
      }

      try {
        this.readerChart = echarts.init(chartDom);
        
        const sortedData = [...data].sort((a, b) => b.count - a.count);
        
        const option = {
          tooltip: {
            trigger: 'axis',
            axisPointer: {
              type: 'shadow'
            }
          },
          grid: {
            left: '3%',
            right: '4%',
            bottom: '3%',
            containLabel: true
          },
          xAxis: {
            type: 'value'
          },
          yAxis: {
            type: 'category',
            data: sortedData.map(item => item.readerName).reverse()
          },
          series: [
            {
              name: '借阅次数',
              type: 'bar',
              data: sortedData.map(item => item.count).reverse()
            }
          ]
        };

        this.readerChart.setOption(option);
      } catch (error) {
        console.error('Init reader chart error:', error);
        this.$message.error('初始化读者图表失败');
      }
    },
    initTrendChart(data) {
      if (!data || data.length === 0) return;

      const chartDom = this.$refs.trendChart;
      if (!chartDom) return;

      try {
        this.trendChart = echarts.init(chartDom);
        
        const option = {
          tooltip: {
            trigger: 'axis',
            axisPointer: {
              type: 'cross',
              label: {
                backgroundColor: '#6a7985'
              }
            }
          },
          grid: {
            left: '3%',
            right: '4%',
            bottom: '3%',
            containLabel: true
          },
          xAxis: {
            type: 'category',
            boundaryGap: false,
            data: data.map(item => item.date)
          },
          yAxis: {
            type: 'value'
          },
          series: [
            {
              name: '借阅数量',
              type: 'line',
              smooth: true,
              lineStyle: {
                width: 3,
                shadowColor: 'rgba(0,0,0,0.3)',
                shadowBlur: 10,
                shadowOffsetY: 8
              },
              areaStyle: {
                color: new echarts.graphic.LinearGradient(0, 0, 0, 1, [
                  {
                    offset: 0,
                    color: 'rgb(128, 255, 165)'
                  },
                  {
                    offset: 1,
                    color: 'rgb(1, 191, 236)'
                  }
                ])
              },
              emphasis: {
                focus: 'series'
              },
              data: data.map(item => item.count)
            }
          ]
        };

        this.trendChart.setOption(option);
      } catch (error) {
        console.error('Init trend chart error:', error);
      }
    },
    handleResize() {
      this.bookChart?.resize();
      this.readerChart?.resize();
      this.trendChart?.resize();
    }
  },
  mounted() {
    this.fetchData();
    window.addEventListener('resize', this.handleResize);
  },
  beforeDestroy() {
    window.removeEventListener('resize', this.handleResize);
    this.bookChart?.dispose();
    this.readerChart?.dispose();
    this.trendChart?.dispose();
  }
}
</script>

<style scoped>
.home-container {
  padding: 20px;
}

.chart-container {
  background: #fff;
  padding: 20px;
  border-radius: 4px;
  box-shadow: 0 2px 12px 0 rgba(0,0,0,0.1);
}

h3 {
  margin-top: 0;
  margin-bottom: 20px;
  text-align: center;
  color: #333;
}

.chart-wrapper {
  position: relative;
  min-height: 400px;
}

.no-data {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  color: #909399;
  font-size: 14px;
}
</style>
