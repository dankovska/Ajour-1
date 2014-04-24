$(document).on('click','#CreateCountry', function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = 'Create Country';
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var url = $(this).attr('href');

    $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            rasizable: false,
            title: 'Create Country',
            position: {
                my: 'center',
                at: 'center'
            },
            buttons: {
                "Save": function () {
                    $('#createCountryForm').submit();
                }
            },
            close: function (event, ui) {
                $(this).dialog('destroy');
                $(this).remove();
            }
        })
        $.validator.unobtrusive.parse(this);
    })
})

$(document).on("click", ".countryEditDialog", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('href');
    var dialogId = "Edit Country"
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    loc = $(dialogDiv).load(url, function () {
        $(this).dialog({
            modal: true,
            height: "auto",
            width: "auto",
            resizable: false,
            title: "Edit Country",
            position: {
                my: "center",
                at: "center"
            },

            open: function () {
                $("#btnSaveCountry").click(function (event) {
                    event.preventDefault();
                    $("#editCountryForm").validate();
                    if ($("#editCountryForm").valid()) {
                        $.ajax({
                            cache: false,
                            url: "/Country/Edit/",
                            type: "POST",
                            data: AddAntiForgeryToken($("#editCountryForm").serialize()),
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
                    url: "/Country/Index/",
                    type: "GET",
                    success: function (data) {
                        $("#CountryData").replaceWith($(data));
                    }
                });
            },

            close: function (event, ui) {
                $(this).dialog("destroy");
                $(this).remove();
            }
        });

        $("#btnSaveCountry, #btnDeleteCountry").button();

        $.validator.unobtrusive.parse(this);
    });
    return false;
});



$(document).on("click", "#btnDeleteCountry", function (event) {
    event.preventDefault();
    var element = $(this);
    var url = $(this).attr('data-href');
    var title = $(this).attr('data-title');
    $("#deleteCountry-Confirm").load(url, function () {

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

$(document).on("click", "#btnCancelDeleteCountry", function (event) {
    $("#deleteCountry-Confirm").dialog("close");
});