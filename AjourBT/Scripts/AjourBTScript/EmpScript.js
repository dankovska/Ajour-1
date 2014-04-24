function myWeekCalc(date) {
    var checkDate = new Date(date.getTime());
    // Find Thursday of this week starting on Sunday
    checkDate.setDate(checkDate.getDate() + 4 - (checkDate.getDay()));
    var time = checkDate.getTime();
    checkDate.setMonth(0); // Compare with Jan 1
    checkDate.setDate(1);
    return Math.floor(Math.round((time - checkDate) / 86400000) / 7) + 1;
}

var scrollPosition;
var dialogEmpEdit;

AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
    return data;
};
$(document).on("click", "#CreateEmployee", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Create Employee";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var dateFormat = element.attr('data-date-format');
    $(dialogDiv).load(this.href, function () {
        var dialogEmpCreate = $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Create Employee",
            position: {
                my: "center",
                at: "center"
            },
            open: function (event, ui) {
                if ($('#createDateEmployed').length > 0) {
                    $('#createDateEmployed').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });

                }
                if ($('#createDateDismissed').length > 0) {
                    $('#createDateDismissed').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true

                    });
                }
                if ($('#createDateBirthDay').length > 0) {
                    $('#createDateBirthDay').datepicker({
                        firstDay: 1,
                        changeMonth: true,
                        changeYear: true,
                        yearRange: "-100:+100",
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true

                    });
                }

                $("#btnSaveOnCreateEmployee").click(function (event) {
                    event.preventDefault();
                    $("#createEmployeeForm").validate();
                    if ($("#createEmployeeForm").valid()) {
                        createEmp = $('#createEmployeeForm').serialize();
                        $.ajax({
                            type: "POST",
                            url: "/Employee/Create",
                            data: createEmp,
                            success: function (data) {
                                if (data.error) {
                                    $("#ModelError").html(data.error);
                                }
                                else {
                                    $(dialogEmpCreate).dialog("close");
                                }
                            },
                            error: function (data) {
                                alert('Server not responding');
                            }
                        });
                    }
                });
            },

            beforeClose: function () {
                $.ajax({
                    cache: false,
                    url: "/Employee/GetEmployeeData/",
                    type: "GET",
                    data: { selectedDepartment: $('#depDropList :selected').val(), searchString: $("#seachInput").val() },
                    success: function (data) {

                        var type = document.getElementById("EmployeeData").getElementsByTagName("th");
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
                        scrollPositionBTBefore = $("div.dataTables_scrollBody").scrollTop();
                        $("#EmployeeData").replaceWith($(data));
                        window.oTable.fnSort([[position, sortType]]);
                        window.oTable.fnDraw();
                        scrollPosition = scrollPositionBTBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPosition);

                        //$("#EmployeeData").replaceWith($(data));
                    }
                })
            },

            close: function (event, ui) {
                $('#createDateEmployed').datepicker("destroy");
                $('#createDateDismissed').datepicker("destroy");
                $('#createDateBirthDay').datepicker("destroy");
                $(this).dialog("destroy");
                $(this).remove();
            },
            //buttons: {
            //    "Save": function () {
            //        $("#createEmployeeForm").submit();
            //    }
            //}


        });
        $("#btnSaveOnCreateEmployee").button();
        $.validator.unobtrusive.parse(this);
    });
    return false;
});

