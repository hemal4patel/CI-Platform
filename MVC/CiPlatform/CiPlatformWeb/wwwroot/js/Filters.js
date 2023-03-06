$("#CountryList li").click(function () {
    $(this).addClass('selected');

    var countryId = $(this).val();
    //console.log(countryId);

    $('.card-div').each(function () {
        var cardCountry = $(this).find('.mission-country').text();
        //console.log(cardCountry);

        if (countryId == cardCountry) {
            $(this).show();
        } else {
            $(this).hide();
        }
    });

    var div = $('.filter-pills');
    var pill = '<div class="rounded-pill alert alert-dismissible fade show border-dark px-2 py-0 m-2">' + $(this).text() +
        '<button type="button" class="border-none close" data-bs-dismiss="alert" aria-label="Close">' +
        '<span aria-hidden="true">&times;</span>' +
        '</button></div>'
    div.html(pill);

    GetCitiesByCountry(countryId);
});


$("#CityList li").click(function () {
    $(this).addClass('checked');
    var cityId = $(this).val();

    pills(cityId);
});

function pills(cityId) {
    var pill = '<div class="rounded-pill alert alert-dismissible fade show border-dark px-2 py-0 m-2">' + $(this).text() +
        '<button type="button" class="border-none close" data-bs-dismiss="alert" aria-label="Close">' +
        '<span aria-hidden="true">&times;</span>' +
        '</button></div>'

    var div = $('.filter-pills');
    div.html(pill);
}

//$(document).ready(function () {
//    if ($('#filter-pills').html() == '' ) {
//        $('.card-div').each(function () {
//            $(this).show();
//        });
//    }
//});

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
                items += `<li> <div class="dropdown-item mb-1 ms-3 form-check"> <input type="checkbox" class="form-check-input" id="exampleCheck1"><label class="form-check-label" for="exampleCheck1" value=` + item.cityId + `>` + item.name + `</label></div></li>`
            })
            dropdown.html(items);
        }
    });

    $.ajax({
        type: "GET",
        url: "/Mission/GetCitiesByCountry",
        data: { countryId: countryId },
        success: function (data) {
            var dropdown = $("#CityListAccordian");
            dropdown.empty();
            var items = "";
            //console.log(data);
            $(data).each(function (i, item) {
                //console.log(item);
                items += `<li> <div class="dropdown-item mb-3 form-check"> <input type="checkbox" class="form-check-input" id="exampleCheck1"><label class="form-check-label" for="exampleCheck1" value=` + item.cityId + `>` + item.name + `</label></div></li>`
            })
            dropdown.html(items);
        }
    });
}



function search() {
    var searchString = document.getElementById("search");
    filter = searchString.value.toUpperCase();
    cards = document.getElementsByClassName("card-div");
    titles = document.getElementsByClassName("card-title");
    for (i = 0; i < cards.length; i++) {
        a = titles[i];
        if (a.innerHTML.toUpperCase().indexOf(filter) > -1) {
            cards[i].classList.remove("d-none");
        } else {
            cards[i].classList.add("d-none");
        }
    }
}