﻿function myWeekCalc(date) {
    var checkDate = new Date(date.getTime());
    // Find Thursday of this week starting on Sunday
    checkDate.setDate(checkDate.getDate() + 4 - (checkDate.getDay()));
    var time = checkDate.getTime();
    checkDate.setMonth(0); // Compare with Jan 1
    checkDate.setDate(1);
    return Math.floor(Math.round((time - checkDate) / 86400000) / 7) + 1;
}

var scrollPosition;
var dialogDelVisa

AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#CreateVisaForm input[name=__RequestVerificationToken]').val();
    return data;
};

$(document).on("click", "#CreateVisa", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "visaDialog-create";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');
    var dateFormat = element.attr('data-date-format');
    var parentID = element.parent().parent().attr("id");
    scrollPosition = $("div#visasView_wrapper>div.dataTables_scroll>div.dataTables_scrollBody").scrollTop();
    $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Create Visa",
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
                        $("#CreateVisaForm").validate();
                        if ($("#CreateVisaForm").valid()) {
                            visa = $('#CreateVisaForm').serialize();
                            $.ajax({
                                type: "POST",
                                url: "/Visa/Create",
                                data: visa,

                                success: function (data) {
                                    if (data.error) {
                                        $("#ModelError").html(data.error);
                                    }
                                    else {
                                        $("#" + dialogId).dialog("close");
                                        //$("#VisasViewBody").replaceWith(data);
                                    }
                                },

                                error: function (data) {
                                    alert("Server is not responding")

                                }
                            });
                        }
                    }
                }
            },
            open: function (event, ui) {
                if ($('#createStartDate').length > 0) {
                    $('#createStartDate').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }
                if ($('#createDueDate').length > 0) {
                    $('#createDueDate').datepicker({
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
                $('#createStartDate').datepicker("destroy");
                $('#createDueDate').datepicker("destroy");
                $(this).dialog("destroy");
                $(this).remove();
            }

        });
        $.validator.unobtrusive.parse(this);
    });
    return false;
});

$(document).on("click", ".visaEditDialog", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "visaDialog-edit";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var dateFormat = element.attr('data-date-format');
    var url = $(this).attr('href');
    scrollPosition = $("div#visasView_wrapper>div.dataTables_scroll>div.dataTables_scrollBody").scrollTop();
    var parentID = element.parent().parent().attr("id");
    $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Edit Visa",
            position: {
                my: "center",
                at: "center"
            },

            open: function (event, ui) {

                if ($('#editStartDate').length > 0) {
                    $('#editStartDate').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }
                if ($('#editDueDate').length > 0) {
                    $('#editDueDate').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true

                    });
                }

                $("#btnSaveVisa,#btnDeleteVisa").button();
                $("#btnSaveVisa").click(function (event) {
                    event.preventDefault();
                    $("#EditVisaForm").validate();
                    if ($("#EditVisaForm").valid()) {
                        $.ajax({
                            type: "POST",
                            url: "/Visa/Edit",
                            data: $('#EditVisaForm').serialize(),
                            success: function (data) {
                                $("#EditVisaForm").validate();
                                if ($("#EditVisaForm").valid()) {
                                    if (data.error) {
                                        $("#ModelError").html(data.error);
                                        //}
                                    }
                                    else {

                                        $("#" + dialogId).dialog("close");
                                        //$("#VisasViewBody").replaceWith(data);
                                    }
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
                $('#editStartDate').datepicker("destroy");
                $('#editDueDate').datepicker("destroy");
                $(this).dialog("destroy");
                $(this).remove();
            }
        });
        $.validator.unobtrusive.parse(this);
    });
    return false;
});

$(document).on("click", "#btnDeleteVisa", function (event) {
    event.preventDefault();
    var url = $(this).attr('data-href');
    var parentID = $(this).attr('data-updateID');
    $("#deleteEmployee-Confirm").load(url, function () {
        dialogDelVisa = $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Delete visa",
            position: {
                my: "center",
                at: "center"
            },
        })
        $("#OKDeleteVisa,#btnCancelDeleteVisa").button();
        $("#OKDeleteVisa").click(function (event) {
            event.preventDefault();
            visa = $('#DeleteVisaForm').serialize();
            $.ajax({
                type: "POST",
                url: "/Visa/Delete",
                data: visa,

                success: function (data) {
                    $("#visaDialog-edit").dialog("close");
                    $(dialogDelVisa).dialog("close");
                    // $("#VisasViewBody").replaceWith(data);
                },

                error: function (data) {
                    alert('Server not responding');
                }
            });
        })
    });

    return false;
});

$(document).on("click", "#btnCancelDeleteVisa", function (event) {
    $(dialogDelVisa).dialog("close");
});

