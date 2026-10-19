
    (function() {
        "use strict";

        const root = document.querySelector(".analytics-page");
        if (!root || root.dataset.anInitialized === "true") return;
        root.dataset.anInitialized = "true";

        const $ = id => root.querySelector("#" + id);

        const widgets = [
            { id: "sofa", name: "Sofa Finder", views: 6420, interactions: 2480, completion: 72.5, recommendations: 1120, status: "Published", icon: "bi-house" },
            { id: "furniture", name: "Furniture Recommendation", views: 4210, interactions: 1510, completion: 65.2, recommendations: 780, status: "Published", icon: "bi-grid" },
            { id: "demo", name: "Demo Recommendation", views: 2210, interactions: 630, completion: 58.1, recommendations: 275, status: "Draft", icon: "bi-stars" }
        ];

        const activities = [
            { type: "completed", title: "Recommendation session completed", detail: "Sofa Finder • A visitor completed the questionnaire.", time: "5 minutes ago", widget: "sofa", days: 0, icon: "bi-check-circle" },
            { type: "started", title: "New widget session started", detail: "Furniture Recommendation • A visitor opened the widget.", time: "18 minutes ago", widget: "furniture", days: 0, icon: "bi-play-circle" },
            { type: "recommended", title: "Recommendation displayed", detail: "Sofa Finder • A sofa recommendation was shown.", time: "42 minutes ago", widget: "sofa", days: 0, icon: "bi-stars" },
            { type: "published", title: "Widget published", detail: "Furniture Recommendation • Demo publishing event.", time: "Yesterday", widget: "furniture", days: 1, icon: "bi-cloud-check" },
            { type: "completed", title: "Recommendation session completed", detail: "Demo Recommendation • A visitor completed the flow.", time: "Yesterday", widget: "demo", days: 1, icon: "bi-check-circle" },
            { type: "started", title: "New widget session started", detail: "Sofa Finder • A visitor opened the widget.", time: "3 days ago", widget: "sofa", days: 3, icon: "bi-play-circle" },
            { type: "recommended", title: "Recommendation displayed", detail: "Furniture Recommendation • A recommendation was shown.", time: "6 days ago", widget: "furniture", days: 6, icon: "bi-stars" },
            { type: "published", title: "Widget published", detail: "Sofa Finder • Demo publishing event.", time: "12 days ago", widget: "sofa", days: 12, icon: "bi-cloud-check" }
        ];

        let toastTimer = null;

        function escapeHtml(value) {
            return String(value ?? "").replace(/[&<>"']/g, ch => ({
                "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;"
            })[ch]);
        }

        function showToast(message) {
            const toast = $("anToast");
            toast.textContent = message;
            toast.classList.add("is-visible");
            clearTimeout(toastTimer);
            toastTimer = setTimeout(() => toast.classList.remove("is-visible"), 2600);
        }

        function filteredWidgets() {
            const selected = $("anWidget").value;
            return widgets.filter(widget => selected === "all" || widget.id === selected);
        }

        function filteredActivities() {
            const widget = $("anWidget").value;
            const type = $("anActivityType").value;
            const days = Number($("anDateRange").value);

            return activities.filter(item =>
                (widget === "all" || item.widget === widget) &&
                (type === "all" || item.type === type) &&
                item.days < days
            );
        }

        function renderKpis() {
            const selectedWidgets = filteredWidgets();
            const factor = Number($("anDateRange").value) / 30;
            const views = selectedWidgets.reduce((sum, item) => sum + item.views, 0) * factor;
            const interactions = selectedWidgets.reduce((sum, item) => sum + item.interactions, 0) * factor;
            const recommendations = selectedWidgets.reduce((sum, item) => sum + item.recommendations, 0) * factor;
            const completionBase = selectedWidgets.length
                ? selectedWidgets.reduce((sum, item) => sum + item.completion, 0) / selectedWidgets.length
                : 0;

            $("anViews").textContent = Math.round(views).toLocaleString();
            $("anInteractions").textContent = Math.round(interactions).toLocaleString();
            $("anRecommendations").textContent = Math.round(recommendations).toLocaleString();
            $("anCompletion").textContent = completionBase.toFixed(1) + "%";
        }

        function svgElement(tag, attributes, text) {
            const node = document.createElementNS("http://www.w3.org/2000/svg", tag);
            Object.entries(attributes || {}).forEach(([key, value]) => node.setAttribute(key, value));
            if (text !== undefined) node.textContent = text;
            return node;
        }

        function renderChart() {
            const svg = $("anPerformanceChart");
            svg.replaceChildren();

            const count = Number($("anDateRange").value);
            const points = count === 7 ? 7 : count === 30 ? 10 : 12;
            const selected = filteredWidgets();
            const viewsTotal = selected.reduce((sum, item) => sum + item.views, 0);
            const interactionsTotal = selected.reduce((sum, item) => sum + item.interactions, 0);

            const viewValues = [];
            const interactionValues = [];

            for (let i = 0; i < points; i++) {
                const wave = 0.72 + ((i * 17 + 11) % 23) / 55;
                viewValues.push(Math.round((viewsTotal / points) * wave));
                interactionValues.push(Math.round((interactionsTotal / points) * (0.70 + ((i * 13 + 7) % 19) / 60)));
            }

            const width = 680, height = 260;
            const left = 48, right = 14, top = 16, bottom = 34;
            const chartW = width - left - right;
            const chartH = height - top - bottom;
            const maxValue = Math.max(1, ...viewValues) * 1.15;

            for (let i = 0; i <= 4; i++) {
                const y = top + (chartH * i / 4);
                svg.appendChild(svgElement("line", {
                    x1: left, y1: y, x2: width - right, y2: y,
                    stroke: "var(--ap-border, #e2e8f0)", "stroke-width": 1,
                    "stroke-dasharray": "3 4"
                }));
                svg.appendChild(svgElement("text", {
                    x: left - 9, y: y + 3, "text-anchor": "end"
                }, Math.round(maxValue * (4 - i) / 4).toLocaleString()));
            }

            const x = i => left + (points === 1 ? chartW / 2 : chartW * i / (points - 1));
            const y = value => top + chartH * (1 - value / maxValue);

            function path(values) {
                return values.map((value, i) =>
                    (i === 0 ? "M" : "L") + x(i).toFixed(2) + " " + y(value).toFixed(2)
                ).join(" ");
            }

            svg.appendChild(svgElement("path", {
                d: path(viewValues),
                fill: "none", stroke: "#0da88b",
                "stroke-width": 3, "stroke-linecap": "round", "stroke-linejoin": "round"
            }));

            svg.appendChild(svgElement("path", {
                d: path(interactionValues),
                fill: "none", stroke: "#6288df",
                "stroke-width": 2.5, "stroke-linecap": "round", "stroke-linejoin": "round"
            }));

            [viewValues, interactionValues].forEach((values, seriesIndex) => {
                const color = seriesIndex === 0 ? "#0da88b" : "#6288df";
                values.forEach((value, i) => {
                    svg.appendChild(svgElement("circle", {
                        cx: x(i), cy: y(value), r: 3.5, fill: color,
                        stroke: "var(--ap-card-bg, #fff)", "stroke-width": 1.5
                    }));
                });
            });

            const labels = count === 7
                ? ["Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun"]
                : count === 30
                    ? ["1", "4", "7", "10", "13", "16", "19", "22", "25", "30"]
                    : ["W1", "W2", "W3", "W4", "W5", "W6", "W7", "W8", "W9", "W10", "W11", "W12"];

            labels.forEach((label, i) => {
                const index = labels.length === points ? i : Math.round(i * (points - 1) / (labels.length - 1));
                svg.appendChild(svgElement("text", {
                    x: x(index), y: height - 10, "text-anchor": "middle"
                }, label));
            });
        }

        function renderFunnel() {
            const selected = filteredWidgets();
            const views = selected.reduce((sum, item) => sum + item.views, 0);
            const interactions = selected.reduce((sum, item) => sum + item.interactions, 0);
            const recommendations = selected.reduce((sum, item) => sum + item.recommendations, 0);
            const started = Math.round(interactions * 1.58);
            const completed = Math.round(interactions * .99);
            const stages = [
                { label: "Widget views", value: views, className: "an-fill-one" },
                { label: "Started", value: started, className: "an-fill-two" },
                { label: "Completed", value: completed, className: "an-fill-three" },
                { label: "Recommendation shown", value: recommendations, className: "an-fill-four" }
            ];
            const max = Math.max(1, views);
            const funnel = $("anFunnel");
            funnel.replaceChildren();

            stages.forEach(stage => {
                const row = document.createElement("div");
                row.className = "an-funnel-row";
                row.innerHTML =
                    '<div class="an-funnel-label"><span>' + escapeHtml(stage.label) +
                    '</span><strong>' + stage.value.toLocaleString() + '</strong></div>' +
                    '<div class="an-funnel-track"><div class="an-funnel-fill ' +
                    stage.className + '" style="width:' +
                    Math.min(100, stage.value / max * 100).toFixed(1) + '%"></div></div>';
                funnel.appendChild(row);
            });

            const note = document.createElement("div");
            note.className = "an-funnel-note";
            note.innerHTML = '<i class="bi bi-info-circle"></i> Illustrative mock data; stages are not yet connected to real event tracking.';
            funnel.appendChild(note);
        }

        function renderWidgetTable() {
            const selected = filteredWidgets();
            const body = $("anWidgetTableBody");
            body.replaceChildren();

            selected.forEach(widget => {
                const row = document.createElement("tr");
                row.innerHTML =
                    '<td><div class="an-widget-name">' +
                    '<span class="an-widget-avatar"><i class="bi ' + widget.icon + '"></i></span>' +
                    '<span><strong>' + escapeHtml(widget.name) + '</strong><small>Widget ID: ' +
                    escapeHtml(widget.id) + '</small></span>' +
                    '</div></td>' +
                    '<td>' + widget.views.toLocaleString() + '</td>' +
                    '<td>' + widget.interactions.toLocaleString() + '</td>' +
                    '<td class="an-completion-cell"><div class="an-progress-row">' +
                    '<div class="an-progress-track"><div class="an-progress-fill" style="width:' +
                    widget.completion + '%"></div></div><strong>' +
                    widget.completion.toFixed(1) + '%</strong></div></td>' +
                    '<td>' + widget.recommendations.toLocaleString() + '</td>' +
                    '<td><span class="an-status ' +
                    (widget.status === "Draft" ? "is-draft" : "") + '">' +
                    escapeHtml(widget.status) + '</span></td>';
                body.appendChild(row);
            });

            $("anWidgetCount").textContent = selected.length +
                (selected.length === 1 ? " widget" : " widgets");
            $("anWidgetEmpty").hidden = selected.length !== 0;
        }

        function renderActivities() {
            const selected = filteredActivities();
            const list = $("anActivityList");
            list.replaceChildren();

            selected.forEach(item => {
                const row = document.createElement("div");
                row.className = "an-activity-row";

                const eventClass = item.type === "completed" ? "an-event-completed"
                    : item.type === "started" ? "an-event-started"
                        : item.type === "published" ? "an-event-published"
                            : "an-event-recommended";

                row.innerHTML =
                    '<span class="an-activity-icon ' + eventClass + '"><i class="bi ' +
                    item.icon + '"></i></span>' +
                    '<div class="an-activity-copy"><strong>' + escapeHtml(item.title) +
                    '</strong><p>' + escapeHtml(item.detail) + '</p></div>' +
                    '<div class="an-activity-meta"><span>' + escapeHtml(item.time) +
                    '</span><span class="an-activity-type">' +
                    escapeHtml(item.type.charAt(0).toUpperCase() + item.type.slice(1)) +
                    '</span></div>';

                list.appendChild(row);
            });

            $("anActivityCount").textContent = selected.length +
                (selected.length === 1 ? " event" : " events");
            $("anActivityEmpty").hidden = selected.length !== 0;
        }

        function renderAll() {
            renderKpis();
            renderChart();
            renderFunnel();
            renderWidgetTable();
            renderActivities();
        }

        ["anDateRange", "anWidget", "anActivityType"].forEach(id => {
            $(id).addEventListener("change", renderAll);
        });

        $("anResetFilters").addEventListener("click", () => {
            $("anDateRange").value = "30";
            $("anWidget").value = "all";
            $("anActivityType").value = "all";
            renderAll();
            showToast("Filters reset.");
        });

        $("anExportBtn").addEventListener("click", () => {
            const rows = [["Widget", "Views", "Interactions", "Completion rate", "Recommendations", "Status"]];
            filteredWidgets().forEach(widget => {
                rows.push([
                    widget.name, widget.views, widget.interactions,
                    widget.completion + "%", widget.recommendations, widget.status
                ]);
            });

            const csv = rows.map(row => row.map(value =>
                '"' + String(value).replace(/"/g, '""') + '"'
            ).join(",")).join("\r\n");

            const blob = new Blob(["\uFEFF" + csv], { type: "text/csv;charset=utf-8;" });
            const url = URL.createObjectURL(blob);
            const link = document.createElement("a");
            link.href = url;
            link.download = "askpearls-analytics-demo.csv";
            document.body.appendChild(link);
            link.click();
            link.remove();
            URL.revokeObjectURL(url);
            showToast("Mock analytics CSV exported.");
        });

        renderAll();
    })();

