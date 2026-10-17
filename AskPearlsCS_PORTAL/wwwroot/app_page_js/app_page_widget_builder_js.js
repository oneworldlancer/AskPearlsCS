
(function ($) {
    "use strict";

    const root = ".page-widget-builder";
    let currentStep = 0;

    const steps = [ "basics", "appearance", "behavior" ];

    function showStep (index) {
        currentStep = Math.max(0, Math.min(index, steps.length - 1));

        $(root + " .wb-step").removeClass("active");
        $(root + " .wb-panel").removeClass("active");

        const stepName = steps[ currentStep ];

        $(root + ' .wb-step[data-wb-tab="' + stepName + '"]')
            .addClass("active");

        $(root + ' .wb-panel[data-wb-panel="' + stepName + '"]')
            .addClass("active");

        $("#wbPreviousButton").prop("disabled", currentStep === 0);

        $("#wbNextButton").html(
            currentStep === steps.length - 1
                ? 'Finish <i class="bi bi-check2 ms-1"></i>'
                : 'Next <i class="bi bi-arrow-right ms-1"></i>'
        );
    }

    function updatePreview () {
        const title = $("#wbWidgetTitle").val().trim();
        const description = $("#wbWidgetDescription").val().trim();
        //const buttonText = $("#wbButtonText").val().trim();
        const buttonText = $("#wbStartButtonText").val().trim();

        $("#wbPreviewPrimary").text(buttonText || "Get Started");


        const primaryColor = $("#wbPrimaryColor").val();
        const corners = $("#wbCornerStyle").val();
        const theme = $('input[name="wbTheme"]:checked').val();

        $("#wbPreviewTitle")
            .text(title || "Your Widget Title")
            .toggle($("#wbShowTitle").is(":checked"));

        $("#wbPreviewDescription")
            .text(description || "Your widget description.")
            .toggle($("#wbShowDescription").is(":checked"));

        $("#wbPreviewPrimary").text(buttonText || "Get Started");

        $("#wbPreviewWidget")
            .attr("data-theme", theme)
            .attr("data-corners", corners);

        $(".page-widget-builder")
            .css("--wb-primary", primaryColor);

        $("#wbColorValue").text(primaryColor);
    }

    $(document)
        .off("click.widgetBuilder", root + " .wb-step")
        .on("click.widgetBuilder", root + " .wb-step", function () {
            showStep(steps.indexOf($(this).data("wb-tab")));
        });

    $(document)
        .off("click.widgetBuilderNext", "#wbNextButton")
        .on("click.widgetBuilderNext", "#wbNextButton", function () {
            if (currentStep < steps.length - 1) {
                showStep(currentStep + 1);
            } else {
                // UI-only: the next page will be connected later.
                alert("Widget Builder UI preview: next step is Questions & Answers.");
            }
        });

    $(document)
        .off("click.widgetBuilderPrevious", "#wbPreviousButton")
        .on("click.widgetBuilderPrevious", "#wbPreviousButton", function () {
            showStep(currentStep - 1);
        });

    $(document)
        .off("input.widgetBuilder change.widgetBuilder",
            root + " input, " + root + " textarea, " + root + " select")
        .on("input.widgetBuilder change.widgetBuilder",
            root + " input, " + root + " textarea, " + root + " select",
            updatePreview);

    $(document)
        .off("click.widgetBuilderPreview", "#wbPreviewButton")
        .on("click.widgetBuilderPreview", "#wbPreviewButton", function () {
            $("#wbPreviewStage")[ 0 ].scrollIntoView({
                behavior: "smooth",
                block: "center"
            });
        });

    $(document)
        .off("click.widgetBuilderSave", "#wbSaveButton")
        .on("click.widgetBuilderSave", "#wbSaveButton", function () {
            alert("UI prototype only: Save Draft will be connected to the API later.");
        });

    $(document)
        .off("click.widgetBuilderPrimary", "#wbPreviewPrimary")
        .on("click.widgetBuilderPrimary", "#wbPreviewPrimary", function () {
            alert("Visitor interaction will be implemented in a later stage.");
        });

    // Reinitialize safely when this partial is loaded through AJAX.
    showStep(0);
    updatePreview();

})(jQuery);

