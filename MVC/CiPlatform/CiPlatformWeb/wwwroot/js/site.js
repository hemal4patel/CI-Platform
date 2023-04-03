

$("#countryDropdown").click(function () {
    var countryId = $(this).val();

    $.ajax({
        type: "GET",
        url: "/User/GetCitiesByCountry",
        data: { countryId: countryId },
        success: function (data) {
            var dropdown = $("#CityDropdown");
            dropdown.empty();
            var items = "";
            items += '<option selected value="0">Select your city</option>'
            $(data).each(function (i, item) {
                items += '<option value=' + item.cityId + '> ' + item.name + ' </option>'
            });
            dropdown.html(items);
        }
    });
});


$('.selectSkills').click(function () {
    var newOption;
    var displaySkillsDiv = $('.upSkillsSelected select');
    var selectedSkills = $('.skillOptions select option:selected');
    //displaySkillsDiv.empty();
    selectedSkills.each(function () {
        newOption = $('<option>', {
            value: $(this).val(),
            text: $(this).text(),
        })
        displaySkillsDiv.append(newOption)
    })


})

//removing skills
$('.deselectSkills').click(function () {
    var selectedSkills = $('.upSkillsSelected select option:selected');
    selectedSkills.each(function () {
        $(this).remove();
    })
})


function saveSkills() {
    console.log("called");

    var container = $('.skillsContainer');
    container.empty();

    skills = "";

    $('.upSkillsSelected select option').each(function () {
        var value = $(this).val();
        var text = $(this).html();
        skills += ` <div data-value=${value}>${text}</div>`

    })
    container.append(skills);
}

// Add change event listener to profile image file input
$('#profile-image-input').change(function () {
    // Read image file and display preview
    var reader = new FileReader();
    reader.onload = function (e) {
        $('.user-image').attr('src', e.target.result);
    }
    reader.readAsDataURL(this.files[0]);
});

$('.edit-icon').click(function () {
    // Open file input dialog
    $('#profile-image-input').click();
});
