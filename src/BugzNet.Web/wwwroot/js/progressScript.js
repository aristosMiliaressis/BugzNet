function StartProgress(processName) {
	var loaderText = document.getElementById("loader-title");

	if (loaderText !== undefined && loaderText !== null) {
		loaderText.innetText  = 'Please wait while ' + processName;
	}

	disableActions('Please wait while ' + processName);
	setBusy(true);
}

function setBusy(loading) {
	var loader = document.getElementById("page-loader");
	if (loading === true) {
	
		if (loader !== undefined && loader !== null) {
			loader.classList.add('is-active');
		}
	}
	else {
		if (loader !== undefined && loader !== null) {
			loader.classList.remove('is-active');
		}
    }
}

function enableActions() {
	var submitButton = document.getElementById("submitButton");
	var backButton = document.getElementById("backButton");
	var progressInfo = document.getElementById("progressInfo");

	if (progressInfo !== undefined && progressInfo !== null) {
		progressInfo.text = '';
	}

	if (submitButton !== undefined && submitButton !== null) {
		submitButton.disabled = false;
	}

	if (backButton !== undefined && backButton !== null) {
		if (!backButton.classList.contains('is-invisible')) {
			backButton.removeAttribute('disabled');
		}
	}
}

function disableActions(infoMessage) {
	var submitButton = document.getElementById("submitButton");
	var backButton = document.getElementById("backButton");
	var progressInfo = document.getElementById("progressInfo");
	
	if (progressInfo !== undefined && progressInfo !== null) {
		progressInfo.innerText = infoMessage;
	}
	
	if (submitButton !== undefined && submitButton !== null) {
		submitButton.disabled = true;
	}

	if (backButton !== undefined && backButton !== null) {
		if (!backButton.classList.contains('is-invisible')) {
			backButton.setAttribute("disabled", true);
		}
	}
}