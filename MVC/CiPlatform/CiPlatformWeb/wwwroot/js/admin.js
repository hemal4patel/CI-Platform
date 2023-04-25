
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
        confirmButtonColor: '#d33',
        cancelButtonColor: '#414141',
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

            var today = new Date();
            var tomorrow = new Date(today);
            tomorrow.setDate(tomorrow.getDate() + 1);
            var yyyy = tomorrow.getFullYear();
            var mm = String(tomorrow.getMonth() + 1).padStart(2, '0');
            var dd = String(tomorrow.getDate()).padStart(2, '0');
            var minDate = yyyy + '-' + mm + '-' + dd;
            var startDate = document.getElementById("startDate");
            startDate.min = minDate;
        },
        error: function (error) {
            console.log(error)
        }
    });
})

//date min max from start date
$('#startDate').on('change', function () {
    var startDate = $(this).val();

    var endDate = document.getElementById("endDate");
    endDate.min = startDate;

    var regDate = document.getElementById("registrationDeadline");
    var today = new Date();
    var yyyy = today.getFullYear();
    var mm = String(today.getMonth() + 1).padStart(2, '0');
    var dd = String(today.getDate()).padStart(2, '0');
    var minDate = yyyy + '-' + mm + '-' + dd;
    regDate.min = minDate

    var max = new Date(startDate);
    max.setDate(max.getDate() - 1);
    var y = max.getFullYear();
    var m = String(max.getMonth() + 1).padStart(2, '0');
    var d = String(max.getDate()).padStart(2, '0');
    var maxDate = y + '-' + m + '-' + d;
    regDate.max = maxDate;

})

//mission type
$('#missionType').on('change', function () {
    var type = $(this).val();

    if (type === "Time") {
        $('#totalSeats').show()
        $('#registration').show()
        $('#goalObjectiveText').hide()
        $('#goalValue').hide()
    }
    else {
        $('#goalObjectiveText').show()
        $('#goalValue').show()
        $('#totalSeats').hide()
        $('#registration').hide()
    }
})

