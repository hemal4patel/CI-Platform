
function validateChangePassword() {
    var flag = true;

    var oldPassoword = $('#oldPassword').val();
    var newPassword = $('#newPassword').val();
    var confirmPassword = $('#confirmPassword').val();

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

        $.ajax({
            type: "POST",
            url: "/User/ChangePassword",
            data: { oldPassoword: oldPassoword, newPassword: newPassword, confirmPassword: confirmPassword },
            success: function (data) {
                $('#changePassword').modal('hide');
                swal.fire({
                    position: 'center',
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
            items += '<option selected value="">Select your city</option>'
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
    $('#selectedSkills').val(selectedUserSkills.join());
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



function validateContactUsForm() {
    var flag = true;

    var subject = $('#contactSubject').val();
    var message = $('#contactMessage').val();

    if (subject == "") {
        $('.valContactSubject').show();
        flag = false;

        $('#contactSubject').on('input', function () {
            if ($('#contactSubject').val().length != 0) {
                $('.valContactSubject').hide();
                flag = true;
            }
        })
    }

    if (message == "") {
        $('.valContactMessage').show();
        flag = false;

        $('#contactMessage').on('input', function () {
            if ($('#contactMessage').val().length != 0) {
                $('.valContactMessage').hide();
                flag = true;
            }
        })
    }


    return flag;
}

$('#submitContactForm').click(function () {

    var subject = $('#contactSubject').val();
    var message = $('#contactMessage').val();


    if (validateContactUsForm()) {
        $.ajax({
            type: "POST",
            url: "/User/ContactUs",
            data: { subject: subject, message: message },
            success: function (data) {
                $('#contactUs').modal('hide');
                swal.fire({
                    position: 'center',
                    icon: 'success',
                    title: "Thank you for contacting us!!!",
                    showConfirmButton: false,
                    timer: 3000
                });
            },
            error: function (error) {
                console.log(error)
            }
        });
    }
})


$('.deleteTimesheet').click(function () {
    var id = $(this).closest('tr').attr('id');
    var row = $(this).closest('tr');
    Swal.fire({
        title: 'Are you sure?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                type: "POST",
                url: "/User/DeleteTimesheetData",
                data: { id: id },
                success: function () {
                    row.remove();
                    swal.fire({
                        position: 'center',
                        icon: 'success',
                        title: 'Entry deleted successfully!!!',
                        showConfirmButton: false,
                        timer: 3000
                    })
                },
                error: function (error) {
                    console.log(error)
                }
            });
        }
    })
})

$('.editTimeTimesheet').click(function () {
    var id = $(this).closest('tr').attr('id');
    $.ajax({
        type: "POST",
        url: "/User/GetTimesheetData",
        data: { id: id },
        success: function (data) {
            $("#addVolHours").modal("show");

            $('#timesheetId').val(data.timesheetId)
            $('#mission').val(data.missionId)
            $('#mission option:not(:selected)').prop("disabled", true);

            const date = new Date(data.dateVolunteered);
            const yyyy = date.getFullYear();
            const mm = String(date.getMonth() + 1).padStart(2, '0');
            const dd = String(date.getDate()).padStart(2, '0');
            const formattedDate = `${yyyy}-${mm}-${dd}`;
            $('#date').val(formattedDate);
            $('#hours').val(data.time.split(":")[0])
            $('#minutes').val(data.time.split(":")[1])
            $('#message').val(data.notes)
        },
        error: function (error) {
            console.log(error)
        }
    });
})


$('.editGoalTimesheet').click(function () {
    var id = $(this).closest('tr').attr('id');
    $.ajax({
        type: "GET",
        url: "/User/GetTimesheetData",
        data: { id: id },
        success: function (data) {
            $("#addVolGoal").modal("show");

            $('#GtimesheetId').val(data.timesheetId)
            $('#Gmission').val(data.missionId)
            $('#Gmission option:not(:selected)').prop("disabled", true);

            $('#Gaction').val(data.action)

            const date = new Date(data.dateVolunteered);
            const yyyy = date.getFullYear();
            const mm = String(date.getMonth() + 1).padStart(2, '0');
            const dd = String(date.getDate()).padStart(2, '0');
            const formattedDate = `${yyyy}-${mm}-${dd}`;
            $('#Gdate').val(formattedDate);
            $('#Gmessage').val(data.notes)
        },
        error: function (error) {
            console.log(error)
        }
    });
})


$('#mission').on('change', function () {
    var startDate = $(this).find('option:selected').data('value')
    startDate = startDate.split(' ')[0]
    var dateParts = startDate.split("-");
    var year = dateParts[2];
    var month = dateParts[1];
    var day = dateParts[0];
    var minDateVal = `${year}-${month}-${day}`

    $('#date').datepicker('destroy')
    $('#date').datepicker({
        dateFormat: "dd-mm-yy",
        maxDate: new Date(),
        minDate: new Date(minDateVal)
    });
})


$('#Gmission').on('change', function () {
    var startDate = $(this).find('option:selected').data('value')
    startDate = startDate.split(' ')[0]
    var dateParts = startDate.split("-");
    var year = dateParts[2];
    var month = dateParts[1];
    var day = dateParts[0];
    var minDateVal = `${year}-${month}-${day}`

    $('#Gdate').datepicker('destroy')
    $('#Gdate').datepicker({
        dateFormat: "dd-mm-yy",
        maxDate: new Date(),
        minDate: new Date(minDateVal)
    });
})