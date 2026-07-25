/* ============================================================
   Hotel Reservation Admin - client-side helpers
   ============================================================ */
(function () {
    'use strict';

    // ----- Mobile sidebar toggle -----
    var sidebar = document.getElementById('sidebar');
    var backdrop = document.getElementById('sidebarBackdrop');
    var toggle = document.getElementById('sidebarToggle');

    function openSidebar() {
        if (!sidebar) return;
        sidebar.classList.add('show');
        if (backdrop) backdrop.classList.add('show');
    }
    function closeSidebar() {
        if (!sidebar) return;
        sidebar.classList.remove('show');
        if (backdrop) backdrop.classList.remove('show');
    }

    if (toggle) toggle.addEventListener('click', openSidebar);
    if (backdrop) backdrop.addEventListener('click', closeSidebar);

    // ----- Highlight active sidebar link -----
    if (sidebar) {
        var path = window.location.pathname.toLowerCase().replace(/\/$/, '');
        var links = sidebar.querySelectorAll('a.nav-link');
        links.forEach(function (link) {
            var href = link.getAttribute('href');
            if (!href || href === '/' || href === '#') return;
            var hrefNorm = href.toLowerCase().replace(/\/$/, '');
            if (path === hrefNorm || (hrefNorm !== '/' && path.indexOf(hrefNorm) === 0)) {
                link.classList.add('active');
            }
        });
    }

    // ----- Auto-dismiss alerts -----
    setTimeout(function () {
        document.querySelectorAll('.alert-auto-dismiss').forEach(function (el) {
            if (!window.bootstrap || !bootstrap.Alert) return;
            var alert = bootstrap.Alert.getOrCreateInstance(el);
            alert.close();
        });
    }, 4000);
})();
