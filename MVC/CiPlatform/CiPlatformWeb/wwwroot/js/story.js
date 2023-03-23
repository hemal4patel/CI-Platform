
$('#missionId').click(function () {
    var missionId = $(this).val();
    $.ajax({
        type: 'GET',
        url: '/Story/GetStory',
        data: { missionId: missionId },
        success: function (result) {
            if (result != null) {
                console.log(result.storyId);
                $('#storyTitle').val(result.title);

                const date = new Date(result.createdAt);
                const yyyy = date.getFullYear();
                const mm = String(date.getMonth() + 1).padStart(2, '0');
                const dd = String(date.getDate()).padStart(2, '0');
                const formattedDate = `${yyyy}-${mm}-${dd}`;

                $('#date').val(formattedDate);
                $('#editor').val(result.description);
                $('#storyId').val(result.storyId);
            }
            else {
                $('#storyId').val('');
                console.log($('#storyId').val());
            }
        },
        error: function (error) {
            console.log(error);
        }
    });
});

$('#saveStory').click(function () {
    $.ajax({
        type: 'POST',
        url: '/Story/SaveStory',
        data: {
            MissionId: $('#missionId').val(),
            StoryTitle: $('#storyTitle').val(),
            Date: $('#date').val(),
            StoryDescription: $('#editor').val(),
            storyId: $('#storyId').val()
        },
        success: function (result) {
            alert(result.message);
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