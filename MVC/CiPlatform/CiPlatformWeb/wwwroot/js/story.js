
$('#missionId').click(function () {
    var missionId = $(this).val();
    $.ajax({
        type: 'GET',
        url: '/Story/GetStory',
        data: { missionId: missionId },
        success: function (result) {
            if (result != null) {

                //swal.fire({
                //    position: 'top-end',
                //    icon: 'warning',
                //    title: 'Draft story available!!!',
                //    showConfirmButton: false,
                //    timer: 3000
                //})

                $('#storyTitle').val(result.title);

                const date = new Date(result.createdAt);
                const yyyy = date.getFullYear();
                const mm = String(date.getMonth() + 1).padStart(2, '0');
                const dd = String(date.getDate()).padStart(2, '0');
                const formattedDate = `${yyyy}-${mm}-${dd}`;
                $('#date').val(formattedDate);

                $('#editor').val(result.description);
                //$('.summernote').summernote('code', result.description);
                //$("#editor.note-codable").val(result.description);
                //$("textarea.summernote").val(result.description);

                var urls = "";
                for (var i = 0; i < result.storyMedia.length; i++) {
                    if (result.storyMedia[i].type === "video") {
                        urls += result.storyMedia[i].path + "\n";
                    }
                }
                $('#videoUrls').val(urls);
            }
            else {
                $('#storyTitle').val('');
                $('#date').val('');
                $('#editor').val('');
                $('#videoUrls').val('');
                $('#image-list').val('');
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
});

$('#saveStory').click(function () {
    var urls = null;
    var u = $('#videoUrls').val();
    if (u != null) {
        urls = u.split('\n');
    }

    var input = $('#file-input');
    var images = input[0].files; // Note: use [0] to access the actual input element

    var imgUrl = [];

    for (var i = 0; i < images.length; i++) {
        var file = images[i];
        var filename = file.name;
        imgUrl.push(filename);
    }

    $.ajax({
        type: 'POST',
        url: '/Story/SaveStory',
        data: {
            MissionId: $('#missionId').val(),
            StoryTitle: $('#storyTitle').val(),
            Date: $('#date').val(),
            StoryDescription: $('#editor').val(),
            VideoUrl: urls,
            Images: imgUrl
        },
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
                $('#editor').val('');
                $('#videoUrls').val('');
                $('#image-list').val('');
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
});


$('#submitStory').click(function () {

    var urls = null;
    var u = $('#videoUrls').val();
    if (u != null) {
        urls = u.split('\n');
    }

    var input = $('#file-input');
    var images = input[0].files; // Note: use [0] to access the actual input element

    var imgUrl = [];

    for (var i = 0; i < images.length; i++) {
        var file = images[i];
        var filename = file.name;
        imgUrl.push(filename);
    }

    $.ajax({
        type: 'POST',
        url: '/Story/SubmitStory',
        data: {
            MissionId: $('#missionId').val(),
            StoryTitle: $('#storyTitle').val(),
            Date: $('#date').val(),
            StoryDescription: $('#editor').val(),
            VideoUrl: urls,
            Images: imgUrl
        },
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
            $('#editor').val('');
            $('#videoUrls').val('');
            $('#image-list').empty();
        },
        error: function (error) {
            console.log(error);
        }
    });
});


//$('#editor').summernote({
//    height: 200, // set the height of the editor
//    toolbar: [
//        // add formatting options to the toolbar
//        ['style', ['bold', 'italic', 'strikethrough', 'subscript', 'superscript', 'underline']]
//    ]
//});

// drag and drop images in share your story page

var dropzone = $('#dropzone');
var imageList = $('#image-list');

// Handle file drop event
dropzone.on('drop', function (e) {
    e.preventDefault();
    e.stopPropagation();

    // Remove dropzone highlight
    dropzone.removeClass('dragover');

    // Add dropped images to the list
    var files = e.originalEvent.dataTransfer.files;
    for (var i = 0; i < files.length; i++) {
        var file = files[i];
        var reader = new FileReader();

        // Create image preview and close icon
        reader.onload = function (e) {
            var image = $('<img>').attr('src', e.target.result);
            var closeIcon = $('<span>').addClass('close-icon').text('x');

            // Add image and close icon to the list
            var item = $('<div>').addClass('image-item').append(image).append(closeIcon);
            imageList.append(item);

            // Handle close icon click event
            closeIcon.on('click', function () {
                item.remove();
            });
        };

        // Read image file as data URL
        reader.readAsDataURL(file);
    }
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
    var files = e.target.files;

    // Add selected images to the list
    for (var i = 0; i < files.length; i++) {
        var file = files[i];
        var reader = new FileReader();

        // Create image preview and close icon
        reader.onload = function (e) {
            var image = $('<img>').attr('src', e.target.result);
            var closeIcon = $('<span>').addClass('close-icon').text('x');

            // Add image and close icon to the list
            var item = $('<div>').addClass('image-item').append(image).append(closeIcon);
            imageList.append(item);

            // Handle close icon click event
            closeIcon.on('click', function () {
                item.remove();
            });
        };

        // Read image file as data URL
        reader.readAsDataURL(file);
    }
});