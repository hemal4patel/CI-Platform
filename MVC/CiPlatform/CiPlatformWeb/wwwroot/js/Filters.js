
var selectedCountry = null;
var selectedSortCase = null;
var currentUrl = window.location.href;
let allDropdowns = $('.dropdown ul');
var selectedExploreOption = null
var currVolPage = 1;

if (currentUrl.includes("PlatformLanding")) {
    showMissions(1);
}
else if (currentUrl.includes("StoryListing")) {
    showStories(1);
}

showRecentVounteers(1);
showComments();
showNotification();




//display notification dropdown content
function showNotification() {
    $.ajax({
        type: 'POST',
        url: '/Notification/GetAllNotifications',
        success: function (data) {
            $('.userNotificationsPartial').empty()
            $('.userNotificationsPartial').append(data)
            $('#notificationUl').show()
            $('#settingsUl').hide()

            $('.notificationCount').text($('#UnreadCount').val())
            if ($('#UnreadCount').val() == 0) {
                $('.clearAllBtn').hide()
            }

        },
        error: function (error) {
            console.log(error)
        }
    });
}

$('#notificationDropdown').on('click', function () {
    showNotification();
})

//mark notification as read
$(document).on('click', '#notificationDropdownList li', function () {
    var id = $(this).attr('id');

    $.ajax({
        type: 'POST',
        url: '/Notification/ChangeNotificationStatus',
        data: { id: id },
        success: function () {
            showNotification();
        },
        error: function (error) {
            console.log(error)
        }
    });
});

//clear all notifications
$(document).on('click', '.clearAllNotifications', function () {

    $.ajax({
        type: 'POST',
        url: '/Notification/ClearAllNotifications',
        success: function () {
            showNotification();
        },
        error: function (error) {
            console.log(error)
        }
    });
})

//show notification settings
$(document).on('click', '.notificationSettings', function () {

    $('#notificationUl').hide()
    $('#settingsUl').show()

    $.ajax({
        type: 'GET',
        url: '/Notification/GetUserNotificationChanges',
        success: function (data) {
            $('.allSettingsDiv input[type="checkbox"]').prop('checked', false);
            data.forEach(function (id) {
                $('.allSettingsDiv input[type="checkbox"][value="' + id + '"]').prop('checked', true);
            });
        },
        error: function (error) {
            console.log(error)
        }
    });
});

//go back to notifications
$(document).on('click', '.cancelNotification', function () {
    showNotification()
})

//save notification settings for user
$(document).on('click', '.saveNotification', function () {
    var settingIds = $('.allSettingsDiv input[type="checkbox"]:checked').map(function () {
        return $(this).val();
    }).get();

    $.ajax({
        type: 'POST',
        url: '/Notification/SaveUserNotificationChanges',
        data: { settingIds: settingIds },
        success: function () {
            Swal.fire({
                title: 'Changes saved!!!',
                showDenyButton: false,
                showCancelButton: false,
                showConfirmButton: false,
                timer: 1500,
                position: 'top-end',
            })
        },
        error: function (error) {
            console.log(error)
        }
    });

})





