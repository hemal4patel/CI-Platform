
function validateChangePassword() {
    var flag = true;

    var oldPassoword = $('#oldPassword').val();
    var newPassword = $('#newPassword').val();
    var confirmPassword = $('#confirmPassword').val();
    console.log(newPassword.length)


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
    if (newPassword == "" || newPassword.length < 8 || newPassword.length > 16) {
        $('.valnewPassword').show();
        $(".valMatchingPassword").hide();
        flag = false;

        $('#newPassword').on('input', function () {
            if (newPassword.length >= 8 && newPassword.length <= 16) {
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
        if (newPassword !== confirmPassword || newPassword.length < 8 || newPassword.length > 16) {
            $('.valMatchingPassword').show();
            flag = false;

            $("#ConfirmPassword").on('input', function () {
                if ($("#ConfirmPassword").val().length >= 8 && $("#ConfirmPassword").val().length <= 16) {
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
    console.log(flag)
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

    var subject = $('#contactSubject').val().trim();
    var message = $('#contactMessage').val().trim();

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

    var subject = $('#contactSubject').val().trim();
    var message = $('#contactMessage').val().trim();


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

            var today = new Date();
            var y = today.getFullYear();
            var m = String(today.getMonth() + 1).padStart(2, '0');
            var d = String(today.getDate()).padStart(2, '0');
            var maxDate = y + '-' + m + '-' + d;
            var startDate = data.startDate.split('T')[0]
            var minDate = startDate;
            var dateTime = document.getElementById("date");
            dateTime.max = maxDate;
            dateTime.min = minDate;

            $('#timesheetId').val(data.timesheet.timesheetId)
            $('#mission').val(data.timesheet.missionId)
            $('#mission option:not(:selected)').prop("disabled", true);
            $('#hours').val(data.timesheet.time.split(":")[0])
            $('#minutes').val(data.timesheet.time.split(":")[1])
            $('#message').val(data.timesheet.notes)

            const date = new Date(data.timesheet.dateVolunteered);
            const yyyy = date.getFullYear();
            const mm = String(date.getMonth() + 1).padStart(2, '0');
            const dd = String(date.getDate()).padStart(2, '0');
            const formattedDate = `${yyyy}-${mm}-${dd}`;
            $('#date').val(formattedDate);
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

            var today = new Date();
            var y = today.getFullYear();
            var m = String(today.getMonth() + 1).padStart(2, '0');
            var d = String(today.getDate()).padStart(2, '0');
            var maxDate = y + '-' + m + '-' + d;
            var startDate = data.startDate.split('T')[0]
            var minDate = startDate
            var dateTime = document.getElementById("Gdate");
            dateTime.max = maxDate;
            dateTime.min = minDate;

            $('#GtimesheetId').val(data.timesheet.timesheetId)
            $('#Gmission').val(data.timesheet.missionId)
            $('#Gmission option:not(:selected)').prop("disabled", true);
            $('#Gaction').val(data.timesheet.action)
            $('#Gmessage').val(data.timesheet.notes)

            const date = new Date(data.timesheet.dateVolunteered);
            const yyyy = date.getFullYear();
            const mm = String(date.getMonth() + 1).padStart(2, '0');
            const dd = String(date.getDate()).padStart(2, '0');
            const formattedDate = `${yyyy}-${mm}-${dd}`;
            $('#Gdate').val(formattedDate);
        },
        error: function (error) {
            console.log(error)
        }
    });
})


$('#mission').on('change', function () {
    var startDate = $(this).find('option:selected').data('value')

    var today = new Date();
    var yyyy = today.getFullYear();
    var mm = String(today.getMonth() + 1).padStart(2, '0');
    var dd = String(today.getDate()).padStart(2, '0');
    var maxDate = yyyy + '-' + mm + '-' + dd;

    startDate = startDate.split(' ')[0]
    var dateParts = startDate.split("-");
    var year = dateParts[2];
    var month = dateParts[1];
    var day = dateParts[0];
    var minDate = `${year}-${month}-${day}`

    var dateTime = document.getElementById("date");
    dateTime.max = maxDate;
    dateTime.min = minDate;
})


$('#Gmission').on('change', function () {
    var startDate = $(this).find('option:selected').data('value')

    var today = new Date();
    var yyyy = today.getFullYear();
    var mm = String(today.getMonth() + 1).padStart(2, '0');
    var dd = String(today.getDate()).padStart(2, '0');
    var maxDate = yyyy + '-' + mm + '-' + dd;

    startDate = startDate.split(' ')[0]
    var dateParts = startDate.split("-");
    var year = dateParts[2];
    var month = dateParts[1];
    var day = dateParts[0];
    var minDate = `${year}-${month}-${day}`

    var dateGoal = document.getElementById("Gdate");
    dateGoal.max = maxDate;
    dateGoal.min = minDate;
})


//reset form
$('.resetFormButton').click(function () {
    $('.resetForm').each(function () {
        this.reset();
        $(this).find('.field-validation-error').text('');
        $(this).find('.field-validation-valid').text('');
        $(this).find('.errorMsg').hide()
    });
})

$('.changePasswordForm').click(function () {
    $('.inputField').each(function () {
        $(this).val('')
    })
    $('.errorMsg').hide();
})
