
var selectedCountry = null;
var selectedSortCase = null;
var currentUrl = window.location.href;
let allDropdowns = $('.dropdown ul');

if (currentUrl.includes("PlatformLanding")) {
    showMissions(1);
}
else if (currentUrl.includes("StoryListing")) {
    showStories(1);
}

var currVolPage = 1;
showRecentVounteers(1);

function showRecentVounteers(currVolPage) {
    var missionId = $('.missionId').text();
    $.ajax({
        type: 'GET',
        url: '/Mission/showRecentVounteers',
        data: { currVolPage: currVolPage, missionId: missionId },
        success: function (data) {

            var recentVols = $('.recentVolunteersDiv');
            recentVols.empty();
            $('.volText').text('');

            recentVols.append(data);

            var start = ((currVolPage - 1) * 2) + 1;
            var total = $('#volCount').val();
            var end = Math.min(currVolPage * 2, total);

            var text = start + ' - ' + end + ' of ' + total + ' Recent Volunteers';
            $('.volText').text(text);

            $('.volPagination button').click(function () {

                var totalVolPages = Math.ceil($('#volCount').val() / 2);

                if (totalVolPages != 1) {
                    if ($(this).hasClass('next')) {
                        currVolPage = currVolPage + 1;
                        if (currVolPage <= totalVolPages) {
                            showRecentVounteers(currVolPage);
                        }
                    }
                    if ($(this).hasClass('prev')) {
                        currVolPage = currVolPage - 1;
                        if (currVolPage > 0) {
                            showRecentVounteers(currVolPage);
                        }
                    }
                }
            });
        },
        error: function (error) {
            console.log(error);
        }
    });
}


$('#searchText').on('keyup', function () {
    if (currentUrl.includes("PlatformLanding")) {
        showMissions();
    }
    else if (currentUrl.includes("StoryListing")) {
        showStories();
    }
});

allDropdowns.on('change', function () {
    if (currentUrl.includes("PlatformLanding")) {
        showMissions();
    }
    else if (currentUrl.includes("StoryListing")) {
        showStories();
    }
});

function showMissions(pageNo) {
    var CountryId = selectedCountry;
    var CityId = $('#CityList input[type="checkbox"]:checked').map(function () { return $(this).val(); }).get();
    var ThemeId = $('#ThemeList input[type="checkbox"]:checked').map(function () { return $(this).val(); }).get();
    var SkillId = $('#SkillList input[type="checkbox"]:checked').map(function () { return $(this).val(); }).get();
    var searchText = $("#searchText").val().toLowerCase().replace(" ", "");;
    var sortCase = selectedSortCase;
    var pagesize = 1;
    var pageNo = pageNo;

    $.ajax({
        type: 'POST',
        url: '/Mission/PlatformLanding',
        data: { CountryId: CountryId, CityId: CityId, ThemeId: ThemeId, SkillId: SkillId, searchText: searchText, sortCase: sortCase, UserId: UserId, pageNo: pageNo, pagesize: pagesize },
        success: function (data) {
            var view = $(".partialViews");
            view.empty();
            view.append(data);
            search();
            totalMission();

            if (document.getElementById('missionCount') != null) {
                var totalRecords = document.getElementById('missionCount').innerText;
            }
            let totalPages = Math.ceil(totalRecords / pagesize);


            let paginationHTML = `
              <li class="page-item">
                <a class="pagination-link first-page" aria-label="Previous">
                  <span aria-hidden="true"><img src="/images/previous.png" /></span>
                </a>
              </li>
              <li class="page-item">
                <a class="pagination-link previous-page" aria-label="Previous">
                  <span aria-hidden="true"><img src="/images/left.png" /></span>
                </a>
              </li>`;

            for (let i = 1; i <= totalPages; i++) {
                let activeClass = '';
                if (i === (pageNo === undefined ? 1 : pageNo)) {
                    activeClass = ' active';
                }
                paginationHTML += `
                <li class="page-item ${activeClass}">
                    <a class="pagination-link" data-page="${i}">${i}</a>
                </li>`;
            }

            paginationHTML += `
              <li class="page-item">
                <a class="pagination-link next-page" aria-label="Next">
                  <span aria-hidden="true"><img src="/images/right-arrow1.png" /></span>
                </a>
              </li>
              <li class="page-item">
                <a class="pagination-link last-page" aria-label="Next">
                  <span aria-hidden="true"><img src="/images/next.png" /></span>
                </a>
              </li>`;

            $('#pagination-container').empty()
            $('#pagination-container').append(paginationHTML)
            $('#pagination-container').parent().parent().show();

            if (totalPages <= 1) {
                $('#pagination-container').parent().parent().hide();
            }

            // pagination
            let currentPage;

            $(document).on('click', '.pagination li', (function () {
                $('.pagination li').each(function () {
                    if ($(this).hasClass('active')) {

                        currentPage = $(this).find('a').data('page');
                        $(this).removeClass('active');
                    }
                })
                pageNo = currentPage;
                if ($(this).find('a').hasClass('first-page')) {
                    pageNo = 1;
                    currentPage = pageNo;
                    $('.pagination li').find('a').each(function () {
                        if ($(this).data('page') == 1) {
                            $(this).parent().addClass('active')
                        }
                    })
                }
                else if ($(this).find('a').hasClass('last-page')) {
                    pageNo = totalPages;
                    currentPage = pageNo;
                    $('.pagination li').find('a').each(function () {
                        if ($(this).data('page') == totalPages) {
                            $(this).parent().addClass('active')
                        }
                    })
                }
                else if ($(this).find('a').hasClass('previous-page')) {
                    if (currentPage > totalPages) {
                        pageNo = currentPage - 1;
                    }
                    $('.pagination li').find('a').each(function () {
                        if ($(this).data('page') == pageNo) {
                            $(this).parent().addClass('active')
                        }
                    })
                    currentPage = pageNo;

                } else if ($(this).find('a').hasClass('next-page')) {
                    if (currentPage < totalPages) {
                        pageNo = currentPage + 1;
                    }

                    $('.pagination li').find('a').each(function () {
                        if ($(this).data('page') == pageNo) {
                            $(this).parent().addClass('active')
                        }
                    })
                    currentPage = pageNo;

                } else {
                    $(this).addClass('active')

                    pageNo = $(this).find('a').data('page');
                    currentPage = pageNo;

                }
                showMissions(pageNo);
            }));

        },
        error: function (error) {
            console.log(error)
        }
    });
}

