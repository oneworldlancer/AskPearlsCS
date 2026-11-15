// Replace your existing setActiveSidebarItem / loadPortalSection / sidebar click block.
// Keep this inside the same script scope where your existing navigation is defined.
let apSectionRequest = null;
function setActiveSidebarItem (section) {
    const $links = $("#askPearlsSidebar .ap-nav-link");
    const $match = $links.filter(function () {
        return $(this).attr("data-page") === section;
    }).first();
    if (!$match.length) {
        console.warn("No sidebar item found for section:", section);
        return;
    }
    $links.removeClass("active").removeAttr("aria-current");
    $match.addClass("active").attr("aria-current", "page");
    // If the active item is inside a collapsed agent accordion, expand it.
    const collapse = $match.closest(".accordion-collapse")[ 0 ];
    if (collapse && window.bootstrap?.Collapse) {
        bootstrap.Collapse.getOrCreateInstance(collapse, { toggle: false }).show();
    }
}
function loadPortalSection (section) {
    if (!section) return;
    if (apSectionRequest && apSectionRequest.readyState !== 4) {
        apSectionRequest.abort();
    }
    apSectionRequest = $.ajax({
        url: "/Dashboard/LoadSection",
        type: "GET",
        data: { section },
        success: function (html) {
            $("#apMainContent").html(html);
            setActiveSidebarItem(section);
            $("#apMainContent").stop(true).scrollTop(0);
            if (section === "notifications") {
                window.AskPearlsNotifications?.loadPage?.();
            }
        },
        error: function (xhr, status) {
            if (status === "abort") return;
            console.error("Section loading failed:", section, xhr.status, xhr.responseText);
            alert("Unable to load this Dashboard section. Check the browser console.");
        }
    });
}
window.loadPortalSection = loadPortalSection;
$(document).off("click.apSidebarNav", "#askPearlsSidebar .ap-nav-link[data-page]")
    .on("click.apSidebarNav", "#askPearlsSidebar .ap-nav-link[data-page]", function (e) {
        e.preventDefault();
        loadPortalSection($(this).attr("data-page"));
    });
