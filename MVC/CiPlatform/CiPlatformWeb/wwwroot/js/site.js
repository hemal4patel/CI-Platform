

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

$('.selectSkill').on('click', function () {
    var selectedSkill = $('.skillOptions select option:selected')
    selectedSkill.prop('selected', true)
    var isSelected = false
    $('.selectedSkills select option').each(function () {
        if (selectedSkill.val() == $(this).val()) {
            isSelected = true;
        }
    })
    if (!isSelected) {
        newOption = $('<option>', {
            value: selectedSkill.val(),
            text: selectedSkill.text(),
        })
        $('.selectedSkills select').append(newOption)
    }
})

$('.deselectSkill').on('click', function () {
    var deselectedSkill = $('.selectedSkills select option:selected')
    $('.skillOptions select option:selected').each(function () {
        if ($(this).val() == deselectedSkill.val()) {
            $(this).prop('selected', false)
        }
    })
    deselectedSkill.remove();
})

function saveSkills() {
    var container = $('.skillsContainer')
    container.empty()
    var selectedSkills = []
    skills = ""
    $('.selectedSkills select option').each(function () {
        var skillId = $(this).val()
        var skillName = $(this).text();
        selectedSkills.push(skillId)
        skills += '<div class="userSkill" data-value="' + skillId + '">' + skillName + '</div>'
    })
    container.append(skills)
    $('#selectedSkills').val(selectedSkills.join())

    $('#addSkillsModal').modal('hide');
}

$('#avatarFile').change(function () {
    var reader = new FileReader();
    console.log(reader)
    reader.onload = function (e) {
        $('.user-image').attr('src', e.target.result);
    }
    reader.readAsDataURL(this.files[0]);
});

$('.edit-icon').click(function () {
    $('#avatarFile').click();
});

$('#changePasswordForm').on('submit', function (e) {
    e.preventDefault();
    if ($(this).valid()) {
        var oldPassword = $('#oldPassword').val()
        var newPassword = $('#newPassword').val()
        
        $.ajax({
            type: 'POST',
            url: '/User/ChangePassword',
            data: { oldPassword: oldPassword, newPassword: newPassword },
            success: function () {
                $('#changePasswordModal').modal('hide');
                swal.fire({
                    position: 'center',
                    icon: 'success',
                    title: "Password updated succcessfully!!!",
                    showConfirmButton: false,
                    timer: 3000
                });
            },
            error: function () {
                $('.valInvalidPassword').show();
                $('#oldPassword').on('input', function () {
                    $('.valInvalidPassword').hide();
                })
            }
        });
    }
})

$('.resetPasswordForm').click(function () {
    var form = $('#changePasswordForm');
    form[0].reset()
    form.find("[data-valmsg-for]").empty();
    $('.valInvalidPassword').hide();
})

