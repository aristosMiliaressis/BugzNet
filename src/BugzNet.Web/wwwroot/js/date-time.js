
RegisterDateAndTimePickers();

function RegisterDateAndTimePickers() {
    document.addEventListener("DOMContentLoaded", SetUpBulmaInputs);

    var pickers = document.getElementsByClassName('m-timepicker');
    for (var picker of pickers) {
        picker.addEventListener("keyup", textFieldTimePicker);
        picker.addEventListener("keydown", textFieldTimePicker);
    }
}

function SetUpBulmaInputs()
{
    // Initialize all input of type date
    bulmaCalendar.attach('[type="date"]', { weekStart: 1, dateFormat: 'DD/MM/YYYY', showTodayButton: 'false' });

    //Remove the clear button bucause it was causing the sumbit of form to be triggered
    //the property showClearButton in options was not working

    var clearButton = document.getElementsByClassName('datetimepicker-clear-button')[0];
    if (clearButton !== undefined && clearButton !== null)
        clearButton.remove();
}


function textFieldTimePicker(e) {
    if (e.type == 'keyup') {
        if (event.target.value.length > 5)
            event.target.value = event.target.value.substring(0, 5);
        if (event.target.value.length == 5) {
            event.target.setCustomValidity("");
        }
        return;
    }

    event.target.setCustomValidity("Invalid field.");

    var alwaysAllowed = ['Backspace', 'Delete'];
    if (alwaysAllowed.includes(e.code))
        return;

    var numbers = ['Digit0', 'Numpad0', 'Digit1', 'Numpad1', 'Digit2', 'Numpad2',
        'Digit3', 'Numpad3', 'Digit4', 'Numpad4', 'Digit5', 'Numpad5', 'Digit6', 'Numpad6',
        'Digit7', 'Numpad7', 'Digit8', 'Numpad8', 'Digit9', 'Numpad9']

    if (event.which != 8 && !numbers.includes(event.code)){
        event.preventDefault();
    } 

    if (event.target.value.length == 2)
        event.target.value = event.target.value + ':';

    if (event.target.value.length == 0) {
        var allowedNumbers = ['Digit0', 'Numpad0', 'Digit1', 'Numpad1', 'Digit2', 'Numpad2']
        if (!allowedNumbers.includes(event.code))
            event.preventDefault();
    }
    else if (event.target.value.length == 1) {
        
        if (!numbers.includes(event.code))
            event.preventDefault();
        var allowedNumbers = ['Digit0', 'Numpad0', 'Digit1', 'Numpad1', 'Digit2', 'Numpad2', 'Digit3', 'Numpad3']
        if (event.target.value == '2' && !allowedNumbers.includes(event.code))
            event.preventDefault();
    }
    else if (event.target.value.length == 3) {
        var allowedNumbers = ['Digit0', 'Numpad0', 'Digit1', 'Numpad1', 'Digit2', 'Numpad2',
            'Digit3', 'Numpad3', 'Digit4', 'Numpad4', 'Digit5', 'Numpad5']
        if (!allowedNumbers.includes(event.code))
            event.preventDefault();
    }
    else if (event.target.value.length == 4) {
        if (!numbers.includes(event.code))
            event.preventDefault();
    }
}