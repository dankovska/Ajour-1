function myWeekCalc(date) {
    var checkDate = new Date(date.getTime());
    // Find Thursday of this week starting on Sunday
    checkDate.setDate(checkDate.getDate() + 4 - (checkDate.getDay()));
    var time = checkDate.getTime();
    checkDate.setMonth(0); // Compare with Jan 1
    checkDate.setDate(1);
    return Math.floor(Math.round((time - checkDate) / 86400000) / 7) + 1;
}

function refreshTableAfterSubmit(data) {
    var type = document.getElementById("ADMtableBTs").getElementsByTagName("th");
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
    scrollPositionBTBefore = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
    $("#tbodyBTADM").replaceWith($(data));
    window.oTable.fnSort([[position, sortType]]);
    window.oTable.fnDraw();
    scrollPositionADM = scrollPositionBTBefore;
    $("div.dataTables_scrollBody").scrollTop(window.scrollPositionADM);
}


var scrollPositionADM;

$(document).on("click", "#PlanForAdm", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Plan-BT";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');
    var dateFormat = element.attr('data-date-format');
    scrollPositionADM = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();

    $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Plan",
            position: {
                my: "center",
                at: "center"
            },
            open: function (event, ui) {
                if ($('#planStartDateBTs').length > 0) {
                    $('#planStartDateBTs').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }
                if ($('#planEndDateBTs').length > 0) {
                    $('#planEndDateBTs').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true

                    });
                }
                $("#btnPlanBT").button();
                $("#btnPlanBT").click(function (event) {
                    event.preventDefault();
                    $("#planBTForm").validate();
                    if ($("#planBTForm").valid()) {
                        $.ajax({
                            type: "POST",
                            url: "/BusinessTrip/Plan",
                            data: $("#planBTForm").serialize(),
                            success: function (data) {
                                if (data.error) {
                                    $("#ModelError").html(data.error);
                                }
                                else {
                                    $("#Plan-BT").dialog("close");
                                }
                            },

                            error: function (data) {
                                alert("Server is not responding");

                            }

                        })
                    }
                })


            },
            beforeClose: function () {
                $.ajax({
                    cache: false,
                    url: "/BusinessTrip/GetBusinessTripDataADM/",
                    type: "GET",
                    data: { selectedDepartment: $('#selectedDepartment :selected').val() },
                    success: function (data) {
                        var type = document.getElementById("ADMtableBTs").getElementsByTagName("th");
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
                        scrollPositionBTBefore = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
                        $("#ADMtableBTs").replaceWith($(data));
                        window.oTable.fnSort([[position, sortType]]);
                        window.oTable.fnDraw();
                        scrollPositionADM = scrollPositionBTBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPositionADM);
                    }
                });
            },
            close: function (event, ui) {
                $('#planStartDateBTs').datepicker("destroy");
                $('#planEndDateBTs').datepicker("destroy");
                $(this).dialog("destroy");
                $(this).remove();
            }

        });
        $.validator.unobtrusive.parse(this);
    });
    return false;
});


$(document).on("click", "#EditPlannedBT", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "EditPlanedBT";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');
    var dateFormat = element.attr('data-date-format');
    scrollPositionADM = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
    $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Edit Planned BT",
            position: {
                my: "center",
                at: "center"

            },

            open: function (event, ui) {
                if ($('#editPlannedBTADMStartDate').length > 0) {
                    $('#editPlannedBTADMStartDate').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }
                if ($('#editPlannedBTADMEndDate').length > 0) {
                    $('#editPlannedBTADMEndDate').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true

                    });
                }
                $("#planEditedBT, #btnDeletePlannedBT").button();
                $("#planEditedBT").click(function (event) {
                    event.preventDefault();
                    $("#editPlanedBTForm").validate();
                    if ($("#editPlanedBTForm").valid()) {
                        $.ajax({
                            type: "POST",
                            url: "/BusinessTrip/Plan",
                            data: $("#editPlanedBTForm").serialize(),
                            success: function (data) {
                                if (data.error) {
                                    $("#ModelError").html(data.error);
                                }
                                else {
                                    $("#" + dialogId).dialog("close");
                                    //$("#tbodyBTADM").replaceWith($(data));
                                }
                            },
                            error: function (data) {
                                alert("Server is not responding");
                            }
                        })
                    }
                })
            },
            beforeClose: function () {
                $.ajax({
                    cache: false,
                    url: "/BusinessTrip/GetBusinessTripDataADM/",
                    type: "GET",
                    data: { selectedDepartment: $('#selectedDepartment :selected').val() },
                    success: function (data) {
                        var type = document.getElementById("ADMtableBTs").getElementsByTagName("th");
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
                        scrollPositionBTBefore = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
                        $("#ADMtableBTs").replaceWith($(data));
                        window.oTable.fnSort([[position, sortType]]);
                        window.oTable.fnDraw();
                        scrollPositionADM = scrollPositionBTBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPositionADM);
                    }
                });
            },
            close: function (event, ui) {
                $('#editPlannedBTADMEndDate').datepicker("destroy");
                $('#editPlannedBTADMStartDate').datepicker("destroy");
                $(this).dialog("destroy");
                $(this).remove();
            }

        });
        $.validator.unobtrusive.parse(this);
    });
    return false;
});


//$(document).on("click", "#ShowBTsDataForEmployeeADM", function (event) {
//    event.preventDefault();
//    var element = $(this);
//    var dialogId = "ShowBTsDataForEmployee-ADM";
//    var dialogDiv = "<div id='" + dialogId + "'></div>";
//    var url = $(this).attr('href');
//    $(dialogDiv).load(url, function () {
//        $(this).dialog({
//            modal: true,
//            height: 'auto',
//            width: 'auto',
//            resizable: false,
//            title: "Employee's BT's Data",
//            position: {
//                my: "center center",
//                at: "center center"
//            },

