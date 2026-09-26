// Shows the choices editor only when the selected question type needs choices
// (e.g. Multiple Choice), and keeps the Choices[i] input names sequential for model binding.
(function () {
    var MAX_CHOICES = 4;

    function init(editor) {
        var select = document.querySelector(editor.dataset.typeSelect);
        if (!select) return;

        var prefix = editor.dataset.prefix;
        var choiceTypeIds = (editor.dataset.choiceTypeIds || '').split(',').filter(Boolean);
        var list = editor.querySelector('.js-choices-list');
        var template = editor.querySelector('.js-choice-template');
        var addButton = editor.querySelector('.js-add-choice');

        function renumber() {
            addButton.disabled = list.children.length >= MAX_CHOICES;
            list.querySelectorAll('.js-choice-row').forEach(function (row, i) {
                row.querySelector('.js-choice-letter').textContent = String.fromCharCode(65 + i);
                row.querySelectorAll('input[name]').forEach(function (input) {
                    input.name = input.name.replace(/Choices\[[^\]]*\]/, 'Choices[' + i + ']');
                });
            });
        }

        function addRow() {
            if (list.children.length >= MAX_CHOICES) return;
            var html = template.innerHTML.replace(/__index__/g, list.children.length);
            list.insertAdjacentHTML('beforeend', html);
            renumber();
        }

        function toggle() {
            var show = choiceTypeIds.indexOf(select.value) !== -1;
            editor.hidden = !show;
            // Disabled inputs are not posted, so hidden choices never reach the server.
            editor.querySelectorAll('.js-choices-list input').forEach(function (input) {
                input.disabled = !show;
            });
            if (show && list.children.length === 0) {
                for (var i = 0; i < MAX_CHOICES; i++) addRow();
            }
        }

        addButton.addEventListener('click', function () {
            addRow();
            var inputs = list.querySelectorAll('input[type="text"]');
            inputs[inputs.length - 1].focus();
        });

        // Only one choice can be correct: mirror the selected radio into each row's hidden IsCorrect.
        list.addEventListener('change', function (e) {
            if (!e.target.classList.contains('js-correct-radio')) return;
            list.querySelectorAll('.js-choice-row').forEach(function (row) {
                row.querySelector('.js-correct-value').value =
                    row.querySelector('.js-correct-radio').checked ? 'true' : 'false';
            });
        });

        list.addEventListener('click', function (e) {
            var button = e.target.closest('.js-remove-choice');
            if (!button) return;
            button.closest('.js-choice-row').remove();
            renumber();
        });

        select.addEventListener('change', toggle);
        renumber();
        toggle();
    }

    document.querySelectorAll('.js-choices-editor').forEach(init);
})();
