(function () {
    function updateClock() {
        var el = document.getElementById('currentTime');
        if (!el) return;
        el.textContent = new Date().toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', second: '2-digit' });
    }

    document.addEventListener('DOMContentLoaded', function () {
        updateClock();
        setInterval(updateClock, 1000);
    });
})();