function showStories(pageNo) {
    var CountryId = selectedCountry;
    var CityId = $('#CityList input[type="checkbox"]:checked').map(function () { return $(this).val(); }).get();
    var ThemeId = $('#ThemeList input[type="checkbox"]:checked').map(function () { return $(this).val(); }).get();
    var SkillId = $('#SkillList input[type="checkbox"]:checked').map(function () { return $(this).val(); }).get();
    var searchText = $("#searchText").val().toLowerCase().replace(" ", "");;
    var pagesize = 3;
    var pageNo = pageNo;

    $.ajax({
        type: 'POST',
        url: '/Story/StoryListing',
        data: { CountryId: CountryId, CityId: CityId, ThemeId: ThemeId, SkillId: SkillId, searchText: searchText, pageNo: pageNo, pagesize: pagesize },

        success: function (data) {
            var view = $(".storyPartial");
            view.empty();
            view.append(data);
            totalStory();

            if (document.getElementById('storyCount') != null) {
                var totalRecords = document.getElementById('storyCount').innerText;
            }
            let totalPages = Math.ceil(totalRecords / pagesize);


            let paginationHTML = `
              <li class="page-item">
                <a class="pagination-link first-page" aria-label="Previous">
                  <span aria-hidden="true"><img src="/images/previous.png" /></span>
                </a>
              </li>
              <li class="page-item">
                <a class="pagination-link previous-page" aria-label="Previous">
                  <span aria-hidden="true"><img src="/images/left.png" /></span>
                </a>
              </li>`;

            for (let i = 1; i <= totalPages; i++) {
                let activeClass = '';
                if (i === (pageNo === undefined ? 1 : pageNo)) {
                    activeClass = ' active';
                }
                paginationHTML += `
                <li class="page-item ${activeClass}">
                    <a class="pagination-link" data-page="${i}">${i}</a>
                </li>`;
            }

            paginationHTML += `
              <li class="page-item">
                <a class="pagination-link next-page" aria-label="Next">
                  <span aria-hidden="true"><img src="/images/right-arrow1.png" /></span>
                </a>
              </li>
              <li class="page-item">
                <a class="pagination-link last-page" aria-label="Next">
                  <span aria-hidden="true"><img src="/images/next.png" /></span>
                </a>
              </li>`;

            $('#pagination-container').empty()
            $('#pagination-container').append(paginationHTML)
            $('#pagination-container').parent().parent().show();

            if (totalPages <= 1) {
                $('#pagination-container').parent().parent().hide();
            }

            // pagination
            let currentPage;

            $(document).on('click', '.pagination li', (function () {
                $('.pagination li').each(function () {
                    if ($(this).hasClass('active')) {

                        currentPage = $(this).find('a').data('page');
                        $(this).removeClass('active');
                    }
                })
                pageNo = currentPage;
                if ($(this).find('a').hasClass('first-page')) {
                    pageNo = 1;
                    currentPage = pageNo;
                    $('.pagination li').find('a').each(function () {
                        if ($(this).data('page') == 1) {
                            $(this).parent().addClass('active')
                        }
                    })
                }
                else if ($(this).find('a').hasClass('last-page')) {
                    pageNo = totalPages;
                    currentPage = pageNo;
                    $('.pagination li').find('a').each(function () {
                        if ($(this).data('page') == totalPages) {
                            $(this).parent().addClass('active')
                        }
                    })
                }
                else if ($(this).find('a').hasClass('previous-page')) {
                    if (currentPage > totalPages) {
                        pageNo = currentPage - 1;
                    }
                    $('.pagination li').find('a').each(function () {
                        if ($(this).data('page') == pageNo) {
                            $(this).parent().addClass('active')
                        }
                    })
                    currentPage = pageNo;

                } else if ($(this).find('a').hasClass('next-page')) {
                    if (currentPage < totalPages) {
                        pageNo = currentPage + 1;
                    }

                    $('.pagination li').find('a').each(function () {
                        if ($(this).data('page') == pageNo) {
                            $(this).parent().addClass('active')
                        }
                    })
                    currentPage = pageNo;

                } else {
                    $(this).addClass('active')

                    pageNo = $(this).find('a').data('page');
                    currentPage = pageNo;

                }
                showStories(pageNo);
            }));
        },
        error: function (error) {
            console.log(error)
        }
    });
}

