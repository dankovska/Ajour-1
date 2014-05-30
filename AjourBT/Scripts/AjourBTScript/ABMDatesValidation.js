$('#buttonSubmit').button();
$('#buttonSubmitAbsence').button();


$('#searchInputABM').keyup(function (event) {
    if (event.keyCode == 13) {
        if ($('#From').val.length > 0 && $('#To').val.length > 0 && $('#To').val >= $('#From').val) {
            $('#formWTR').submit();
        }
        else {   }
    }
});

$('#buttonSubmit').click(function () {
    var from = $.datepicker.parseDate("dd.mm.yy", $('#From').val());
    var to = $.datepicker.parseDate("dd.mm.yy", $('#To').val());
    if ($('#From').val().length > 0 && $('#To').val().length > 0 && to >= from) {

        $('#errorFrom').text('');
        $('#From').css('border', '1px solid rgb(226,226,226)');
        $('#errorTo').text('');
        $('#To').css('border', '1px solid rgb(226, 226, 226)');

        $('#formWTR').submit();
    }
    else
    {
        if($('#From').val().length == 0)
        {
            $('#errorFrom').text('');
            $('#errorFrom').append('The From field is required');
            $('#From').css('border', '1px solid rgb(232,12,77)');
        }

        if ($('#To').val().length == 0) {
            $('#errorTo').text('');
            $('#errorTo').append('The To field is required');
            $('#To').css('border', '1px solid rgb(232,12,77)');
        }

        if (from > to)
        {
            $('#errorFrom').text('');
            $('#errorTo').text('');
            $('#errorTo').append('The From Date must be less than To date');
        }
    }
})


$('#buttonSubmitAbsence').click(function () {
    var from = $.datepicker.parseDate("dd.mm.yy", $('#FromAbsence').val());
    var to = $.datepicker.parseDate("dd.mm.yy", $('#ToAbsence').val());
    if ($('#FromAbsence').val().length > 0 && $('#ToAbsence').val().length > 0 && to >= from) {

        $('#errorFromAbsence').text('');
        $('#FromAbsence').css('border', '1px solid rgb(226,226,226)');
        $('#errorToAbsence').text('');
        $('#ToAbsence').css('border', '1px solid rgb(226, 226, 226)');

        $('#formAbsence').submit();
    }
    else {
        if ($('#FromAbsence').val().length == 0) {
            $('#errorFromAbsence').text('');
            $('#errorFromAbsence').append('The From field is required');
            $('#FromAbsence').css('border', '1px solid rgb(232,12,77)');
        }

        if ($('#ToAbsence').val().length == 0) {
            $('#errorToAbsence').text('');
            $('#errorToAbsence').append('The To field is required');
            $('#ToAbsence').css('border', '1px solid rgb(232,12,77)');
        }

        if (from > to) {
            $('#errorFromAbsence').text('');
            $('#errorToAbsence').text('');
            $('#errorToAbsence').append('The From Date must be less than To date');
        }
    }
})


/*ABM(Absence)*/

$('#absenceSearchInput').keyup(function (event) {
    if (event.keyCode == 13) {
        if ($('#fromDate').val.length > 0 && $('#toDate').val.length > 0 && $('#toDate').val >= $('#fromDate').val) {
            $('#formAbsence').submit();
        }
        else { }
    }
});

$('#absenceButton').click(function () {
    var from = $.datepicker.parseDate("dd.mm.yy", $('#fromDate').val());
    var to = $.datepicker.parseDate("dd.mm.yy", $('#toDate').val());
    if ($('#fromDate').val().length > 0 && $('#toDate').val().length > 0 && to >= from) {

        $('#errorFromAbs').text('');
        $('#fromDate').css('border', '1px solid rgb(226,226,226)');
        $('#errorToAbs').text('');
        $('#toDate').css('border', '1px solid rgb(226, 226, 226)');

        $('#formAbsence').submit();
    }
    else {
        if ($('#fromDate').val().length == 0) {
            $('#errorFromAbs').text('');
            $('#errorFromAbs').append('The From field is required');
            $('#fromDate').css('border', '1px solid rgb(232,12,77)');
        }

        if ($('#toDate').val().length == 0) {
            $('#errorToAbs').text('');
            $('#errorToAbs').append('The To field is required');
            $('#toDate').css('border', '1px solid rgb(232,12,77)');
        }

        if (from > to) {
            $('#errorFromAbs').text('');
            $('#errorToAbs').text('');
            $('#errorToAbs').append('The From Date must be less than To date');
        }
    }
});

$('#calendarAbsenceButton').button();

