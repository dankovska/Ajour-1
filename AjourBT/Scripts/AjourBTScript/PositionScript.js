var pos;

$(document).on("click", "#CreatePosition", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Create Position";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');
    $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: 'Create Position',
            position: {
                my: "center",
                at: "center"
            },
            buttons: {
                "Save": function () {
                    $("#createPositionForm").submit();
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

$(document).on("click", ".posEditDialog", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('href');
    var dialogId = "Edit Position"
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    pos = $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: "Edit Position",
            position: {
                my: "center",
                at: "center"
            },

            open: function () {
            $("#btnSavePosition").click(function (event) {
                event.preventDefault();
                $("#editPositionForm").validate();
                if ($("#editPositionForm").valid()) {
                    $.ajax({
                        cache: false,
                        url: "/Position/Edit/",
                        type: "Post",
                        data: AddAntiForgeryToken($("#editPositionForm").serialize()),
                        success: function (data) {
                            if (data.error) {
                                $("#PositionModelError").html(data.error);
                            }
                            else {
                                $(pos).dialog("close");
                            }
                        }

                    });
                }
            })
            },
            beforeClose: function () {
                $.ajax({
                    cache: false,
                    url: "/Position/Index/",
                    type: "GET",
                    success: function (data) {
                        $("#PositionData").replaceWith($(data));
                    }
                });
            },

            close: function (event, ui) {
                $(this).dialog("destroy");
                $(this).remove();
            }


        });
        $("#btnSavePosition, #btnDeletePosition").button();
        $.validator.unobtrusive.parse(this);
    });
    return false;
});

$(document).on("click", "#btnDeletePosition", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('data-href');
    var title = $(this).attr('data-title');
    $("#deletePosition-Confirm").load(url, function () {

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

$(document).on("click", "#btnCancelDeletePosition", function (event) {
    $("#deletePosition-Confirm").dialog("close");
});