function totalMission() {
    if (document.getElementById('missionCount') != null) {

        var count = document.getElementById('missionCount').innerText;
        $('#exploreText').text("Explore " + count + " missions");

        if (count == 0) {
            $('.NoMissionFound').show();
        }
        else {
            $('.NoMissionFound').hide();
        }
    }

}

function totalStory() {
    if (document.getElementById('storyCount') != null) {

        var count = document.getElementById('storyCount').innerText;

        if (count == 0) {
            $('.NoStoryFound').show();
        }
        else {
            $('.NoStoryFound').hide();
        }
    }
}

$("#sortList li").click(function () {
    selectedSortCase = $(this).val();
    showMissions();
});

$("#CountryList li").click(function () {
    allDropdowns.find('input[type="checkbox"]').prop('checked', false);
    $('.filter-pills').empty();
    var countryId = $(this).val();
    selectedCountry = countryId;
    console.log(selectedCountry);

    GetCitiesByCountry(countryId);

    if (currentUrl.includes("PlatformLanding")) {
        showMissions();
    }
    else if (currentUrl.includes("StoryListing")) {
        showStories();
    }
});

function GetCitiesByCountry(countryId) {
    $.ajax({
        type: "GET",
        url: "/Mission/GetCitiesByCountry",
        data: { countryId: countryId },
        success: function (data) {
            var dropdown = $("#CityList");
            dropdown.empty();
            var items = "";
            $(data).each(function (i, item) {
                items += `<li> <div class="dropdown-item mb-1 ms-3 form-check"> <input type="checkbox" class="form-check-input" id="exampleCheck1" value =` + item.cityId + `><label class="form-check-label" for="exampleCheck1" value=` + item.cityId + `>` + item.name + `</label></div></li>`
            })
            dropdown.html(items);

            var dropdown = $("#CityListAccordian");
            dropdown.empty();
            var items = "";
            //console.log(data);
            $(data).each(function (i, item) {
                //console.log(item);
                items += `<li> <div class="dropdown-item mb-1 form-check"> <input type="checkbox"  class="form-check-input" id="exampleCheck1" value =` + item.cityId + `><label class="form-check-label" for="exampleCheck1" value=` + item.cityId + `>` + item.name + `</label></div></li>`

            })
            dropdown.html(items);
        }
    });
}

