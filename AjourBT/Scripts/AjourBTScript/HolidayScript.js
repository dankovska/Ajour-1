function myWeekCalc(date) {
    var checkDate = new Date(date.getTime());
    // Find Thursday of this week starting on Sunday
    checkDate.setDate(checkDate.getDate() + 4 - (checkDate.getDay()));
    var time = checkDate.getTime();
    checkDate.setMonth(0); // Compare with Jan 1
    checkDate.setDate(1);
    return Math.floor(Math.round((time - checkDate) / 86400000) / 7) + 1;
}

function SetNoErrorStyle_AddHoliday()
{
    $('#validatorCountryID').text('');
    $('#dropDownCountry').css('border', '1px solid rgb(226,226,226)');

    $('#validatorTitle').text('');
    $('#Title').css('border', '1px solid rgb(226, 226, 226)');

    $('#validatorHolidayDate').text('');
    $('#HolidayDate').css('border', '1px solid rgb(226,226,226)');
}

function SetNoErrorStyle_SaveHoliday()
{
    $('#validatorTitle').text('');
    $('#Title').css('border', '1px solid rgb(226, 226, 226)');

    $('#validatorHolidayDate').text('');
    $('#HolidayDate').css('border', '1px solid rgb(226,226,226)');
}


$(document).on('click', '#CreateHoliday', function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = 'Create Holiday';
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');
    var dataFormat = $(this).attr('data-date-format');

    $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            rasizable: false,
            title: 'Create Holiday',
            position: {
                my: 'center',
                at: 'center'
            },

            open: function (event, ui) {
                var startYear = $('#selectedYear option:first-child').val();
                var endYear = $('#selectedYear option:last-child').val();
                $('#HolidayDate').datepicker({
                    firstDay: 1,
                    dateFormat: dataFormat,
                    showWeek: true,
                    calculateWeek: myWeekCalc,
                    showOn: 'button',
                    buttonImage: '/Content/themes/base/images/calendar2.gif',
                    buttonImageOnly: true,
                    //minDate: new Date(startYear, 0, 01),
                    //maxDate: new Date(endYear, 11, 31)
                })

                $('#buttonSaveHoliday').button();
                $('#buttonSaveHoliday').click(function (event) {
                    var reg = new RegExp("^[0-9]{2}\.[0-9]{2}\.[0-9]{4}");
                    var regResult = reg.test($('#HolidayDate').val());                 

                    SetNoErrorStyle_AddHoliday();

                    if ($('#dropDownCountry').val() != "" && $('#Title').val().length > 0 && (regResult == true))
                    {                                             
                        SetNoErrorStyle_AddHoliday();
                        $('#createHolidayForm').submit();
                    }

                    else {

                        if ($('#dropDownCountry').val() == "")
                        {
                            $('#validatorCountryID').text('');
                            $('#validatorCountryID').append('Choose Country')
                            $('#dropDownCountry').css('border', '1px solid rgb(232,12,77)');                          
                        }

                        if ($('#Title').val().length == 0)
                        {
                            $('#validatorTitle').text('');
                            $('#validatorTitle').append('Choose Holiday Title');
                            $('#Title').css('border', '1px solid rgb(232, 12, 77)');
                        }

                        if (regResult == false)
                        {
                            $('#validatorHolidayDate').text('');
                            $('#validatorHolidayDate').append('Wrong DateTime Value');
                            $('#HolidayDate').css('border', '1px solid rgb(232,12,77)');
                        }
                    }
                })
            },
            close: function (event, ui) {
                $(this).dialog('destroy');
                $(this).remove();
            }
        })
    });
})

