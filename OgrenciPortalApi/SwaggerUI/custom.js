// custom.js (Daha Güçlü Versiyon)

(function() {

    // Swagger UI nesnesinin hazýr olmasýný beklemek için bir fonksiyon
    function initializeSwaggerUi() {

        // window.swaggerUi nesnesi henüz tanýmlanmamýþ olabilir, 100ms sonra tekrar dene.
        if (typeof window.swaggerUi === 'undefined') {
            setTimeout(initializeSwaggerUi, 100);
            return;
        }

        console.log("Swagger UI object found. Attaching interceptor...");

        var swaggerUi = window.swaggerUi;
        var apiKeyName = "Authorization";

        var requestInterceptor = function(request) {
            console.log("Interceptor fired for URL: " + request.url);

            if (swaggerUi.api && swaggerUi.api.clientAuthorizations) {
                var key = swaggerUi.api.clientAuthorizations.authz[apiKeyName];

                if (key && key.value) {
                    var token = 'Bearer ' + key.value;
                    request.headers[apiKeyName] = token;
                    console.log("Token found. Adding header:", apiKeyName, "=", token);
                } else {
                    console.log("Token not found in UI storage.");
                }
            } else {
                console.log("clientAuthorizations object not found.");
            }
            return request;
        };

        swaggerUi.requestInterceptor = requestInterceptor;
        console.log("Swagger UI request interceptor attached successfully.");
    }

    // Script yüklendiðinde baþlat
    $(function() {
        console.log("custom.js loaded. Waiting for Swagger UI object...");
        initializeSwaggerUi();
    });

})();