//checked skills in input
$('.allSkills').on('click', function () {
    var selectedSkillsArray = [];
    $('.allSkills input[type="checkbox"]:checked').each(function () {
        selectedSkillsArray.push($(this).val());
    })
    $('#selectedSkills').val(selectedSkillsArray.join())
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
        confirmButtonColor: '#d33',
        cancelButtonColor: '#414141',
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

// drag and drop images in admin mission
var allfiles = [];
var fileInput = document.getElementById('file-input');
var fileList;
function handleFiles(e) {
    var files = e.target.files || e.originalEvent.dataTransfer.files;
    if (allfiles.length < 20) {

        for (var i = 0; i < files.length && allfiles.length < 20; i++) {
            var file = files[i];
            var reader = new FileReader();
            if (file.type === "image/jpeg" || file.type === "image/png") {
                allfiles.push(files[i]);

                reader.onload = (function (file) {
                    return function (e) {
                        var image = $('<img>').attr('src', e.target.result);
                        var closeIcon = $('<span>').addClass('close-icon').text('x');

                        var item = $('<div>').addClass('image-item').append(image).append(closeIcon);
                        imageList.append(item);

                        closeIcon.on('click', function () {
                            item.remove();
                            allfiles.splice(allfiles.indexOf(file), 1);
                        });
                    };
                })(file);

                reader.readAsDataURL(file);
            }
            else {
                $('.valImages').show();
            }
        }

    }
    else {
        $('.valImages').show();
    }
    var dataTransfer = new DataTransfer();
    fileList = dataTransfer.files;
}

var dropzone = $('#dropzone');
var imageList = $('#image-list');

dropzone.on('drop', function (e) {
    e.preventDefault();
    e.stopPropagation();

    dropzone.removeClass('dragover');
    $('.note-dropzone').remove();

    $('.valImages').hide();
    handleFiles(e);
});

dropzone.on('dragover', function (e) {
    e.preventDefault();
    e.stopPropagation();

    dropzone.addClass('dragover');
});

dropzone.on('dragleave', function (e) {
    e.preventDefault();
    e.stopPropagation();

    dropzone.removeClass('dragover');
});

$('#file-input').on('change', function (e) {
    $('.valImages').hide();
    handleFiles(e);
});

//submit mission form
$('#addMissionForm').on('submit', function (e) {
    e.preventDefault();

    if (!validateUrls()) {
        return;
    }

    if (allfiles.length != 0) {
        var defaultImg = $('.image-item.selected').index();
        if (defaultImg == -1) {
            $('.valDefImg').show();
            return;
        }
    }

    var editorDesc = tinymce.get('missionDescription');
    editorDesc.on('change', function () {
        var text = editorDesc.getContent().trim();
        console.log(text)
        if (text !== '') {
            $('#valDesc').hide();
        }
    });
    var description = editorDesc.getContent().trim();
    if (description === '') {
        $('#valDesc').show();
        return;
    }

    var editor = tinymce.get('organizationDetail');
    editor.on('change', function () {
        var text = editor.getContent().trim();
        console.log(text)
        if (text !== '') {
            $('#orgDetails').hide();
        }
    });
    var orgDetail = editor.getContent().trim();
    if (orgDetail.trim() === "") {
        $('#orgDetails').show();
        return;
    }

    if ($(this).valid()) {
        var formData = new FormData($(this)[0]);

        //images
        if (allfiles.length != 0) {
            for (var i = 0; i < allfiles.length; i++) {
                formData.append("images", allfiles[i]);
            }
        }

        //default image
        formData.append("defaultImage", defaultImg)

        //documents
        if (allDocs.length != 0) {
            for (var i = 0; i < allDocs.length; i++) {
                formData.append("documents", allDocs[i]);
            }
        }

        //urls
        var urls = null;
        var u = $('#videoUrls').val();
        if (u != null) {
            urls = u.split('\n');

            for (var i = 0; i < urls.length; i++) {
                formData.append("videos", urls[i]);
            }
        }
        else {
            formData.append("videos", null);
        }

        //mission description        
        formData.append("description", description);

        //organization detail        
        formData.append("orgDetail", orgDetail);

        $.ajax({
            type: 'POST',
            url: '/Admin/AdminMission',
            data: formData,
            contentType: false,
            processData: false,
            success: function (data) {
                swal.fire({
                    position: 'center',
                    icon: data.icon,
                    title: data.message,
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
        })
    }
})

//edit mission
$('.editMission').click(function () {
    var missionId = $(this).closest('tr').attr('id')

    $.ajax({
        type: 'GET',
        url: "/Admin/EditMission",
        data: { missionId: missionId },
        success: async function (data) {
            var container = $('.adminMissionContainer');
            container.empty();
            container.append(data);
            $('.addEditMission').text('Edit Mission')

            //check skills
            var skills = []
            skills = $('#selectedSkills').val().split(',')
            $('.form-check-input').each(function () {
                var skillId = $(this).val();
                if (skills.includes(skillId)) {
                    $(this).prop('checked', true);
                }
            });

            //mission type
            if ($('#missionType').val() === "Time") {
                $('#totalSeats').show()
                $('#registration').show()
                $('#goalObjectiveText').hide()
                $('#goalValue').hide()
            }
            else {
                $('#goalObjectiveText').show()
                $('#goalValue').show()
                $('#totalSeats').hide()
                $('#registration').hide()
            }

            $('#missionType').attr('disabled', true)

            //image name
            var imageNames = []
            imageNames = $('#imageName').val().split(',')
            for (var i = 0; i < imageNames.length; i++) {
                var array = (imageNames[i].split(':'))
                var name = array[0]
                var defaultVal = array[1]
                if (name != "") {
                    var image = $('<img>').attr('src', '/Upload/MissionPhotos/' + name);
                    var closebtn = $('<span>').addClass('close-icon').text('x');
                    var item = $('<div>').addClass('image-item').append(image).append(closebtn);
                    if (defaultVal == 1) {
                        const Selected = document.createElement('div');
                        Selected.className = 'default-image';
                        Selected.innerHTML = '<i class="bi bi-check-circle-fill"></i>';

                        item.addClass('selected');
                        item.append(Selected)
                    }
                    $('#image-list').append(item);
                    blobData(name)
                    closebtn.on('click', function () {
                        var index = $(this).parent().index();
                        allfiles.splice(index, 1);
                        $(this).parent().remove();
                    });
                }
            }

            //dates
            var todayDate = new Date();
            var startDate = $('#startDate').val();
            startDate = new Date(startDate)
            var endDate = $('#endDate').val()
            endDate = new Date(endDate)
            if (startDate > todayDate) { //not started
                var yyyy = todayDate.getFullYear();
                var mm = String(todayDate.getMonth() + 1).padStart(2, '0');
                var dd = String(todayDate.getDate()).padStart(2, '0');
                var minDate = yyyy + '-' + mm + '-' + dd;
                $('#startDate').prop('min', minDate)

                var yyyy = startDate.getFullYear();
                var mm = String(startDate.getMonth() + 1).padStart(2, '0');
                var dd = String(startDate.getDate()).padStart(2, '0');
                var minDate = yyyy + '-' + mm + '-' + dd;
                $('#endDate').prop('min', minDate)

                var yyyy = todayDate.getFullYear();
                var mm = String(todayDate.getMonth() + 1).padStart(2, '0');
                var dd = String(todayDate.getDate()).padStart(2, '0');
                var minDate = yyyy + '-' + mm + '-' + dd;
                console.log(minDate)
                $('#registrationDeadline').prop('min', minDate)

                var max = new Date(startDate);
                max.setDate(max.getDate() - 1);
                var y = max.getFullYear();
                var m = String(max.getMonth() + 1).padStart(2, '0');
                var d = String(max.getDate()).padStart(2, '0');
                var maxDate = y + '-' + m + '-' + d;
                $('#registrationDeadline').prop('max', maxDate)
            }
            else if (startDate < todayDate) { //ongoing
                $('#startDate').attr('disabled', true)
                $('#registrationDeadline').attr('disabled', true)
                if (endDate > todayDate) { //ongoing
                    var yyyy = todayDate.getFullYear();
                    var mm = String(todayDate.getMonth() + 1).padStart(2, '0');
                    var dd = String(todayDate.getDate()).padStart(2, '0');
                    var minDate = yyyy + '-' + mm + '-' + dd;
                    $('#endDate').prop('min', minDate)
                }
                else { //closed
                    $('#endDate').attr('disabled', true)
                }
            }

            //document name
            var documentNames = []
            documentNames = $('#documentName').val().split(',')
            for (var i = 0; i < documentNames.length; i++) {
                var fileName = documentNames[i];
                if (fileName != "") {
                    var docIcon = $('<i>').addClass('far fa-file-alt');
                    var docName = $('<div>').addClass('doc-name ms-3').text(fileName);
                    var closeIcon = $('<span>').addClass('close-icon ms-3').text('x');
                    var item = $('<div>').addClass('doc-item d-flex align-items-center justify-content-between me-2 mt-2').append(docIcon).append(docName).append(closeIcon);
                    docList.append(item);
                    const response = await fetch('/Upload/MissionDocuments/' + fileName);
                    const blob = await response.blob();
                    const files = new File([blob], fileName, { type: blob.type });
                    allDocs.push(files);
                    closeIcon.on('click', function () {
                        var index = $(this).parent().index();
                        allDocs.splice(index, 1);
                        $(this).parent().remove();
                    });
                }
            }

        },
        error: function (error) {
            console.log(error)
        }
    });
});

async function blobData(file) {
    const response = await fetch('/Upload/MissionPhotos/' + file);
    const blob = await response.blob();
    const files = new File([blob], file, { type: blob.type });
    allfiles.push(files);
}

//set default image
$(document).on('click', '.image-item', function () {

    const Selected = document.createElement('div');
    Selected.className = 'default-image';
    Selected.innerHTML = '<i class="bi bi-check-circle-fill"></i>';

    $(this).siblings('.image-item').addBack().each(function () {
        $(this).removeClass('selected').find('.default-image').remove();
    });

    $(this).addClass('selected')
    $(this).append(Selected)

    $('.valDefImg').hide();
})

//youtube url validation
function isYoutubeUrl(url) {
    var pattern = /^.*(youtube.com\/|youtu.be\/|\/v\/|\/e\/|u\/\w+\/|embed\/|v=)([^#\&\?]*).*/;
    return pattern.test(url);
}

function validateUrls() {
    var urls = $('#videoUrls').val().split('\n');

    if (urls.length > 20) {
        $('.valUrlCount').show();
        return false;
    }
    else {
        $('.valUrlCount').hide();
    }

    for (var i = 0; i < urls.length; i++) {
        var url = urls[i].trim();
        if (url.length > 0 && !isYoutubeUrl(url)) {
            $('.valUrl').show();
            return false;
        }
    }

    return true;
}

$('#videoUrls').on('input', function () {
    $('.valUrlCount').hide();
    $('.valUrl').hide();
})

//documents grag drop
var allDocs = [];
var docInput = document.getElementById('doc-input');

function handleDocs(e) {
    var files = e.target.files || e.originalEvent.dataTransfer.files;
    if (allDocs.length < 5) {
        for (var i = 0; i < files.length && allDocs.length < 5; i++) {
            var file = files[i];
            var reader = new FileReader();

            if (file.type === "application/pdf" ||
                file.type === "application/vnd.openxmlformats-officedocument.wordprocessingml.document" ||
                file.type === "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
                allDocs.push(files[i]);

                reader.onload = (function (file) {
                    return function (e) {
                        var fileName = file.name;
                        var docIcon = $('<i>').addClass('far fa-file-alt');
                        var docName = $('<div>').addClass('doc-name ms-3').text(fileName);
                        var closeIcon = $('<span>').addClass('close-icon ms-3').text('x');
                        var item = $('<div>').addClass('doc-item d-flex align-items-center justify-content-between me-2 mt-2').append(docIcon).append(docName).append(closeIcon);
                        docList.append(item);

                        closeIcon.on('click', function () {
                            item.remove();
                            allDocs.splice(allDocs.indexOf(file), 1);
                        });
                    };
                })(file);

                reader.readAsDataURL(file);
            }
            else {
                $('.valDocs').show();
            }
        }
    } else {
        $('.valDocs').show();
    }
}

var docsdropzone = $('#docs-dropzone');
var docList = $('#docs-list');

docsdropzone.on('drop', function (e) {
    e.preventDefault();
    e.stopPropagation();

    docsdropzone.removeClass('dragover');
    $('.note-dropzone').remove();

    $('.valDocs').hide();
    handleDocs(e);
});

docsdropzone.on('dragover', function (e) {
    e.preventDefault();
    e.stopPropagation();

    docsdropzone.addClass('dragover');
});

docsdropzone.on('dragleave', function (e) {
    e.preventDefault();
    e.stopPropagation();

    docsdropzone.removeClass('dragover');
});

$('#doc-input').on('change', function (e) {
    $('.valDocs').hide();
    console.log(allDocs)
    handleDocs(e);
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
        confirmButtonColor: '#d33',
        cancelButtonColor: '#414141',
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
        confirmButtonColor: '#d33',
        cancelButtonColor: '#414141',
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

// delete skill
$('.deleteSkill').on('click', function () {
    var skillId = $(this).closest('tr').attr('id')
    var row = $(this).closest('tr')
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#414141',
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





//change application status
$(document).on('click', '.changeApplicationStatus', function () {
    var applicationId = $(this).closest('tr').attr('id');
    var row = $(this).closest('tr')
    var status = $(this).data('value')
    var s = "";
    var confirmButtonColor = ''
    if (status == 1) {
        s = "approve"
        confirmButtonColor = '#198754'
    }
    else {
        s = "decline"
        confirmButtonColor = '#d33'
    }

    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: confirmButtonColor,
        cancelButtonColor: '#414141',
        confirmButtonText: 'Yes, ' + s + ' it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: "/Admin/ChangeApplicationStatus",
                data: { applicationId: applicationId, status: status },
                success: function () {
                    row.remove();
                    swal.fire({
                        position: 'center',
                        icon: 'success',
                        title: 'Application ' + s + 'd successfully!!!',
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
})






//change story status
$(document).on('click', '.changeStoryStatus', function () {
    var storyId = $(this).closest('tr').attr('id');
    var row = $(this).closest('tr')
    var status = $(this).data('value')

    var s = "";
    var confirmButtonColor = ''
    if (status == 1) {
        s = "approve"
        confirmButtonColor = '#198754'
    }
    else {
        s = "decline"
        confirmButtonColor = '#d33'
    }

    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: confirmButtonColor,
        cancelButtonColor: '#414141',
        confirmButtonText: 'Yes, ' + s + ' it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: "/Admin/ChangeStoryStatus",
                data: { storyId: storyId, status: status },
                success: function () {
                    row.remove();
                    swal.fire({
                        position: 'center',
                        icon: 'success',
                        title: 'Story ' + s + 'd successfully!!!',
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

    //$.ajax({
    //    type: "POST",
    //    url: "/Admin/ChangeStoryStatus",
    //    data: { storyId: storyId, status: status },
    //    success: function () {
    //        var container = $('.showStoryButtons-' + storyId);
    //        container.empty();
    //        if (status == 0) {
    //            container.html('<i class="bi bi-check-circle ms-2 changeStoryStatus" data-value="1" style="color: #14C506;"></i><i class="bi bi-x-circle-fill ms-2" data-value="0" style="color: #f20707;"></i>');
    //        }
    //        else {
    //            container.html('<i class="bi bi-check-circle-fill ms-2" data-value="1" style="color: #14C506;"></i><i class="bi bi-x-circle ms-2 changeStoryStatus" data-value="0" style="color: #f20707;"></i>');
    //        }
    //    },
    //    error: function (error) {
    //        console.log(error)
    //    }
    //});
})

$(document).on('click', '.storyStatusButtons', function () {
    var status = $(this).data('value')
    var storyId = $('#storyId').val()

    var s = "";
    var confirmButtonColor = ''
    if (status == 1) {
        s = "approve"
        confirmButtonColor = '#198754'
    }
    else {
        s = "decline"
        confirmButtonColor = '#d33'
    }

    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: confirmButtonColor,
        cancelButtonColor: '#414141',
        confirmButtonText: 'Yes, ' + s + ' it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: "/Admin/ChangeStoryStatus",
                data: { storyId: storyId, status: status },
                success: function () {
                    swal.fire({
                        position: 'center',
                        icon: 'success',
                        title: 'Story ' + s + 'd successfully!!!',
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
        confirmButtonColor: '#d33',
        cancelButtonColor: '#414141',
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

$(document).on('click', '.delStory', function () {
    var storyId = $('#storyId').val()

    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#414141',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: "/Admin/DeleteStory",
                data: { storyId: storyId },
                success: function () {
                    location.reload();
                    swal.fire({
                        position: 'center',
                        icon: 'success',
                        title: 'Story deleted successfully!!!',
                        showConfirmButton: false,
                        timer: 3000
                    });
                },
                error: function (error) {
                    console.log(error)
                }
            });
        }
    })
})







//add banner
$('#addBanner').on('click', function () {

    $.ajax({
        type: 'GET',
        url: '/Admin/AddBanner',
        success: function (data) {
            var container = $('.adminBannerContainer');
            container.empty();
            container.append(data);
        },
        error: function (error) {
            console.log(error)
        }
    });
})

//display selected banner image
$('#bannerImage').on('change', function (e) {
    $('#valBanner').hide();
    var imagePreview = $('#imagePreview')

    var uploadedFile = e.target.files[0];
    var imageUrl = URL.createObjectURL(uploadedFile);
    var imagePreview = $('#imagePreview')
    imagePreview.empty();
    var image = $('<img>').attr('src', imageUrl);
    var item = $('<div>').addClass('image-banner').append(image);
    imagePreview.append(item);
})

//edit banner
$('.editBanner').on('click', function () {
    var bannerId = $(this).closest('tr').attr('id');

    $.ajax({
        type: 'GET',
        url: "/Admin/EditBanner",
        data: { bannerId: bannerId },
        success: function (data) {
            var container = $('.adminBannerContainer');
            container.empty();
            container.append(data);
            $('.addEditBanner').text('Edit Banner')

            var imageName = $('#imageName').val()
            var imagePreview = $('#imagePreview')
            imagePreview.empty();
            var image = $('<img>').attr('src', '/Upload/Banner/' + imageName);
            var item = $('<div>').addClass('image-banner').append(image);
            imagePreview.append(item);
        },
        error: function (error) {
            console.log(error)
        }
    });
})

//submit banner form
$('#addBannerForm').on('submit', function (e) {
    e.preventDefault();

    var image = $('#imagePreview').find('img').attr('src');
    if (image == null) {
        $('#valBanner').show();
        return false;
    }

    if ($(this).valid()) {
        var formData = new FormData($(this)[0]);

        $.ajax({
            type: 'POST',
            url: '/Admin/AdminBanner',
            data: formData,
            contentType: false,
            processData: false,
            success: function (data) {
                swal.fire({
                    position: 'center',
                    icon: data.icon,
                    title: data.message,
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
        })
    }
})

// delete banner
$('.deleteBanner').on('click', function () {
    var bannerId = $(this).closest('tr').attr('id')
    var row = $(this).closest('tr')
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#414141',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: "/Admin/DeleteBanner",
                data: { bannerId: bannerId },
                success: function () {
                    row.remove();
                    swal.fire({
                        position: 'center',
                        icon: 'success',
                        title: 'Banner deleted successfully!!!',
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







//cancel button
$(document).on('click', '.cancelButton', function () {
    console.log('called')
    window.location.reload()
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

//Admin banner table
var bannerTable = $('#bannerTable').DataTable({
    lengthChange: false,
    ordering: false,
    paging: true,
    searching: true,
    pageLength: 3,
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

$('#searchBanner').on('keyup', function () {
    bannerTable.search($(this).val()).draw();
})


//toggle
$(document).ready(function () {

    var location = window.location.href;
    if (location.includes('AdminUser')) {
        $("a[href='/Admin/AdminUser']").addClass("admin-nav-active");
    }

    else if (location.includes('AdminCms')) {
        $("a[href='/Admin/AdminCms']").addClass("admin-nav-active");
    }

    else if (location.includes('AdminMission')) {
        $("a[href='/Admin/AdminMission']").addClass("admin-nav-active");
    }

    else if (location.includes('AdminTheme')) {
        $("a[href='/Admin/AdminTheme']").addClass("admin-nav-active");
    }

    else if (location.includes('AdminSkill')) {
        $("a[href='/Admin/AdminSkill']").addClass("admin-nav-active");
    }

    else if (location.includes('AdminApplication')) {
        $("a[href='/Admin/AdminApplication']").addClass("admin-nav-active");
    }

    else if (location.includes('AdminStory')) {
        $("a[href='/Admin/AdminStory']").addClass("admin-nav-active");
    }
    else {
        $("a[href='/Admin/AdminBanner']").addClass("admin-nav-active");
    }
})

