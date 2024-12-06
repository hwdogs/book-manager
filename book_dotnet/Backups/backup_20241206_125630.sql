-- MySQL dump 10.13  Distrib 8.0.37, for Win64 (x86_64)
--
-- Host: localhost    Database: book_manager
-- ------------------------------------------------------
-- Server version	8.0.37

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `book`
--

DROP TABLE IF EXISTS `book`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `book` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(30) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `author` varchar(30) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  `publish` varchar(30) CHARACTER SET utf8mb3 COLLATE utf8mb3_general_ci DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=135 DEFAULT CHARSET=utf8mb3 ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `book`
--

LOCK TABLES `book` WRITE;
/*!40000 ALTER TABLE `book` DISABLE KEYS */;
INSERT INTO `book` VALUES (1,'解忧杂货店','东野圭吾','电子工业出版社'),(2,'追风筝的人','卡勒德·胡赛尼','中信出版社'),(3,'人间失格','太宰治','作家出版社'),(4,'这就是二十四节气','高春香','电子工业出版社'),(5,'白夜行','东野圭吾','南海出版公司'),(6,'摆渡人','克莱儿·麦克福尔','百花洲文艺出版社'),(7,'暖暖心绘本','米拦弗特毕','湖南少儿出版社'),(8,'天才在左疯子在右','高铭','北京联合出版公司'),(9,'我们仨','杨绛','生活.读书.新知三联书店'),(10,'活着','余华','作家出版社'),(119,'时间的秩序',' [意] 卡洛·罗韦利','湖南科学技术出版社'),(120,'偷影子的人',' [法] 马克·李维','湖南文艺出版社'),(121,'在路上','(美) 杰克·凯鲁亚克','人民文学出版社'),(122,'呼吸',' [美] 特德·姜','译林出版社'),(123,'书店日记','[英] 肖恩·白塞尔','广西师范大学出版社'),(124,'mybook','my','mypublish'),(127,'红楼梦','[清] 曹雪芹','人民文学出版社'),(128,'1984','[英] 乔治·奥威尔 / 刘绍铭','北京十月文艺出版社'),(129,'哈利·波特','J.K.罗琳 (J.K.Rowling) / 苏农','人民文学出版社'),(130,'三体全集 : 地球往事三部曲','刘慈欣 ','重庆出版社'),(131,'百年孤独','[哥伦比亚] 加西亚·马尔克斯 / 范晔 ','南海出版公司'),(132,'飘','[美国] 玛格丽特·米切尔 / 李美华','译林出版社'),(133,'三国演义（全二册）','[明] 罗贯中 ','人民文学出版社'),(134,'动物农场','[英] 乔治·奥威尔 / 荣如德 ','上海译文出版社');
/*!40000 ALTER TABLE `book` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `lend_return`
--

DROP TABLE IF EXISTS `lend_return`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `lend_return` (
  `id` int NOT NULL AUTO_INCREMENT,
  `reader_id` int NOT NULL,
  `book_id` int NOT NULL,
  `lend_data` datetime NOT NULL,
  `return_data` datetime DEFAULT NULL,
  PRIMARY KEY (`id`),
  KEY `fk_reader_id` (`reader_id`),
  KEY `fk_book_id` (`book_id`),
  CONSTRAINT `fk_book_id` FOREIGN KEY (`book_id`) REFERENCES `book` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_reader_id` FOREIGN KEY (`reader_id`) REFERENCES `reader` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=15 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `lend_return`
--

LOCK TABLES `lend_return` WRITE;
/*!40000 ALTER TABLE `lend_return` DISABLE KEYS */;
INSERT INTO `lend_return` VALUES (1,1,1,'2024-12-04 19:55:43','2024-12-04 21:37:20'),(2,31,123,'2024-12-04 19:56:06','2024-12-04 23:15:16'),(4,1,10,'2024-12-03 19:56:38',NULL),(5,1,2,'2024-12-04 21:45:04','2024-12-04 21:45:13'),(6,10,119,'2024-12-04 21:54:56','2024-12-04 23:08:06'),(7,20,8,'2024-10-01 22:18:16','2024-12-04 23:06:11'),(8,20,123,'2024-09-25 22:19:32','2024-12-04 23:08:07'),(9,20,124,'2024-09-02 23:08:37','2024-12-04 23:10:30'),(10,60,127,'2024-12-04 23:13:47','2024-12-04 23:14:17'),(11,60,127,'2024-12-04 23:16:37','2024-12-04 23:16:42'),(13,60,128,'2024-10-01 22:04:40',NULL),(14,60,131,'2024-10-02 22:05:38',NULL);
/*!40000 ALTER TABLE `lend_return` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `manager`
--

DROP TABLE IF EXISTS `manager`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `manager` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(32) NOT NULL,
  `password` varchar(32) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `manager`
--

LOCK TABLES `manager` WRITE;
/*!40000 ALTER TABLE `manager` DISABLE KEYS */;
INSERT INTO `manager` VALUES (1,'hwsh','111111'),(2,'hwsh1','222222');
/*!40000 ALTER TABLE `manager` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `reader`
--

DROP TABLE IF EXISTS `reader`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `reader` (
  `id` int NOT NULL AUTO_INCREMENT,
  `name` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `sex` tinyint NOT NULL DEFAULT '1',
  `w_address` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `h_address` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `email` varchar(64) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `create_time` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `notes` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE=InnoDB AUTO_INCREMENT=61 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci ROW_FORMAT=DYNAMIC;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `reader`
--

LOCK TABLES `reader` WRITE;
/*!40000 ALTER TABLE `reader` DISABLE KEYS */;
INSERT INTO `reader` VALUES (1,'陈一1',1,'北京市东城区东华门街道','北京市朝阳区酒仙桥街道','chenyi@example.com','2024-12-03 17:58:42','喜欢看历史类书籍'),(2,'李二',0,'上海市黄浦区南京东路街道','上海市徐汇区漕河泾街道','lier@example.com','2024-12-03 17:58:42','热爱科技类书籍'),(3,'张三',1,'广州市越秀区北京路街道','广州市天河区珠江新城街道','zhangsan@example.com','2024-12-03 17:58:42','喜欢音乐类书籍'),(4,'李四',1,'深圳市福田区华强北街道','深圳市南山区高新南街道','lisi@example.com','2024-12-03 17:58:42','热爱旅游类书籍'),(5,'王五',0,'武汉市江汉区建设大道街道','武汉市洪山区关山街道','wangwu@example.com','2024-12-03 17:58:42','喜欢电影类书籍'),(6,'赵六',1,'重庆市渝中区解放碑街道','重庆市九龙坡区华岩镇','zhaoliu@example.com','2024-12-03 17:58:42','热爱艺术类书籍'),(7,'孙七',1,'成都市锦江区春熙路街道','成都市武侯区高升桥街道','sunqi@example.com','2024-12-03 17:58:42','喜欢动漫类书籍'),(8,'周八',0,'南京市玄武区中山门街道','南京市鼓楼区华侨路街道','zhouba@example.com','2024-12-03 17:58:42','热爱历史类书籍'),(9,'吴九',1,'杭州市上城区湖滨街道','杭州市滨江区长河街道','wuwu@example.com','2024-12-03 17:58:42','喜欢外语类书籍'),(10,'郑十',1,'苏州市姑苏区虎丘街道','苏州市相城区元和街道','zhengshi@example.com','2024-12-03 17:58:42','热爱心理学类书籍'),(11,'冯十一',0,'济南市历下区趵突泉街道','济南市历城区华龙街道','fengfeng@example.com','2024-12-03 17:58:42','喜欢文学类书籍'),(12,'陈十二',1,'青岛市市南区台东街道','青岛市市北区浮山所街道','chenchen@example.com','2024-12-03 17:58:42','热爱经济类书籍'),(13,'卫十三',1,'天津市和平区劝业场街道','天津市河西区梅江街道','weishi@example.com','2024-12-03 17:58:42','喜欢哲学类书籍'),(14,'蒋十四',0,'西安市新城区长乐西路街道','西安市雁塔区小寨路街道','jiangjiang@example.com','2024-12-03 17:58:42','热爱教育类书籍'),(15,'沈十五',1,'长沙市芙蓉区五一广场街道','长沙市岳麓区岳麓山街道','shenshen@example.com','2024-12-03 17:58:42','喜欢自然科学类书籍'),(16,'韩十六',1,'南昌市东湖区东湖街道','南昌市西湖区八一桥街道','hanhan@example.com','2024-12-03 17:58:42','热爱计算机类书籍'),(17,'杨十七',0,'郑州市管城回族区紫荆山路街道','郑州市中原区二七街道','yangyang@example.com','2024-12-03 17:58:42','喜欢体育类书籍'),(18,'朱十八',1,'哈尔滨市道里区松花江街道','哈尔滨市南岗区革新街道','zhuzhu@example.com','2024-12-03 17:58:42','热爱军事类书籍'),(19,'秦十九',1,'石家庄市裕华区世纪公园街道','石家庄市长安区胜利北街道','qinqin@example.com','2024-12-03 17:58:42','喜欢政治类书籍'),(20,'尤二十',0,'太原市杏花岭区迎泽西街道','太原市小店区南内环街道','youyou@example.com','2024-12-03 17:58:42','热爱法律类书籍'),(21,'许二十一',1,'合肥市蜀山区望江西路街道','合肥市包河区金寨路街道','xuxu@example.com','2024-12-03 17:58:42','喜欢艺术类书籍'),(22,'何二十二',1,'济南市历下区趵突泉南路街道','济南市天桥区中心街道','hehe@example.com','2024-12-03 17:58:42','热爱旅游类书籍'),(23,'吕二十三',0,'青岛市市南区海泊桥街道','青岛市市北区浮山路街道','lvlu@example.com','2024-12-03 17:58:42','喜欢哲学类书籍'),(24,'施二十四',1,'天津市和平区南市街道','天津市河东区大王庄街道','shishi@example.com','2024-12-03 17:58:42','热爱心理学类书籍'),(25,'张二十五',1,'西安市雁塔区小寨路街道','西安市未央区辛家庙街道','zhangzhang@example.com','2024-12-03 17:58:42','喜欢电影类书籍'),(26,'孔二十六',0,'长沙市岳麓区望月湖街道','长沙市开福区马王堆街道','kongkong@example.com','2024-12-03 17:58:42','热爱历史类书籍'),(27,'曹二十七',1,'南昌市西湖区孺子路街道','南昌市青山湖区西湖街道','caocao@example.com','2024-12-03 17:58:42','喜欢文学类书籍'),(28,'严二十八',1,'郑州市金水区经三路街道','郑州市二七区嵩山路街道','yanyan@example.com','2024-12-03 17:58:42','热爱哲学类书籍'),(29,'华二十九',0,'哈尔滨市南岗区南岗街道','哈尔滨市道里区松北街道','huahua@example.com','2024-12-03 17:58:42','喜欢外语类书籍'),(30,'胡三十',1,'石家庄市长安区裕华西路街道','石家庄市桥西区桥西街道','huhu@example.com','2024-12-03 17:58:42','热爱科幻类书籍'),(31,'王小明',1,'湖南省长沙市雨花区芙蓉中路','四川省成都市武侯区高升桥街道','wangxiaoming@example.com','2024-12-03 17:58:42','研究教育理论的学者'),(32,'李小红',0,'四川省成都市锦江区春熙路','湖南省长沙市天心区解放西路','lixiaohong@example.com','2024-12-03 17:58:42','关注教育改革的教师'),(33,'张小刚',1,'湖南省湘潭市雨湖区潇湘南路','四川省成都市青羊区人民中路','zhangxiaogang@example.com','2024-12-03 17:58:42','教育行业从业者'),(34,'赵小伟',1,'四川省绵阳市涪城区红星路','湖南省株洲市天元区芦淞北路','zhaoxiaowei@example.com','2024-12-03 17:58:42','关注学生心理健康的辅导员'),(35,'钱小芳',0,'湖南省衡阳市石鼓区城南路','四川省成都市武侯区天府大道','qianxiaofang@example.com','2024-12-03 17:58:42','学前教育工作者'),(36,'孙小强',1,'四川省成都市金牛区西安中路','湖南省湘潭市雨湖区雨湖路','sunxiaoqiang@example.com','2024-12-03 17:58:42','研究教育心理学的专家'),(37,'周小艾',1,'湖南省株洲市荷塘区东塘路','四川省德阳市旌阳区凤鸣南路','zhouxiaoa@example.com','2024-12-03 17:58:42','中小学教师'),(38,'吴小娜',0,'四川省成都市锦江区大慈寺路','湖南省衡阳市蒸湘区中心路','wuxiaona@example.com','2024-12-03 17:58:42','关注教育行政的工作人员'),(39,'朱小明',1,'湖南省长沙市芙蓉区芙蓉中路','四川省成都市青羊区青羊大道','zhuxiaoming@example.com','2024-12-03 17:58:42','关注教育行政的工作人员'),(40,'陈小红',0,'四川省成都市武侯区桂溪街道','湖南省株洲市天元区栗树街道','chenxiaohong@example.com','2024-12-03 17:58:42','热爱教育类书籍'),(41,'林小刚',1,'湖南省湘潭市雨湖区潇湘南路','四川省成都市青羊区人民中路','linxiaogang@example.com','2024-12-03 17:58:42','喜欢体育类书籍'),(42,'黄小伟',1,'四川省绵阳市涪城区红星路','湖南省株洲市天元区芦淞北路','huangxiaowei@example.com','2024-12-03 17:58:42','喜欢科幻类书籍'),(43,'徐小芳',0,'湖南省衡阳市石鼓区城南路','四川省成都市武侯区天府大道','xuxiaofang@example.com','2024-12-03 17:58:42','喜欢文学类书籍'),(44,'胡小强',1,'四川省成都市金牛区西安中路','湖南省湘潭市雨湖区雨湖路','huxiaoqiang@example.com','2024-12-03 17:58:42','喜欢历史类书籍'),(45,'朱小艾',1,'湖南省株洲市荷塘区东塘路','四川省德阳市旌阳区凤鸣南路','zhuxiaoa@example.com','2024-12-03 17:58:42','喜欢哲学类书籍'),(46,'刘小娜',0,'四川省成都市锦江区大慈寺路','湖南省衡阳市蒸湘区中心路','liuxiaona@example.com','2024-12-03 17:58:42','喜欢艺术类书籍'),(47,'丁小明',1,'湖南省长沙市芙蓉区芙蓉中路','四川省成都市青羊区青羊大道','dingxiaoming@example.com','2024-12-03 17:58:42','喜欢心理学类书籍'),(48,'许小红',0,'四川省成都市武侯区桂溪街道','湖南省株洲市天元区栗树街道','xuxiaohong@example.com','2024-12-03 17:58:42','喜欢经济类书籍'),(49,'曹小刚',1,'湖南省湘潭市雨湖区潇湘南路','四川省成都市青羊区人民中路','caoxiaogang@example.com','2024-12-03 17:58:42','喜欢政治类书籍'),(50,'程小伟',1,'四川省绵阳市涪城区红星路','湖南省株洲市天元区芦淞北路','chengxiaowei@example.com','2024-12-03 17:58:42','喜欢法律类书籍'),(51,'苏小芳',0,'湖南省衡阳市石鼓区城南路','四川省成都市武侯区天府大道','suxiaofang@example.com','2024-12-03 17:58:42','喜欢教育类书籍'),(52,'杨小强',1,'四川省成都市金牛区西安中路','湖南省湘潭市雨湖区雨湖路','yangxiaoqiang@example.com','2024-12-03 17:58:42','喜欢心理学类书籍'),(53,'秦小艾',1,'湖南省株洲市荷塘区东塘路','四川省德阳市旌阳区凤鸣南路','qinxiaoai@example.com','2024-12-03 17:58:42','喜欢经济类书籍'),(54,'许小娜',0,'四川省成都市锦江区大慈寺路','湖南省衡阳市蒸湘区中心路','xuxiaona@example.com','2024-12-03 17:58:42','喜欢政治类书籍'),(55,'黄小明',1,'湖南省长沙市芙蓉区芙蓉中路','四川省成都市青羊区青羊大道','huangxiaoming@example.com','2024-12-03 17:58:42','喜欢科幻类书籍'),(56,'徐小红',0,'四川省成都市武侯区桂溪街道','湖南省株洲市天元区栗树街道','xuxiaohong@example.com','2024-12-03 17:58:42','喜欢文学类书籍'),(57,'胡小刚',1,'湖南省湘潭市雨湖区潇湘南路','四川省成都市青羊区人民中路','huxiaogang@example.com','2024-12-03 17:58:42','喜欢历史类书籍'),(58,'朱小伟',1,'四川省绵阳市涪城区红星路','湖南省株洲市天元区芦淞北路','zhuxiaowei@example.com','2024-12-03 17:58:42','喜欢哲学类书籍'),(59,'陈一2',1,'北京市东城区东华门街道2','北京市朝阳区酒仙桥街道2','chenyi2@example.com','2024-12-04 21:01:56',''),(60,'黄万首',1,'guet','北京市朝阳区酒仙桥街道2','hwsh@example.com','2024-12-04 23:12:30','你好');
/*!40000 ALTER TABLE `reader` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2024-12-06 12:56:31