let filterPills = $('.filter-pills');
allDropdowns.each(function () {
    let dropdown = $(this);
    $(this).on('change', 'input[type="checkbox"]', function () {

        // if the check box is checked then add it to pill
        if ($(this).is(':checked')) {
            let selectedOptionText = $(this).next('label').text();
            let selectedOptionValue = $(this).val();
            const closeAllButton = filterPills.children('.closeAll');

            // creating a new pill
            let pill = $('<div></div>').addClass('pill ');

            // adding the text to pill
            let pillText = $('<span></span>').text(selectedOptionText);
            pill.append(pillText);

            // add the close icon (bootstrap)
            let closeIcon = $('<span></span>').addClass('close').html(' x');
            pill.append(closeIcon);


            // for closing the pill when clicking on close icon
            closeIcon.click(function () {
                const pillToRemove = $(this).closest('.pill');
                pillToRemove.remove();
                // Uncheck the corresponding checkbox
                const checkboxElement = dropdown.find(`input[type="checkbox"][value="${selectedOptionValue}"]`);
                checkboxElement.prop('checked', false);
                if (currentUrl.includes("PlatformLanding")) {
                    showMissions();
                }
                else if (currentUrl.includes("StoryListing")) {
                    showStories();
                }
                if (filterPills.children('.pill').length === 1) {
                    filterPills.children('.closeAll').remove();
                }
            });

            // Add "Close All" button
            if (closeAllButton.length === 0) {
                filterPills.append('<div class=" closeAll"><span>Close All</span></div>');
                filterPills.children('.closeAll').click(function () {
                    allDropdowns.find('input[type="checkbox"]').prop('checked', false);
                    filterPills.empty();
                    if (currentUrl.includes("PlatformLanding")) {
                        showMissions();
                    }
                    else if (currentUrl.includes("StoryListing")) {
                        showStories();
                    }
                });

                //add the pill before the close icon
                filterPills.prepend(pill);

            }
            else {
                filterPills.children('.closeAll').before(pill);
            }

        }
        // if the checkbox is not checked then we have to check for its value if it is exists in the pills section then we have to remove it
        else {
            let selectedOptionText = $(this).next('label').text() + ' x';
            let selectedOptionValue = $(this).val();
            $('.pill').each(function () {
                const pillText = $(this).text();
                if (pillText === selectedOptionText) {
                    $(this).remove();
                }
            });
            if ($('.pill').length === 1) {
                $('.closeAll').remove();
            }
        }

        if (currentUrl.includes("PlatformLanding")) {
            showMissions();
        }
        else if (currentUrl.includes("StoryListing")) {
            showStories();
        }
    });
});

function addToFavourites(missionId) {
    $.ajax({
        url: '/Mission/AddToFavorites',
        type: 'POST',
        data: { missionId: missionId },
        success: function (result) {
            console.log(missionId)
            var icon = $("#" + missionId);
            var text = $(".favText");
            if (currentUrl.includes("PlatformLanding")) {
                if (icon.hasClass("bi-heart")) {
                    icon.removeClass("text-light bi-heart").addClass("text-danger bi-heart-fill");
                } else {
                    icon.removeClass("text-danger bi-heart-fill").addClass("text-light bi-heart");
                }

                var icon = $("#list-" + missionId);
                if (icon.hasClass("bi-heart")) {
                    icon.removeClass("text-light bi-heart").addClass("text-danger bi-heart-fill");
                } else {
                    icon.removeClass("text-danger bi-heart-fill").addClass("text-light bi-heart");
                }
            }
            else if (currentUrl.includes("VolunteeringMission")) {
                if (icon.hasClass("bi-heart")) {
                    icon.removeClass("text-danger bi-heart").addClass("text-danger bi-heart-fill");
                    text.empty();
                    text.append("Added to favourites");
                } else {
                    icon.removeClass("text-danger bi-heart-fill").addClass("text-danger bi-heart");
                    text.empty();
                    text.append("Add to favourites");
                }
            }

        },
        error: function (error) {
            console.log("error")
            alert("Please login!!");
            //var url = '/Home/Login';
            //window.location.href = url;
            window.location.href = "/Home/Index";
        }
    });
}

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
    console.log(comment.length);
    if (comment.length == 0) {
        $('.valComment').show();

        $('.newComment').on('input', function () {
            if ($('.newComment').val().length != 0) {
                $('.valComment').hide();
            }
        })

        return;
    }
    console.log(comment);
    $.ajax({
        type: 'POST',
        url: '/Mission/PostComment',
        data: { comment: comment, missionId: missionId },
        success: function (result) {
            $('.newComment').val('');
            swal.fire({
                position: 'top-end',
                icon: result.icon,
                title: result.message,
                showConfirmButton: false,
                timer: 3000
            })
        },
        error: function (error) {
            console.log("error");
        }
    });


});

