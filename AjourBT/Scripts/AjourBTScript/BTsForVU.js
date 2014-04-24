var scrollPositionVUBTsDataInQuarters;
var scrollPositionVUPrepBTsData;

$(document).on("click", "a#ShowBTInformation", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Show BT Data";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var dateFormat = element.attr('data-date-format');
    scrollPositionVUBTsDataInQuarters = $('div#tableBodyVUForQuarter > div#VUBusinessTripDataInQuarter > div#BTsInQuarterForViewerexample_wrapper >  div.dataTables_scroll > div.dataTables_scrollBody').scrollTop();
    $(dialogDiv).load(this.href, function () {
        $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "BT's Data",
            position: {
                my: "center",
                at: "center"
            },
            open: function (event, ui) {
                $(this).load(url);
            },
            beforeClose: function () {
                $.ajax({
                    cache: false,
                    url: "/BusinessTripViewer/GetBusinessTripDataInQuarterVU/",
                    type: "Get",
                    data: { selectedKey: $('#selectedKey :selected').val(), selectedDepartment: $("#ListOfDepartments :selected").val(), searchString: $('#BTsInQuarterForViewerexample_filter input').val() },
                    success: function (data) {

                        var type = document.getElementById("VUBusinessTripDataInQuarter").getElementsByTagName("th");
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
                        scrollPositionBTBefore = $('div#tableBodyVUForQuarter > div#VUBusinessTripDataInQuarter > div#BTsInQuarterForViewerexample_wrapper >  div.dataTables_scroll > div.dataTables_scrollBody').scrollTop();
                        //alert(data);
                        $("#VUBusinessTripDataInQuarter").replaceWith($(data));
                        window.sortTable.fnSort([[position, sortType]]);
                        window.sortTable.fnDraw();
                        scrollPositionVUBTsDataInQuarters = scrollPositionBTBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPositionVUBTsDataInQuarters);

                        //$("#VUBusinessTripDataInQuarter").replaceWith($(data));
                    }
                    //},
                    //error: function (data){
                    //    alert(data);
                    //}

                })
            }
        });
        $("#ShowBTInformation-VU").dialog('open');
        return false;
    });
});




$(document).on("click", "a#PrepBTsDataShowBTInformation", function (event) {
    event.preventDefault();
    var element = $(this);
    var dialogId = "Show BT Data";
    var dialogDiv = "<div id='" + dialogId + "'></div>";
    var dateFormat = element.attr('data-date-format');
    scrollPositionVUPrepBTsData = $('div#PrepBTsDataVU > div#prepBTDataVU_wrapper > div.dataTables_scroll > div.dataTables_scrollBody').scrollTop();
    $(dialogDiv).load(this.href, function () {
        $(this).dialog({
            modal: true,
            height: 'auto',
            width: 'auto',
            resizable: false,
            title: "BT's Data",
            position: {
                my: "center",
                at: "center"
            },
            open: function (event, ui) {
                $(this).load(url);
            },
            beforeClose: function () {
                $.ajax({
                    cache: false,
                    url: "/BusinessTripViewer/GetPrepBusinessTripDataVU/",
                    type: "Get",
                    success: function (data) {

                        var type = document.getElementById("PrepBTsDataVU").getElementsByTagName("th");
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
                        scrollPositionBTBefore = $('div#PrepBTsDataVU > div#prepBTDataVU_wrapper > div.dataTables_scroll > div.dataTables_scrollBody').scrollTop();
                        $("#PrepBTsDataVU").replaceWith($(data));
                        window.sortTable.fnSort([[position, sortType]]);
                        window.sortTable.fnDraw();
                        scrollPositionVUPrepBTsData = scrollPositionBTBefore;
                        $("div.dataTables_scrollBody").scrollTop(window.scrollPositionVUPrepBTsData);


                        //$("#PrepBTsDataVU").replaceWith($(data));
                    }
                })
            }
        });
        $("#ShowBTInformation-VU").dialog('open');
        return false;
    });
});













//$(document).on("click", "a#ShowBTInformation", function (event) {
//    event.preventDefault();

//    var url = $(this).attr('href');
//    $("#ShowBTInformation-VU").load(url).dialog({
//        title: "BT's Data",
//        autoOpen: false,
//        resizable: false,
//        height: "auto",
//        width: "auto",
//        modal: true,
//        position: {
//            my: "center",
//            at: "center"
//        },
//        open: function (event, ui) {
//            $(this).load(url);
//        }
//    });
//    $("#ShowBTInformation-VU").dialog('open');
//    return false;
//});