$('#calendarAbsenceSubmitBtn').click(function () {
    var from = $.datepicker.parseDate("dd.mm.yy", $('#calendarFromDate').val());
    var to = $.datepicker.parseDate("dd.mm.yy", $('#calendarToDate').val());
    if ($('#calendarFromDate').val().length > 0 && $('#calendarToDate').val().length > 0 && to >= from) {

        $('#errorFromAbs').text('');
        $('#calendarFromDate').css('border', '1px solid rgb(226,226,226)');
        $('#errorToAbs').text('');
        $('#calendarToDate').css('border', '1px solid rgb(226, 226, 226)');

        $.ajax({
            cache: false,
            url: "/Calendar/getCalendarData",
            type: "Post",
            data: { calendarFromDate: $('#calendarFromDate').val(), calendarToDate: $("#calendarToDate").val(), selectedDepartment: $('#depDropList :selected').val() },
            success: function (data) {
                $("#CalendarData").html(data);
            }
        })
    }
    else {
        if ($('#calendarFromDate').val().length == 0) {
            $('#errorFromAbs').text('');
            $('#errorFromAbs').append('The From field is required');
            $('#calendarFromDate').css('border', '1px solid rgb(232,12,77)');
        }

        if ($('#calendarToDate').val().length == 0) {
            $('#errorToAbs').text('');
            $('#errorToAbs').append('The To field is required');
            $('#calendarToDate').css('border', '1px solid rgb(232,12,77)');
        }

        if (from > to) {
            $('#errorFromAbs').text('');
            $('#errorToAbs').text('');
            $('#errorToAbs').append('The From Date must be less than To date');
        }
    }
})

$('#depDropList').on("change", function () {
    var from = $.datepicker.parseDate("dd.mm.yy", $('#calendarFromDate').val());
    var to = $.datepicker.parseDate("dd.mm.yy", $('#calendarToDate').val());
    if ($('#calendarFromDate').val().length > 0 && $('#calendarToDate').val().length > 0 && to >= from) {

        $('#errorFromAbs').text('');
        $('#calendarFromDate').css('border', '1px solid rgb(226,226,226)');
        $('#errorToAbs').text('');
        $('#calendarToDate').css('border', '1px solid rgb(226, 226, 226)');

        $.ajax({
            cache: false,
            url: "/Calendar/getCalendarData",
            type: "Post",
            data: { calendarFromDate: $('#calendarFromDate').val(), calendarToDate: $("#calendarToDate").val(), selectedDepartment: $('#depDropList :selected').val() },
            success: function (data) {
                $("#CalendarData").html(data);
            }
        })
    }
    else {
        if ($('#calendarFromDate').val().length == 0) {
            $('#errorFromAbs').text('');
            $('#errorFromAbs').append('The From field is required');
            $('#calendarFromDate').css('border', '1px solid rgb(232,12,77)');
        }

        if ($('#calendarToDate').val().length == 0) {
            $('#errorToAbs').text('');
            $('#errorToAbs').append('The To field is required');
            $('#calendarToDate').css('border', '1px solid rgb(232,12,77)');
        }

        if (from > to) {
            $('#errorFromAbs').text('');
            $('#errorToAbs').text('');
            $('#errorToAbs').append('The From Date must be less than To date');
        }
    }
})

$('#pdfPrintBtn').click(function (event) {
    event.preventDefault();
    var from = $.datepicker.parseDate("dd.mm.yy", $('#calendarFromDate').val());
    var to = $.datepicker.parseDate("dd.mm.yy", $('#calendarToDate').val());
    if ($('#calendarFromDate').val().length > 0 && $('#calendarToDate').val().length > 0 && to >= from) {

        $('#errorFromAbs').text('');
        $('#calendarFromDate').css('border', '1px solid rgb(226,226,226)');
        $('#errorToAbs').text('');
        $('#calendarToDate').css('border', '1px solid rgb(226, 226, 226)');
        $('#printCalendarToPdf').submit();
    }
    else {
        if ($('#calendarFromDate').val().length == 0) {
            $('#errorFromAbs').text('');
            $('#errorFromAbs').append('The From field is required');
            $('#calendarFromDate').css('border', '1px solid rgb(232,12,77)');
        }

        if ($('#calendarToDate').val().length == 0) {
            $('#errorToAbs').text('');
            $('#errorToAbs').append('The To field is required');
            $('#calendarToDate').css('border', '1px solid rgb(232,12,77)');
        }

        if (from > to) {
            $('#errorFromAbs').text('');
            $('#errorToAbs').text('');
            $('#errorToAbs').append('The From Date must be less than To date');
        }
    }
})