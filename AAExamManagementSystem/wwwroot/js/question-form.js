// Choice editor for the Question Create/Edit forms (Pages/Questions/_QuestionForm.cshtml).
(function () {
    var typeSelect = document.querySelector('.js-question-type');
    var card = document.querySelector('.js-choices-card');
    var list = document.querySelector('.js-choice-list');
    var template = document.getElementById('choiceRowTemplate');
    if (!typeSelect || !card || !list || !template) {
        return;
    }

    var choiceTypeIds = (typeSelect.dataset.choiceTypeIds || '').split(',').filter(Boolean);
    var baseName = list.dataset.name;

    function rows() {
        return list.querySelectorAll('.js-choice-row');
    }

    // Keep indexes contiguous so the model binder sees every row.
    function reindex() {
        rows().forEach(function (row, i) {
            var prefix = baseName + '[' + i + ']';
            row.querySelector('.js-choice-correct').name = prefix + '.IsCorrect';
            row.querySelector('.js-choice-correct-default').name = prefix + '.IsCorrect';
            row.querySelector('.js-choice-text').name = prefix + '.ChoiceText';
        });
    }

    function addRow(text) {
        var row = template.content.firstElementChild.cloneNode(true);
        row.querySelector('.js-choice-text').value = text || '';
        list.appendChild(row);
        reindex();
        return row;
    }

    function isTrueOrFalse() {
        var option = typeSelect.options[typeSelect.selectedIndex];
        return option && option.text === 'True or False';
    }

    function allBlank() {
        return Array.prototype.every.call(rows(), function (row) {
            return !row.querySelector('.js-choice-text').value.trim();
        });
    }

    function refresh() {
        var usesChoices = choiceTypeIds.indexOf(typeSelect.value) !== -1;
        card.classList.toggle('d-none', !usesChoices);
        if (!usesChoices) {
            return;
        }
        if (isTrueOrFalse() && allBlank()) {
            list.innerHTML = '';
            addRow('True');
            addRow('False');
        } else if (rows().length === 0) {
            for (var i = 0; i < 4; i++) addRow('');
        }
    }

    card.querySelector('.js-add-choice').addEventListener('click', function () {
        addRow('').querySelector('.js-choice-text').focus();
    });

    list.addEventListener('click', function (e) {
        var button = e.target.closest('.js-remove-choice');
        if (!button) return;
        button.closest('.js-choice-row').remove();
        reindex();
    });

    // True or False has a single answer: ticking one unticks the other.
    list.addEventListener('change', function (e) {
        if (!e.target.classList.contains('js-choice-correct') || !e.target.checked || !isTrueOrFalse()) return;
        list.querySelectorAll('.js-choice-correct').forEach(function (box) {
            if (box !== e.target) box.checked = false;
        });
    });

    typeSelect.addEventListener('change', refresh);
    refresh();
})();
