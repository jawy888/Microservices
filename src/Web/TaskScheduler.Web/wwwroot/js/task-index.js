var $taskVue = new Vue({
    el: "#task-container",
    data: {
        paginations: {
            page_index: 1, //当前页
            total: 0, //总数
            page_size: 10, //一页显示多少
            page_sizes: [10, 15, 20], //每页显示多少条
            layout: 'total, sizes, prev, pager, next, jumper'
        },
        allTableData: [],
        options: [
        ],
        formInline: {
            name: '',
            group_name: ''
        },
        //环境变量数据
        master_user: {
            sel: null,//选中行   
            columns: [
                { field: "key", title: "变量名", width: 230 },
                { field: "val", title: "变量值", width: 230 }
            ],
            data: [],
        },
        dialogTableVisible: false,

        select: {
            currentRow: [],
            rows: {}
        },
        selectCom: {
            model: 'post',
            data: [{ value: 'post', label: 'post' }, { value: 'get', label: 'get' }]
        },
        log: {
            index: 0,
            model: false,
            title: '日志记录',
            group: '',
            closable: true,
            footerHide: true,
            spin: false,
            page: 0,
            data: []
        },
        isAdd: true,
        closable: false,
        footerHide: true,
        model: false,
        modelMessage: '任务管理',
        activedIndex: 0,
        taskValidate: {
            name: '', group_name: '', interval: '', api_url: '', auth_key: '', auth_value:
                '', describe: '', requestType: ''
        },
        ruleValidate: {
            name: [{ required: true, message: '作业名称必填', trigger: 'blur' }],
            group_name: [{ required: true, message: '分组名称必填', trigger: 'blur' }],
            interval: [{ required: true, message: '间隔(Cron)必填', trigger: 'blur' }],
            request_method: [{ required: true, message: '请选择请求方式', trigger: 'change' }],
            api_url: [{ required: true, message: 'ApiUrl必填', trigger: 'blur' }]
        },
        taskForm: [
            { name: 'name', text: '作业名称', value: '', placeholder: '作业名称与触发器名称默认相同', readOnly: false },
            { name: 'group_name', text: '分组', value: '', placeholder: '分组名称与分组名称默认相同', readOnly: false },
            { name: 'interval', text: '间隔(Cron)', value: '', placeholder: '如10分钟执行一次：0/0 0/10 * * * ? ' },
            { name: 'api_url', text: 'ApiUrl', value: '', placeholder: "远程调用接口URL" },
            { name: 'auth_key', text: 'header(key)', value: '', placeholder: '请求header验证的Key' },
            { name: 'auth_value', text: 'header(value)', value: '', placeholder: '请求header验证的Key' },
            {
                name: 'request_method', text: '请求方式', value: '', onChange: (data, value) => {
                }, placeholder: 'post/get', type: 'select'
            },
            { name: 'describe', text: '描述', value: '', type: 'textarea' }
        ],
        columns: [
            {
                hidden: true,
                title: 'id',
                key: 'id',
                width: 120
            },
            {
                type: 'selection',
                width: 60,
                align: 'center'
            },
            {
                title: '作业名称',
                key: 'name',
                width: 150
            }, {
                title: '分组',
                key: 'group_name',
                width: 120
            },
            {
                title: '最后执行时间',
                key: 'last_time',
                width: 170
            }, {
                title: '间隔(Cron)',
                key: 'interval',
                width: 140
            },
            {
                title: '状态',
                key: 'status',
                width: 80,
                render: (h, params) => {
                    var style = { color: 'white', background: 'red', padding: '3px 10px', borderRadius: '4px' };
                    var text = '';
                    switch (params.row.status) {
                        case 0:
                            style.background = '#0acb0a';
                            text = '正常';
                            break;
                        case 1:
                            style.background = '#ed4014';
                            text = '暂停';
                            break;
                        case 2:
                            style.background = '#fc2f2f';
                            text = '完成';
                            break;
                        case 3:
                            style.background = '#607D8B';
                            text = '异常';
                            break;
                        case 4:
                            style.background = '#607D8B';
                            text = '阻塞';
                            break;
                        case 5:
                            style.background = '#607D8B';
                            text = '停止';
                            break;
                        default:
                            style.background = '#f90';
                            text = '不存在';
                            break;
                    }
                    return h('div', [
                        h('Button', {
                            props: {
                                //type: 'error',
                                size: 'small'
                            }, style: style,
                            on: {
                                click: function () {
                                }
                            }
                        }, text)
                    ]);
                }
            },
            {
                title: '描述',
                key: 'describe',
                minWidth: 200
            },
            {
                title: '请求地址',
                key: 'api_url',
                minWidth: 180
            },
            {
                title: '请求方式',
                key: 'request_method',
                width: 95
            },
            {
                title: '操作',
                key: 'operat',
                width: 100,
                render: (h, params) => {
                    var style = { 'font-size': '12px' };

                    return h('div', [
                        h('i-button', {
                            props: {
                                size: 'small'

                            }, style: style,
                            on: {
                                click: function () {
                                    $taskVue.getJobRunLog(params);
                                }
                            }
                        }, '执行记录')
                    ]);
                }
            }
        ],
        rows: []//数据
    },
    methods: {
        setPaginations() {
            this.paginations.total = this.allTableData.length; //数据的数量
            this.paginations.page_index = 1; //默认显示第一页
            this.paginations.page_size = 10; //每页显示多少数据

            console.log(this.allTableData.length)
            //显示数据
            this.rows = this.allTableData.filter((item, index) => {
                return index < this.paginations.page_size;
            })
        },
        handleSizeChange(page_size) {
            this.paginations.page_index = 1; //第一页
            this.paginations.page_size = page_size; //每页先显示多少数据
            this.rows = this.allTableData.filter((item, index) => {
                return index < page_size
            })
        },
        handleCurrentChange(page) {
            // 跳转页数
            //获取当前页
            let index = this.paginations.page_size * (page - 1);
            //获取总数
            let allData = this.paginations.page_size * page;

            let tablist = [];
            for (let i = index; i < allData; i++) {
                if (this.allTableData[i]) {
                    tablist.push(this.allTableData[i])
                }
                this.rows = tablist
            }
        },
        onSubmit() {
            axios.get('/api/task/all', {
                params: {
                    name: this.formInline.name,
                    groupName: this.formInline.group_name
                }

            }, { emulateJSON: true },
                {
                    headers: { "Content-Type": "application/x-www-form-urlencoded;charset=utf-8", }
                }
            ).then(res => {
                console.log(res.data)
                res.data.forEach(function (row) {
                    row.cellClassName = { operat: 'view-log' };
                });
                $taskVue.rows = res.data;
                const dataall = res.data;
                this.allTableData = dataall;
                this.setPaginations()

            })
        },
        onChange(item, value) {
            if (item.onChange && typeof item.onChange == "function") {
                item.onChange(value, item);
            }
        },
        getColumns: function () {
            var columns = [];
            this.columns.forEach(function (item) {
                if (!item.hidden) {
                    columns.push(item);
                }
            })
            return columns;
        },
        selectRow: function (selection, row) {
            this.select.currentRow = row;
            this.select.rows = selection;
        },
        first: function () {
            $taskVue.log.index = 0;
            $taskVue.log.page = 0;
            $taskVue.log.data = [];
            this.getJobRunLog(null, true);
        },
        next: function () {
            this.getJobRunLog(null, true);
        },
        getJobRunLog: function (params, next) {
            console.log(JSON.stringify(params.row));
            if (!next) {
                $taskVue.log.page = 0;
                $taskVue.log.index = 0;
                $taskVue.log.title = params.row.name;
                $taskVue.log.id = params.row.id;
                $taskVue.log.groupName = params.row.group_name;
                $taskVue.log.data = [];
            }
            $taskVue.log.model = true;
            $taskVue.log.page++;
            //任务日志
            $taskVue.ajaxget("/api/task/run-log", {
                id: $taskVue.log.id,
            }, function (data) {
                if (data.length === 0) {
                    if ($taskVue.log.page >= 1) {
                        $taskVue.log.page--;
                    }
                    if (next) {
                        $taskVue.$Message.success('未查到数据!');
                    }
                    return;
                }
                if (next) {
                    $taskVue.log.data = data;
                } else {
                    $taskVue.log.data = data;
                }
                $taskVue.log.index += $taskVue.log.index ? data.length : 1;
            });
        },
        getTaskValidate: function () {
        },
        add: function () {
            for (var key in this.taskValidate) {
                this.taskValidate[key] = '';
            }
            this.setFormClass(false);
            this.model = true;
        },
        tiggerAction: function (action) {
            if (!this.select.rows.length)
                return $taskVue.$Message.success('请选择作业!');
            this.ajax('/api/task/' + action,
                this.select.rows[0], function (data) {
                    if (data.status) {
                        if ($taskVue.formInline.group_name == "" && $taskVue.formInline.name == "") {
                            location.reload();
                        } else {
                            $taskVue.onSubmit();
                        }
                    }
                    return $taskVue.$Message.success(data.msg);
                });

        },
        update: function () {
            if (!this.select.rows.length)
                return $taskVue.$Message.success('请选择作业!');
            this.model = true;
            for (var key in this.select.rows[0]) {
                this.taskValidate[key] = this.select.rows[0][key];
            }
            this.setFormClass(true);
        },
        refresh: function (_init) {
            this.select.currentRow = [];
            this.select.rows = {};

            axios.get('/api/task/all', {
            }, { emulateJSON: true },
                {
                    headers: { "Content-Type": "application/x-www-form-urlencoded;charset=utf-8", }
                }
            ).then(res => {

                res.data.forEach(function (row) {
                    row.cellClassName = { operat: 'view-log' };
                });
                $taskVue.rows = res.data;

                const dataall = res.data;
                this.allTableData = dataall;
                this.setPaginations()
                if (!_init) {
                    $taskVue.onSubmit();
                    return $taskVue.$Message.success('刷新成功!');
                }

            });
            axios.get('/api/task/groupname-list', {
            }, { emulateJSON: true },
                {
                    headers: { "Content-Type": "application/x-www-form-urlencoded;charset=utf-8", }
                }
            ).then(res => {
                this.options = res.data;//分组下拉选择赋值

            })
        },
        handleSelectAll(status) {
            this.$refs.selection.selectAll(status);
        },
        handleSubmit(name) {
            this.$refs[name].validate((valid) => {
                if (!valid) {
                    return this.$Message.error('数据填写不完整!');
                }
                this.ajax("/api/task/" + (this.isAdd ? 'add' : 'update'), this.taskValidate, function (data) {
                    $taskVue.$Message.success(data.msg || '保存成功');
                    if (data.status) {
                        $taskVue.model = false;
                        // $taskVue.refresh(true);
                        $taskVue.onSubmit();
                    }
                });
            });

        },
        setFormClass: function (readOnly) {
            this.isAdd = !readOnly;
            this.modelMessage = !readOnly ? '新建任务' : '修改任务';
            this.taskForm.forEach(x => {
                if (x.name === "name" || x.name === "group_name") {
                    x.readOnly = readOnly;
                }
            });
        },
        ajax: function (url, params, fun) {
            axios({
                method: 'post',
                url: url,
                params: params,
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }).then(function (response) {
                fun && fun(response.data);
            }).catch(function (error) {
                if (error.response.status === 401) {
                    return window.location.href = '/home/login';
                }
                $taskVue.$Message.success('出错啦!');
                console.log(error);
            });
        },
        ajaxget: function (url, params, fun) {
            axios({
                method: 'get',
                url: url,
                params: params,
                headers: { 'X-Requested-With': 'XMLHttpRequest' }
            }).then(function (response) {
                fun && fun(response.data);
            }).catch(function (error) {

                $taskVue.$Message.success('出错啦!');
                console.log(error);
            });
        },
        //环境变量
        //读取表格数据
        readMasterUser() {

            axios.get('/api/task/env-list', {
            }, { emulateJSON: true },
                {
                    headers: { "Content-Type": "application/x-www-form-urlencoded;charset=utf-8", }
                }
            ).then(res => {
                let datalist = res.data;
                this.master_user.data = datalist;
                datalist.map(i => {
                    i.id = i.id;//模拟后台插入成功后有了id
                    i.isSet = false;//给后台返回数据添加`isSet`标识
                    return i;
                });

            })
        },
        //添加账号
        addMasterUser() {
            for (let i of $taskVue.master_user.data) {
                if (i.isSet) return $taskVue.$message.warning("请先保存当前编辑项");
            }
            let j = { id: 0, "key": "", "val": "", "isSet": true, "_temporary": true };
            $taskVue.master_user.data.push(j);
            $taskVue.master_user.sel = JSON.parse(JSON.stringify(j));
        },
        //修改
        pwdChange(row, index, cg) {

            // 点击修改 判断是否已经保存所有操作
            for (let i of $taskVue.master_user.data) {
                if (i.isSet && i.id != row.id) {
                    $taskVue.$message.warning("请先保存当前编辑项");
                    return false;
                }
            }
            //是否是取消操作
            if (!cg) {
                if (!$taskVue.master_user.sel.id) $taskVue.master_user.data.splice(index, 1);
                return row.isSet = !row.isSet;
            }
            //提交数据
            if (row.isSet) {
                //项目是模拟请求操作  自己修改下
                (function () {
                    let data = JSON.parse(JSON.stringify($taskVue.master_user.sel));
                    for (let k in data) row[k] = data[k];
                    $taskVue.ajax('/api/task/opera-env',
                        { id: data.id, key: data.key, val: data.val, isSet: data.isSet }, function (data) {
                            $taskVue.$Message.success(data.msg || '保存成功');
                            //然后这边重新读取表格数据
                            $taskVue.readMasterUser();
                            row.isSet = false;
                        });

                })();
            } else {
                $taskVue.master_user.sel = JSON.parse(JSON.stringify(row));
                row.isSet = true;
            }
        },
        //删除
        delChange(row) {
            $taskVue.ajax('/api/task/del-env',
                { id: row.id }, function (data) {
                    $taskVue.$Message.success(data.msg || '删除成功');
                    //然后这边重新读取表格数据
                    $taskVue.readMasterUser();
                    row.isSet = false;
                });
        }
    }, created: function () {
        this.refresh(true);
        this.readMasterUser();
    }, mounted: function () {
    }
});