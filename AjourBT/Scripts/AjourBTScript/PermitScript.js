function myWeekCalc(date) {
    var checkDate = new Date(date.getTime());
    // Find Thursday of this week starting on Sunday
    checkDate.setDate(checkDate.getDate() + 4 - (checkDate.getDay()));
    var time = checkDate.getTime();
    checkDate.setMonth(0); // Compare with Jan 1
    checkDate.setDate(1);
    return Math.floor(Math.round((time - checkDate) / 86400000) / 7) + 1;
}

var dialog;
var scrollPosition;
var PermitEditDialog;
var PermitDelDialog
AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#PermitEditForm input[name=__RequestVerificationToken]').val();
    return data;
};

$(document).on("click", "#CreatePermit", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "permitDialog-create";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var dateFormat = element.attr('data-date-format');
    var url = element.attr('href');
    var parentID = element.parent().parent().attr("id");
    scrollPosition = $("div#visasView_wrapper>div.dataTables_scroll>div.dataTables_scrollBody").scrollTop();
    $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: 'Create Permit',
            position: {
                my: "center",
                at: "center"
            },
            buttons: {
                "Save": {
                    text: 'Save',
                    id: "btnSave",
                    click: function (event) {
                        event.preventDefault();
                        $("#CreatePermitForm").validate();
                        if ($("#CreatePermitForm").valid()) {
                            visa = $('#CreatePermitForm').serialize();
                            $.ajax({
                                type: "POST",
                                url: "/Permit/Create",
                                data: visa,

                                success: function (data) {
                                    if (data.error) {
                                        $("#PermitModelError").html(data.error);
                                    }
                                    else {
                                        $("#" + dialogId).dialog("close");
                                        //$("#VisasViewBody").replaceWith($(data));
                                    }
                                },

                                error: function (data) {
                                    alert('Server not responding');
                                }
                            });
                        }
                    }
                }
            },
            open: function (event, ui) {
                if ($('#StartDatePermitCreate').length > 0) {
                    $('#StartDatePermitCreate').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }
                if ($('#EndDatePermitCreate').length > 0) {
                    $('#EndDatePermitCreate').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }
            },

            beforeClose: function () {
                $.ajax({
                    cache: false,
                    url: "/Visa/GetVisaDataBTM/",
                    type: "GET",
                    data: { searchString: $("#seachInput").val() },
                    success: function (data) {
                        var type = document.getElementById("visaTableForBTM").getElementsByTagName("th");
                        var text;
                        var sortType;
                        var position;
                        for (var i = 0; i < (type.length / 2) ; i++) {
                            text = type[i].className;

                            if (text == 'sorting_asc' || text == 'sorting_desc') {
                                position = i;
                                sortType = text.replace("sorting_", "");
                            }
                        }
                        scrollPositionBTBefore = $("div#visasView_wrapper>div.dataTables_scroll>div.dataTables_scrollBody").scrollTop();
                        $("#visaTableForBTM").replaceWith($(data));
                        window.sortTable.fnSort([[position, sortType]]);
                        window.sortTable.fnDraw();
                        scrollPosition = scrollPositionBTBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPosition);
                    }
                });
            },

            close: function (event, ui) {
                $('#StartDatePermitCreate').datepicker("destroy");
                $('#EndDatePermitCreate').datepicker("destroy");
                $(this).dialog("destroy");
                $(this).remove();
            }
        });
        $.validator.unobtrusive.parse(this);

    });
    return false;
});


