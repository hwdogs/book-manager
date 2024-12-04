/*
 Navicat Premium Dump SQL

 Source Server         : local
 Source Server Type    : MySQL
 Source Server Version : 80037 (8.0.37)
 Source Host           : localhost:3306
 Source Schema         : book_manager

 Target Server Type    : MySQL
 Target Server Version : 80037 (8.0.37)
 File Encoding         : 65001

 Date: 04/12/2024 19:58:00
*/

SET NAMES utf8mb4;
SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------
-- Table structure for lend_return
-- ----------------------------
DROP TABLE IF EXISTS `lend_return`;
CREATE TABLE `lend_return`  (
  `id` int NOT NULL AUTO_INCREMENT,
  `reader_id` int NOT NULL,
  `book_id` int NOT NULL,
  `lend_data` datetime NOT NULL,
  `return_data` datetime NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE,
  INDEX `fk_reader_id`(`reader_id` ASC) USING BTREE,
  INDEX `fk_book_id`(`book_id` ASC) USING BTREE,
  CONSTRAINT `fk_book_id` FOREIGN KEY (`book_id`) REFERENCES `book` (`id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_reader_id` FOREIGN KEY (`reader_id`) REFERENCES `reader` (`id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE = InnoDB AUTO_INCREMENT = 5 CHARACTER SET = utf8mb4 COLLATE = utf8mb4_0900_ai_ci ROW_FORMAT = Dynamic;

-- ----------------------------
-- Records of lend_return
-- ----------------------------
INSERT INTO `lend_return` VALUES (1, 1, 1, '2024-12-04 19:55:43', NULL);
INSERT INTO `lend_return` VALUES (2, 31, 123, '2024-12-04 19:56:06', NULL);
INSERT INTO `lend_return` VALUES (4, 1, 10, '2024-12-03 19:56:38', NULL);

SET FOREIGN_KEY_CHECKS = 1;