function recommendToCoWorker(ToUserId, MissionId, FromUserId) {
    console.log(MissionId);
    $.ajax({
        type: "POST",
        url: "/Mission/MissionInvite",
        data: { ToUserId: ToUserId, MissionId: MissionId, FromUserId: FromUserId },
        success: function () {
            $('.Invited-' + ToUserId + '.Invited-' + MissionId).html(' <button class="btn btn-outline-success" data-mission-Id="@Model.MissionDetails.MissionId">Invited</button>');
        }
    });
}

$('#applyToMission').click(function () {
    var missionId = $('.missionId').text();
    console.log(missionId);
    $.ajax({
        type: "POST",
        url: "/Mission/ApplyToMission",
        data: { missionId: missionId },
        success: function (result) {
            swal.fire({
                position: 'top-end',
                icon: result.icon,
                title: result.message,
                showConfirmButton: false,
                timer: 3000
            });
            $("#applyToMission").css({
                "background": "#F88634",
                "color": "white",
                "border": "2px solid #F88634"
            });
            $("#applyToMission").removeClass('btn-apply');
            $("#applyToMission").html('Status Pending <i class="bi bi-exclamation-circle ms-2"></i>');
            $("#applyToMission").attr('disabled', true);
            $("#applyToMission").removeAttr('id');

        }
    });
});

function searchUser() {
    console.log("called");
    var text = $(".searchUserName").val().toLowerCase();
    var userNames = $('.userName');
    var parentElements = userNames.parent();
    for (var i = 0; i < userNames.length; i++) {
        var name = userNames.eq(i).text().toLowerCase().trim().replace(" ", "");
        if (name.indexOf(text) == -1) {
            parentElements.eq(i).hide();
        } else {
            parentElements.eq(i).show();
        }
    }
}

function search() {
    $('.searchUserByName').each(function () {
        $(this).keyup(function () {
            var missionId = $(this).data('value');
            console.log(missionId);

            if (localStorage.getItem('view') == 1) {
                var text = $("#searchUserByNameGrid-" + missionId).val().toLowerCase();
                console.log(text);
                var userNames = $('.userName-' + missionId);
                var parentElements = userNames.parent();
                for (var i = 0; i < userNames.length; i++) {
                    var name = userNames.eq(i).text().trim().toLowerCase().replace(" ", "");
                    if (name.indexOf(text) == -1) {
                        parentElements.eq(i).hide();
                    } else {
                        parentElements.eq(i).show();
                    }
                }
            }
            else {
                var text = $("#searchUserByNameList-" + missionId).val().toLowerCase();
                console.log(text);
                var userNames = $('.userName-' + missionId);
                var parentElements = userNames.parent();
                for (var i = 0; i < userNames.length; i++) {
                    var name = userNames.eq(i).text().trim().toLowerCase().replace(" ", "");
                    if (name.indexOf(text) == -1) {
                        parentElements.eq(i).hide();
                    } else {
                        parentElements.eq(i).show();
                    }
                }
            }
        })
    })
}

//$(".volMissionModal").on('click', function () {
//    console.log("called");
//    $(".searchUserName").val('');
//});


$('#notificationDropdown').click(function () {
    $.ajax({
        type: "GET",
        url: "/Mission/GetInvitations",
        success: function (result) {
            $('#invitesDropdown').empty();
            var items = "";
            var combinedList = $.merge(result.missionInvites, result.storyInvites);
            combinedList.sort((a, b) => (a.updatedAt > b.updatedAt) ? -1 : 1);
            console.log(combinedList.length)

            if (combinedList.length != 0) {

                $(combinedList).each(function (i, item) {
                    var url = "";
                    if (item.missionId) {
                        console.log("m");
                        url = '/Mission/VolunteeringMission?MissionId=' + item.missionId
                        items += '<li><a class="dropdown-item text-wrap" href="' + url + '"><i class="bi bi-person-circle"></i>&nbsp; ' + item.fromUser.firstName + ' ' + item.fromUser.lastName + ': Recommended this mission - <strong>' + item.mission.title + '</strong></a></li>'
                    }
                    else {
                        console.log("s");
                        url = '/Story/StoryDetail?MissionId=' + item.story.missionId + '&UserId=' + item.story.userId
                        items += '<li><a class="dropdown-item text-wrap" href="' + url + '"><i class="bi bi-person-circle"></i>&nbsp; ' + item.fromUser.firstName + ' ' + item.fromUser.lastName + ': Recommended this story - <strong>' + item.story.title + '</strong></a></li>'
                    }

                })
            }
            else {
                items = '<li class="text-center" style="font-size: 21px;">No invites</li>'
            }
            $('#invitesDropdown').html(items);
        },
        error: function (error) {
            console.log(error);
        }
    });
})
