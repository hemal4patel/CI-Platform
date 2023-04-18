
//get cities from country
$('.countryList').on('change', function () {
    var countryId = $(this).val();
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
            $('.addEditUser').text('Edit User')
        },
        error: function (error) {
            console.log(error)
        }
    });
});

//delete user
$('.deleteUser').on('click', function () {
    var userId = $(this).closest('tr').attr('id')
    var row = $(this).closest('tr')
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: "/Admin/DeleteUser",
                data: { userId: userId },
                success: function () {
                    row.remove();
                    swal.fire({
                        position: 'center',
                        icon: 'success',
                        title: 'User deleted successfully!!!',
                        showConfirmButton: false,
                        timer: 3000
                    })
                    setTimeout(function () {
                        location.reload();
                    }, 1000);
                },
                error: function (error) {
                    console.log(error)
                }
            });
        }
    })
});






//add mission
$('.addMission').on('click', function () {
    $.ajax({
        type: 'GET',
        url: '/Admin/AddMission',
        success: function (data) {
            var container = $('.adminMissionContainer');
            container.empty();
            container.append(data);
        },
        error: function (error) {
            console.log(error)
        }
    });
})

//mission type
$('#missionType').on('change', function () {
    var type = $(this).val();

    if (type === "Time") {
        $('#totalSeats').attr('disabled', false)
        $('#goalObjectiveText').attr('disabled', true)
        $('#goalValue').attr('disabled', true)
    }
    else {
        $('#goalObjectiveText').attr('disabled', false)
        $('#goalValue').attr('disabled', false)
        $('#totalSeats').attr('disabled', true)
    }
})

//checked skills in input
$('.allSkills').on('click', function () {
    var selectedSkillsArray = [];
    $('.allSkills input[type="checkbox"]:checked').each(function () {
        selectedSkillsArray.push($(this).val());
    })
    $('#selectedSkills').val(selectedSkillsArray.join())
    console.log($('#selectedSkills').val())
})

//delete mission
$('.deleteMission').on('click', function () {
    var missionId = $(this).closest('tr').attr('id')
    var row = $(this).closest('tr')
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: "/Admin/DeleteMission",
                data: { missionId: missionId },
                success: function () {
                    row.remove();
                    swal.fire({
                        position: 'center',
                        icon: 'success',
                        title: 'Mission deleted successfully!!!',
                        showConfirmButton: false,
                        timer: 3000
                    })
                    setTimeout(function () {
                        location.reload();
                    }, 1000);
                },
                error: function (error) {
                    console.log(error)
                }
            });
        }
    })
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

//edit cms
$('.editCms').on('click', function () {
    var cmsId = $(this).closest('tr').attr('id');
    console.log(cmsId)
    $.ajax({
        type: 'GET',
        url: "/Admin/EditCms",
        data: { cmsId: cmsId },
        success: function (data) {
            var container = $('.adminCmsContainer');
            container.empty();
            container.append(data);
            $('.addEditCms').text('Edit Cms Page')
        },
        error: function (error) {
            console.log(error)
        }
    });
});

//delete cms
$('.deleteCms').on('click', function () {
    var cmsId = $(this).closest('tr').attr('id')
    var row = $(this).closest('tr')
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: "/Admin/DeleteCmsPage",
                data: { cmsId: cmsId },
                success: function () {
                    row.remove();
                    swal.fire({
                        position: 'center',
                        icon: 'success',
                        title: 'Cms Page deleted successfully!!!',
                        showConfirmButton: false,
                        timer: 3000
                    })
                    setTimeout(function () {
                        location.reload();
                    }, 1000);
                },
                error: function (error) {
                    console.log(error)
                }
            });
        }
    })
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
            $('.addEditTheme').text('Edit Theme')
        },
        error: function (error) {
            console.log(error)
        }
    });
});

//delete theme
$('.deleteTheme').on('click', function () {
    var themeId = $(this).closest('tr').attr('id')
    var row = $(this).closest('tr')
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: "/Admin/DeleteTheme",
                data: { themeId: themeId },
                success: function () {
                    row.remove();
                    swal.fire({
                        position: 'center',
                        icon: 'success',
                        title: 'Theme deleted successfully!!!',
                        showConfirmButton: false,
                        timer: 3000
                    })
                    setTimeout(function () {
                        location.reload();
                    }, 1000);
                },
                error: function (error) {
                    console.log(error)
                }
            });
        }
    })
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
            $('.addEditSkill').text('Edit Skill')
        },
        error: function (error) {
            console.log(error)
        }
    });
});

//change application status
$(document).on('click', '.changeApplicationStatus', function () {
    console.log('called')
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

// delete skill
$('.deleteSkill').on('click', function () {
    var skillId = $(this).closest('tr').attr('id')
    var row = $(this).closest('tr')
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: "/Admin/DeleteSkill",
                data: { skillId: skillId },
                success: function () {
                    row.remove();
                    swal.fire({
                        position: 'center',
                        icon: 'success',
                        title: 'Skill deleted successfully!!!',
                        showConfirmButton: false,
                        timer: 3000
                    })
                    setTimeout(function () {
                        location.reload();
                    }, 1000);
                },
                error: function (error) {
                    console.log(error)
                }
            });
        }
    })
});







//change story status
$(document).on('click', '.changeStoryStatus', function () {
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

$(document).on('click', '.storyButtons button', function () {
    var status = $(this).data('value')
    console.log(status)
    var container = $('.storyButtons');
    container.empty();
    if (status == 0) {

        container.html('<button type="button" class="btn btn-outline-success storyButtons" data-value="1"><i class="bi bi-check-circle me-2"></i>Approve</button><button type="button" class="btn btn-danger" data-value="0"><i class="bi bi-x-circle me-2"></i>Declined</button><button type="button" class="btn btn-outline-dark"><i class="bi bi-trash3 me-2"></i>Delete</button>');
    }
    else {
        container.html('<button type="button" class="btn btn-success" data-value="1"><i class="bi bi-check-circle me-2"></i>Approved</button><button type="button" class="btn btn-outline-danger storyButtons" data-value="0"><i class="bi bi-x-circle me-2"></i>Decline</button><button type="button" class="btn btn-outline-dark"><i class="bi bi-trash3 me-2"></i>Delete</button>');
    }

})

//view story details
$('.viewStory').click(function () {
    var storyId = $(this).closest('tr').attr('id');
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

// delete story
$('.deleteStory').on('click', function () {
    var storyId = $(this).closest('tr').attr('id')
    var row = $(this).closest('tr')
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: "/Admin/DeleteStory",
                data: { storyId: storyId },
                success: function () {
                    row.remove();
                    swal.fire({
                        position: 'center',
                        icon: 'success',
                        title: 'Story deleted successfully!!!',
                        showConfirmButton: false,
                        timer: 3000
                    })
                    setTimeout(function () {
                        location.reload();
                    }, 1000);
                },
                error: function (error) {
                    console.log(error)
                }
            });
        }
    })
});







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



