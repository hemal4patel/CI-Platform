
var selectedCountry = null;
var selectedSortCase = null;
var currentUrl = window.location.href;
let allDropdowns = $('.dropdown ul');

if (currentUrl.includes("PlatformLanding")) {
    spFilterSortSearchPagination(1);
}
else if (currentUrl.includes("StoryListing")) {
    spFilterStory(1);
}



$('#searchText').on('keyup', function () {
    if (currentUrl.includes("PlatformLanding")) {
        spFilterSortSearchPagination();
    }
    else if (currentUrl.includes("StoryListing")) {
        spFilterStory();
    }
});

allDropdowns.on('change', function () {
    if (currentUrl.includes("PlatformLanding")) {
        spFilterSortSearchPagination();
    }
    else if (currentUrl.includes("StoryListing")) {
        spFilterStory();
    }
});

function spFilterSortSearchPagination(pageNo) {
    var CountryId = selectedCountry;
    var CityId = $('#CityList input[type="checkbox"]:checked').map(function () { return $(this).val(); }).get().join();
    var ThemeId = $('#ThemeList input[type="checkbox"]:checked').map(function () { return $(this).val(); }).get().join();
    var SkillId = $('#SkillList input[type="checkbox"]:checked').map(function () { return $(this).val(); }).get().join();
    var searchText = $("#searchText").val();
    var sortCase = selectedSortCase;
    var pagesize = 6;
    var pageNo = pageNo;

    $.ajax({
        type: 'POST',
        url: '/Mission/PlatformLanding',
        data: { CountryId: CountryId, CityId: CityId, ThemeId: ThemeId, SkillId: SkillId, searchText: searchText, sortCase: sortCase, UserId: UserId, pageNo: pageNo, pagesize: pagesize },
        success: function (data) {
            var view = $(".partialViews");
            view.empty();
            view.append(data);
            totalMission();

            if (document.getElementById('missionCount') != null) {
                var totalRecords = document.getElementById('missionCount').innerText;
            }
            let totalPages = Math.ceil(totalRecords / pagesize);

            if (totalPages <= 1) {
                $('#pagination-container').parent().parent().hide();
            }
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
                spFilterSortSearchPagination(pageNo);
            }));

        },
        error: function (error) {
            console.log(error)
        }
    });
}

function spFilterStory(pageNo) {
    console.log("hii");
    var CountryId = selectedCountry;
    var CityId = $('#CityList input[type="checkbox"]:checked').map(function () { return $(this).val(); }).get().join();
    var ThemeId = $('#ThemeList input[type="checkbox"]:checked').map(function () { return $(this).val(); }).get().join();
    var SkillId = $('#SkillList input[type="checkbox"]:checked').map(function () { return $(this).val(); }).get().join();
    var searchText = $("#searchText").val();
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

            if (totalPages <= 1) {
                $('#pagination-container').parent().parent().hide();
            }
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
                spFilterStory(pageNo);
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
    spFilterSortSearchPagination();
});

$("#CountryList li").click(function () {
    allDropdowns.find('input[type="checkbox"]').prop('checked', false);
    $('.filter-pills').empty();
    var countryId = $(this).val();
    selectedCountry = countryId;
    console.log(selectedCountry);

    GetCitiesByCountry(countryId);

    if (currentUrl.includes("PlatformLanding")) {
        spFilterSortSearchPagination();
    }
    else if (currentUrl.includes("StoryListing")) {
        spFilterStory();
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
                    spFilterSortSearchPagination();
                }
                else if (currentUrl.includes("StoryListing")) {
                    spFilterStory();
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
                        spFilterSortSearchPagination();
                    }
                    else if (currentUrl.includes("StoryListing")) {
                        spFilterStory();
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
            spFilterSortSearchPagination();
        }
        else if (currentUrl.includes("StoryListing")) {
            spFilterStory();
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
            if (icon.hasClass("bi-heart")) {
                icon.removeClass("text-light bi-heart").addClass("text-danger bi-heart-fill");
                text.empty();
                text.append("Added to favourites");
            } else {
                icon.removeClass("text-danger bi-heart-fill").addClass("text-light bi-heart");
                text.empty();
                text.append("Add to favourites");
            }
        },
        error: function (error) {
            console.log("error")
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
        success: function () {
            $('.newComment').val('');
        },
        error: function (error) {
            console.log("error");
        }
    });


});

function recommendToCoWorker(ToUserId, MissionId, FromUserId) {
    console.log(MissionId);
    debugger
    $.ajax({
        type: "POST",
        url: "/Mission/MissionInvite",
        data: { ToUserId: ToUserId, MissionId: MissionId, FromUserId: FromUserId },
        success: function () {
            $('.Invited-' + ToUserId + '.Invited-' + MissionId).html(' <button class="btn btn-outline-success" data-mission-Id="@Model.MissionDetails.MissionId">Invited</button>');
        }
    });
}

