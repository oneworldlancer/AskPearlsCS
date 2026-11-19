
/*
 * AskPearls — Global Widget Version Helper
 * Shared by all widget dashboard pages.
 */

(function (window) {
    "use strict";

    function normalizeVersion (value) {
        const number = Number.parseInt(
            String(value ?? "0"),
            10
        );

        return Number.isFinite(number) && number > 0
            ? number
            : 0;
    }

    function widgetVersionLabel (widget) {
        if (!widget) {
            return "Draft v0 · Live v0";
        }

        const draft = normalizeVersion(
            widget.draftVersionNumber ??
            widget.DraftVersionNumber ??
            widget.widgetDraftVersion ??
            widget.WidgetDraftVersion
        );

        const live = normalizeVersion(
            widget.publishedVersionNumber ??
            widget.PublishedVersionNumber ??
            widget.widgetPublishedVersion ??
            widget.WidgetPublishedVersion
        );

        return `Draft v${ draft } · Live v${ live }`;
    }

    function updateWidgetVersion (elementId, widget) {
        const element = document.getElementById(elementId);

        if (!element) {
            return;
        }

        element.textContent = widget
            ? widgetVersionLabel(widget)
            : "—";
    }

    window.AskPearlsWidgetVersion = Object.freeze({
        format: widgetVersionLabel,
        update: updateWidgetVersion
    });

})(window);
