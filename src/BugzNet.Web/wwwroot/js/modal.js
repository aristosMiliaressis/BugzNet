
function hookModalEvents(page, jsRoot) {
    //Get the edit link elements
    var elements = document.getElementsByClassName("modal-link");
    //Create the event handler function
    var getModalContent = function () {
        var entity = this.getAttribute("data-entity");
        var displayName = this.getAttribute("data-display");
        var id = this.getAttribute("data-id");
        var parentId = this.getAttribute("data-bugId");
        var modalTileEl = document.getElementById('modaltitle');

        if (modalTileEl !== undefined) {
            modalTileEl.innerText = "Edit " + ((displayName === undefined)
                ? entity
                : displayName);
        }            

        fetch('./' + page + '?id=' + id + '&bugid=' + parentId + '&handler=' + entity + 'Partial')
            .then((response) => {
                return response.text();
            })
            .then((result) => {
                //Parse the response html text to get a document object
                const parser = new DOMParser();
                const htmlDocument = parser.parseFromString(result, "text/html");

                //rebind client validation scripts TODO : Check this sometimes is not working
                var partialForm = htmlDocument.getElementById('partial-form');
                var $form = $(partialForm);
                $form.unbind();
                $form.data("validator", null);
                $.validator.unobtrusive.parse($form);

                if ($form.data("unobtrusiveValidation") !== undefined)
                    $form.validate($form.data("unobtrusiveValidation").options);

                //Get the Scripts we must load from the hidden input of partial
                var jsScripts = htmlDocument.getElementsByClassName('js-script');
                //Load all the scripts
                for (var i = 0; i < jsScripts.length; i++) {
                    if (jsScripts[i] !== null) {
                        var contentPath = jsRoot + jsScripts[i].value;
                        const scriptPromise = new Promise((resolve, reject) => {
                            const script = document.createElement('script');
                            document.body.appendChild(script);
                            script.onload = resolve;
                            script.onerror = reject;
                            script.async = true;
                            script.src = contentPath;
                        });

                        scriptPromise.then(() => {  });
                    }
                }
                //Get the modal size from the hidden input
                var modalSize = htmlDocument.getElementById('partial-size').value;
                //Get the modal content element set the content and style it
                var modalContentEl = document.getElementById('modal-content');
                //Clear previews classed of size style
                modalContentEl.classList.remove('is-huge', 'is-large', 'is-normal', 'is-small');
                //Add the new Style
                modalContentEl.classList.add(modalSize);
                //Get the modal body element
                var modalBodyEl = document.getElementById('modal-body');

                // remove meta tags to prevent open redirects & dangling markup caused by html injection
                var blacklistedTags = ["script", "meta"];
                for (var tag of blacklistedTags) {
                    var elements = htmlDocument.getElementsByTagName(tag);
                    for (element of elements)
                        element.remove();
                }

                //Set the dynamic content
                modalBodyEl.innerHTML = htmlDocument.body.innerHTML;
                //Get the model container element and show it to the user
                var modalEl = document.getElementById('modal');
                modalEl.classList.add('is-active');

                var cancelButton = document.getElementById('modal-cancel');
                if (cancelButton != null)
                    cancelButton.onclick = function () { $(".modal").removeClass("is-active"); }
            });
    };
    //Hook the event handler to all
    for (var i = 0; i < elements.length; i++) {
        elements[i].addEventListener('click', getModalContent, false);
    }
}
