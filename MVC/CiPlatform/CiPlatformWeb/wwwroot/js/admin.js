﻿
//get cities from country
$('.countryList').on('click', function () {
    var countryId = $(this).val();
    console.log(countryId)
    $.ajax({
        type: "GET",
        url: "/Admin/GetCitiesByCountry",
        data: { countryId: countryId },
        success: function (data) {
            var dropdown = $(".cityList");
            dropdown.empty();
            var items = '<option value="">Select city</option>'
            $(data).each(function (i, item) {
                items += '<option value=' + item.cityId + '>' + item.name + '</option>'
            });
            dropdown.html(items);
        },
        error: function (error) {
            console.log(error)
        }
    });
});


//validation
$('.validateAdminForm').removeData('validator').removeData('unobtrusiveValidation');
$.validator.unobtrusive.parse('.validateAdminForm');

//add user
$('#addUser').on('click', function () {

    $.ajax({
        type: 'GET',
        url: '/Admin/AddUser',
        success: function (data) {
            var container = $('.adminUserContainer');
            container.empty();
            container.append(data);
        },
        error: function (error) {
            console.log(error)
        }
    });
})

//edit user
$('.editUser').on('click', function () {
    var userId = $(this).closest('tr').attr('id');
    console.log(userId)
    $.ajax({
        type: 'GET',
        url: "/Admin/EditUser",
        data: { userId: userId },
        success: function (data) {
            var container = $('.adminUserContainer');
            container.empty();
            container.append(data);
        },
        error: function (error) {
            console.log(error)
        }
    });
});

//delete user
$('.deleteUser').on('click', function () {
    var userId = $(this).closest('tr').attr('id')
    console.log(userId)
});


//add cms
$('#addCms').on('click', function () {
    $.ajax({
        type: 'GET',
        url: '/Admin/AddCms',
        success: function (data) {
            var container = $('.adminCmsContainer');
            container.empty();
            container.append(data);
        },
        error: function (error) {
            console.log(error)
        }
    });
})

//edit user
$('.editCms').on('click', function () {
    var userId = $(this).closest('tr').attr('id');
    console.log(userId)
    $.ajax({
        type: 'GET',
        url: "/Admin/EditCms",
        data: { userId: userId },
        success: function (data) {
            var container = $('.adminCmsContainer');
            container.empty();
            container.append(data);
        },
        error: function (error) {
            console.log(error)
        }
    });
});


//add theme
$('#addTheme').on('click', function () {
    $.ajax({
        type: 'GET',
        url: '/Admin/AddTheme',
        success: function (data) {
            var container = $('.adminThemeContainer');
            container.empty();
            container.append(data);
        },
        error: function (error) {
            console.log(error)
        }
    });
})

//edit theme
$('.editTheme').on('click', function () {
    var themeId = $(this).closest('tr').attr('id');
    console.log(themeId)
    $.ajax({
        type: 'GET',
        url: "/Admin/EditTheme",
        data: { themeId: themeId },
        success: function (data) {
            var container = $('.adminThemeContainer');
            container.empty();
            container.append(data);
        },
        error: function (error) {
            console.log(error)
        }
    });
});


//add skill
$('#addSkill').on('click', function () {
    $.ajax({
        type: 'GET',
        url: '/Admin/AddSkill',
        success: function (data) {
            var container = $('.adminSkillContainer');
            container.empty();
            container.append(data);
        },
        error: function (error) {
            console.log(error)
        }
    });
})

//edit skill
$('.editSkill').on('click', function () {
    var skillId = $(this).closest('tr').attr('id');
    console.log(skillId)
    $.ajax({
        type: 'GET',
        url: "/Admin/EditSkill",
        data: { skillId: skillId },
        success: function (data) {
            var container = $('.adminSkillContainer');
            container.empty();
            container.append(data);
        },
        error: function (error) {
            console.log(error)
        }
    });
});

//change application status
$('.changeApplicationStatus').click(function () {
    var applicationId = $(this).closest('tr').attr('id');
    var status = $(this).data('value')

    $.ajax({
        type: "POST",
        url: "/Admin/ChangeApplicationStatus",
        data: { applicationId: applicationId, status: status },
        success: function () {
            var container = $('.showApplicationButtons-' + applicationId);
            container.empty();
            if (status == 0) {
                container.html('<i class="bi bi-check-circle changeApplicationStatus" data-value="1" style="color: #14C506;"></i><i class="bi bi-x-circle-fill ms-2" data-value="0"  style="color: #f20707;"></i>');
            }
            else {
                container.html('<i class="bi bi-check-circle-fill" data-value="1" style="color: #14C506;"></i><i class="bi bi-x-circle ms-2 changeApplicationStatus" data-value="0" style="color: #f20707;"></i>');
            }
        },
        error: function (error) {
            console.log(error)
        }
    });
})


