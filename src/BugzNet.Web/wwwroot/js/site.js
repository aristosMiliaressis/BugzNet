/* Generic handling of Bulma Modals */
'use strict';

/* Tabs Interaction of Bulma Tabs */
function openTab(evt, tabName) {
    var i, x, tablinks;
    x = document.getElementsByClassName("content-tab");
    for (i = 0; i < x.length; i++) {
        x[i].style.display = "none";
    }
    tablinks = document.getElementsByClassName("tab");
    for (i = 0; i < x.length; i++) {
        tablinks[i].className = tablinks[i].className.replace(" is-active", "");
    }
    document.getElementById(tabName).style.display = "block";
    evt.currentTarget.className += " is-active";
}

/* Prevent Default behavior right click of <a> elements*/
$(document).ready(function () {
    $("a").on("contextmenu", function () {
        return false;
    });
    $(document).on('click', 'a', function (e) {
        e.stopImmediatePropagation();
    });
    $('a').mousedown(function (event) {
        event.stopImmediatePropagation();
        switch (event.which) {
            case 2:
                event.preventDefault();
                event.stopPropagation();
                return false;
            case 3:
                event.preventDefault();
                event.stopPropagation();
                return false;
            default:

        }
    });

    var dropdowns = document.querySelectorAll('.dropdown');
    var i;
    for (i = 0; i < dropdowns.length; i++) {
        dropdowns[i].addEventListener('click', function (event) {
            event.stopPropagation();
            event.currentTarget.classList.toggle('is-active');
        });
    }
});

/*Function to Select All CheckBoxes in the same column on a table*/
function SelectAll(obj) {
    // find the index of column
    var table = $(obj).closest('table');
    var th_s = table.find('th');
    var current_th = $(obj).closest('th');
    var columnIndex = th_s.index(current_th) + 1;

    console.log('The Column is = ' + columnIndex);

    // select all checkboxes from the same column index
    table.find('td:nth-child(' + (columnIndex) + ') input').prop("checked", obj.checked);
}

$('.submit').on('click', function (e) {
    var form = e.currentTarget.form;
    if (form !== null) {
        if ($(form).valid()) {
            return DisplayProgressMessage(e, 'Saving...');
        }
    }
});

function DisplayProgressMessage(ctl, msg) {
    $(ctl).prop("disabled", true).text(msg);
    $('.loader-wrapper').addClass('is-active');
    // Wrap in setTimeout so the UI
    // can update the spinners
    setTimeout(function () {
        $('.loader-wrapper').removeClass('is-active');
    }, 1);
    return true;
}

function delectAllCard(obj) {
    // find the Card-Content class that has the checkBoxes
    var cardContentDiv = $(obj).closest('.card').find('.card-content');

    cardContentDiv.find('input[type=checkbox]').each(function () {
        var chk = $(this);
        if (chk.prop("disabled") === false)
            chk.prop("checked", !chk.prop("checked"));
    });
    cardContentDiv.find('input[type=radio]').each(function () {
        var chk = $(this);
        chk.prop("checked", false);
    });

}

function showLoading(interval) {
    $('.loader-wrapper').addClass('is-active');
    setTimeout(function () {
        $('.loader-wrapper').removeClass('is-active');
    }, interval);
}

function toggleCard(cardId) {
    var card = document.getElementById(cardId);
    var check = document.getElementById(cardId + 'Check');
    if (check.checked) {
        card.classList.remove('opacitygrayscale');
        check.value = "true";
        check.setAttribute('data-val', 'true');
    } else {
        card.classList.add('opacitygrayscale');
        check.value = "false";
        check.setAttribute('data-val', 'false');
    }
}


   
