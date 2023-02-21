
$(document).ready(function () {
    //Get all the action links that need confirmation by the user
    var elements = document.getElementsByClassName("modal-action-link");
    //The confirmation dialog function
    const showConfirmationModal = function () {
        //Get the actual action handler that must be triggered after 
        //the user confirms this action
        var actionHandler = this.getAttribute("data-action-location");
        var typeOfNotification = this.getAttribute("data-action-type");
        var actionName = this.getAttribute("data-action-name");
        var fullText = this.getAttribute("data-full-text");
        //Handle to the modal confirmation dialog
        var modalDlg = document.querySelector('#confirm-modal');
        //Handle to the close button
        var modalCloseBtn = document.querySelector('#modal-close');
        //Handle to the action button
        var modalActionButton = document.querySelector('#modal-action');
        var notificationArea = document.querySelector('#confirmContent');
        var question = document.querySelector('#question');

        notificationArea.classList.add(typeOfNotification);
        if (actionHandler === undefined || actionHandler === null || actionHandler === "")
            modalActionButton.onclick = this.dataActionHandler;
        else
            modalActionButton.href = actionHandler;

        if (fullText !== undefined && fullText !== "" && fullText !== null)
            question.innerHTML = fullText;
        else
            question.innerText = 'Are you sure you want to ' + actionName + ' the selected entity ?';
        modalDlg.classList.add('is-active');

        if (modalCloseBtn != undefined) {
            modalCloseBtn.addEventListener('click', function () {
                modalDlg.classList.remove('is-active');
                question.innerHTML = '';
            });
        }
        
        if (this.onloadHandler !== undefined)
            this.onloadHandler();
    };
    //Hook the event handler to all
    for (var i = 0; i < elements.length; i++) {
        elements[i].addEventListener('click', showConfirmationModal, false);
    }
});