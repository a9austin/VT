function JavascriptFunction() {    
    var url = '@Url.Action("GetReworkForm", "VT")';
    $("#divLoading").show();
    $.post(url, null,
            function (data) {
                $("#PID")[0].innerHTML = data;
                $("#divLoading").hide();
            });        
}