$(document).on("click", ".empEditDialog", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Edit Employee";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var dateFormat = element.attr('data-date-format');
    scrollPosition = $("div.dataTables_scrollBody").scrollTop();
    $(dialogDiv).load(this.href, function () {
        dialogEmpEdit = $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Edit Employee",
            position: {
                my: "center center",
                at: "center center"
            },
            open: function (event, ui) {
                if ($('#editDateEmployed').length > 0) {
                    $('#editDateEmployed').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }
                if ($('#editDateDismissed').length > 0) {
                    $("#editDateDismissed").datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true
                    });
                }

                if ($('#editDateBirthDay').length > 0) {
                    $('#editDateBirthDay').datepicker({
                        firstDay: 1,
                        dateFormat: dateFormat,
                        showWeek: true,
                        calculateWeek: myWeekCalc,
                        showOn: 'button',
                        buttonImage: '/Content/themes/base/images/calendar2.gif',
                        buttonImageOnly: true

                    });
                }

                $("#btnSaveEmployee").click(function (event) {
                    event.preventDefault();
                    $("#editEmployeeForm").validate();
                    if ($("#editEmployeeForm").valid()) {
                        visa = $('#editEmployeeForm').serialize();
                        $.ajax({
                            type: "POST",
                            url: "/Employee/Edit",
                            data: visa,
                            success: function (data) {
                                if (data.error) {
                                    $("#ModelError").html(data.error);
                                }
                                else {
                                $(dialogEmpEdit).dialog("close");
                               // $("#PUEmployee").replaceWith(data);
                                }
                            },

                            error: function (data) {
                                alert('Server not responding');
                            }
                        });
                    }
                });

            },

            beforeClose: function(){
                $.ajax({
                    cache: false,
                    url: "/Employee/GetEmployeeData/",
                    type: "GET",
                    data: { selectedDepartment: $('#depDropList :selected').val(), searchString: $("#seachInput").val() },
                    success: function (data) {

                        var type = document.getElementById("EmployeeData").getElementsByTagName("th");
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
                        scrollPositionBTBefore = $("div.dataTables_scrollBody").scrollTop();
                        $("#EmployeeData").replaceWith($(data));
                        window.oTable.fnSort([[position, sortType]]);
                        window.oTable.fnDraw();
                        scrollPosition = scrollPositionBTBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPosition);

                        //$("#EmployeeData").replaceWith($(data));
                    }
                })
            },

            close: function (event, ui) {
                $('#editDateEmployed').datepicker("destroy");
                $("#editDateDismissed").datepicker("destroy");
                $('#editDateBirthDay').datepicker("destroy");
                $(this).dialog("destroy");
                $(this).remove();
            }
        });
        $("#btnSaveEmployee, #btnDeleteEmployee, #btnResetPassword").button();
        $.validator.unobtrusive.parse(this);
    });
    return false;
});

$(document).on("click", "#btnResetPassword", function (event) {
    event.preventDefault();
    editEmployeeForm = $('#editEmployeeForm').serialize();
    $.ajax({
        url: "/Employee/ResetPassword/",
        data: editEmployeeForm,
        method: "get",
        success: function (data) {
            var rcdialog = $("#resetPassword-Confirm").html(data).dialog({
                title: "Reset Password",
                buttons: {
                    "Ok": function (event) {
                        $.ajax({
                            url: "/Employee/ResetPasswordConfirmed/",
                            type: "post",
                            data: editEmployeeForm,
                            success: function (event) {
                                $(rcdialog).dialog("destroy");
                                var dialog = $("<div />", { text: event }).dialog(
                                {
                                    title: "Reset password",
                                    buttons:
                                    {
                                        "OK": function () {
                                            $(dialog).dialog("destroy");
                                        }
                                    },
                                    close: function () {
                                        $(dialog).dialog("destroy");
                                    }
                                });
                            }
                        })
                    },
                    "Cancel": function () {
                        $(rcdialog).dialog("destroy");
                    }
                },
                close: function () {
                    $(rcdialog).dialog("destroy");
                }
            })

        }
    })
    return false;
});

$(document).on("click", "#btnDeleteEmployee", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('data-href');
    var title = $(this).attr('data-title');
    $("#deleteEmployee-Confirm").load(url, function () {
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
        })

        $("#OKDeleteEmployee").button();
        $("#OKDeleteEmployee").click(function (event) {
            scrollPosition = $("div.dataTables_scrollBody").scrollTop();
            event.preventDefault();
            $("#DeleteEmployeeForm").validate();
            if ($("#DeleteEmployeeForm").valid()) {
                visa = $('#DeleteEmployeeForm').serialize();
                $.ajax({
                    type: "POST",
                    url: "/Employee/Delete",
                    data: visa,

                    success: function (data) {
                        $("#deleteEmployee-Confirm").dialog("close");
                        $(dialogEmpEdit).dialog("close");
                        $("#PUEmployee").replaceWith(data);
                    },

                    error: function (data) {
                        alert('Server not responding');

                    }
                });
            }
        })

    });
    return false;
});

$(document).on("click", "#btnCancelDeleteEmployee", function (event) {
    $("#deleteEmployee-Confirm").dialog("close");
});

$(document).on("change", "#HasPassportEmp", function (event) {
    event.preventDefault();
    var eid = $(this).attr('value');
    var ich = $(this).attr('checked');
    $.ajax({
        url: "/Passport/ModifyPassport/",
        type: "post",
        data: AddAntiForgeryToken({ id: eid, isChecked: ich }),
        success: function (data) {
            $('#response').html(data);

        }
    });
    return false;
});