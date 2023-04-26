

async function blobData(file) {
    const response = await fetch('/Upload/StoryPhotos/' + file.path);
    const blob = await response.blob();
    const files = new File([blob], file.path, { type: blob.type });
    allfiles.push(files);
}

$('#missionId').click(function () {

    var missionId = $(this).val();
    allfiles.splice(0, allfiles.length);
    $('#image-list').empty();


    $.ajax({
        type: "GET",
        url: "/Story/GetStory",
        data: { missionId: missionId },
        success: function (story) {
            if (story != null) {

                const date = new Date(story.createdAt);
                const yyyy = date.getFullYear();
                const mm = String(date.getMonth() + 1).padStart(2, '0');
                const dd = String(date.getDate()).padStart(2, '0');
                const formattedDate = `${yyyy}-${mm}-${dd}`;

                var urls = "";
                for (var i = 0; i < story.storyMedia.length; i++) {
                    if (story.storyMedia[i].type === "video") {
                        urls += story.storyMedia[i].path + '\n';
                    }
                    else {
                        var file = story.storyMedia[i];
                        var image = $('<img>').attr('src', '/Upload/StoryPhotos/' + story.storyMedia[i].path);
                        var closebtn = $('<span>').text('x');
                        var item = $('<div>').addClass('image-item').append(image).append(closebtn);
                        $('#image-list').append(item);


                        blobData(file)



                        closebtn.on('click', function () {
                            var index = $(this).parent().index();
                            allfiles.splice(index, 1);
                            $(this).parent().remove();
                        });

                    }
                }



                $('#storyTitle').val(story.title);
                $('#date').val(formattedDate);
                $('.note-editable').html(story.description);
                $('#videoUrls').val(urls);

                $('.valMission').hide();
                $('.valstoryTitle').hide();
                $('.valDate').hide();
                $('.valStory').hide();



                $('#saveStory').prop('disabled', false);
                $('#previewStory').prop('disabled', false);
                $('#submitStory').prop('disabled', false)

            }
            else {
                $('#storyTitle').val('');
                $('#date').val('');
                $('.note-editable').text('');
                $('#videoUrls').val('');
                $('#image-list').empty();

                $('#previewStory').prop('disabled', true);
                $('#submitStory').prop('disabled', true)
            }
        },
        error: function () {
            alert("Error getting story data.");
        }
    });




})

$('#videoUrls').on('input', function () {
    $('.valUrlCount').hide();
    $('.valUrl').hide();
})

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

function formValidation() {
    var flag = true;

    var missionId = $('#missionId').val();
    var storyTitle = $('#storyTitle').val().trim();
    var date = $('#date').val();
    var story = $('.note-editable').text().trim();
    
    if (missionId == null) {
        $('.valMission').show();
        flag = false;

        $('#missionId').on('input', function () {
            if ($('#missionId').val().length != 0) {
                $('.valMission').hide();
                flag = true;
            }
        })
    }
    if (storyTitle.length < 10 || storyTitle.length > 255) {
        $('.valstoryTitle').show();
        flag = false;

        $('#storyTitle').on('input', function () {
            if ($('#storyTitle').val().trim().length >= 10 && $('#storyTitle').val().trim().length <= 255) {
                $('.valstoryTitle').hide();
                flag = true;
            }
        })
    }
    if (date.length < 10) {
        $('.valDate').show();
        flag = false;

        $('#date').on('input', function () {
            if ($('#date').val().length != 0) {
                $('.valDate').hide();
                flag = true;
            }
        })
    }
    if (story.length < 20 || story.length > 40000) {
        $('.valStory').show();
        flag = false;

        $('.note-editable').on('input', function () {
            if ($('.note-editable').text().trim().length >= 20 && $('.note-editable').text().trim().length <= 40000) {
                $('.valStory').hide();
                flag = true;
            }
        })
    }

    var f = validateUrls();

    return (flag && f);
}