$(document).on("click", ".permitEditDialog", function (event) {

    event.preventDefault();
    var element = $(this);
    var dialogId = "permitDialogEdit";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var dateFormat = element.attr('data-date-format');
    var url = element.attr('href');
    var parentID = element.parent().parent().attr("id");
    scrollPosition = $("div#visasView_wrapper>div.dataTables_scroll>div.dataTables_scrollBody").scrollTop();
    $(dialogDiv).load(url, function () {
        PermitEditDialog = $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: 'Edit Permit',
            position: {
                my: "center",
                at: "center"
            },

            open: function (event, ui) {
                if ($('#editStartDatePermit').length > 0) {
                    $('#editStartDatePermit').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }
                if ($('#editEndDatePermit').length > 0) {
                    $('#editEndDatePermit').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }
                if ($('#CancelRequestDatePermitCreate').length > 0) {
                    $('#CancelRequestDatePermitCreate').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }
                if ($('#ProlongRequestDatePermitCreate').length > 0) {
                    $('#ProlongRequestDatePermitCreate').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }



                $("#btnSavePermit,#btnDeletePermit").button();
                $("#btnSavePermit").click(function (event) {
                    event.preventDefault();
                    $("#PermitEditForm").validate();
                    if ($("#PermitEditForm").valid()) {
                        permit = $('#PermitEditForm').serialize();
                        $.ajax({
                            type: "POST",
                            url: "/Permit/Edit",
                            data: AddAntiForgeryToken(permit),
                            success: function (data) {
                                if (data.error) {
                                    $("#PermitModelError").html(data.error);
                                }
                                else {
                                    $(PermitEditDialog).dialog("close");
                                    //$("#VisasViewBody").replaceWith($(data));
                                }
                            },

                            error: function (data) {
                                alert('Server not responding');

                            }
                        });
                    }
                })
            },
            beforeClose: function () {
                $.ajax({
                    cache: false,
                    url: "/Visa/GetVisaDataBTM/",
                    type: "GET",
                    data: { searchString: $("#seachInput").val() },
                    success: function (data) {
                        var type = document.getElementById("visaTableForBTM").getElementsByTagName("th");
                        var text;
                        var sortType;
                        var position;
                        for (var i = 0; i < (type.length / 2) ; i++) {
                            text = type[i].className;

                            if (text == 'sorting_asc' || text == 'sorting_desc') {
                                position = i;
                                sortType = text.replace("sorting_", "");
                            }
                        }
                        scrollPositionBTBefore = $("div#visasView_wrapper>div.dataTables_scroll>div.dataTables_scrollBody").scrollTop();
                        $("#visaTableForBTM").replaceWith($(data));
                        window.sortTable.fnSort([[position, sortType]]);
                        window.sortTable.fnDraw();
                        scrollPosition = scrollPositionBTBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPosition);
                    }
                });
            },

            close: function (event, ui) {
                $('#editStartDatePermit').datepicker("destroy");
                $('#editEndDatePermit').datepicker("destroy");
                $('#CancelRequestDatePermitCreate').datepicker("destroy");
                $('#ProlongRequestDatePermitCreate').datepicker("destroy");
                $(PermitEditDialog).dialog("destroy");
                $(PermitEditDialog).remove();
            }
        });
        $.validator.unobtrusive.parse(this);
    });
    return false;
});

$(document).on("click", "#btnDeletePermit", function (event) {
    event.preventDefault();
    var url = $(this).attr('data-href');
    var title = $(this).attr('data-title');
    $("#DeletePermit-confirm").load(url, function () {
        PermitDelDialog = $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Delete permit",
            position: {
                my: "center",
                at: "center"
            },
        })
        $("#OKDeletePermit,#btnCancelDeletePermit").button();
        $("#OKDeletePermit").click(function (event) {
            event.preventDefault();
            visa = $('#DeletePermitForm').serialize();
            $.ajax({
                type: "POST",
                url: "/Permit/Delete",
                data: visa,

                success: function (data) {
                    $("#permitDialogEdit").dialog("close");
                    $(PermitDelDialog).dialog("close");
                    //$("#VisasViewBody").replaceWith(data);
                },

                error: function (data) {
                    alert('Server not responding');
                }
            });
        })

    });

    return false;
});

$(document).on("click", "#btnCancelDeletePermit", function (event) {
    $(PermitDelDialog).dialog("close");
});