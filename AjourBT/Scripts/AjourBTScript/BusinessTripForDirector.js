
$(document).on("click", "#RejectBTbyDIR", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Reject-BT";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    $(dialogDiv).load(this.href, function () {
        $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "Reject BT",
            position: {
                my: "center",
                at: "center",
                of: "#tabs15"
            },
            open: function () {
                $("#RejectBtnDIR").button();
                $("#RejectBtnDIR").click(function (event) {
                    event.preventDefault();
                    $("#RejectBTDIRForm").validate;
                    if ($("#RejectBTDIRForm").valid()) {
                        $.ajax({
                            cache: false,
                            url: "/BusinessTrip/Reject_BT_DIR/",
                            type: "POST",
                            data: $("#RejectBTDIRForm").serialize(),
                            success: function (data) {
                                if (data.error) {
                                    $("#ModelError").html(data.error);
                                }
                                else {
                                    $("#" + dialogId).dialog("close");
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
                    url: "/BusinessTrip/GetBusinessTripDataDIR/",
                    type: "Get",
                    data: { selectedDepartment: $('#selectedDepartment :selected').val() },
                    success: function (data) {
                        $("#BTsDataForDirector").replaceWith($(data));
                    }

                })
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


AddAntiForgeryToken = function (data) {
    data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
    return data;
};




