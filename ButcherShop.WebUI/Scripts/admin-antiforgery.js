(function () {
    function getToken() {
        var el = document.querySelector('#__af input[name="__RequestVerificationToken"]')
            || document.querySelector('input[name="__RequestVerificationToken"]');
        return el ? el.value : null;
    }

    // jQuery yoksa çık
    if (!window.jQuery) return;

    // Tüm AJAX POST'lara token ekle
    $.ajaxPrefilter(function (options, originalOptions, jqXHR) {
        var method = (options.type || originalOptions.type || "GET").toUpperCase();
        if (method !== "POST") return;

        var token = getToken();
        if (!token) return;

        // data string ise sonuna ekle
        if (typeof originalOptions.data === "string") {
            if (originalOptions.data.length > 0) originalOptions.data += "&";
            originalOptions.data += "__RequestVerificationToken=" + encodeURIComponent(token);
            options.data = originalOptions.data;
            return;
        }

        // data object ise içine ekle
        originalOptions.data = originalOptions.data || {};
        if (!originalOptions.data.__RequestVerificationToken) {
            originalOptions.data.__RequestVerificationToken = token;
        }
        options.data = $.param(originalOptions.data);
    });
})();
