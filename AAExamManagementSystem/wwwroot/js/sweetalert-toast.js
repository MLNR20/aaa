(function () {
    function showSuccessToast(message) {
        if (!message || typeof Swal === 'undefined') {
            return;
        }

        Swal.fire({
            toast: true,
            position: 'top-end',
            icon: 'success',
            title: message,
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true
        });
    }

    // Blocking popup for failures the user must acknowledge (e.g. a delete that was refused).
    function showErrorAlert(message, title) {
        if (!message || typeof Swal === 'undefined') {
            return;
        }

        Swal.fire({
            icon: 'error',
            title: title || 'Something went wrong',
            text: message,
            confirmButtonColor: '#0d6efd'
        });
    }

    window.showSuccessToast = showSuccessToast;
    window.showErrorAlert = showErrorAlert;
})();