$('#saveStory').click(function () {

    if (formValidation()) {
        var formData = new FormData();

        var urls = null;
        var u = $('#videoUrls').val();
        if (u != null) {
            urls = u.split('\n');

            for (var i = 0; i < urls.length; i++) {
                formData.append("VideoUrl", urls[i]);
            }
        }
        else {
            formData.append("VideoUrl", null);
        }

        for (var i = 0; i < allfiles.length; i++) {
            formData.append("Images", allfiles[i]);
        }

        formData.append("MissionId", $('#missionId').val());
        formData.append("StoryTitle", $('#storyTitle').val().trim());
        formData.append("Date", $('#date').val());
        formData.append("StoryDescription", $('.note-editable').html());

        $.ajax({
            type: 'POST',
            url: '/Story/SaveStory',
            data: formData,
            processData: false,
            contentType: false,
            success: function (result) {
                swal.fire({
                    position: 'center',
                    icon: result.icon,
                    title: result.message,
                    showConfirmButton: false,
                    timer: 3000
                })
                if (result.published == 0) {
                    $('#previewStory').removeAttr('disabled');
                    $('#submitStory').removeAttr('disabled');
                }
                if (result.published == 1) {
                    $('#missionId').val('default');
                    $('#storyTitle').val('');
                    $('#date').val('');
                    $('.note-editable').text('');
                    $('#videoUrls').val('');
                    $('#image-list').empty();
                }
            },
            error: function (error) {
                console.log(error);
            }
        });
    }
});


$('#previewStory').click(function () {
    var missionId = $('#missionId').val();
    var userId = $('#UserId').val();

    $.ajax({
        type: 'GET',
        url: '/Story/StoryDetail',
        data: { MissionId: missionId, UserId: userId },
        success: function (result) {
            var url = '/Story/StoryDetail?MissionId=' + missionId + '&UserId=' + userId;
            window.open(url, '_blank');
        },
        error: function (error) {
            console.log(error);
        }
    });
});


$('#submitStory').click(function () {

    if (formValidation()) {
        var formData = new FormData();

        var urls = null;
        var u = $('#videoUrls').val();
        if (u != null) {
            urls = u.split('\n');

            for (var i = 0; i < urls.length; i++) {
                formData.append("VideoUrl", urls[i]);
            }
        }
        else {
            formData.append("VideoUrl", null);
        }

        for (var i = 0; i < allfiles.length; i++) {
            formData.append("Images", allfiles[i]);
        }

        formData.append("MissionId", $('#missionId').val());
        formData.append("StoryTitle", $('#storyTitle').val().trim());
        formData.append("Date", $('#date').val());
        formData.append("StoryDescription", $('.note-editable').html());

        $.ajax({
            type: 'POST',
            url: '/Story/SubmitStory',
            data: formData,
            processData: false,
            contentType: false,
            success: function (result) {
                swal.fire({
                    position: 'center',
                    icon: result.icon,
                    title: result.message,
                    showConfirmButton: false,
                    timer: 3000
                })
                $('#missionId').val('default');
                $('#storyTitle').val('');
                $('#date').val('');
                $('.note-editable').text('');
                $('#videoUrls').val('');
                $('#image-list').empty();

                $('#previewStory').prop('disabled', true);
                $('#submitStory').prop('disabled', true)
            },
            error: function (error) {
                console.log(error);
            }
        });
    }
}); 

function storyInvite(ToUserId) {
    var StoryId = $('#storyId').text();
    var storyUserId = $('#storyUserId').text();
    var storyMissionId = $('#storyMissionId').text();
    var FromUserId = $('#fromUserId').text();

    $.ajax({
        type: "POST",
        url: "/Story/StoryInvite",
        data: { ToUserId: ToUserId, StoryId: StoryId, FromUserId: FromUserId, storyUserId: storyUserId, storyMissionId: storyMissionId },
        success: function () {
            $('.Invited-' + ToUserId + '.Invited-' + StoryId).html(' <button class="btn btn-outline-success">Invited</button>');
        }
    });
}

function openMission() {
    var MissionId = $('#storyMissionId').text();
    $.ajax({
        type: "GET",
        url: "/Mission/VolunteeringMission",
        data: { MissionId: MissionId },
        success: function () {
            var url = '/Mission/VolunteeringMission?MissionId=' + MissionId;
            window.location.href = url;
        }
    });
}


// drag and drop images in share your story page
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

