(function () {
    var STORAGE_KEY = 'ems-sidebar-collapsed';

    function applyState(sidebar, toggle, collapsed) {
        sidebar.classList.toggle('collapsed', collapsed);
        if (toggle) {
            toggle.setAttribute('aria-expanded', String(!collapsed));
        }
    }

    function highlightActiveLink(sidebar) {
        var currentPath = window.location.pathname.replace(/\/$/, '').toLowerCase();
        sidebar.querySelectorAll('.sidebar-link').forEach(function (link) {
            var linkPath = (link.getAttribute('href') || '').replace(/\/$/, '').toLowerCase();
            if (linkPath && (currentPath === linkPath || currentPath.indexOf(linkPath + '/') === 0)) {
                link.classList.add('active');
            }
        });
    }

    document.addEventListener('DOMContentLoaded', function () {
        var sidebar = document.getElementById('sidebar');
        var toggle = document.getElementById('sidebarToggle');
        if (!sidebar) return;

        var collapsed = false;
        try {
            collapsed = localStorage.getItem(STORAGE_KEY) === 'true';
        } catch (e) { /* localStorage unavailable */ }

        applyState(sidebar, toggle, collapsed);
        highlightActiveLink(sidebar);

        if (toggle) {
            toggle.addEventListener('click', function () {
                var willCollapse = !sidebar.classList.contains('collapsed');
                applyState(sidebar, toggle, willCollapse);
                try {
                    localStorage.setItem(STORAGE_KEY, String(willCollapse));
                } catch (e) { /* localStorage unavailable */ }
            });
        }
    });
})();
