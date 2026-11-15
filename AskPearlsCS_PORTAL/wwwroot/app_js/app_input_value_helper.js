(function (window) {
    "use strict";
    const AppInputValue = Object.freeze({
        clean (value) {
            if (value === null || value === undefined) return "";
            const text = String(value).trim();
            return text === "0" ? "" : text;
        },
        isEmpty (value) { return this.clean(value) === ""; },
        toDatabaseValue (value) { return this.clean(value) || "0"; },
        setInput (element, value) {
            if (!element || element.type === "checkbox" || element.type === "radio") return;
            element.value = this.clean(value);
        }
    });
    window.AppInputValue = AppInputValue;
})(window);