function showRecentVounteers(currVolPage) {
    var missionId = $('.missionId').text();
    var pagesize = 2;
    $.ajax({
        type: 'GET',
        url: '/Mission/showRecentVounteers',
        data: { currVolPage: currVolPage, missionId: missionId },
        success: function (data) {

            var recentVols = $('.recentVolunteersDiv');
            recentVols.empty();
            $('.volText').text('');

            recentVols.append(data);

            var start = ((currVolPage - 1) * pagesize) + 1;
            var total = $('#volCount').val();
            var end = Math.min(currVolPage * pagesize, total);

            var text = start + ' - ' + end + ' of ' + total + ' Recent Volunteers';
            $('.volText').text(text);

            $('.volPagination button').click(function () {

                var totalVolPages = Math.ceil($('#volCount').val() / pagesize);

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

$('.exploreOptions li').on('click', function () {
    selectedExploreOption = $(this).val()
    showMissions()
})

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
    var exploreOption = selectedExploreOption
    var pageNo = pageNo;
    var pagesize = 6;
    $.ajax({
        type: 'POST',
        url: '/Mission/PlatformLanding',
        data: {
            CountryId: CountryId,
            CityId: CityId,
            ThemeId: ThemeId,
            SkillId: SkillId,
            searchText: searchText,
            sortCase: sortCase,
            exploreOption: exploreOption,
            UserId: UserId,
            pageNo: pageNo,
            pagesize: pagesize
        },
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
        data: {
            CountryId: CountryId,
            CityId: CityId,
            ThemeId: ThemeId,
            SkillId: SkillId,
            searchText: searchText,
            pageNo: pageNo,
            pagesize: pagesize
        },

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
                items += `<li> <div class="dropdown-item mb-1 ms-3 form-check"> <input type="checkbox" class="form-check-input" value =` + item.cityId + `><label class="form-check-label" value=` + item.cityId + `>` + item.name + `</label></div></li>`
            })
            dropdown.html(items);

            var dropdown = $(".CityListAccordian");
            dropdown.empty();
            var items = "";
            $(data).each(function (i, item) {
                items += `<li> <div class="dropdown-item mb-1 form-check"> <input type="checkbox"  class="form-check-input" value =` + item.cityId + `><label class="form-check-label" value=` + item.cityId + `>` + item.name + `</label></div></li>`

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
            console.log(error)
            alert("Please login!!");
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

            //update ratings
            $.ajax({
                url: '/Mission/GetUpdatedRatings',
                type: 'GET',
                data: { missionId: missionId },
                success: function (data) {
                    var ratings = data.item1
                    var volunteers = data.item2

                    $('.updateRatings').empty()
                    var html = ""
                    for (var i = 0; i < 5; i++) {
                        if (i < ratings) {
                            html += '<i class="bi bi-star-fill text-warning"></i> '
                        }
                        else {
                            html += '<i class="bi bi-star"></i> '
                        }
                    }
                    html += '<span>(Rated by ' + volunteers + ' Volunteers)</span>'
                    $('.updateRatings').append(html)
                },
                error: function () {
                    console.log("error");
                }
            });
        },
        error: function () {
            console.log("error");
        }
    });
});

function showComments() {
    var missionId = $('.missionId').text();

    $.ajax({
        type: 'POST',
        url: '/Mission/GetComments',
        data: { missionId: missionId },
        success: function (data) {
            $('.commentsContainer').empty()
            $('.commentsContainer').append(data)
        }
    });
}

$('.commentButton').click(function () {
    var comment = $('.newComment').val().trim();
    var missionId = $(this).data('mission-id');
    if (comment.length == 0 || comment.length > 600) {
        $('.valComment').show();
        $('.newComment').on('input', function () {
            if ($('.newComment').val().trim().length > 0 && $('.newComment').val().trim().length <= 600) {
                $('.valComment').hide();
            }
        })
        return;
    }
    $.ajax({
        type: 'POST',
        url: '/Mission/PostComment',
        data: { comment: comment, missionId: missionId },
        success: function (result) {
            $('.newComment').val('');
            //swal.fire({
            //    position: 'center',
            //    icon: result.icon,
            //    title: result.message,
            //    showConfirmButton: false,
            //    timer: 3000
            //})
            showComments();
        },
        error: function (error) {
            console.log("error");
        }
    });


});

function recommendToCoWorker(ToUserId, MissionId, FromUserId) {
    $.ajax({
        type: "POST",
        url: "/Mission/MissionInvite",
        data: { ToUserId: ToUserId, MissionId: MissionId, FromUserId: FromUserId },
        success: function () {
            $('.Invited-' + ToUserId + '.Invited-' + MissionId).html(' <button disabled class="btn disabled btn-success">Invited</button>');
        }
    });
}

$('#applyToMission').click(function () {
    var missionId = $('.missionId').text();
    $.ajax({
        type: "POST",
        url: "/Mission/ApplyToMission",
        data: { missionId: missionId },
        success: function (result) {
            swal.fire({
                position: 'center',
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

            if (localStorage.getItem('view') == 1) {
                var text = $("#searchUserByNameGrid-" + missionId).val().toLowerCase();
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




