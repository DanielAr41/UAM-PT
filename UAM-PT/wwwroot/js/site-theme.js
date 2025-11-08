// site-theme.js - helpers for NovaMer theme
document.addEventListener('DOMContentLoaded', function () {

    // Logo click -> home (safe check)
    const logo = document.querySelector('.logo img, #logoNovaMer, .nav_image');
    if (logo) {
        logo.addEventListener('click', () => {
            window.location.href = '/Home/Inicio';
        });
    }

    // Search icon behaviour: focus input if clicked
    document.querySelectorAll('.header .search .icon').forEach(el => {
        el.addEventListener('click', () => {
            const input = el.closest('.search').querySelector('input');
            if (input) { input.focus(); }
        });
    });

    // Small helper: open modal by id via data attributes (optional)
    document.querySelectorAll('[data-open-modal]').forEach(btn => {
        btn.addEventListener('click', () => {
            const id = btn.getAttribute('data-open-modal');
            const modalEl = document.getElementById(id);
            if (modalEl) new bootstrap.Modal(modalEl).show();
        });
    });

});