function validateContactUsForm() {
    var flag = true;

    var subject = $('#contactSubject').val().trim().length;
    var message = $('#contactMessage').val().trim().length;

    if (subject < 10 || subject > 255) {
        $('.valContactSubject').show();
        flag = false;

        $('#contactSubject').on('input', function () {
            if ($('#contactSubject').val().trim().length >= 10 && $('#contactSubject').val().trim().length <= 255) {
                $('.valContactSubject').hide();
                flag = true;
            }
        })
    }

    if (message < 10 || message > 60000) {
        $('.valContactMessage').show();
        flag = false;

        $('#contactMessage').on('input', function () {
            if ($('#contactMessage').val().trim().length >= 10 && $('#contactMessage').val().trim().length <= 60000) {
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
                if (data.status == 1) {
                    swal.fire({
                        position: 'center',
                        icon: 'success',
                        title: "Thank you for contacting us!!!",
                        showConfirmButton: false,
                        timer: 3000
                    });
                }
                else {
                    window.location.href = '/Home/Index'
                }
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
            $('.field-validation-error').text('');
            $('.field-validation-valid').text('');

            $('#mission').prop('disabled', false)
            $('#hours').prop('disabled', false)
            $('#minutes').prop('disabled', false)
            $('#message').prop('disabled', false)
            $('#date').prop('disabled', false)

            $('.modal-footer').show()

            $('.modal-title').text('Please input below Volunteering Hours')

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

            $('#Gmission').prop('disabled', false)
            $('#Gaction').prop('disabled', false)
            $('#Gmessage').prop('disabled', false)
            $('#Gdate').prop('disabled', false)

            $('.modal-footer').show()

            $('.modal-title').text('Please input below Volunteering Goal')

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

//view time based timesheet
$('.viewTimeTimesheet').click(function () {
    var id = $(this).closest('tr').attr('id');

    $.ajax({
        type: "GET",
        url: "/User/GetTimesheetData",
        data: { id: id },
        success: function (data) {
            $('#addVolHours').modal('show')
            $('.modal-title').text('Volunteeering Hours')
            $('#mission').val(data.timesheet.missionId)
            $('#hours').val(data.timesheet.time.split(":")[0])
            $('#minutes').val(data.timesheet.time.split(":")[1])
            $('#message').val(data.timesheet.notes)

            const date = new Date(data.timesheet.dateVolunteered);
            const yyyy = date.getFullYear();
            const mm = String(date.getMonth() + 1).padStart(2, '0');
            const dd = String(date.getDate()).padStart(2, '0');
            const formattedDate = `${yyyy}-${mm}-${dd}`;
            $('#date').val(formattedDate);

            $('#mission').prop('disabled', true)
            $('#hours').prop('disabled', true)
            $('#minutes').prop('disabled', true)
            $('#message').prop('disabled', true)
            $('#date').prop('disabled', true)

            $('.modal-footer').hide()
        },
        error: function (error) {
            console.log(error)
        }
    })

    
})

//view goal based timesheet
$('.viewGoalTimesheet').click(function () {
    var id = $(this).closest('tr').attr('id');

    $.ajax({
        type: "GET",
        url: "/User/GetTimesheetData",
        data: { id: id },
        success: function (data) {
            $('#addVolGoal').modal('show')
            $('.modal-title').text('Volunteeering Goal')
            $('#Gmission').val(data.timesheet.missionId)
            $('#Gaction').val(data.timesheet.action)
            $('#Gmessage').val(data.timesheet.notes)

            const date = new Date(data.timesheet.dateVolunteered);
            const yyyy = date.getFullYear();
            const mm = String(date.getMonth() + 1).padStart(2, '0');
            const dd = String(date.getDate()).padStart(2, '0');
            const formattedDate = `${yyyy}-${mm}-${dd}`;
            $('#Gdate').val(formattedDate);

            $('#Gmission').prop('disabled', true)
            $('#Gaction').prop('disabled', true)
            $('#Gmessage').prop('disabled', true)
            $('#Gdate').prop('disabled', true)

            $('.modal-footer').hide()
        },
        error: function (error) {
            console.log(error)
        }
    })


})


//reset form
$('.resetFormButton').click(function () {
    $('.resetForm').each(function () {
        this.reset();
        $(this).find('.field-validation-error').text('');
        $(this).find('.field-validation-valid').text('');
        $(this).find('.errorMsg').hide()

        $('#mission option').prop("disabled", false);
        $('#date').removeAttr('min').removeAttr('max');

        $('#Gmission option').prop("disabled", false);
        $('#Gdate').removeAttr('min').removeAttr('max');

        $('#mission').prop('disabled', false)
        $('#hours').prop('disabled', false)
        $('#minutes').prop('disabled', false)
        $('#message').prop('disabled', false)
        $('#date').prop('disabled', false)

        $('#Gmission').prop('disabled', false)
        $('#Gaction').prop('disabled', false)
        $('#Gmessage').prop('disabled', false)
        $('#Gdate').prop('disabled', false)

        $('.modal-footer').show()

        $('#addVolHours .modal-title').text('Please input below Volunteering Hours')
        $('#addVolGoal .modal-title').text('Please input below Volunteering Goal')

    });
})


