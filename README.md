
# 图书管理系统

## 技术栈：

.Net Core WebAPI，
Vue CLI3，
element UI

## 代码

book_dotnet：.Net Core WebAPI相关代码。

book_vue：Vue相关代码都在这里，需要你执行yarn install
这个命令是安装所需要的依赖。

## 安装

book_dotnet:需要一个叫做book_manager的数据库，sql我已经上传在根目录，
自行导入，修改appsettings.json中的数据库连接配置。
```
dotnet restore
```
```
dotnet build
```
```
dotnet run
```

运行，访问localhost:9999

book_vue:
```
yarn install
```
```
yarn serve
```
运行，访问localhost:8080