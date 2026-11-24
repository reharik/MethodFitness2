// The menu button for phones and upright tablets (styled in mobile.css; hidden on a desktop).
// The menu opens over the page, and closes when a screen is chosen or the page is tapped.
(function ($) {
    $(function () {
        var $body = $("body");
        $("#menuToggle").on("click", function (e) {
            e.stopPropagation();
            // under the header, wherever the page has been scrolled to
            $("#left-navigation").css("top", $("#main-body").offset().top + "px");
            $body.toggleClass("menu-open");
        });
        $(window).on("hashchange", function () { $body.removeClass("menu-open"); });
        $(document).on("click", function (e) {
            if ($body.hasClass("menu-open") && !$(e.target).closest("#left-navigation").length) {
                $body.removeClass("menu-open");
            }
        });
    });
})(jQuery);
