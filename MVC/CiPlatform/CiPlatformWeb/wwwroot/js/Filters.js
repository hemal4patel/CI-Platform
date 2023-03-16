
var selectedCountry = null;
var selectedSortCase = null;
spFilterSortSearchPagination();

function spFilterSortSearchPagination() {
    var CountryId = selectedCountry;
    var CityId = $('#CityList input[type="checkbox"]:checked').map(function () { return $(this).val(); }).get().join();
    var ThemeId = $('#ThemeList input[type="checkbox"]:checked').map(function () { return $(this).val(); }).get().join();
    var SkillId = $('#SkillList input[type="checkbox"]:checked').map(function () { return $(this).val(); }).get().join();
    var searchText = $("#searchText").val();
    var sortCase = selectedSortCase;
    console.log(UserId);
    $.ajax({
        type: 'POST',
        url: '/Mission/PlatformLanding',
        data: { CountryId: CountryId, CityId: CityId, ThemeId: ThemeId, SkillId: SkillId, searchText: searchText, sortCase: sortCase, UserId: UserId },
        success: function (data) {
            console.log("Done");
            var view = $(".partialViews");
            view.empty();
            view.append(data);
        },
        error: function (error) {
            console.log(error)
        }
    });
}

$("#sortList li").click(function () {
    selectedSortCase = $(this).val();
    spFilterSortSearchPagination();
});

$("#CountryList li").click(function () {

    var countryId = $(this).val();
    selectedCountry = countryId;
    console.log(selectedCountry);

    GetCitiesByCountry(countryId);
    spFilterSortSearchPagination();
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
            //console.log(data);
            $(data).each(function (i, item) {
                //console.log(item);
                items += `<li> <div class="dropdown-item mb-1 ms-3 form-check"> <input type="checkbox" class="form-check-input" id="exampleCheck1" value =` + item.cityId + `><label class="form-check-label" for="exampleCheck1" value=` + item.cityId + `>` + item.name + `</label></div></li>`
            })
            dropdown.html(items);

            var dropdown = $("#CityListAccordian");
            dropdown.empty();
            var items = "";
            //console.log(data);
            $(data).each(function (i, item) {
                //console.log(item);
                items += `<li> <div class="dropdown-item mb-3 form-check"> <input type="checkbox"  class="form-check-input" id="exampleCheck1" value =` + item.cityId + `><label class="form-check-label" for="exampleCheck1" value=` + item.cityId + `>` + item.name + `</label></div></li>`

            })
            dropdown.html(items);
        }
    });
}

let filterPills = $('.filter-pills');
let allDropdowns = $('.dropdown ul');
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
                spFilterSortSearchPagination();
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
                    spFilterSortSearchPagination();
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

        spFilterSortSearchPagination();
    });
});

//$(document).on('click', '.favouriteButton', function () {
//    console.log("Hello");

//    var button = $(this);
//    var missionId = $(this).data('mission-id');
//    console.log(missionId);
//    $.ajax({
//        url: '/Mission/AddToFavorites',
//        type: 'POST',
//        data: { missionId: missionId },
//        success: function (result) {
//            console.log(missionId)
//            var allMissionId = $('.favouriteButton')
//            allMissionId.each(function () {
//                if ($(this).data('mission-id') === missionId) {
//                    if ($(this).hasClass('bi-heart')) {
//                        $(this).addClass('bi-heart-fill text-danger')
//                        $(this).removeClass('bi-heart text-light')
//                        console.log("added")
//                    }
//                    else {
//                        $(this).addClass('bi-heart text-light')
//                        $(this).removeClass('bi-heart-fill text-danger')
//                        console.log("remove")
//                    }
//                }
//            })
//        },
//        error: function (error) {
//            console.log("error")
//        }
//    });
//});

function addToFavourites(missionId) {
    $.ajax({
        url: '/Mission/AddToFavorites',
        type: 'POST',
        data: { missionId: missionId },
        success: function (result) {
            var icon = $("i." + missionId);
            if (icon.hasClass("bi-heart")) {
                icon.removeClass("text-light bi-heart").addClass("text-danger bi-heart-fill");
            } else {
                icon.removeClass("text-danger bi-heart-fill").addClass("text-light bi-heart");

            }
        },
        error: function (error) {
            console.log("error")
        }
    });
}