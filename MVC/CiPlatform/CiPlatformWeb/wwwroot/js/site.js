
function validateChangePassword() {
    var flag = true;

    var oldPassoword = $('#oldPassword').val();
    var newPassword = $('#newPassword').val();
    var confirmPassword = $('#confirmPassword').val();

    console.log(oldPassoword);

    if (oldPassoword == "") {
        $('.valOldPassword').show();
        flag = false;

        $('#oldPassword').on('input', function () {
            if ($('#oldPassword').val().length != 0) {
                $('.valOldPassword').hide();
                flag = true;
            }
        })
    }
    if (newPassword == "") {
        $('.valnewPassword').show();
        $(".valMatchingPassword").hide();
        flag = false;

        $('#newPassword').on('input', function () {
            if ($('#newPassword').val().length != 0) {
                $('.valnewPassword').hide();
                $(".valMatchingPassword").hide();
                flag = true;
            }
        })
    }
    if (confirmPassword == "") {
        $('.valConfirmPassword').show();
        flag = false;

        $('#confirmPassword').on('input', function () {
            if ($('#confirmPassword').val().length != 0) {
                $('.valConfirmPassword').hide();
                $(".valMatchingPassword").hide();
                flag = true;
            }
        })
    }
    else {
        if (newPassword !== confirmPassword) {
            $('.valMatchingPassword').show();
            flag = false;

            $("#ConfirmPassword").on('input', function () {
                if ($("#ConfirmPassword").val().lenght != 0) {
                    $(".valMatchingPassword").hide();
                    flag = true;
                }
            });
        }
        else {
            $(".valMatchingPassword").hide();
            flag = true;
        }
    }

    return flag;
}



$('#changePassowrd').click(function () {

    if (validateChangePassword()) {
        var oldPassoword = $('#oldPassword').val();
        var newPassword = $('#newPassword').val();
        var confirmPassword = $('#confirmPassword').val();
        console.log(oldPassoword + newPassword + confirmPassword);

        $.ajax({
            type: "POST",
            url: "/User/ChangePassword",
            data: { oldPassoword: oldPassoword, newPassword: newPassword, confirmPassword: confirmPassword },
            success: function (data) {
                $('#changePassword').modal('hide');
                swal.fire({
                    position: 'top-end',
                    icon: 'success',
                    title: "Password updated successfully!!!",
                    showConfirmButton: false,
                    timer: 3000
                });
            },
            error: function () {
                $('.valInvalidPassword').show();
                $('#oldPassword').on('input', function () {
                    if ($('#oldPassword').val().length != 0) {
                        $('.valInvalidPassword').hide();
                    }
                })
            }
        });
    }

})

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
    displaySkillsDiv.empty();
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
    var container = $('.skillsContainer');
    container.empty();
    var selectedUserSkills = [];
    skills = "";

    $('.upSkillsSelected select option').each(function () {
        var value = $(this).val();
        var text = $(this).html();
        skills += ` <div class="userSkill" data-value=${value}>${text}</div>`
        selectedUserSkills.push($(this).val());

    })
    container.append(skills);
    console.log(selectedUserSkills);
    $('#selectedSkills').val(selectedUserSkills.join());
    console.log($('#selectedSkills').val());
}


// Add change event listener to profile image file input
$('#avatarFile').change(function () {
    // Read image file and display preview
    var reader = new FileReader();
    reader.onload = function (e) {
        $('.user-image').attr('src', e.target.result);
    }
    reader.readAsDataURL(this.files[0]);
});

$('.edit-icon').click(function () {
    // Open file input dialog
    $('#avatarFile').click();
});

$('#saveProfile').click(function () {
    var firstName = $('#firstName').val();
    var lastName = $('#lastName').val();
    var empId = $('#empId').val();
    var manager = $('#manager').val();
    var title = $('#title').val();
    var dept = $('#dept').val();
    var profileText = $('#ProfileText').val();
    var whyIVolunteer = $('#whyIVolunteer').val();
    var city = $('#CityDropdown').val();
    var country = $('#countryDropdown').val();
    var availability = $('#availability').val();
    var linkedInUrl = $('#linkedInUrl').val();

    $('.userSkill').each(function () {
        console.log($(this).data('value'));
    })

});


//$('#avatarFile').on('input', function () {
//    var avatar = $('#avatarFile')[0].files[0];
//    $('#AvatarImage').val(avatar);
//    console.log($('#AvatarImage').val());
//})
