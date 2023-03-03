$("#CountryList li").click(function () {
    var countryId = $(this).val();
    console.log(countryId);
    GetCitiesByCountry(countryId);
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
            console.log(data);
            $(data).each(function (i, item) {
                console.log(item);
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
            console.log(data);
            $(data).each(function (i, item) {
                console.log(item);
                items += `<li> <div class="dropdown-item mb-3 form-check"> <input type="checkbox" class="form-check-input" id="exampleCheck1"><label class="form-check-label" for="exampleCheck1" value=` + item.cityId + `>` + item.name + `</label></div></li>`
            })
            dropdown.html(items);
        }
    });
}