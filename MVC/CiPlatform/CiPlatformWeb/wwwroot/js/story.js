﻿

$('#missionId').click(function () {
    var missionId = $(this).val();

    $.ajax({
        type: 'GET',
        url: '/Story/GetStory',
        data: { missionId: missionId },
        success: async function (result) {
            if (result != null) {

                $('#storyTitle').val(result.title);

                const date = new Date(result.createdAt);
                const yyyy = date.getFullYear();
                const mm = String(date.getMonth() + 1).padStart(2, '0');
                const dd = String(date.getDate()).padStart(2, '0');
                const formattedDate = `${yyyy}-${mm}-${dd}`;
                $('#date').val(formattedDate);

                $('.note-editable').html(result.description);
                $('#image-list').empty();

                var urls = "";
                for (var i = 0; i < result.storyMedia.length; i++) {
                    if (result.storyMedia[i].type === "video") {
                        urls += result.storyMedia[i].path + "\n";
                    }
                    else {
                        var file = result.storyMedia[i];
                        var image = $('<img>').attr('src', '/Upload/StoryPhotos/' + result.storyMedia[i].path);
                        var closebtn = $('<span>').text('x');
                        var item = $('<div>').addClass('image-item').append(image).append(closebtn);
                        $('#image-list').append(item);

                        const response = await fetch('/Upload/StoryPhotos/' + file.path);
                        const blob = await response.blob();
                        const files = new File([blob], file.path, { type: blob.type });

                        allfiles.push(files);

                        closebtn.on('click', function () {
                            var index = $(this).parent().index();
                            allfiles.splice(index, 1);
                            $(this).parent().remove();
                            console.log(allfiles);
                        });

                    }
                }

                $('#videoUrls').val(urls);

                $('.valMission').hide();
                $('.valstoryTitle').hide();
                $('.valDate').hide();
                $('.valStory').hide();
            }
            else {
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
});

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
    var storyTitle = $('#storyTitle').val();
    var date = $('#date').val();
    var story = $('.note-editable').text();

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
    if (storyTitle.length == 0) {
        $('.valstoryTitle').show();
        flag = false;

        $('#storyTitle').on('input', function () {
            if ($('#storyTitle').val().length != 0) {
                $('.valstoryTitle').hide();
                flag = true;
            }
        })
    }
    if (date.length == 0) {
        $('.valDate').show();
        flag = false;

        $('#date').on('input', function () {
            if ($('#date').val().length != 0) {
                $('.valDate').hide();
                flag = true;
            }
        })
    }
    if (story.length == 0) {
        $('.valStory').show();
        flag = false;

        $('.note-editable').on('input', function () {
            if ($('.note-editable').text() != null) {
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
        formData.append("StoryTitle", $('#storyTitle').val());
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
                    position: 'top-end',
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
            console.log(result);
            var url = '/Story/StoryDetail?MissionId=' + missionId + '&UserId=' + userId;
            window.location.href = url;
        },
        error: function (error) {
            console.log(error);
        }
    });
});


$('#submitStory').click(function () {

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
    formData.append("StoryTitle", $('#storyTitle').val());
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
                position: 'top-end',
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
        },
        error: function (error) {
            console.log(error);
        }
    });
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
            console.log("sent");
            $('.Invited-' + ToUserId + '.Invited-' + StoryId).html(' <button class="btn btn-outline-success">Invited</button>');
        }
    });
}

function openMission() {
    console.log("hello")
    var MissionId = $('#storyMissionId').text();
    console.log(storyMissionId)
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

    // Add dropped images or selected images to the list
    var files = e.target.files || e.originalEvent.dataTransfer.files;

    // Add selected images to the list
    for (var i = 0; i < files.length; i++) {
        var file = files[i];
        var reader = new FileReader();
        allfiles.push(files[i]);
        //formData.append('file', file);

        // Create image preview and close icon
        // Create image preview and close icon
        reader.onload = (function (file) {
            return function (e) {
                var image = $('<img>').attr('src', e.target.result);
                var closeIcon = $('<span>').addClass('close-icon').text('x');

                // Add image and close icon to the list
                var item = $('<div>').addClass('image-item').append(image).append(closeIcon);
                imageList.append(item);

                // Handle close icon click event
                closeIcon.on('click', function () {
                    item.remove();
                    allfiles.splice(allfiles.indexOf(file), 1);


                    console.log(allfiles);
                });
            };
        })(file);

        // Read image file as data URL
        reader.readAsDataURL(file);
    }
    // Create a new DataTransfer object
    var dataTransfer = new DataTransfer();
    // Create a new FileList object from the DataTransfer object
    fileList = dataTransfer.files;
}

//var allfiles = new DataTransfer().files;
var dropzone = $('#dropzone');
var imageList = $('#image-list');

// Handle file drop event
dropzone.on('drop', function (e) {
    e.preventDefault();
    e.stopPropagation();

    // Remove dropzone highlight
    dropzone.removeClass('dragover');
    $('.note-dropzone').remove();
    //$('.note-dropzone-message').remove();
    handleFiles(e);
});

// Handle file dragover event
dropzone.on('dragover', function (e) {
    e.preventDefault();
    e.stopPropagation();

    // Highlight dropzone
    dropzone.addClass('dragover');
});

// Handle file dragleave event
dropzone.on('dragleave', function (e) {
    e.preventDefault();
    e.stopPropagation();

    // Remove dropzone highlight
    dropzone.removeClass('dragover');
});


// Handle file input change event
$('#file-input').on('change', function (e) {
    handleFiles(e);
});

