$(document).ready(function() {
    'use strict';

    // 初始化 DataTables，用於顯示使用者列表
    var usersTable = $('#usersTable').DataTable({
        ajax: {
            url: '/api/users', // 從 API 取得使用者資料
            type: 'GET'
        },
        columns: [
            {
                data: 'username',
                title: '帳號'
            },
            {
                data: 'email',
                title: '電子郵件'
            },
            {
                data: 'role',
                title: '角色',
                render: function(data, type, row) {
                    // 將角色轉成 badge 顯示
                    if (data === 1) {
                        return '<span class="badge badge-primary">Admin</span>';
                    } else {
                        return '<span class="badge badge-secondary">User</span>';
                    }
                }
            },
            {
                data: 'isActive',
                title: '狀態',
                render: function(data, type, row) {
                    // 將狀態轉成 badge 顯示
                    if (data) {
                        return '<span class="badge badge-success">啟用</span>';
                    } else {
                        return '<span class="badge badge-danger">停用</span>';
                    }
                }
            },
            {
                data: 'createdAt',
                title: '建立時間',
                render: function(data, type, row) {
                    // 格式化日期時間
                    return new Date(data).toLocaleString('zh-TW');
                }
            },
            {
                data: 'id',
                title: '操作',
                orderable: false,
                render: function(data, type, row) {
                    // 生成操作按鈕
                    return '<a href="/User/Details/' + data + '" class="btn btn-info btn-sm" title="檢視"><i class="fas fa-eye"></i></a> ' +
                           '<a href="/User/Edit/' + data + '" class="btn btn-warning btn-sm" title="編輯"><i class="fas fa-edit"></i></a> ' +
                           '<button class="btn btn-danger btn-sm delete-btn" data-user-id="' + data + '" title="刪除"><i class="fas fa-trash"></i></button>';
                }
            }
        ],
        language: {
            // DataTables 中文語言設定
            "sProcessing": "處理中...",
            "sLengthMenu": "顯示 _MENU_ 項結果",
            "sZeroRecords": "沒有匹配結果",
            "sInfo": "顯示第 _START_ 至 _END_ 項結果，共 _TOTAL_ 項",
            "sInfoEmpty": "顯示第 0 至 0 項結果，共 0 項",
            "sInfoFiltered": "(由 _MAX_ 項結果過濾)",
            "sInfoPostFix": "",
            "sSearch": "搜尋:",
            "sUrl": "",
            "sEmptyTable": "表格中沒有資料",
            "sLoadingRecords": "載入中...",
            "sInfoThousands": ",",
            "oPaginate": {
                "sFirst": "首頁",
                "sPrevious": "上頁",
                "sNext": "下頁",
                "sLast": "末頁"
            },
            "oAria": {
                "sSortAscending": ": 以升序排列此列",
                "sSortDescending": ": 以降序排列此列"
            }
        },
        pageLength: 10, // 預設每頁顯示 10 項
        responsive: true // 響應式設計
    });

    // 處理刪除按鈕點擊事件
    $(document).on('click', '.delete-btn', function() {
        var userId = $(this).data('user-id'); // 取得使用者 ID

        // 使用 SweetAlert2 顯示確認對話框
        Swal.fire({
            title: '確定要刪除此使用者嗎？',
            text: '此操作無法復原！',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#3085d6',
            confirmButtonText: '是的，刪除它！',
            cancelButtonText: '取消'
        }).then((result) => {
            if (result.isConfirmed) {
                // 確認刪除，呼叫 API
                window.apiService.del('/api/users/' + userId, function() {
                    // 刪除成功，顯示成功訊息並重新載入表格
                    toastr.success('使用者已成功刪除。');
                    usersTable.ajax.reload(); // 重新載入 DataTables
                });
            }
        });
    });
});