//change story status
$('.changeStoryStatus').click(function () {
    var storyId = $(this).closest('tr').attr('id');
    var status = $(this).data('value')
    console.log(storyId, status)

    $.ajax({
        type: "POST",
        url: "/Admin/ChangeStoryStatus",
        data: { storyId: storyId, status: status },
        success: function () {
            var container = $('.showStoryButtons-' + storyId);
            container.empty();
            if (status == 0) {
                container.html('<i class="bi bi-check-circle ms-2 changeStoryStatus" data-value="1" style="color: #14C506;"></i><i class="bi bi-x-circle-fill ms-2" data-value="0" style="color: #f20707;"></i>');
            }
            else {
                container.html('<i class="bi bi-check-circle-fill ms-2" data-value="1" style="color: #14C506;"></i><i class="bi bi-x-circle ms-2 changeStoryStatus" data-value="0" style="color: #f20707;"></i>');
            }
        },
        error: function (error) {
            console.log(error)
        }
    });
})


//view story details
$('.viewStory').click(function () {
    var storyId = $(this).closest('tr').attr('id');
    console.log(storyId)
    
    $.ajax({
        type: 'GET',
        url: '/Admin/ViewStory',
        data: { storyId: storyId },
        success: function (data) {
            var container = $('.adminStoryContainer');
            container.empty();
            container.append(data);
        },
        error: function (error) {
            console.log(error)
        }
    });
})


//Admin user table
var userTable = $('#userTable').DataTable({
    lengthChange: false,
    ordering: false,
    paging: true,
    searching: true,
    pageLength: 7,
    pagingType: "full_numbers",
    language: {
        paginate: {
            first: '<span class="image-class-first"><i class="bi bi-chevron-double-left"></i></span>',
            previous: '<span class="image-class-previous"><i class="bi bi-chevron-left"></i></span>',
            next: '<span class="image-class-next"><i class="bi bi-chevron-right"></i></span>',
            last: '<span class="image-class-last"><i class="bi bi-chevron-double-right"></i></span>'
        }
    },
    drawCallback: function (settings) {
        var api = this.api();
        var numRows = api.rows({ search: "applied" }).count();
        if (numRows === 0) {
            $(this).closest('.dataTables_wrapper').find('.dataTables_paginate').hide();
        } else {
            $(this).closest('.dataTables_wrapper').find('.dataTables_paginate').show();
        }
    }
});

$('#searchUser').on('keyup', function () {
    userTable.search($(this).val()).draw();
})


//Admin cms table
var cmsTable = $('#cmsTable').DataTable({
    lengthChange: false,
    ordering: false,
    paging: true,
    searching: true,
    pageLength: 7,
    pagingType: "full_numbers",
    language: {
        paginate: {
            first: '<span class="image-class-first"><i class="bi bi-chevron-double-left"></i></span>',
            previous: '<span class="image-class-previous"><i class="bi bi-chevron-left"></i></span>',
            next: '<span class="image-class-next"><i class="bi bi-chevron-right"></i></span>',
            last: '<span class="image-class-last"><i class="bi bi-chevron-double-right"></i></span>'
        }
    },
    drawCallback: function (settings) {
        var api = this.api();
        var numRows = api.rows({ search: "applied" }).count();
        if (numRows === 0) {
            $(this).closest('.dataTables_wrapper').find('.dataTables_paginate').hide();
        } else {
            $(this).closest('.dataTables_wrapper').find('.dataTables_paginate').show();
        }
    }
});

$('#searchCms').on('keyup', function () {
    cmsTable.search($(this).val()).draw();
})


//Admin mission table
var missionTable = $('#missionTable').DataTable({
    lengthChange: false,
    ordering: false,
    paging: true,
    searching: true,
    pageLength: 7,
    pagingType: "full_numbers",
    language: {
        paginate: {
            first: '<span class="image-class-first"><i class="bi bi-chevron-double-left"></i></span>',
            previous: '<span class="image-class-previous"><i class="bi bi-chevron-left"></i></span>',
            next: '<span class="image-class-next"><i class="bi bi-chevron-right"></i></span>',
            last: '<span class="image-class-last"><i class="bi bi-chevron-double-right"></i></span>'
        }
    },
    drawCallback: function (settings) {
        var api = this.api();
        var numRows = api.rows({ search: "applied" }).count();
        if (numRows === 0) {
            $(this).closest('.dataTables_wrapper').find('.dataTables_paginate').hide();
        } else {
            $(this).closest('.dataTables_wrapper').find('.dataTables_paginate').show();
        }
    }
});