$(document).on("click", ".holidayEditDialog", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('href');
    var dialogId = "Edit Holiday"
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var startYear = $('#selectedYear option:first-child').val();
    var endYear = $('#selectedYear option:last-child').val();
    var dataFormat = $(this).attr('data-date-format');

    loc = $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: "Edit Holiday",
            position: {
                my: "center",
                at: "center"
            },

            open: function () {

                $('#HolidayDate').datepicker({
                    firstDay: 1,
                    dateFormat: dataFormat,
                    showWeek: true,
                    calculateWeek: myWeekCalc,
                    showOn: 'button',
                    buttonImage: '/Content/themes/base/images/calendar2.gif',
                    buttonImageOnly: true,
                    //minDate: new Date(startYear, 0, 01),
                    //maxDate: new Date(endYear, 11, 31)
                })

                $("#btnSaveHoliday").click(function (event) {
                    var reg = new RegExp("^[0-9]{2}\.[0-9]{2}\.[0-9]{4}");
                    var regResult = reg.test($('#HolidayDate').val());

                    event.preventDefault();
                    SetNoErrorStyle_SaveHoliday();

                    if ($('#Title').val().length > 0 && (regResult == true)) {
                        SetNoErrorStyle_SaveHoliday();

                        $.ajax({
                            cache: false,
                            url: "/Holiday/Edit/?date=" + $('#HolidayDate').val() + '&countryID=' + $('#selectedCountryID').val(),   //ADD date and countryID
                            type: "POST",
                            data: AddAntiForgeryToken($("#editHolidayForm").serialize()),
                            success: function (data) {
                                if (data.error) {
                                    $("#ModelError").html(data.error);
                                }
                                else {
                                    $(loc).dialog("close");
                                }
                            }
                        });
                    }

                    else {

                        if ($('#Title').val().length == 0)
                        {
                            $('#validatorTitle').text('');
                            $('#validatorTitle').append('Choose Holiday Title');
                            $('#Title').css('border', '1px solid rgb(232,12,77)');
                        }

                        if (regResult == false)
                        {
                            $('#validatorHolidayDate').text('');
                            $('#validatorHolidayDate').append('Wrong DateTime Value');
                            $('#HolidayDate').css('border', '1px solid rgb(232,12,77)');

                        }
                    }
                })
            },

            beforeClose: function () {
                $.ajax({
                    cache: false,
                    url: "/Holiday/GetHolidayData/?selectedYear=" + $('#selectedYear').val() +'&selectedCountryID='+$('#selectedCountryID').val(),
                    type: "GET",
                    success: function (data) {
                        $("#HolidayData").replaceWith($(data));
                    }
                });
            },

            close: function (event, ui) {
                $(this).dialog("destroy");
                $(this).remove();
            }
        });

        $("#btnSaveHoliday, #btnDeleteHoliday").button();
    });
    return false;
});



$(document).on("click", "#btnDeleteHoliday", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('data-href');
    var title = $(this).attr('data-title');
    var reg = new RegExp("^[0-9]{2}\.[0-9]{2}\.[0-9]{4}");
    var regResult = reg.test($('#HolidayDate').val());

    SetNoErrorStyle_SaveHoliday()

    if ($('#Title').val().length > 0 && (regResult == true)) {

        SetNoErrorStyle_SaveHoliday();

        $("#deleteHoliday-Confirm").load(url, function () {

            $(this).dialog({
                modal: true,
                height: 'auto',
                width: 'auto',
                resizable: false,
                title: title,
                position: {
                    my: "center",
                    at: "center"
                }
            });
        });
    }
    else
    {
        if ($('#Title').val().length == 0) {
            $('#validatorTitle').text('');
            $('#validatorTitle').append('Choose Holiday Title');
            $('#Title').css('border', '1px solid rgb(232,12,77)');
        }

        if (regResult == false) {
            $('#validatorHolidayDate').text('');
            $('#validatorHolidayDate').append('Wrong DateTime Value');
            $('#HolidayDate').css('border', '1px solid rgb(232,12,77)');

        }
    }

    return false;
});

$(document).on("click", "#btnCancelDeleteHoliday", function (event) {
    $("#deleteHoliday-Confirm").dialog("close");
});