$(document).on("click", "#CreateVisaRegistrationDate", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "visaRegistrationDateDialog-create";
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
            title: 'Create Visa Registration Date',
            position: {
                my: "center",
                at: "center"
            },

            open: function (event, ui) {
                if ($('#RegistrationDateCreate').length > 0) {
                    $('#RegistrationDateCreate').datepicker({
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
            buttons: {
                "Save": {
                    text: 'Save',
                    id: "btnSaveRegDate",
                    click: function (event) {
                        event.preventDefault();
                        var reg = new RegExp("^([01]?[0-9]|2[0-3]):[0-5][0-9]$");
                        var regResult = reg.test($('#visaRegTime').val().trim());
                        if ($('#visaRegTime').val().trim().length == 0) {
                            regResult = true;
                        }
                        if (regResult == true) {
                            $("#CreateVisaregDateForm").validate();
                            if ($("#CreateVisaregDateForm").valid()) {
                                $.ajax({
                                    type: "POST",
                                    url: "/VisaRegistrationDate/Create",
                                    data: AddAntiForgeryToken($('#CreateVisaregDateForm').serialize()),
                                    success: function (data) {
                                        if (data.error) {
                                            $("#ModelError").html(data.error);
                                        }
                                        else {
                                            $("#" + dialogId).dialog("close");
                                            //$("#VisasViewBody").replaceWith(data);
                                        }
                                    },

                                    error: function (data) {
                                        alert("Server is not responding");
                                    }
                                });
                            }
                        }
                        else {
                            alert("Time format is incorrect,\n please enter hh:mm");
                        }
                    }
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
                $('#RegistrationDateCreate').datepicker("destroy");
                $(this).dialog("destroy");
                $(this).remove();
            }
        });
        $.validator.unobtrusive.parse(this);

    });
    return false;
});

$(document).on("click", ".visaRegistrationDateEdit", function (event) {
    event.preventDefault();

    var element = $(this);
    var dialogId = "editVisaRegistrationDateDialogEdit";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var dateFormat = element.attr('data-date-format');
    var url = element.attr('href');
    var parentID = element.parent().parent().attr("id");
    scrollPosition = $("div#visasView_wrapper>div.dataTables_scroll>div.dataTables_scrollBody").scrollTop();

    $(dialogDiv).load(url, function () {
        var dialogRegDate = $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: 'Update Visa Registration Date',
            position: {
                my: "center",
                at: "center"
            },


            open: function (event, ui) {

                $('#editVisaRegDate').datepicker({
                    firstDay: 1,
                    dateFormat: dateFormat,
                    showWeek: true,
                    calculateWeek: myWeekCalc,
                    showOn: 'button',
                    buttonImage: '/Content/themes/base/images/calendar2.gif',
                    buttonImageOnly: true
                });


                $("#btnSave,#btnDeleteVisaRegistrationDate").button();
                $("#btnSave").click(function (event) {
                    event.preventDefault();

                    var reg = new RegExp("^([01]?[0-9]|2[0-3]):[0-5][0-9]$");
                    var regResult = reg.test($('#editVisaRegTime').val().trim());

                    if ($('#editVisaRegTime').val().trim().length == 0) {
                        regResult = true;
                    }
                    if (regResult == true) {


                        $("#EditRegDateForm").validate();
                        if ($("#EditRegDateForm").valid()) {
                            $.ajax({
                                type: "POST",
                                url: "/VisaRegistrationDate/Edit",
                                data: AddAntiForgeryToken($('#EditRegDateForm').serialize()),

                                success: function (data) {
                                    if (data.error) {
                                        $("#ModelError").html(data.error);
                                    }
                                    else {
                                        (dialogRegDate).dialog("close");
                                        //$("#VisasViewBody").replaceWith($(data));
                                    }
                                },
                                error: function (data) {
                                    alert('Server not responding');
                                }
                            });
                        }
                    }

                    else {
                        alert("Time format is incorrect,\n please enter hh:mm");
                    }
                }),

                $("#btnDeleteVisaRegistrationDate").click(function (event) {
                    event.preventDefault();
                    visa = $('#DeleteRegDate').serialize();
                    $.ajax({
                        type: "POST",
                        url: "/VisaRegistrationDate/Delete",
                        data: visa,
                        success: function (data) {
                            $(dialogRegDate).dialog("close");
                            //$("#VisasViewBody").replaceWith($(data));
                        },

                        error: function (data) {
                            alert('Server not responding');

                        }
                    });

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
                $('#editVisaRegDate').datepicker("destroy");
                (dialogRegDate).dialog("destroy");
                (dialogRegDate).remove();
            }
        });
        $.validator.unobtrusive.parse(this);

    });
    return false;
});

AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
    return data;
};

function ReloadPage() {
    var searchString = document.getElementById('seachInput').value;
    $.ajax({
        url: "/Visa/GetVisaDataBTM/",
        data: { searchString: searchString },
        success: function (data) {
            $("#tableBodyVisa").html(data);
        }
    });
}

$(document).on("change", "#HasPassport", function (event) {
    event.preventDefault();
    var parentID = $(this).parent().parent().attr("id");
    var eid = $(this).attr('value');
    var ich = $(this).attr('checked');
    scrollPosition = $("div#visasView_wrapper>div.dataTables_scroll>div.dataTables_scrollBody").scrollTop();
    $.ajax({
        url: "/Passport/ModifyPassport/",
        type: "post",
        data: AddAntiForgeryToken({ id: eid, isChecked: ich, searchString: document.getElementById('seachInput').value }),
        success: function (data) {
            if (data.error) {
                var dialog = $("<div />", { text: data.error }).dialog(
                {
                    title: "Error",
                    buttons:
                    {
                        "OK": function () {
                            $(dialog).dialog("close");
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


                    close: function () {
                        $(dialog).dialog("destroy");
                    }
                });
            }
            else {
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

                //$("#VisasViewBody").replaceWith(data);
            }
        }
    });
    return false;
});