$('#searchMission').on('keyup', function () {
    missionTable.search($(this).val()).draw();
})


//Admin theme table
var themeTable = $('#themeTable').DataTable({
    lengthChange: false,
    ordering: false,
    paging: true,
    searching: true,
    pageLength: 7,
    pagingType: "full_numbers",
    language: {
        paginate: {
            first: '<span class="image-class-first"><i class="bi bi-chevron-double-left"></i></span>',
            previous: '<span class="image-class-previous"><i class="bi bi-chevron-left"></i></span>',
            next: '<span class="image-class-next"><i class="bi bi-chevron-right"></i></span>',
            last: '<span class="image-class-last"><i class="bi bi-chevron-double-right"></i></span>'
        }
    },
    drawCallback: function (settings) {
        var api = this.api();
        var numRows = api.rows({ search: "applied" }).count();
        if (numRows === 0) {
            $(this).closest('.dataTables_wrapper').find('.dataTables_paginate').hide();
        } else {
            $(this).closest('.dataTables_wrapper').find('.dataTables_paginate').show();
        }
    }
});

$('#searchTheme').on('keyup', function () {
    themeTable.search($(this).val()).draw();
})


//Admin skill table
var skillTable = $('#skillTable').DataTable({
    lengthChange: false,
    ordering: false,
    paging: true,
    searching: true,
    pageLength: 7,
    pagingType: "full_numbers",
    language: {
        paginate: {
            first: '<span class="image-class-first"><i class="bi bi-chevron-double-left"></i></span>',
            previous: '<span class="image-class-previous"><i class="bi bi-chevron-left"></i></span>',
            next: '<span class="image-class-next"><i class="bi bi-chevron-right"></i></span>',
            last: '<span class="image-class-last"><i class="bi bi-chevron-double-right"></i></span>'
        }
    },
    drawCallback: function (settings) {
        var api = this.api();
        var numRows = api.rows({ search: "applied" }).count();
        if (numRows === 0) {
            $(this).closest('.dataTables_wrapper').find('.dataTables_paginate').hide();
        } else {
            $(this).closest('.dataTables_wrapper').find('.dataTables_paginate').show();
        }
    }
});

$('#searchSkill').on('keyup', function () {
    skillTable.search($(this).val()).draw();
})




//Admin application table
var applicationTable = $('#applicationTable').DataTable({
    lengthChange: false,
    ordering: false,
    paging: true,
    searching: true,
    pageLength: 7,
    pagingType: "full_numbers",
    language: {
        paginate: {
            first: '<span class="image-class-first"><i class="bi bi-chevron-double-left"></i></span>',
            previous: '<span class="image-class-previous"><i class="bi bi-chevron-left"></i></span>',
            next: '<span class="image-class-next"><i class="bi bi-chevron-right"></i></span>',
            last: '<span class="image-class-last"><i class="bi bi-chevron-double-right"></i></span>'
        }
    },
    drawCallback: function (settings) {
        var api = this.api();
        var numRows = api.rows({ search: "applied" }).count();
        if (numRows === 0) {
            $(this).closest('.dataTables_wrapper').find('.dataTables_paginate').hide();
        } else {
            $(this).closest('.dataTables_wrapper').find('.dataTables_paginate').show();
        }
    }
});

$('#searchApplication').on('keyup', function () {
    applicationTable.search($(this).val()).draw();
})



//Admin story table
var storyTable = $('#storyTable').DataTable({
    lengthChange: false,
    ordering: false,
    paging: true,
    searching: true,
    pageLength: 7,
    pagingType: "full_numbers",
    language: {
        paginate: {
            first: '<span class="image-class-first"><i class="bi bi-chevron-double-left"></i></span>',
            previous: '<span class="image-class-previous"><i class="bi bi-chevron-left"></i></span>',
            next: '<span class="image-class-next"><i class="bi bi-chevron-right"></i></span>',
            last: '<span class="image-class-last"><i class="bi bi-chevron-double-right"></i></span>'
        }
    },
    drawCallback: function (settings) {
        var api = this.api();
        var numRows = api.rows({ search: "applied" }).count();
        if (numRows === 0) {
            $(this).closest('.dataTables_wrapper').find('.dataTables_paginate').hide();
        } else {
            $(this).closest('.dataTables_wrapper').find('.dataTables_paginate').show();
        }
    }
});

$('#searchStory').on('keyup', function () {
    storyTable.search($(this).val()).draw();
})