//        });

//        return false;
//    });
//});

$(document).on("click", "#ShowBTDataADM", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('href');
    var dialogId = "ShowBTData-ADM";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    scrollPositionADM = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
    $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: "BT's Data",
            position: {
                my: "center",
                at: "center"
            },

            beforeClose: function () {
                $.ajax({
                    cache: false,
                    url: "/BusinessTrip/GetBusinessTripDataADM/",
                    type: "GET",
                    data: { selectedDepartment: $('#selectedDepartment :selected').val() },
                    success: function (data) {
                        var type = document.getElementById("ADMtableBTs").getElementsByTagName("th");
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
                        scrollPositionBTBefore = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
                        $("#ADMtableBTs").replaceWith($(data));
                        window.oTable.fnSort([[position, sortType]]);
                        window.oTable.fnDraw();
                        scrollPositionADM = scrollPositionBTBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPositionADM);
                    }
                });
            },
        });
        $("#cancelConfirmedBtAdm").button();
        return false;
    });
});

$(document).on("click", "#btnDeletePlannedBT", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('data-href');
    $("#DeletePlannedBT-ADM").load(url, function () {
        var deleteDialog = $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Delete Planned BT",
            position: {
                my: "center",
                at: "center"
            }
        });
        $(this).dialog('open');
        $("#deleteBTADMConfirmation").button();
        $("#deleteBTADMConfirmation").click(function (event) {
            event.preventDefault();
            $.ajax({
                type: "POST",
                url: "/BusinessTrip/DeletePlannedBT",
                data: $("#DeletePlannedBTForm").serialize(),
                success: function (data) {
                    $(deleteDialog).dialog("close");
                    $("#EditPlanedBT").dialog("close");
                   // $("#tbodyBTADM").replaceWith($(data));
                },
                error: function (data) {
                    alert("Server is not responding");
                }
            })
        })
    });

    return false;
});


$(document).on("click", "#registerPlanedBt", function (event) {
    event.preventDefault();
    var BTs = [];
    scrollPositionADM = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
    $("#selectedPlannedBTs:checked").each(function () {
        BTs.push($(this).attr('value'));
    })
    $.ajax({
        type: "POST",
        url: "/BusinessTrip/RegisterPlannedBTs",
        traditional: true,
        data: { selectedPlannedBTs: BTs, selectedDepartment: $('#selectedDepartment :selected').val() },
        success: function(data){
            refreshTableAfterSubmit (data) 
            //$("#tbodyBTADM").replaceWith($(data));
        },
        error: function (data) {
            alert("Server is not responding");
        }
    })

});

$(document).on("click", "#confirmPlanedBt", function (event) {
    event.preventDefault();
    var BTs = [];
    scrollPositionADM = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
    $("#selectedPlannedBTs:checked").each(function () {
        BTs.push($(this).attr('value'));
    })
    $.ajax({
        type: "POST",
        url: "/BusinessTrip/ConfirmPlannedBTs",
        traditional: true,
        data: { selectedPlannedBTs: BTs, selectedDepartment: $('#selectedDepartment :selected').val() },
        success: function (data) {
            refreshTableAfterSubmit(data)
          //  $("#tbodyBTADM").replaceWith($(data));
        },
        error: function (data) {
            alert("Server is not responding");
        }
    })

});

$(document).on("click", "#confirmRegisterBt", function (event) {
    event.preventDefault();
    var BTs = [];
    scrollPositionADM = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
    $("#selectedRegisteredBTs:checked").each(function () {
        BTs.push($(this).attr('value'));
    })

    $.ajax({
        type: "POST",
        url: "/BusinessTrip/ConfirmRegisteredBTs",
        traditional: true,
        data: { selectedRegisteredBTs: BTs, selectedDepartment: $('#selectedDepartment :selected').val() },
        success: function (data) {
            refreshTableAfterSubmit(data)
            //$("#tbodyBTADM").replaceWith($(data));
        },
        error: function (data) {
            alert("Server is not responding");
        }
    })

});

$(document).on("click", "#replanRegisterBt", function (event) {
    event.preventDefault();
    var BTs = [];
    scrollPositionADM = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
    $("#selectedRegisteredBTs:checked").each(function () {
        BTs.push($(this).attr('value'));
    })
    $.ajax({
        type: "POST",
        url: "/BusinessTrip/ReplanRegisteredBTs",
        traditional: true,
        data: { selectedRegisteredBTs: BTs, selectedDepartment: $('#selectedDepartment :selected').val() },
        success: function (data) {
            refreshTableAfterSubmit(data)
        },
        error: function (data) {
            alert("Server is not responding");
        }

    })
});

$(document).on("click", "#cancelRegisterBt", function (event) {
    event.preventDefault();
    var BTs = [];
    scrollPositionADM = $("div#example_wrapper > div.dataTables_scroll > div.dataTables_scrollBody").scrollTop();
    $("#selectedRegisteredBTs:checked").each(function () {
        BTs.push($(this).attr('value'));
    })
    $.ajax({
        type: "POST",
        url: "/BusinessTrip/CancelRegisteredBTs",
        traditional: true,
        data: { selectedRegisteredBTs: BTs, selectedDepartment: $('#selectedDepartment :selected').val() },
        success: function (data) {
            refreshTableAfterSubmit(data)
            //$("#tbodyBTADM").replaceWith($(data));
        },
        error: function (data) {
            alert("Server is not responding");

        }
    })
});

AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('input[name=__RequestVerificationToken]').val();
    return data;
};