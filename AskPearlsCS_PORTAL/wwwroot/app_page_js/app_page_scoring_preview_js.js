
    (function() {
        "use strict";

    const root = document.querySelector(".sp-page");
    if (!root || root.dataset.initialized === "true") return;
    root.dataset.initialized = "true";

    const MAX_PER_QUESTION = 50;

    const questions = [
    {
        key: "users",
    text: "How many people will use the sofa?",
    answers: [
    { value: "1-2", text: "1–2 people" },
    { value: "3-4", text: "3–4 people" },
    { value: "5-plus", text: "5 or more people" }
    ]
},
    {
        key: "priority",
    text: "What matters most to you?",
    answers: [
    { value: "comfort", text: "Comfort" },
    { value: "style", text: "Style" },
    { value: "space-saving", text: "Space saving" }
    ]
},
    {
        key: "room_size",
    text: "How much space do you have?",
    answers: [
    { value: "small", text: "Small room" },
    { value: "medium", text: "Medium room" },
    { value: "large", text: "Large room" }
    ]
}
    ];

    const recommendations = [
    { key: "oslo_sofa", name: "Oslo Sofa", description: "Compact and practical for everyday comfort." },
    { key: "luna_sofa", name: "Luna Sofa", description: "A contemporary, style-focused option." },
    { key: "haven_sofa", name: "Haven Sofa", description: "A spacious option for larger living areas." }
    ];

    // Demo defaults only. Every answer distributes exactly 50 points.
    // Shape: allocations[questionKey][answerValue] = [Oslo, Luna, Haven].
    const allocations = {
        users: {
        "1-2": [30, 10, 10],
    "3-4": [15, 20, 15],
    "5-plus": [5, 10, 35]
},
    priority: {
        comfort: [20, 15, 15],
    style: [10, 30, 10],
    "space-saving": [35, 10, 5]
},
    room_size: {
        small: [35, 10, 5],
    medium: [15, 20, 15],
    large: [5, 10, 35]
}
};

    const visitorAnswers = {
        users: "3-4",
    priority: "comfort",
    room_size: "small"
};

const $ = id => root.querySelector("#" + id);
    const editQuestion = $("spEditQuestion");
    const editAnswer = $("spEditAnswer");

    $("spQuestionCount").textContent = questions.length;
    $("spMaximum").textContent = questions.length * MAX_PER_QUESTION;

    function getQuestion(key) {
return questions.find(q => q.key === key);
}

    function getAnswerLabel(questionKey, answerValue) {
const question = getQuestion(questionKey);
const answer = question && question.answers.find(a => a.value === answerValue);
    return answer ? answer.text : answerValue;
}

    function renderAnswerOptions() {
const question = getQuestion(editQuestion.value);
editAnswer.innerHTML = question.answers.map(a =>
    `<option value="${a.value}">${ a.text }</option>`
    ).join("");
    renderAllocationEditor();
}

    function renderAllocationEditor() {
const qKey = editQuestion.value;
    const aValue = editAnswer.value;
    const values = allocations[qKey][aValue];

$("spAllocationEditor").innerHTML = recommendations.map((r, i) => `
    <div class="col-12 col-md-4">
        <div class="sp-allocation-card">
            <label for="spPoints${i}">${ r.name } points</label>
            <input class="form-control sp-point-input" type="number"
                id="spPoints${i}" min="0" max="50" step="1"
                value="${values[i]}" data-index="${i}">
                <small class="text-muted d-block mt-2">${ r.description }</small>
        </div>
    </div>
    `).join("");

root.querySelectorAll(".sp-point-input").forEach(input => {
        input.addEventListener("input", () => {
            const index = Number(input.dataset.index);
            const value = input.value === "" ? 0 : Number(input.value);
            if (!Number.isFinite(value) || value < 0 || value > 50 || !Number.isInteger(value)) {
                input.classList.add("is-invalid");
                updateAllocationStatus();
                return;
            }
            input.classList.remove("is-invalid");
            allocations[ qKey ][ aValue ][ index ] = value;
            updateAllocationStatus();
            renderResults();
        });
});

    updateAllocationStatus();
}

    function updateAllocationStatus() {
const values = allocations[editQuestion.value][editAnswer.value];
const total = values.reduce((sum, n) => sum + n, 0);
    const valid = total === MAX_PER_QUESTION;
    $("spAllocationTotal").textContent = `${ total } / ${ MAX_PER_QUESTION }`;
    $("spAllocationStatus").textContent = valid ? "Valid allocation" : `Needs ${ MAX_PER_QUESTION - total } points`;
    $("spAllocationStatus").className = valid ? "sp-valid" : "sp-invalid";
    $("spAllocationBar").style.width = Math.max(0, Math.min(100, total / MAX_PER_QUESTION * 100)) + "%";
    $("spAllocationBar").classList.toggle("invalid", !valid);
}

    function renderVisitorAnswers() {
        $("spVisitorAnswers").innerHTML = questions.map(q => `
                <div class="col-12 col-md-4">
                    <div class="sp-answer-card">
                        <label for="spVisitor_${ q.key }">${ q.text }</label>
                        <select class="form-select" id="spVisitor_${ q.key }" data-question="${ q.key }">
                            ${ q.answers.map(a => `
                                <option value="${ a.value }" ${ visitorAnswers[ q.key ] === a.value ? "selected" : "" }>${ a.text }</option>
                            `).join("") }
                        </select>
                    </div>
                </div>
            `).join("");

            root.querySelectorAll("[data-question]").forEach(select => {
        select.addEventListener("change", () => {
            visitorAnswers[ select.dataset.question ] = select.value;
            renderResults();
        });
            });
        }

    function calculateResults() {
            const results = recommendations.map((recommendation, index) => {
        let score = 0;
    const reasons = [];

                questions.forEach(q => {
                    const answer = visitorAnswers[q.key];
    const points = allocations[q.key][answer][index];
    score += points;
    reasons.push(`${ q.text }: ${ getAnswerLabel(q.key, answer) } (+${ points })`);
                });

    return {
        ...recommendation,
        score,
        maximum: questions.length * MAX_PER_QUESTION,
    percentage: Math.round(score / (questions.length * MAX_PER_QUESTION) * 100),
    reasons
                };
            });

            return results.sort((a, b) => b.score - a.score);
        }

    function renderResults() {
            const allValid = questions.every(q =>
                q.answers.every(a =>
                    allocations[q.key][a.value].reduce((sum, n) => sum + n, 0) === MAX_PER_QUESTION
    )
    );

    $("spRankingEmpty").classList.toggle("d-none", allValid);
    $("spRanking").classList.toggle("d-none", !allValid);

    if (!allValid) {
        $("spBreakdown").innerHTML = "";
    $("spBreakdownFooter").innerHTML = "";
    return;
            }

    const results = calculateResults();

            $("spRanking").innerHTML = results.map((r, index) => `
    <div class="sp-result ${index === 0 ? " sp-winner" : ""}">
    <div class="sp-result-top">
        <div class="d-flex align-items-center gap-3">
            <span class="sp-rank">${ index + 1 }</span>
            <div>
                <div class="fw-bold">${ r.name } ${ index === 0 ? '<span class="sp-valid ms-2">Top match</span>' : "" }</div>
                <div class="text-muted small">${ r.description }</div>
            </div>
        </div>
        <div class="text-end">
            <div class="sp-score">${ r.score } / ${ r.maximum }</div>
            <div class="text-muted small">${ r.percentage }% match</div>
        </div>
    </div>
    <div class="sp-match-track"><div style="width:${r.percentage}%"></div></div>
    <div class="text-muted small">${ r.reasons.join(" · ") }</div>
</div>
`).join("");

            $("spBreakdown").innerHTML = questions.map(q => {
                const answer = visitorAnswers[q.key];
                const points = allocations[q.key][answer];
                return `
    < tr >
    <td><strong>${ q.text }</strong><div class="text-muted small">${ getAnswerLabel(q.key, answer) }</div></td>
                        ${ points.map(value => `<td><span class="sp-points">${ value }</span> <span class="text-muted small">pts</span></td>`).join("") }
                    </tr >
    `;
            }).join("");

            const totals = recommendations.map((r, i) =>
                questions.reduce((sum, q) => sum + allocations[q.key][visitorAnswers[q.key]][i], 0)
            );

            $("spBreakdownFooter").innerHTML = `
    < tr >
    <th>Total score</th>
                    ${ totals.map(total => `<th>${ total } / ${ questions.length * MAX_PER_QUESTION }</th>`).join("") }
                </tr >
    `;
        }

        editQuestion.addEventListener("change", renderAnswerOptions);
        editAnswer.addEventListener("change", renderAllocationEditor);

        $("spResetAnswers").addEventListener("click", () => {
            visitorAnswers.users = "3-4";
            visitorAnswers.priority = "comfort";
            visitorAnswers.room_size = "small";
            renderVisitorAnswers();
            renderResults();
        });

        renderAnswerOptions();
        renderVisitorAnswers();
        renderResults();
    })();
