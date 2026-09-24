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

    window.showSuccessToast = showSuccessToast;
})();
