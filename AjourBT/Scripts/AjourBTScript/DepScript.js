var dialogId;
var dialogDiv;
var dep;

$(document).on("click", "#CreateDepartment", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Create Department";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');
    $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: 'Create department',
            position: {
                my: "center",
                at: "center"
            },
            buttons: {
                "Save": function () {
                    $("#createDepartmentForm").submit();
                }
            },
            close: function (event, ui) {
                $(this).dialog("destroy");
                $(this).remove();
            }

        });
        $.validator.unobtrusive.parse(this);
    });

    return false;
});

$(document).on("click", ".depEditDialog", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('href');
    dialogId = "Edit department"
    dialogDiv = "<div id='" + dialogId + "'></div>";
    dep = $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: "Edit Department",
            position: {
                my: "center",
                at: "center"
            },

            open: function () {
                $("#btnSaveDepartment").click(function (event) {
                    event.preventDefault();
                    $("#editDepartmentForm").validate();
                    if ($("#editDepartmentForm").valid()) {
                        $.ajax({
                            cache: false,
                            url: "/Department/Edit/",
                            type: "POST",
                            data: AddAntiForgeryToken($("#editDepartmentForm").serialize()),
                            success: function (data) {
                                if (data.error) {
                                    $("#ModelError").html(data.error);
                                }
                                else {
                                    $(dep).dialog("close");
                                    $("#CreateDepartment").button();
                                }

                            }

                        });
                    }
                })
            },
            
            beforeClose: function () {
                $.ajax({
                    cache: false,
                    url: "/Department/Index/",
                    type: "GET",
                    success: function (data) {
                        $("#DepartmentData").replaceWith($(data));
                    }
                });

            },

            close: function (event, ui) {
                $(this).dialog("destroy");
                $(this).remove();
            }
        });
        $("#btnSaveDepartment, #btnDeleteDepartment").button();

        $.validator.unobtrusive.parse(this);
    });

    return false;
});


$(document).on("click", "#btnDeleteDepartment", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('data-href');
    var title = $(this).attr('data-title');
    $("#deleteDepartment-Confirm").load(url, function () {

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

    return false;
});

$(document).on("click", "#btnCancelDeleteDepartment", function (event) {
    $("#deleteDepartment-Confirm").dialog("close");
});


    AddAntiForgeryToken = function (data) {
        data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
        return data;
    };