$(document).on("click", "#ReloadMessagesButtonBTM", function (event) {
    event.preventDefault();
    $.ajax({
        url: $(this).attr("href"),
        success: function (response) {
            $("#tabsBTM #tabs-11 #messagesContainer").html($(response).find("#tabs-11 #messagesContainer"));
            //$("[id^=ReloadMessagesButton]").button();
        }
    });
    return false;
});

$(document).on("click", "#ReloadMessagesButtonADM", function (event) {
    event.preventDefault();
    $.ajax({
        url: $(this).attr("href"),
        success: function (response) {
            $("#tabsADM #tabs-8 #messagesContainer").html($(response).find("#tabs-8 #messagesContainer"));
        }
    });
    return false;
});

$(document).on("click", "#ReloadMessagesButtonACC", function (event) {
    event.preventDefault();
    $.ajax({
        url: $(this).attr("href"),
        success: function (response) {
            $("#tabsACC #tabs-14 #messagesContainer").html($(response).find("#tabs-14 #messagesContainer"));
        }
    });
    return false;
});

$(document).on("click", "#ReloadMessagesButtonDIR", function (event) {
    event.preventDefault();
    $.ajax({
        url: $(this).attr("href"),
        success: function (response) {
            $("#tabsDIR #tabs16 #messagesContainer").html($(response).find("#tabs16 #messagesContainer"));
        }
    });
    return false;
});

$(document).on("click", "#ReloadMessagesButtonPU", function (event) {
    event.preventDefault();
    $.ajax({
        url: $(this).attr("href"),
        success: function (response) {
            $("#tabsPU #tabs6 #messagesContainer").html($(response).find("#tabs6 #messagesContainer"));
        }
        
    });

    return false;
});

//    $(function () {
//        $("#ReloadMessagesButtonPU")
//          .button()
//    });

//$(function () {
//    $("#ReloadMessagesButtonDIR")
//      .button()
//});
//$(function () {
//    $("#ReloadMessagesButtonACC")
//      .button()
//});
//$(function () {
//    $("#ReloadMessagesButtonADM")
//      .button()
//});
//$(function () {
//    $("#ReloadMessagesButtonBTM")
//      .button()
//});
