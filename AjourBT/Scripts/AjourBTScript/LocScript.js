var loc;

$(document).on("click", "#CreateLocation", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "CreateLocation";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');
    $(dialogDiv).load(url, function () {
        var createLoc = $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: 'Create Location',
            position: {
                my: "center",
                at: "center"
            },
            buttons: {
                "Save": function () {
                    //$("#createLocationForm").submit();
                    event.preventDefault();
                    $("#createLocationForm").validate();
                    if ($("#createLocationForm").valid()) {
                        $.ajax({
                            type: "POST",
                            url: "/Location/Create",
                            data: $("#createLocationForm").serialize(),
                            success: function (data) {
                                if (data.error) {
                                    $("#ModelError").html(data.error);
                                }
                                else {
                                    $(createLoc).dialog("close");
                                }
                            },
                            error: function (data) {
                                alert("Server is not responding");
                            }
                        })
                    }
                }
            },

            beforeClose: function() {
                $.ajax({
                    cache: false,
                    url: "/Location/Index/",
                    type: "GET",
                    success: function (data) {                
                        $("#LocationData").replaceWith($(data));
                    }
                })
            },

            close: function (event, ui) {
                $(createLoc).dialog("destroy");
                $(createLoc).remove();
            }
        });
        $.validator.unobtrusive.parse(this);
    });
    return false;
});

$(document).on("click", ".locEditDialog", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('href');
    var dialogId = "Edit Location"
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    loc = $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: "Edit Location",
            position: {
                my: "center",
                at: "center"
            },

            open: function () {
                $("#btnSaveLocation").click(function (event) {
                    event.preventDefault();
                    $("#editLocationForm").validate();
                    if ($("#editLocationForm").valid()) {
                        $.ajax({
                            cache: false,
                            url: "/Location/Edit/",
                            type: "POST",
                            data: AddAntiForgeryToken($("#editLocationForm").serialize()),
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
                })
            },

            beforeClose: function () {
                $.ajax({
                    cache: false,
                    url: "/Location/Index/",
                    type: "GET",
                    success: function (data) {
                        $("#LocationData").replaceWith($(data));
                    }
                });
            },

            close: function (event, ui) {
                $(this).dialog("destroy");
                $(this).remove();
            }
        });

        $("#btnSaveLocation, #btnDeleteLocation").button();

        $.validator.unobtrusive.parse(this);
    });
    return false;
});

$(document).on("click", "#btnDeleteLocation", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('data-href');
    var title = $(this).attr('data-title');
    $("#deleteLocation-Confirm").load(url, function () {

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

//$(document).on("click", "#AddResponsibleForLoc", function (event) {
//    event.preventDefault();
//    var element = $(this);
//    var dialogId = "responsibleForLocDialog-create";
//    var dialogDiv = "<div id='" + dialogId + "'></div>";
//    var url = element.attr('href');

//    $(dialogDiv).load(url, function () {
//        $(this).dialog({
//            modal: true,
//            height: "auto",
//            width: "auto",
//            resizable: false,
//            title: "Add Responsible For Location",
//            position: {
//                my: "center",
//                at: "center"
//            },

//            open: function (event, ui) {
//                $("#btnSaveResponsibleForLoc").button();
//                $("#btnSaveResponsibleForLoc").click(function (event) {
//                    event.preventDefault();
//                    $("#AddResponsibleForLocForm").validate();
//                    if ($("#AddResponsibleForLocForm").valid()) {
//                        $.ajax({
//                            type: "POST",
//                            url: "/Location/AddResponsibleForLoc/",
//                            data: AddAntiForgeryToken($('#AddResponsibleForLocForm').serialize()),
//                            success: function (data) {
//                                if (data.error) {
//                                    $("#ModelError").html(data.error);
//                                }
//                                else {
//                                    $("#" + dialogId).dialog("close");
//                                }
//                            },
//                            error: function (data) {
//                                alert("Server is not responding");
//                            }
//                        })
//                    }
//                })
//            },

//            beforeClose: function () {
//                $.ajax({
//                    cache: false,
//                    url: "/Location/Index/",
//                    type: "GET",
//                    success: function (data) {
//                        $("#LocationData").replaceWith($(data));
//                    }
//                });
//            },

//            close: function (event, ui) {
//                $(this).dialog("destroy");
//                $(this).remove();
//            }
//        });
//        $.validator.unobtrusive.parse(this);
//    });
//    return false;
//});

//$(document).on("click", "#responsibleForLocEdit", function (event) {
//    event.preventDefault();
//    var element = $(this);
//    var dialogId = "responsibleForLoc-edit";
//    var dialogDiv = "<div id='" + dialogId + "'></div>";
//    var url = element.attr('href');
//    $(dialogDiv).load(url, function () {
//        $(this).dialog({
//            modal: true,
//            height: "auto",
//            width: "auto",
//            resizable: false,
//            title: 'Update Responsible For Location',
//            position: {
//                my: "center",
//                at: "center"
//            },

//            open: function (event, ui) {
//                $("#btnSaveEditedResponsibleForLoc").button();
//                $("#btnSaveEditedResponsibleForLoc").click(function (event) {
//                    event.preventDefault();
//                    $("#editResponsibleForLocForm").validate();
//                    if ($("#editResponsibleForLocForm").valid()) {
//                        $.ajax({
//                            type: "POST",
//                            url: "/Location/EditResponsibleForLoc/",
//                            data: $("#editResponsibleForLocForm").serialize(),
//                            success: function (data) {
//                                if (data.error) {
//                                    $("#ModelError").html(data.error);
//                                }
//                                else {
//                                    $("#" + dialogId).dialog("close");
//                                }
//                            },
//                            error: function (data) {
//                                alert("Server is not responding");
//                            }
//                        })
//                    }
//                })
//            },

//            beforeClose: function () {
//                $.ajax({
//                    cache: false,
//                    url: "/Location/Index/",
//                    type: "GET",
//                    success: function (data) {
//                        $("#LocationData").replaceWith($(data));
//                    }
//                });
//            },

//            close: function (event, ui) {
//                $(this).dialog("destroy");
//                $(this).remove();
//            }
//        });
//        $.validator.unobtrusive.parse(this);
//    });
//    return false;
//});

$(document).on("click", "#btnCancelDeleteLocation", function (event) {
    $("#deleteLocation-Confirm").dialog("close");
});
