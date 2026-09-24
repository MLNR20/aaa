// Asks for SweetAlert confirmation before submitting any <form data-confirm-title="...">.
//   data-confirm-title    popup title (required)
//   data-confirm-text     message; "{name}" is replaced with the value of data-confirm-name-field
//   data-confirm-name-field  name of the input holding the item name, e.g. "Role.Name"
//   data-confirm-name     fixed item name, used when there is no name field
//   data-confirm-button   confirm button label (default "Confirm")
//   data-confirm-variant  "danger" | "primary" (default) — sets icon and button color
(function () {
    if (typeof Swal === 'undefined') {
        return;
    }

    var variants = {
        danger: { icon: 'warning', color: '#dc3545' },
        primary: { icon: 'question', color: '#0d6efd' }
    };

    document.querySelectorAll('form[data-confirm-title]').forEach(function (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();

            // Let jQuery unobtrusive validation block invalid forms before asking.
            if (window.jQuery && jQuery.fn.valid && !jQuery(form).valid()) {
                return;
            }

            var data = form.dataset;
            var nameField = data.confirmNameField ? form.querySelector('[name="' + data.confirmNameField + '"]') : null;
            var name = nameField ? nameField.value.trim() : (data.confirmName || '');
            var variant = variants[data.confirmVariant] || variants.primary;
            var modal = form.closest('.modal');

            Swal.fire({
                target: modal || 'body',
                title: data.confirmTitle,
                text: (data.confirmText || 'Are you sure?').replace('{name}', name),
                icon: variant.icon,
                showCancelButton: true,
                confirmButtonText: data.confirmButton || 'Confirm',
                confirmButtonColor: variant.color,
                cancelButtonText: 'Cancel'
            }).then(function (result) {
                if (result.isConfirmed) {
                    form.submit();
                }
            });
        });
    });
})();
