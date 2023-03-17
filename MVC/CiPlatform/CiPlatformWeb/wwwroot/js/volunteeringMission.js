

//$(document).on('click', '.btn-apply', function () {
//    var missionId = $(this).data('mission-id');
//    console.log(missionId);
//    $.ajax({
//        url: '/Mission/VolunteeringMission',
//        type: 'POST',
//        data: { missionId: missionId },
//        success: function (data) {
//            console.log(missionId);
//        },
//        error: function (error) {
//            console.log(error);
//        }
//    });
//});


$('.rateMission i').click(function () {
    var rating = $(this).index() + 1;
    var missionId = $(this).data('mission-id');
    var selectedIcon = $(this).prevAll().addBack();
    var unselectedIcon = $(this).nextAll();

    $.ajax({
        url: '/Mission/RateMission',
        type: 'POST',
        data: { rating: rating, missionId: missionId },
        success: function () {
            console.log("done");
            selectedIcon.removeClass('bi-star').addClass('bi-star-fill text-warning');
            unselectedIcon.removeClass('bi-star-fill text-warning').addClass('bi-star');
        },
        error: function () {
            console.log("error");
        }
    });
});

$('.commentButton').click(function () {
    var comment = $('.newComment').val();
    var missionId = $(this).data('mission-id');
    if (comment != null) {
        console.log(comment);
        $.ajax({
            type: 'POST',
            url: '/Mission/PostComment',
            data: { comment: comment, missionId: missionId },
            success: function () {
                $('.newComment').val('');
            },
            error: function (error) {
                console.log("error");
            }
        });
    }
    else {
        console.log("null");
    }
});


