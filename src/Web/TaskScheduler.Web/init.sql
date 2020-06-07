-- Table
CREATE TABLE `task_log`  (
  `id` varchar(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `task_id` varchar(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `response` varchar(500) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `execute_time` datetime(0) NULL DEFAULT NULL,
  `apm` bigint(18) NULL DEFAULT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

CREATE TABLE `task`  (
  `id` varchar(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `group_name` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `interval` varchar(20) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `api_url` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `auth_key` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `auth_value` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `describe` varchar(200) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `request_method` varchar(5) CHARACTER SET utf8 COLLATE utf8_general_ci NULL DEFAULT NULL,
  `last_time` datetime(0) NULL DEFAULT NULL,
  `create_time` datetime(0) NULL,
  `status` int(11) NOT NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;

CREATE TABLE `env`  (
  `id` varchar(36) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `key` varchar(50) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `val` varchar(255) CHARACTER SET utf8 COLLATE utf8_general_ci NOT NULL,
  `create_time` datetime(0) NULL,
  PRIMARY KEY (`id`) USING BTREE
) ENGINE = InnoDB CHARACTER SET = utf8 COLLATE = utf8_general_ci ROW_FORMAT = Dynamic;



INSERT INTO env VALUES ('cb9c56073a544592a8580585ef89e761', 'HDI', 'http://10.9.58.17:8002', '2020-04-17 13:37:55.1500948');
INSERT INTO env VALUES ('78cf8683172c428bb0b9bb1dddda46cc', 'POM', 'https://pom.cccc-cdc.com', '2020-04-28 10:14:51.4411758');
INSERT INTO env VALUES ('9afce57f878b41b7b660e4e6647c2e91', 'PM', 'https://pm.cccc-sdc.com', '2020-05-29 14:39:20.329623');
-- ----------------------------
-- Records of "task"
-- ----------------------------
INSERT INTO task VALUES ('b8e766f60f944d1b994cae4ef4addf1f', '同步项目', '安全隐患排查系统', '0 30 3  * * ? ', '{HDI}/api/task/project-sync', 'token', 'df974270-dbba-4f87-8121-427636dab396', '每天凌晨3点30执行', 'post', '2020-05-31 03:30:13.1014184', '2020-04-09 18:51:05.6571804', 0);
INSERT INTO task VALUES ('71d80c4d4dee490f9fa797628040635e', '同步船舶', '安全隐患排查系统', '0 35 3  * * ? ', '{HDI}/api/task/ship-sync', 'token', 'df974270-dbba-4f87-8121-427636dab396', '每天凌晨3点35执行', 'post', '2020-05-31 03:35:02.0005854', '2020-04-09 18:52:38.5715735', 0);
INSERT INTO task VALUES ('74b3f363ed964699bcc88f5c7c7f4d79', '项目初始人员', '安全隐患排查系统', '0 45 3 ? * MON', '{HDI}/api/task/project-leader-week', 'token', 'df974270-dbba-4f87-8121-427636dab396', '每周一凌晨3点45分初始化项目的领导班子初始人员', 'post', '2020-05-25 03:45:00.4358511', '2020-04-09 19:05:17.7456662', 0);
INSERT INTO task VALUES ('87cce6f5f51e4aa596c76d26bccca01a', '项目开展率', '安全隐患排查系统', '0 50 3 ? * MON', '{HDI}/api/task/project-start-week', 'token', 'df974270-dbba-4f87-8121-427636dab396', '每周一凌晨3点50计算上周项目的开展率', 'post', '2020-05-25 03:50:00.4872082', '2020-04-09 19:07:37.6422826', 0);
INSERT INTO task VALUES ('de83c707b7dc4b2a969ce80ffaf353b1', '项目计分', '安全隐患排查系统', '0 0 4 ? * MON', '{HDI}/api/task/project-score-week', 'token', 'df974270-dbba-4f87-8121-427636dab396', '每周一凌晨4点计算上周的项目扣分', 'post', '2020-05-25 04:00:00.3913678', '2020-04-09 19:10:26.0796103', 0);
INSERT INTO task VALUES ('f27590791b464b6ea222e7c0ae541a5d', '单位计分', '安全隐患排查系统', '0 10 4 ? * MON', '{HDI}/api/task/unit-score-week', 'token', 'df974270-dbba-4f87-8121-427636dab396', '每周一凌晨4点10计算上周的单位扣分', 'post', '2020-05-25 04:10:02.0382142', '2020-04-09 19:12:02.9515634', 0);
INSERT INTO task VALUES ('f8b260aa6f3c4145b015b31accb6a47c', '测试HDI连通性', '安全隐患排查系统', '0 0 0/1 * * ? ', '{HDI}/api/task/test', 'token', 'df974270-dbba-4f87-8121-427636dab396', '测试连接HDI连通性', 'post', '2020-05-31 10:00:01.1046166', '2020-04-09 19:29:03.4870085', 0);
INSERT INTO task VALUES ('64eafbdef5114b28887e34f456574fe2', '项目状态同步', '项目运行监控中心', '0 57 1 * * ?', '{POM}/api/Sync/project-state-sync', 'token', 'df974270-dbba-4f87-8121-427636dab396', '项目状态每天凌晨1点57分进行同步', 'post', '2020-05-31 01:57:00.6970555', '2020-04-13 16:16:17.1244249', 0);
INSERT INTO task VALUES ('590162fd658747cd833647b6e8f196de', '项目水运工程项目施工工况数据同步', '项目运行监控中心', '0 25 3 * * ?', '{POM}/api/Sync/project-condition-grade-sync', 'token', 'df974270-dbba-4f87-8121-427636dab396', '项目水运工程项目施工工况数据每天凌晨3点25分同步', 'post', '2020-05-31 03:25:00.7666636', '2020-04-13 16:28:32.8984837', 0);
INSERT INTO task VALUES ('ceece7f41b674ae5a0df21e2ea7fbc43', '自有船舶数据同步', '项目运行监控中心', '0 15 3 * * ?', '{POM}/api/Sync/owned-ship-sync', 'token', 'df974270-dbba-4f87-8121-427636dab396', '自有船舶数据每天凌晨3点15同步', 'post', '2020-05-31 03:15:00.8975672', '2020-04-13 16:30:25.927188', 0);
INSERT INTO task VALUES ('46588e59ebef44dbb72071ddf41f1e2e', '公司数据同步', '项目运行监控中心', '0 13 3 * * ?', '{POM}/api/Sync/company-sync', 'token', 'df974270-dbba-4f87-8121-427636dab396', '公司数据每天凌晨3点13分同步', 'post', '2020-05-31 03:13:00.7411711', '2020-04-13 16:31:11.9374974', 0);
INSERT INTO task VALUES ('1b6989b27a814e4190b1a446dc2a8816', '船级社数据同步', '项目运行监控中心', '0 10 3 * * ?', '{POM}/api/Sync/ship-classic-sync', 'token', 'df974270-dbba-4f87-8121-427636dab396', '船级社数据每天凌晨3点10分同步', 'post', '2020-05-31 03:10:00.6140868', '2020-04-13 16:32:01.0167861', 0);
INSERT INTO task VALUES ('fc8c69a0b09547eeb03972a0c55a013c', '船舶状态数据同步', '项目运行监控中心', '0 11 2 * * ?', '{POM}/api/Sync/ship-status-sync', 'token', 'df974270-dbba-4f87-8121-427636dab396', '船舶状态数据每天凌晨2点11分同步', 'post', '2020-05-31 02:11:00.6537844', '2020-04-13 16:34:08.5150088', 0);
INSERT INTO task VALUES ('49dfbbec448a4eb09e26ecbbe8b2ce1c', '合同状态数据同步', '项目运行监控中心', '0 10 2 * * ?', '{POM}/api/Sync/contract-status-sync', 'token', 'df974270-dbba-4f87-8121-427636dab396', '合同状态数据每天凌晨2点10分同步', 'post', '2020-05-31 02:10:00.5750876', '2020-04-13 16:38:00.9620016', 0);
INSERT INTO task VALUES ('baa8e997cb3f460782f52dc5bcbd7843', '合同类别数据同步', '项目运行监控中心', '0 15 2 * * ?', '{POM}/api/Sync/contract-category-sync', 'token', 'df974270-dbba-4f87-8121-427636dab396', '合同类别数据每天凌晨2点15同步', 'post', '2020-05-31 02:15:00.6953844', '2020-04-13 16:39:03.589487', 0);
INSERT INTO task VALUES ('64070a45d34e4585b0377d0250b511e2', '人员数据同步', '项目运行监控中心', '0 25 2 * * ?', '{POM}/api/Sync/user-sync', 'token', 'df974270-dbba-4f87-8121-427636dab396', '人员数据每天凌晨2点25同步', 'post', '2020-05-31 02:25:23.9727663', '2020-04-13 16:40:09.0156323', 0);
INSERT INTO task VALUES ('fbf07a2d8dcd4579bff520ee4add1af4', '用户组织关系同步; 项目部数据同步', '项目运行监控中心', '0 35 2 * * ?', '{POM}/api/Sync/organization-sync', 'token', 'df974270-dbba-4f87-8121-427636dab396', '用户组织关系,项目部数据每天凌晨2点35进行同步', 'post', '2020-05-31 02:35:06.8703042', '2020-04-13 16:41:22.2734504', 0);
INSERT INTO task VALUES ('4f0462069b40473ab3778ba5725c8ee4', '项目类型同步', '项目运行监控中心', '0 40 2 * * ?', '{POM}/api/Sync/project-type-sync', 'token', 'df974270-dbba-4f87-8121-427636dab396', '项目类型每天凌晨2点40同步', 'post', '2020-05-31 02:40:00.7370781', '2020-04-13 16:42:34.8138359', 0);
INSERT INTO task VALUES ('d1748fae6e9c45a9b5d48d9c2756d7ee', '往来单位数据同步', '项目运行监控中心', '0 46 2 * * ?', '{POM}/api/Sync/business-related-unit-sync', 'token', 'df974270-dbba-4f87-8121-427636dab396', '往来单位数据每天凌晨2点45同步', 'post', '2020-05-31 02:46:18.0435012', '2020-04-14 10:54:20.3385658', 0);
INSERT INTO task VALUES ('d4412511acb8418f982c8453f8d7dade', '船舶类型数据同步', '项目运行监控中心', '0 43 2 * * ?', '{POM}/api/Sync/ship-type-sync', 'token', 'df974270-dbba-4f87-8121-427636dab396', '船舶类型数据每天凌晨2点43同步', 'post', '2020-05-31 02:43:00.6162136', '2020-04-21 17:10:11.4024132', 0);
INSERT INTO task VALUES ('035b755e10554dfca0c23c8594322c47', '组织机构与项目部映射同步', '项目运行监控中心', '0 5 2 * * ?', '{POM}/api/Sync/project-dep-relation-sync', 'token', 'df974270-dbba-4f87-8121-427636dab396', '组织机构与项目部映射凌晨2点5分同步', 'post', '2020-05-31 02:05:06.4872627', '2020-04-21 17:12:27.9899845', 0);
INSERT INTO task VALUES ('cdde6a0bf4ab48a782fdd495c95fcfeb', '国内项目规模数据同步', '项目运行监控中心', '0 47 2 * * ?', '{POM}/api/Sync/data-item-sync', 'token', 'df974270-dbba-4f87-8121-427636dab396', '国内项目规模数据2点47同步', 'post', '2020-05-31 02:47:02.1409504', '2020-04-21 17:13:31.1292922', 0);
INSERT INTO task VALUES ('6b1940d5b6914475afa8db70f180287d', '一级区域数据同步', '项目运行监控中心', '0 5 3 * * ?', '{POM}/api/Sync/project-region-sync', 'token', 'df974270-dbba-4f87-8121-427636dab396', '一级区域数据3点5分同步', 'post', '2020-05-31 03:05:00.1796101', '2020-04-21 17:14:36.0097589', 0);
INSERT INTO task VALUES ('89c143feaa9b44c0a9f7c55578307f48', '二级区域数据同步', '项目运行监控中心', '0 45 3 * * ?', '{POM}/api/Sync/project-area-sync', 'token', 'df974270-dbba-4f87-8121-427636dab396', '二级区域数据3点45分同步', 'post', '2020-05-31 03:45:00.4345826', '2020-04-21 17:15:29.872564', 0);
INSERT INTO task VALUES ('43866bf280f647bc8a3ece31c30853ed', '项目等级数据同步', '项目运行监控中心', '0 55 3 * * ?', '{POM}/api/Sync/project-grade-sync', 'token', 'df974270-dbba-4f87-8121-427636dab396', '项目等级数据3点55分同步', 'post', '2020-05-31 03:55:00.3766625', '2020-04-21 17:16:28.3329579', 0);
INSERT INTO task VALUES ('478d08ee69d8495c8982f0547bf6b89b', '项目数据同步(首页数据显示)', '项目运行监控中心', '0 47 3 * * ?', '{POM}/api/Sync/umc-project-sync', 'token', 'df974270-dbba-4f87-8121-427636dab396', '项目数据3点47同步(首页数据显示)', 'post', '2020-05-31 03:47:00.7409328', '2020-04-21 17:17:31.9900116', 0);
INSERT INTO task VALUES ('364f6cef045746aab6cf3b3a34438fdc', '合同币种数据同步', '项目运行监控中心', '0 58 3 * * ?', '{POM}/api/Sync/currency-sync', 'token', 'df974270-dbba-4f87-8121-427636dab396', '合同币种数据3点58分同步', 'post', '2020-05-31 03:58:00.3327518', '2020-04-21 17:18:24.0456448', 0);
INSERT INTO task VALUES ('1764313bf2c948a68db0c255f55d61fb', '合同签订方式数据同步', '项目运行监控中心', '0 59 3 * * ?', '{POM}/api/Sync/contract-sign-type-sync', 'token', 'df974270-dbba-4f87-8121-427636dab396', '合同签订方式数据3点59同步', 'post', '2020-05-31 03:59:00.3044552', '2020-04-21 17:52:34.2109642', 0);
INSERT INTO task VALUES ('cb6d721ff7a24ea386cdefc57cb09d7e', '每天17:30定时给项目经理进行发送消息', '项目运行监控中心', '0 30 17 * * ?', '{POM}/api/Sync/send-project-agency-message-sync', 'token', 'df974270-dbba-4f87-8121-427636dab396', '每天17:30定时给项目经理进行发送消息
', 'post', '2020-04-21 06:01:09', '2020-04-21 17:59:46.1724177', 1);
INSERT INTO task VALUES ('9c73d29e6774483eac3aae583410717d', '项目月计分', '安全隐患排查系统', '0 15 4 ? * MON', '{HDI}/api/task/project-score-month', 'token', 'df974270-dbba-4f87-8121-427636dab396', '每周一凌晨4点15执行', 'post', '2020-05-25 04:15:00.0519029', '2020-04-26 21:12:28.7938235', 0);
INSERT INTO task VALUES ('1d2c8027f00943ad96f4be83e395a627', '同步公司', '安全隐患排查系统', '0 55 3 * * ?', '{HDI}/api/task/company-sync', 'token', 'df974270-dbba-4f87-8121-427636dab396', '每天凌晨3点55执行', 'post', '2020-05-31 03:55:00.0977375', '2020-05-11 15:05:55.9413397', 0);
INSERT INTO task VALUES ('c8d68ebb4b704d2789acaa3bee118a86', '人员出勤', '上航局综合业务管理系统', '0 0 2 * * ? ', '{PM}/CommonApi/HolidaySchedule', NULL, NULL, '每天凌晨2点定时执行人员出勤服务
', 'post', '2020-05-31 02:00:34.6262351', '2020-05-29 14:29:20.0230644', 0);
INSERT INTO task VALUES ('7da00c007c37446da2cdc2ccde20c59b', '项目天气接口', '上航局综合业务管理系统', '0 0 4 * * ?', '{PM}/CommonApi/InitWeatherInfo', NULL, NULL, '项目天气接口每天凌晨4点执行', 'post', NULL, '2020-05-29 16:08:46.8507544', 1);
