var app = {
    init: function () {
        this.events();
        this.initToggleTarget();
        this.initselect2();
        this.initToastr();
        this.ajaxError();
        this.initCKEditor();
        this.initBackToTop();
        this.initLazyload();
    },
    lastElement: undefined,
    ajaxEvents:
    {
        OnBegin: function () {
            //showLoading();
            //app.resetResult();
        },
        OnComplete: function (element) {
            //hideLoading();
        },
        OnSuccess: function (response, status, xhr) {
            if (response != null) {
                if (response.Completed) {

                    if (response.Message != null) {
                        toastr.success(response.Message, 'Thông báo');
                    }

                    if (response.Cb != null && response.Cb.length > 0) {
                        eval(atob(response.Cb));
                    }

                    if (response.ReturnUrl != null) {
                        window.location.href = response.ReturnUrl;
                        return;
                    }
                }
                else if (response.Message != null) {
                    var username = '', msg = response.Message;
                    if (response.Message.indexOf('The provided anti-forgery token was meant for user') > -1) {
                        response.Message = response.Message.replace(/"/g, "'");
                        var str = response.Message.match(/'(.*?)'/g);
                        if (str != null) username = str[1];

                        msg = username.length > 0
                            ? 'Tài khoản: <b>' +
                            username +
                            '</b> hiện đã đăng nhập. Quý khách vui lòng refresh lại trang web để tiếp tục sử dụng.'
                            : response.Message;
                        msg += '<p>Để được hỗ trợ về đăng nhập tài khoản. Quý khách vui lòng liên hệ với LuatVietnam:</p>';
                        msg += '<p>Hotline: <a href="tel:0938361919" title="Hotline: 0938361919" style="color: #ed1d35">0938361919</a></p>';
                        msg += '<p>Email: <a href="mailto:cskh@luatvietnam.vn" title="Email: cskh@luatvietnam.vn">cskh@luatvietnam.vn</a></p>';
                    }

                    toastr.error(msg, 'Thông báo');
                }else {
                    if (response.ReturnUrl != null) {
                        window.location.href = response.ReturnUrl;
                        return;
                    }
                }
            }
            //hideLoading();
        },
        OnFailed: function () {
            //hideLoading();
            app.resetResult();
            toastr.error('Quý khách vui lòng thử lại sau.', 'Thông báo');
        }
    },
    events: function () {
        $(document).on('click', '.btn-search', function (e) {
            e.preventDefault();
            var form = $(this).closest('form');
            form.find('input[name="Page"]').first().val(1);
            form.attr('method', 'get').submit();
        });
        $(document).on('click', '.btn-export-excel', function (e) {
            e.preventDefault();
            $(this).closest('form').attr('method', 'post').submit();
        });
        $(document).on('change', '.load-districts', function () {
            var self = $(this), siteId = $('select[name="SiteId"] option:selected').val(), provinceId = self.val() || 0,
                select_districts = $('select[name="DistrictId"]'),
                select_wards = $('select[name="WardId"]'),
                select_streets = $('select[name="StreetId"]'),
                select_projects = $('select[name="ProjectId"]'),
                districtId = 0;

            select_districts.empty().append($('<option/>', {
                'value': '0',
                'text': '-- Chọn Quận / Huyện -- '
            }));

            select_wards.empty().append($('<option/>', {
                'value': '0',
                'text': '-- Chọn Phường / Xã -- '
            }));

            select_streets.empty().append($('<option/>', {
                'value': '0',
                'text': '-- Chọn Đường / Phố --'
            }));

            select_projects.empty().append($('<option/>', {
                'value': '0',
                'text': '-- Chọn Dự án --'
            }));

            app.fetchData(
                {
                    url: '/api/district/get.html',
                    dataType: 'json',
                    type: 'get',
                    data:
                    {
                        siteId: siteId,
                        provinceId: provinceId
                    },
                    beforeSend: function () {
                        select_districts.find('option')
                            .removeAttr('selected')
                            .filter('[value="0"]')
                            .text('Đang tải dữ liệu...')
                            .attr('selected', true)
                    },
                    always: function () {
                        select_districts.find('option')
                            .removeAttr('selected')
                            .filter('[value="0"]')
                            .text('-- Chọn Quận / Huyện --')
                            .attr('selected', true);

                        if (jQuery.fn.select2) {
                            $('select.select2').select2();
                        }
                    }
                })
                .then((response) => {
                    if (response != null && response.length > 0) {
                        $.each(response, function (i, obj) {
                            select_districts.append($('<option></option>').val(obj.Id)
                                .html(obj.Name));
                        });
                    }
                })
                .catch((error) => {
                    console.error(error)
                });

            app.fetchData(
                {
                    url: '/api/project/get.html',
                    dataType: 'json',
                    type: 'get',
                    data:
                    {
                        siteId: siteId,
                        provinceId: provinceId,
                        districtId: districtId
                    },
                    beforeSend: function () {
                        select_projects.find('option')
                            .removeAttr('selected')
                            .filter('[value="0"]')
                            .text('Đang tải dữ liệu...')
                            .attr('selected', true)
                    },
                    always: function () {
                        select_projects.find('option')
                            .removeAttr('selected')
                            .filter('[value="0"]')
                            .text('-- Chọn Dự án --')
                            .attr('selected', true);

                        if (jQuery.fn.select2) {
                            $('select.select2').select2();
                        }
                    }
                })
                .then((response) => {
                    if (response != null && response.length > 0) {
                        $.each(response, function (i, obj) {
                            select_projects.append($('<option></option>').val(obj.Id)
                                .html(obj.Name));
                        });
                    }
                })
                .catch((error) => {
                    console.error(error)
                });
        })
        $(document).on('change', '.load-wards', function () {
            var self = $(this), siteId = $('select[name="SiteId"] option:selected').val(), districtId = self.val() || 0,
                form = self.closest('form'),
                select_provinces = $('select[name="ProvinceId"]', form)
                select_wards = $('select[name="WardId"]', form),
                select_streets = $('select[name="StreetId"]', form),
                select_projects = $('select[name="ProjectId"]'),
                provinceId = select_provinces.find(':selected').val() || 0;

            select_wards.empty().append($('<option/>', {
                'value': '0',
                'text': '-- Chọn Phường / Xã --'
            }));

            select_streets.empty().append($('<option/>', {
                'value': '0',
                'text': '-- Chọn Đường / Phố --'
            }));

            select_projects.empty().append($('<option/>', {
                'value': '0',
                'text': '-- Chọn Dự án --'
            }));

            app.fetchData(
                {
                    url: '/api/ward/get.html',
                    dataType: 'json',
                    type: 'get',
                    data:
                    {
                        siteId: siteId,
                        districtId: districtId
                    },
                    beforeSend: function () {
                        select_wards.find('option')
                            .removeAttr('selected')
                            .filter('[value="0"]')
                            .text('Đang tải dữ liệu...')
                            .attr('selected', true)
                    },
                    always: function () {
                        select_wards.find('option')
                            .removeAttr('selected')
                            .filter('[value="0"]')
                            .text('-- Chọn Phường / Xã --')
                            .attr('selected', true);

                        if (jQuery.fn.select2) {
                            if (self.hasClass('js')) {
                                $('select.select2.js').select2({
                                    width: '100%',
                                    dropdownParent: $('#_editForm')
                                });
                            } else {
                                $('select.select2').select2({
                                    width: '100%',
                                    dropdownParent: form
                                });
                            }
                        }
                    }
                })
                .then((response) => {
                    if (response != null && response.length > 0) {
                        $.each(response, function (i, obj) {
                            select_wards.append($('<option></option>').val(obj.Id)
                                .html(obj.Name));
                        });
                    }
                })
                .catch((error) => {
                    console.error(error)
                });

            app.fetchData(
                {
                    url: '/api/project/get.html',
                    dataType: 'json',
                    type: 'get',
                    data:
                    {
                        siteId: siteId,
                        provinceId: provinceId,
                        districtId: districtId
                    },
                    beforeSend: function () {
                        select_projects.find('option')
                            .removeAttr('selected')
                            .filter('[value="0"]')
                            .text('Đang tải dữ liệu...')
                            .attr('selected', true)
                    },
                    always: function () {
                        select_projects.find('option')
                            .removeAttr('selected')
                            .filter('[value="0"]')
                            .text('-- Chọn Dự án --')
                            .attr('selected', true);

                        if (jQuery.fn.select2) {
                            $('select.select2').select2();
                        }
                    }
                })
                .then((response) => {
                    if (response != null && response.length > 0) {
                        $.each(response, function (i, obj) {
                            select_projects.append($('<option></option>').val(obj.Id)
                                .html(obj.Name));
                        });
                    }
                })
                .catch((error) => {
                    console.error(error)
                });
        })
        $(document).on('change', '.load-streets', function () {
            var self = $(this), siteId = $('select[name="SiteId"] option:selected').val(), wardId = self.val() || 0,
                form = self.closest('form'),
                select_streets = $('select[name="StreetId"]', form);

            select_streets.empty().append($('<option/>', {
                'value': '0',
                'text': '-- Chọn Đường / Phố --'
            }))

            app.fetchData(
                {
                    url: '/api/street/get.html',
                    dataType: 'json',
                    type: 'get',
                    data:
                    {
                        siteId: siteId,
                        wardId: wardId
                    },
                    beforeSend: function () {
                        select_streets.find('option')
                            .removeAttr('selected')
                            .filter('[value="0"]')
                            .text('Đang tải dữ liệu...')
                            .attr('selected', true)
                    },
                    always: function () {
                        select_streets.find('option')
                            .removeAttr('selected')
                            .filter('[value="0"]')
                            .text('-- Chọn Đường / Phố --')
                            .attr('selected', true);

                        if (jQuery.fn.select2) {
                            if (self.hasClass('js')) {
                                $('select.select2.js').select2({
                                    width: '100%',
                                    dropdownParent: $('#_editForm')
                                });
                            } else {
                                $('select.select2').select2({
                                    width: '100%',
                                    dropdownParent: form
                                });
                            }
                        }
                    }
                })
                .then((response) => {
                    if (response != null && response.length > 0) {
                        $.each(response, function (i, obj) {
                            select_streets.append($('<option></option>').val(obj.Id)
                                .html(obj.Name));
                        });
                    }
                })
                .catch((error) => {
                    console.error(error)
                })
        })
        $(document).on('change', 'select[name="ProvinceId"].push', function () {
            var self = $(this), provinceId = self.val();
            app.pushState({ 'ProvinceId': provinceId });
            app.pushState({ 'DistrictId': '0' });
            app.pushState({ 'WardId': '0' });
            app.pushState({ 'StreetId': '0' });
            app.pushState({ 'ProjectId': '0' });
        })
        $(document).on('change', 'select[name="DistrictId"].push', function () {
            var self = $(this), districtId = self.val();
            app.pushState({ 'DistrictId': districtId });
            app.pushState({ 'WardId': '0' });
            app.pushState({ 'StreetId': '0' });
            app.pushState({ 'ProjectId': '0' });
        })
        $(document).on('change', 'select[name="WardId"].push', function () {
            var self = $(this), wardId = self.val();
            app.pushState({ 'WardId': wardId });
            app.pushState({ 'StreetId': '0' });
        })
        $(document).on('change', 'select[name="StreetId"].push', function () {
            var self = $(this), streetId = self.val();
            app.pushState({ 'StreetId': streetId });
        })
        $(document).on('change', 'select[name="ProjectId"].push', function () {
            var self = $(this), projectId = self.val();
            app.pushState({ 'ProjectId': projectId });
        })
        $(document).on('click', '.add-ward', function () {
            var self = $(this), href = self.data('href'), provinceId = $('select[name="ProvinceId"] option:selected').val(),
                districtId = $('select[name="DistrictId"] option:selected').val(),
                wardId = $('select[name="WardId"] option:selected').val();
            if (typeof href != 'undefined') {
                if (typeof provinceId != 'undefined' && provinceId > 0 && typeof districtId != 'undefined' && districtId > 0 && typeof wardId != 'undefined' && wardId > 0) {
                    window.location.href = `${href}?ProvinceId=${provinceId}&DistrictId=${districtId}&WardId=${wardId}`
                } else if (typeof provinceId != 'undefined' && provinceId > 0) {
                    window.location.href = `${href}?ProvinceId=${provinceId}`
                } else if (typeof districtId != 'undefined' && districtId > 0) {
                    window.location.href = `${href}?DistrictId=${districtId}`
                } else if (typeof wardId != 'undefined' && wardId > 0) {
                    window.location.href = `${href}?WardId=${wardId}`
                } else window.location.href = href
            }
        })
        $(document).on('click', '.add-location', function () {
            var self = $(this), href = self.data('href'), provinceId = $('select[name="ProvinceId"] option:selected').val(),
                districtId = $('select[name="DistrictId"] option:selected').val(),
                wardId = $('select[name="WardId"] option:selected').val(),
                streetId = $('select[name="StreetId"] option:selected').val(),
                redirectUrl = href;
            if (typeof href != 'undefined') {
                //if (typeof provinceId != 'undefined' && provinceId > 0 && typeof districtId != 'undefined' && districtId > 0 && typeof wardId != 'undefined' && wardId > 0 && typeof streetId != 'undefined' && streetId > 0) {
                    //window.location.href = `${href}?ProvinceId=${provinceId}&DistrictId=${districtId}&WardId=${wardId}&StreetId=${streetId}`
                //} else
                if (typeof provinceId != 'undefined' && provinceId > 0) {
                    redirectUrl = app.setQueryParameter(redirectUrl, 'ProvinceId', provinceId)
                    //redirectUrl += `${href}?ProvinceId=${provinceId}`
                }
                if (typeof districtId != 'undefined' && districtId > 0) {
                    redirectUrl = app.setQueryParameter(redirectUrl, 'DistrictId', districtId)
                    //window.location.href = `${href}?DistrictId=${DistrictId}`
                }
                if (typeof wardId != 'undefined' && wardId > 0) {
                    redirectUrl = app.setQueryParameter(redirectUrl, 'WardId', wardId)
                    //window.location.href = `${href}?WardId=${wardId}`
                }
                if (typeof streetId != 'undefined' && streetId > 0) {
                    redirectUrl = app.setQueryParameter(redirectUrl, 'StreetId', streetId)
                    ///window.location.href = `${href}?StreetId=${streetId}`
                } 

                window.location.href = redirectUrl
            }
        })
        //$(document).on('click', '.add-street', function () {
        //    var self = $(this), href = self.data('href'), provinceId = $('select[name="ProvinceId"] option:selected').val(),
        //        districtId = $('select[name="DistrictId"] option:selected').val(),
        //        wardId = $('select[name="WardId"] option:selected').val();
        //    if (typeof href != 'undefined') {
        //        if (typeof provinceId != 'undefined' && provinceId > 0 && typeof districtId != 'undefined' && districtId > 0 && typeof wardId != 'undefined' && wardId > 0) {
        //            window.location.href = `${href}?ProvinceId=${provinceId}&DistrictId=${districtId}&WardId=${wardId}`
        //        } else if (typeof provinceId != 'undefined' && provinceId > 0 && typeof districtId != 'undefined' && districtId > 0) {
        //            window.location.href = `${href}?ProvinceId=${provinceId}&DistrictId=${districtId}`
        //        } else if (typeof districtId != 'undefined' && districtId > 0 && typeof wardId != 'undefined' && wardId > 0) {
        //            window.location.href = `${href}?DistrictId=${districtId}&WardId=${wardId}`
        //        } else if (typeof provinceId != 'undefined' && provinceId > 0) {
        //            window.location.href = `${href}?ProvinceId=${provinceId}`
        //        } else if (typeof districtId != 'undefined' && districtId > 0) {
        //            window.location.href = `${href}?DistrictId=${districtId}`
        //        } else if (typeof wardId != 'undefined' && wardId > 0) {
        //            window.location.href = `${href}?WardId=${wardId}`
        //        } else window.location.href = href
        //    }
        //})
        $(document).on('change',
            'select[name="Year"], select[name="Month"]',
            function () {
                var y = $('select[name="Year"] option:selected').val();
                var m = $('select[name="Month"] option:selected').val();
                var d = $('select[name="Day"] option:selected').val();
                app.getBirthDay(d, m, y);
            });
        $(document).on('click',
            '#select-all',
            function () {
                if (this.checked) {
                    $('.checkbox-select').each(function () {
                        $(this).prop('checked', true);
                        $(this).closest('td').find('input[type=hidden]').remove();
                    });
                } else {
                    $('.checkbox-select').each(function () {
                        $(this).prop('checked', false);
                        $(this).closest('td')
                            .append('<input type="hidden" value="' + $(this).val() + '" name="IdRemove">');
                    });
                }
            });
        $(document).on('change',
            '.checkbox-select',
            function () {
                if (this.checked) {
                    $(this).closest('td').find('input[type=hidden]').remove();
                } else {
                    var input = $(this).closest('td').find('input[type=hidden]');
                    if (input.length) {
                        input.val($(this).val());
                    } else {
                        $(this).closest('td')
                            .append('<input type="hidden" value="' + $(this).val() + '" name="IdRemove">');
                    }
                }
            });
        $(document).on('change',
            '.role',
            function () {
                if (this.checked) {
                    $(this).closest('label').find('input[type=hidden]').remove();
                } else {
                    var input = $(this).closest('label').find('input[type=hidden]');
                    if (input.length) {
                        input.val($(this).val());
                    } else {
                        $(this).closest('label')
                            .append('<input type="hidden" value="' + $(this).val() + '" name="RoleIdsRemove">');
                    }
                }
            });
        $(document).on('click',
            '.submit-form',
            function () {
                var parent = $(this).closest('.page-body'), table_responsive = parent.find('.table-responsive').first(),
                    form = table_responsive.find('form').first();
                if (form.length > 0)
                    form.submit();
            });
        $(document).on('change',
            'select[name="SiteId"]',
            function () {
                var form = $(this).closest('form').first();
                if (form.length > 0) {
                    var keywords = form.find('input[name="Keywords"]').first();
                    if (keywords.length > 0 && keywords.val().trim().length > 0) {
                        window.location.href = `${location.pathname}?SiteId=${$(this).val()}&Keywords=${keywords}`
                    }
                    else {
                        window.location.href = `${location.pathname}?SiteId=${$(this).val()}`
                    }
                }
            });
        $(document).on('click',
            '.btn-submit',
            function () {
                $(this).closest('form').submit();
            });
        $(document).on('keypress keyup blur', '.currency', function (event) {
            if ((event.which < 48 || event.which > 57)) {
                event.preventDefault();
            }
            var value = $(this).val().replace(/^(0*)/, '');
            $(this).val(app.currencyFormat(value));
        })
        $(document).on('keypress keyup blur', '.number', function (event) {
            if ((event.which < 48 || event.which > 57)) {
                event.preventDefault();
            }
            var value = $(this).val().replace(/^(0*)/, '');
            $(this).val(value);
        })
        $(document).on('keypress keyup blur', '.display-order', function (event) {
            if ((event.which < 48 || event.which > 57)) {
                event.preventDefault();
            }
        })
        $(document).on('click', '.go-back', function (event) {
            if ('referrer' in document) {
                window.location = document.referrer;
            } else {
                window.history.back();
            }
            //window.history.go(-1);
        })
        $(document).on('click', '.page-link.js', function (event) {
            event.preventDefault();
            var self = $(this), parent = self.closest('.tab-content'),
                form = parent.find('form').first(), page = self.data('page');
            if (form.length > 0 && typeof page != 'undefined')
            {
                form.find('input[type="hidden"][name="Page"]').first().val(page);
                form.submit();
            }
        })
        $(document).on('click', '.select-file', function (event) {
            event.preventDefault();
            var self = $(this), viewType = self.data('type') || '', width = self.data('w') || 1200,
                editForm = $('#_selectFileForm'), entry_popup = editForm.find('.entry-popup').first();
            editForm.find('.popup-wrapper').first().css('maxWidth', `${width}px`);
            app.lastElement = self;

            entry_popup.empty();
            entry_popup.html(`<div class="row row-cards">
                        <div class="col-12">
                            <div class="card placeholder-glow">
                                <div class="ratio ratio-21x9 card-img-top placeholder"></div>
                                <div class="card-body">
                                    <div class="placeholder col-9 mb-3"></div>
                                    <div class="placeholder placeholder-xs col-10"></div>
                                    <div class="placeholder placeholder-xs col-11"></div>
                                    <div class="mt-3">
                                        <span class="btn btn-primary disabled placeholder col-12" aria-hidden="true"></span>
                                    </div>
                                    <div class="mt-3">
                                        <span class="btn btn-primary disabled placeholder col-12" aria-hidden="true"></span>
                                    </div>
                                    <div class="mt-3">
                                        <span class="btn btn-primary disabled placeholder col-12" aria-hidden="true"></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>`)

            app.showDialog('#_selectFileForm');

            if (self.attr('requestRunning')) {
                app.showMessage({
                    messages: ['Đang xử lý dữ liệu. Quý khách vui lòng đợi...']
                });
                return false;
            }

            self.attr('requestRunning', true);

            setTimeout(function () {
                app.fetchData(
                    {
                        url: `/admin/file.html?viewType=${viewType}`,
                        dataType: 'html',
                        type: 'get',
                        always: function () {
                            self.removeAttr('requestRunning');
                        }
                    })
                    .then((response) => {
                        if (response != null && response.length > 0) {
                            entry_popup.html(response);
                            
                            app.validatorForm();

                            var myDropzone = new Dropzone("#kt_dropzonejs_example_1", {
                                paramName: "files", // The name that will be used to transfer the file
                                url: "/admin/ajax/uploadfiles",
                                maxFiles: 10,
                                maxFilesize: 10, // MB
                                acceptedFiles: 'image/*',    
                                addRemoveLinks: false,
                                init: function () {
                                    var myDropzone = this;

                                    // Listen to the sendingmultiple event. In this case, it's the sendingmultiple event instead
                                    // of the sending event because uploadMultiple is set to true.
                                    this.on("sendingmultiple", function () {
                                        // Gets triggered when the form is actually being sent.
                                        // Hide the success button or the complete form.
                                    });
                                    this.on("success", function (files, response) {
                                        app.bindTableData('/admin/file/binddata', 'file-table-result');
                                        $('.nav-tabs a[href="#tabs-home-14"]').tab('show');
                                        myDropzone.removeAllFiles();
                                        // Gets triggered when the files have successfully been sent.
                                        // Redirect user or notify of success.
                                    });
                                    this.on("errormultiple", function (files, response) {
                                        // Gets triggered when there was an error sending the files.
                                        // Maybe show form again, and notify user of error
                                    });
                                }
                            });
                            //app.showDialog('#_editForm');
                        }
                    })
                    .catch((error) => {
                        console.error(error);
                        self.removeAttr('requestRunning');
                        app.closeDialog('#_selectFileForm');
                    })
            }, 300)
            
        })
        $(document).on('click', '.selected-file', function (event) {
            event.preventDefault();
            var self = $(this), image = self.find('img').first(),
                filePath = image.attr('src');
            if (typeof filePath != 'undefined') {
                filePath = filePath.toLowerCase().replace('/mobile/', '/original/').replace('/standard/', '/orginal/').replace('/thumb/', '/orginal/').replace('/icon/', '/orginal/');
                $('.select-file').attr('src', filePath);
                $('.hidden-file-selected').val(filePath);
                app.closeDialog('#_selectFileForm');
            }
        })
        $(document).on('click', '.editor-selected-file', function (event) {
            event.preventDefault();
            var self = $(this), image = self.find('img').first(),
                filePath = image.attr('src'), fileName = image.attr('alt');
            if (typeof filePath != 'undefined' && typeof fileName != 'undefined') {
                filePath = filePath.toLowerCase().replace('/mobile/', '/original/').replace('/standard/', '/orginal/').replace('/thumb/', '/orginal/').replace('/icon/', '/orginal/');
                var html = `<figure class="the-article-image">
											<img alt="${fileName}" src="${filePath}">
										<figcaption class="caption-image">${fileName}</figcaption>
									</figure><br/>`;

                if (CKEDITOR.instances['SeoFooter'])
                    CKEDITOR.instances['SeoFooter'].insertHtml(html);

                if (CKEDITOR.instances['ArticleContent'])
                    CKEDITOR.instances['ArticleContent'].insertHtml(html);

                app.closeDialog('#_selectFileForm');
            }
        })
        $(document).on('click', '.attach-files', function (event) {
            event.preventDefault();
            var self = $(this), selectType = self.data('type') || 0, width = self.data('w') || 800,
                editForm = $('#_attachFilesForm'), entry_popup = editForm.find('.entry-popup').first();
            editForm.find('.popup-wrapper').first().css('maxWidth', `${width}px`);
            app.lastElement = self;

            //entry_popup.empty();
            

            //Dropzone.autoDiscover = false;
            //var dropzoneOptions = {
            //    url: "/admin/ajax/uploadfiles",
            //    paramName: "files",
            //    maxFilesize: 20, // MB
            //    addRemoveLinks: false,
            //    init: function () {
            //        this.on("maxfilesexceeded", function (file) {
            //            this.removeFile(file);
            //        });
            //    }
            //};

            //var imgDropzone = new Dropzone("#imgDropzone", dropzoneOptions);


            // set the dropzone container id
            //const id = "#kt_dropzonejs_example_3";
            //const dropzone = document.querySelectorAll(id);

            //// set the preview element template
            //var previewNode = dropzone.querySelectorAll(".dropzone-item");
            //previewNode.id = "";
            //var previewTemplate = previewNode.parentNode.innerHTML;
            //previewNode.parentNode.removeChild(previewNode);

            //var myDropzone = new Dropzone(id, { // Make the whole body a dropzone
            //    url: "/admin/ajax/uploadfiles", // Set the url for your upload script location
            //    parallelUploads: 20,
            //    maxFilesize: 1, // Max filesize in MB
            //    previewTemplate: previewTemplate,
            //    previewsContainer: id + " .dropzone-items", // Define the container to display the previews
            //    clickable: id + " .dropzone-select" // Define the element that should be used as click trigger to select files.
            //});

            //myDropzone.on("addedfile", function (file) {
            //    // Hookup the start button
            //    const dropzoneItems = dropzone.querySelectorAll('.dropzone-item');
            //    dropzoneItems.forEach(dropzoneItem => {
            //        dropzoneItem.style.display = '';
            //    });
            //});

            //// Update the total progress bar
            //myDropzone.on("totaluploadprogress", function (progress) {
            //    const progressBars = dropzone.querySelectorAll('.progress-bar');
            //    progressBars.forEach(progressBar => {
            //        progressBar.style.width = progress + "%";
            //    });
            //});

            //myDropzone.on("sending", function (file) {
            //    // Show the total progress bar when upload starts
            //    const progressBars = dropzone.querySelectorAll('.progress-bar');
            //    progressBars.forEach(progressBar => {
            //        progressBar.style.opacity = "1";
            //    });
            //});

            //// Hide the total progress bar when nothing"s uploading anymore
            //myDropzone.on("complete", function (progress) {
            //    const progressBars = dropzone.querySelectorAll('.dz-complete');

            //    setTimeout(function () {
            //        progressBars.forEach(progressBar => {
            //            progressBar.querySelector('.progress-bar').style.opacity = "0";
            //            progressBar.querySelector('.progress').style.opacity = "0";
            //        });
            //    }, 300);
            //});

            app.showDialog('#_attachFilesForm');

            //if (self.attr('requestRunning')) {
            //    app.showMessage({
            //        messages: ['Đang xử lý dữ liệu. Quý khách vui lòng đợi...']
            //    });
            //    return false;
            //}

            //self.attr('requestRunning', true);

            //setTimeout(function () {
            //    app.fetchData(
            //        {
            //            url: '/admin/file.html',
            //            dataType: 'html',
            //            type: 'get',
            //            always: function () {
            //                self.removeAttr('requestRunning');
            //            }
            //        })
            //        .then((response) => {
            //            if (response != null && response.length > 0) {
            //                entry_popup.html(response);
            //                if ($(response).find('#PostedFile').length > 0) {
            //                    app.initFileValidator();
            //                }
            //                app.validatorForm();
            //                //app.showDialog('#_editForm');
            //            }
            //        })
            //        .catch((error) => {
            //            console.error(error);
            //            self.removeAttr('requestRunning');
            //            app.closeDialog('#_attachFilesForm');
            //        })
            //}, 300)

        })
        $(document).on('click', '.delete-item', function (event) {
            event.preventDefault();
            var self = $(this), urlRequest = self.data('url');

            if (self.attr('requestRunning')) {
                app.showMessage({
                    messages: ['Đang xử lý dữ liệu. Quý khách vui lòng đợi...']
                });
                return false;
            }

            if (typeof urlRequest != 'undefined') {

                app.showMessage({
                    buttons: [{
                        Name: 'Đóng',
                        ClickEvent: function () { }
                    },
                    {
                        Name: 'Đồng ý',
                        ClassName: 'btn-ok',
                        ClickEvent: function () {
                            self.attr('requestRunning', true);

                            app.fetchData(
                                {
                                    url: urlRequest,
                                    dataType: 'json',
                                    type: 'post',
                                    always: function () {
                                        self.removeAttr('requestRunning');
                                    }
                                })
                                .then((response) => {
                                    if (response != null) {

                                        if (response.Completed) {

                                            if (response.Message != null) {
                                                toastr.success(response.Message, 'Thông báo');
                                            }

                                            if (response.Cb != null && response.Cb.length > 0) {
                                                eval(atob(response.Cb));
                                            }

                                            if (response.ReturnUrl != null) {
                                                window.location.href = response.ReturnUrl;
                                                return;
                                            }
                                        }
                                        else if (response.Message != null) {
                                            var username = '', msg = response.Message;
                                            if (response.Message.indexOf('The provided anti-forgery token was meant for user') > -1) {
                                                response.Message = response.Message.replace(/"/g, "'");
                                                var str = response.Message.match(/'(.*?)'/g);
                                                if (str != null) username = str[1];

                                                msg = username.length > 0
                                                    ? 'Tài khoản: <b>' +
                                                    username +
                                                    '</b> hiện đã đăng nhập. Quý khách vui lòng refresh lại trang web để tiếp tục sử dụng.'
                                                    : response.Message;
                                                msg += '<p>Để được hỗ trợ về đăng nhập tài khoản. Quý khách vui lòng liên hệ với LuatVietnam:</p>';
                                                msg += '<p>Hotline: <a href="tel:0938361919" title="Hotline: 0938361919" style="color: #ed1d35">0938361919</a></p>';
                                                msg += '<p>Email: <a href="mailto:cskh@luatvietnam.vn" title="Email: cskh@luatvietnam.vn">cskh@luatvietnam.vn</a></p>';

                                                toastr.error(msg, 'Thông báo');
                                            }
                                        } else {
                                            if (response.ReturnUrl != null) {
                                                window.location.href = response.ReturnUrl;
                                                return;
                                            }
                                        }

                                    }
                                })
                                .catch((error) => {
                                    console.log(error);
                                    self.removeAttr('requestRunning');
                                });
                        }
                    }],
                    messages: ['Xác nhận xóa dữ liệu đã chọn ? \r\n Dữ liệu sẽ không thể phục hồi !!!']
                });
            }
        })
        $(document).on('click', '.add-form, .edit-form', function (e) {
            e.preventDefault();
            var self = $(this), urlRequest = self.data('url'), width = self.data('w') || 800,
                editForm = $('#_editForm'), entry_popup = editForm.find('.entry-popup').first();
            editForm.find('.popup-wrapper').first().css('maxWidth', `${width}px`);
            app.lastElement = self;

            if (location.search.length > 0)
            {
                if (urlRequest.indexOf('?') != -1) {
                    urlRequest = `${urlRequest}&${location.search.replace('?', '')}`;
                } else urlRequest = `${urlRequest}?${location.search}`;
            }

            entry_popup.empty();
            entry_popup.html(`<div class="row row-cards">
                        <div class="col-12">
                            <div class="card placeholder-glow">
                                <div class="ratio ratio-21x9 card-img-top placeholder"></div>
                                <div class="card-body">
                                    <div class="placeholder col-9 mb-3"></div>
                                    <div class="placeholder placeholder-xs col-10"></div>
                                    <div class="placeholder placeholder-xs col-11"></div>
                                    <div class="mt-3">
                                        <span class="btn btn-primary disabled placeholder col-12" aria-hidden="true"></span>
                                    </div>
                                    <div class="mt-3">
                                        <span class="btn btn-primary disabled placeholder col-12" aria-hidden="true"></span>
                                    </div>
                                    <div class="mt-3">
                                        <span class="btn btn-primary disabled placeholder col-12" aria-hidden="true"></span>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>`)

            app.showDialog('#_editForm');

            if (self.attr('requestRunning')) {
                app.showMessage({
                    messages: ['Đang xử lý dữ liệu. Quý khách vui lòng đợi...']
                });
                return false;
            }

            if (typeof urlRequest != 'undefined') {
                self.attr('requestRunning', true);

                setTimeout(function () {
                    app.fetchData(
                        {
                            url: urlRequest,
                            dataType: 'html',
                            type: 'get',
                            always: function () {
                                self.removeAttr('requestRunning');
                            }
                        })
                        .then((response) => {
                            if (response != null && response.length > 0) {
                                entry_popup.html(response);
                             
                                app.validatorForm();
                                if (jQuery.fn.select2) {
                                    $('select.select2.js').select2({
                                        width: '100%',
                                        dropdownParent: editForm
                                    });
                                }
                                app.initCKEditor();
                                //app.showDialog('#_editForm');
                            }
                        })
                        .catch((error) => {
                            console.error(error);
                            self.removeAttr('requestRunning');
                            app.closeDialog('#_editForm');
                        })
                }, 300)
            }
        });
        $(document).on('click', '.approved, .unapproved', function (event) {
            event.preventDefault();
            var self = $(this), reviewStatusId = self.data('id'), parent = self.closest('.page-body'), table_responsive = parent.find('.table-responsive').first(),
                form = table_responsive.find('form').first();

            if (form.length > 0 && typeof reviewStatusId != 'undefined') {

                if ($('.checkbox-select', form).filter(':checked').length < 1) {
                    app.showMessage({ messages: ['Bạn chưa chọn bài viết để thao tác.'] });
                    return false;
                }

                form.find('input[type="hidden"][name="ReviewStatusId"]').val(reviewStatusId);
                form.submit();
            }
        })
    },
    formatState: function (state) {
        if (!state.id) {
            return state.text;
        }
        var $state = $(
            !$(state.element).data('p') && $(state.element).siblings('option[data-p="' + state.id + '"]').length > 0 ? '<strong>' + state.text + '</strong>' : '<span>' + state.text + '</span>'
        );
        return $state;
    },
    showMessage: function (options) {
        var defaults = {
            id: '_message',
            buttons: [{
                Name: 'Đóng',
                ClassName: 'btn-quit',
                ClickEvent: {}
            }],
            callback: {},
            success: true,
            title: 'Thông báo',
            messages: [],
            returnUrl: ''
        }
        options = $.extend(defaults, options);
        options.messages = $.isArray(options.messages) ? options.messages : [];
        var popup = `<div id="${options.id}" class="popup-bg target-hidden"> 
            <div class="popup-wrapper">
                <div class="popup-body">
                    <div class="popup-content">
                        <div class="form-pad16">
                            <div class="form-group- text-center">
                                <div class="text-successful color-orange">${options.title}</div> 
                            </div>`;
        if (options.messages.length > 0) {
            popup += '<div class="form-group- text-center text-content">';
            for (var i = 0; i < options.messages.length; i++) {
                popup += '<p>' + options.messages[i] + '</p>';
            }
            popup += `</div>
            <div class="form-group- popup-button text-center">`;
            if (options.buttons.length > 0) {
                for (var i = 0; i < options.buttons.length; i++) {
                    if (options.buttons[i].Name == 'Đóng') {
                        popup += `<span class="btn-quit" data-toggle-target="#${options.id}">Đóng</span>`;
                    } else {
                        popup += `<span class="${options.buttons[i].ClassName}">${options.buttons[i].Name}</span>`;
                    }
                }
            }
            popup += '</div></div>';
        }

        if (options.success) {
            $(`#${options.id}`).remove();
            $('body').append(popup);
            for (var i = 0; i < options.buttons.length; i++) {
                var el = options.buttons[i];
                var event = el.ClickEvent;
                $(`#${options.id}`).on('click',
                    `.${el.ClassName}`,
                    $.isFunction(event) ? event : $.noop
                );
            }
        }
        $('#' + options.id).addClass('target-expanded').css('display', '');
    },
    showDialog: function (dialog) {
        $('body').css({
            'overflow': 'hidden',
            'padding-right': '6px'
        });
        $(dialog).addClass('target-expanded');
    },
    closeDialog: function (dialog) {

        if ($('.popup-bg.target-expanded').length == 1) {
            $('body').css({
                'overflow': 'unset',
                'padding-right': '0'
            });
        }
        
        $(dialog).removeClass('target-expanded').addClass('target-hidden');
    },
    ckeditorClearValue(cKeditorId) {
        CKEDITOR.instances[cKeditorId].setData('');
    },
    initToggleTarget: function () {
        $(document).on('click', '[data-toggle-target]', function (e) {
            e.preventDefault();
            var self = $(this), target = self.attr('data-toggle-target');
            if (typeof target != typeof undefined) {
                $(target).toggleClass('target-expanded').css('display', '');
                if (!$(target).hasClass('target-expanded')) {
                    $('body').css({
                        'overflow': 'unset',
                        'padding-right': '0'
                    });
                }
            }
        });
    },
    initLazyload: function () {
        if (jQuery().lazyload) { $('.lazyload').lazyload(); }
    },
    bindTableData: function (path, destination) {
        destination = destination || 'table-result';
        console.log(destination);
        app.fetchData(
            {
                url: path + location.search,
                dataType: 'html',
                type: 'post'
            })
            .then((response) => {
                $(`#${destination}`).html(response);
            })
            .catch((error) => {
                console.error(error)
            })
    },
    daysList: function (month, year) {
        var day = new Date(year, month, 0);
        return day.getDate();
    },
    getBirthDay: function (d, m, y) {
        var date = new Date(), days = app.daysList(m == 0 ? date.getMonth() + 1 : m, y == 0 ? date.getFullYear() : y),
            selectDay = $('select[name="Day"]');
        selectDay.html('<option value="0"> Ngày </option>');
        for (var i = 1; i <= days; i++) {
            selectDay.append('<option value="' + i + '" ' + (i == d ? 'selected' : '') + '>' + i + '</option>');
        };
    },
    currencyFormat: function (num) {
        var str = num.toString(), parts = false, output = [], i = 1, formatted = null;
        if (str.indexOf(".") > 0) {
            parts = str.split(".");
            str = parts[0];
        }
        str = str.split("").reverse();
        for (var j = 0, len = str.length; j < len; j++) {
            if (str[j] != ",") {
                output.push(str[j]);
                if (i % 3 == 0 && j < (len - 1)) {
                    output.push(",");
                }
                i++;
            }
        }
        formatted = output.reverse().join("");
        return (formatted + ((parts) ? "." + parts[1].substr(0, 2) : ""));
    },
    fetchData: function (options) {
        return new Promise((resolve, reject) => {
            $.ajax({
                cache: options.cache || true,
                url: options.url,
                type: options.type || 'GET',
                data: options.data || {},
                dataType: options.dataType || 'json',
                beforeSend: options.beforeSend || app.showLoading(),
                success: function (data) {
                    resolve(data);
                },
                error: function (error) {
                    reject(error);
                }
            }).always(options.always || app.hideLoading());
        });
    },
    addParam: function (currentUrl, key, val) {
        var url = new URL(currentUrl);
        url.searchParams.set(key, val);
        return url.href; 
    },
    setQueryParameter: function (uri, key, value) {
        var re = new RegExp("([?&])(" + key + "=)[^&#]*", "g");
        if (uri.match(re))
            return uri.replace(re, '$1$2' + value);

        // need to add parameter to URI
        var paramString = (uri.indexOf('?') < 0 ? "?" : "&") + key + "=" + value;
        var hashIndex = uri.indexOf('#');
        if (hashIndex < 0)
            return uri + paramString;
        else
            return uri.substring(0, hashIndex) + paramString + uri.substring(hashIndex);
    },
    pushState: function (object) {
        if (window.history && window.history.pushState) {
            var url = new URL(window.location.href);
            for (var key in object) {
                url.searchParams.set(key, object[key]);
            }
            window.history.pushState(null, '', url);
        }
    },
    validatorForm: function () {
        $.validator.unobtrusive.parse('form');
    },
    showLoading: function () {

    },
    hideLoading: function () {

    },
    initselect2: function () {
        if (jQuery.fn.select2) {
            $('select.select2').select2({
                width: '100%'
            });
            $('select.select22').select2({
                width: '100%',
                templateResult: app.formatState
            });
        }
    },
    initCKEditor: function () {
        if (typeof (CKEDITOR) != 'undefined') {
            $('.js-ckeditor').each(function (index, element) {
                CKEDITOR.replace($(element).attr('id'), {
                    language: 'vi',
                    removeButtons: 'Underline,Subscript,Superscript,Cut,Copy,Paste,PasteText,PasteFromWord,Scayt,Image,Html5video,Table,Maximize,Anchor,About,SpecialChar,Undo,Redo'
                });
            })
        }
    },
    initBackToTop() {
        var oldscrollTop = 0;
        $(window).scroll(function () {
            if ($(this).scrollTop() - oldscrollTop < 0) {
                $('.backtotop').addClass('show_backtotop');
            } else {
                $('.backtotop').removeClass('show_backtotop');
            }
            oldscrollTop = $(this).scrollTop();
        });
    },
    initToastr: function () {
        if (typeof window['toastr'] !== 'undefined') {
            toastr.options = {
                "closeButton": true,
                "debug": false,
                "newestOnTop": true,
                "progressBar": true,
                "positionClass": "toast-bottom-right",
                "preventDuplicates": true,
                "preventOpenDuplicates": true,
                "onclick": null,
                "showDuration": "300",
                "hideDuration": "1000",
                "timeOut": "5000",
                "extendedTimeOut": "1000",
                "showEasing": "swing",
                "hideEasing": "linear",
                "showMethod": "fadeIn",
                "hideMethod": "fadeOut"
            }
        } 
    },
    ajaxError: function () {
        $(document).ajaxError(function (event, xhr, options, exception) {
            app.closeDialog('#_editForm');
            if (app.lastElement && app.lastElement.attr('requestRunning')) {
                app.lastElement.removeAttr('requestRunning');
            }
            if (xhr.status) {

                var response;
                if (app.isJSON(xhr.responseText)) {
                    response = $.parseJSON(xhr.responseText);
                }

                if (xhr.status === 404) {
                    window.location.href = '/404.html';
                    return;
                }

                if (xhr.status === 401) {
                    if (response && response.ReturnUrl) {
                        window.location.href = response.ReturnUrl;
                    } else {
                        window.location.href = '/login.html';
                    }

                    return;
                }

                if (xhr.status === 403) {
                    if (response && response.ReturnUrl) {
                        window.location.href = response.ReturnUrl;
                    } else {
                        window.location.href = '/403.html';
                    }

                    return;
                }

                app.showMessage({ messages: [app.formatErrorMessage(xhr, exception)] })
            }
        });
    },
    isJSON: function (value) {
        if (typeof value != 'string')
            value = JSON.stringify(value);

        try {
            JSON.parse(value);
            return true;
        } catch (e) {
            return false;
        }
    },
    formatErrorMessage: function (jqXHR, exception) {
        if (jqXHR.status === 0) {
            return ('Không có kết nối mạng.\nVui lòng kiểm tra lại kết nối của bạn.');
        } else if(exception === 'timeout') {
            return ('Hết thời gian yêu cầu. Vui lòng thử lại sau.');
        } else if (exception === 'abort') {
            return ('Yêu cầu đã bị hủy bỏ.');
        } else {
            return ('Quý khách vui lòng thử lại sau.');
        }
    },
    convertBytesToMegabytes: function (bytes) {
        return (bytes / 1024) / 1024;
    },
    initFileValidator: function () {
        $.validator.unobtrusive.adapters.addSingleVal("allowfilesize", "filesize");
        $.validator.unobtrusive.adapters.addSingleVal("allowfileextensions", "fileextensions");

        $.validator.addMethod('allowfilesize', function (value, element, maxSize) {
            if (this.optional(element)) { // No value is always valid
                return true;
            }
            return app.convertBytesToMegabytes(element.files[0].size) <= parseFloat(maxSize);
        });

        $.validator.addMethod('allowfileextensions', function (value, element, validFileTypes) {
            if (this.optional(element)) { // No value is always valid
                return true;
            }
            if (validFileTypes.indexOf(',') > -1) {
                validFileTypes = validFileTypes.split(',');
            } else {
                validFileTypes = [validFileTypes];
            }

            var fileType = value.split('.')[value.split('.').length - 1];

            for (var i = 0; i < validFileTypes.length; i++) {
                if (validFileTypes[i] === fileType) {
                    return true;
                }
            }

            return false;
        });
    }
}
$(function () {
    app.init();
});