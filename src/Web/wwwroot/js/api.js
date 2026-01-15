(function() {
    'use strict';

    // 共用 API 服務，提供 AJAX 呼叫功能
    window.apiService = {
        /**
         * 從 localStorage 取得 JWT Token
         * @returns {string|null} JWT Token 或 null
         */
        getAuthToken: function() {
            return localStorage.getItem('jwtToken');
        },

        /**
         * 發送 GET 請求
         * @param {string} url - 請求 URL
         * @param {function} onSuccess - 成功回調函式
         * @param {function} onError - 錯誤回調函式（可選）
         */
        get: function(url, onSuccess, onError) {
            $.ajax({
                url: url,
                type: 'GET',
                beforeSend: function(xhr) {
                    var token = this.getAuthToken();
                    if (token) {
                        xhr.setRequestHeader('Authorization', 'Bearer ' + token);
                    }
                }.bind(this),
                success: onSuccess,
                error: function(xhr, status, error) {
                    this.handleError(xhr, onError);
                }.bind(this)
            });
        },

        /**
         * 發送 POST 請求
         * @param {string} url - 請求 URL
         * @param {object} data - 請求資料
         * @param {function} onSuccess - 成功回調函式
         * @param {function} onError - 錯誤回調函式（可選）
         */
        post: function(url, data, onSuccess, onError) {
            $.ajax({
                url: url,
                type: 'POST',
                contentType: 'application/json',
                data: JSON.stringify(data),
                beforeSend: function(xhr) {
                    var token = this.getAuthToken();
                    if (token) {
                        xhr.setRequestHeader('Authorization', 'Bearer ' + token);
                    }
                }.bind(this),
                success: onSuccess,
                error: function(xhr, status, error) {
                    this.handleError(xhr, onError);
                }.bind(this)
            });
        },

        /**
         * 發送 PUT 請求
         * @param {string} url - 請求 URL
         * @param {object} data - 請求資料
         * @param {function} onSuccess - 成功回調函式
         * @param {function} onError - 錯誤回調函式（可選）
         */
        put: function(url, data, onSuccess, onError) {
            $.ajax({
                url: url,
                type: 'PUT',
                contentType: 'application/json',
                data: JSON.stringify(data),
                beforeSend: function(xhr) {
                    var token = this.getAuthToken();
                    if (token) {
                        xhr.setRequestHeader('Authorization', 'Bearer ' + token);
                    }
                }.bind(this),
                success: onSuccess,
                error: function(xhr, status, error) {
                    this.handleError(xhr, onError);
                }.bind(this)
            });
        },

        /**
         * 發送 DELETE 請求
         * @param {string} url - 請求 URL
         * @param {function} onSuccess - 成功回調函式
         * @param {function} onError - 錯誤回調函式（可選）
         */
        del: function(url, onSuccess, onError) {
            $.ajax({
                url: url,
                type: 'DELETE',
                beforeSend: function(xhr) {
                    var token = this.getAuthToken();
                    if (token) {
                        xhr.setRequestHeader('Authorization', 'Bearer ' + token);
                    }
                }.bind(this),
                success: onSuccess,
                error: function(xhr, status, error) {
                    this.handleError(xhr, onError);
                }.bind(this)
            });
        },

        /**
         * 統一處理 AJAX 錯誤
         * @param {object} xhr - XMLHttpRequest 物件
         * @param {function} onError - 自訂錯誤回調函式（可選）
         */
        handleError: function(xhr, onError) {
            if (xhr.status === 401) {
                // 未授權，導向登入頁
                window.location.href = '/Account/Login';
            } else if (xhr.status === 403) {
                // 權限不足
                toastr.error('權限不足，請聯絡管理員。');
            } else {
                // 其他錯誤
                var message = xhr.responseJSON && xhr.responseJSON.message ? xhr.responseJSON.message : '發生未知錯誤，請稍後再試。';
                toastr.error(message);
            }

            // 如果有自訂錯誤回調，呼叫它
            if (onError) {
                onError(xhr);
            }
        }
    };
})();