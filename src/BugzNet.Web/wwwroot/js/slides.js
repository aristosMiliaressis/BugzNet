$(document).on('load', function (){
    resetIndex();
});

var slidesList = document.getElementsByClassName('.slides');

if (slidesList != null) {
    resetIndex();
}

$(".slides").sortable({
    placeholder: 'slide-placeholder',
    axis: "y",
    revert: 150,
    start: function (e, ui) {

        var placeholderHeight = ui.item.outerHeight();
        ui.placeholder.height(placeholderHeight + 15);
        $('<div class="slide-placeholder-animator" data-height="' + placeholderHeight + '"></div>').insertAfter(ui.placeholder);
    },
    change: function (event, ui) {

        ui.placeholder.stop().height(0).animate({
            height: ui.item.outerHeight() + 15
        }, 300);

        var placeholderAnimatorHeight = parseInt($(".slide-placeholder-animator").attr("data-height"));

        $(".slide-placeholder-animator").stop().height(placeholderAnimatorHeight + 15).animate({
            height: 0
        }, 300, function () {
            $(this).remove();
            var placeholderHeight = ui.item.outerHeight();
            $('<div class="slide-placeholder-animator" data-height="' + placeholderHeight + '"></div>').insertAfter(ui.placeholder);
        });
    },
    stop: function (e, ui) {
        $(".slide-placeholder-animator").remove();
        resetIndex();
    },
});

function resetIndex() {
    $(".slides li").each(function (i) {
        $(this).find(".indexNumber").html((i));
        $(this).find("#indexno").val(i);
        $(this).find(".ipcol").attr("id", 'ipdiv' + i);
        $(this).find(".dialcol").attr("id", 'dialupdiv' + i);
    });
    var count_elements = $(".slides li").length;

    $("#slidescount").text(count_elements);
};

$(document).ready(function () {
    var count_elements = $(".slides li").length;
    for (var i = 0; i < count_elements; i++) {
        var dropList = $('#commtype' + i);

        refreshContent(dropList.val(), i);

        dropList.on('change', function (e) {
            var selectedType = this.value;
            var rowIndex = $(this).closest("div.columns").find("input[id='indexno']").val();
            refreshContent(selectedType, rowIndex);
        });
    }
});
function refreshContent(type, indexId) {
    var diapUpDiv = document.getElementById('dialupdiv' + indexId);
    var IpDiv = document.getElementById('ipdiv' + indexId);

    if (type == 3) {
        diapUpDiv.style.display = "block";
        IpDiv.style.display = "none";
        changeInputsState(IpDiv, true);
        changeInputsState(diapUpDiv, false);
    }
    else {
        diapUpDiv.style.display = "none";
        IpDiv.style.display = "block";
        changeInputsState(IpDiv, false);
        changeInputsState(diapUpDiv, true);
    }
}

function changeInputsState(div, state) {
    var inputs = div.getElementsByTagName("input");
    for (var i = 0; i < inputs.length; i++) {
        inputs[i].disabled = state;
    }
}

function AddEndPoint() {
    var t = $("input[name='xsrf_token']").val();
    var count_elements = $(".slides li").length;
    var url = "./CreateEdit/?handler=AddEndPoint";

    $.ajax({
        type: "GET",
        url: url,
        headers: { "RequestVerificationToken": t },
        dataType: "Html",
        data: { viewName: '_EndPointEntry', index: count_elements },
        success: function (data) {
            count_elements++;
            $(".slides").append('<li id="EndPoint' + count_elements + '" class="slide">' + data + '</li>');
            resetIndex();
            $("form").each(function () { $.data($(this)[0], 'validator', false); });
            $.validator.unobtrusive.parse("form");

            var indexId = (count_elements - 1);
            var dropList = $('#commtype' + indexId);

            refreshContent(dropList.val(), indexId);

            dropList.on('change', function (e) {
                var selectedType = this.value;
                refreshContent(selectedType, indexId);
            });
        },
        error: function (reponse) {
            alert("error : " + reponse);
        }
    });
}

