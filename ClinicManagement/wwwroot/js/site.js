// ═══════════════════════════════════════════════════════
// Hospital Clinic Management System — JavaScript
// ═══════════════════════════════════════════════════════

// Sidebar toggle
document.addEventListener('DOMContentLoaded', function () {
    const toggle = document.getElementById('sidebarToggle');
    const sidebar = document.getElementById('sidebar');
    const backdrop = document.getElementById('sidebarBackdrop');

    if (toggle) {
        toggle.addEventListener('click', () => {
            sidebar.classList.toggle('open');
            backdrop.classList.toggle('show');
        });
    }
    if (backdrop) {
        backdrop.addEventListener('click', () => {
            sidebar.classList.remove('open');
            backdrop.classList.remove('show');
        });
    }

    // Init DataTables
    if (typeof $.fn.DataTable !== 'undefined') {
        document.querySelectorAll('.data-table').forEach(table => {
            $(table).DataTable({
                pageLength: 10,
                responsive: true,
                language: { search: '', searchPlaceholder: 'Search...' },
                dom: '<"d-flex justify-content-between align-items-center mb-3"lf>tip'
            });
        });
    }

    // Toast notifications
    showToasts();

    // Notification bell
    loadNotifications();

    // Confirm dialogs
    document.querySelectorAll('[data-confirm]').forEach(el => {
        el.addEventListener('click', function (e) {
            if (!confirm(this.getAttribute('data-confirm'))) {
                e.preventDefault();
            }
        });
    });
});

// Toast system
function showToasts() {
    const container = document.getElementById('toastContainer');
    if (!container) return;

    container.querySelectorAll('.toast').forEach(toast => {
        setTimeout(() => {
            toast.style.opacity = '0';
            toast.style.transform = 'translateX(40px)';
            setTimeout(() => toast.remove(), 300);
        }, 4000);
    });
}

function showToast(message, type = 'success') {
    let container = document.getElementById('toastContainer');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toastContainer';
        container.className = 'toast-container';
        document.body.appendChild(container);
    }
    const icons = { success: 'bi-check-circle-fill', error: 'bi-exclamation-triangle-fill', info: 'bi-info-circle-fill' };
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.innerHTML = `<i class="bi ${icons[type] || icons.info}"></i> ${message}`;
    container.appendChild(toast);
    setTimeout(() => { toast.style.opacity = '0'; setTimeout(() => toast.remove(), 300); }, 4000);
}

// Loading spinner
function showLoading() {
    const overlay = document.getElementById('loadingOverlay');
    if (overlay) overlay.classList.add('active');
}
function hideLoading() {
    const overlay = document.getElementById('loadingOverlay');
    if (overlay) overlay.classList.remove('active');
}

// Notification bell
function loadNotifications() {
    const bell = document.getElementById('notificationBell');
    const dropdown = document.getElementById('notificationDropdown');
    const badge = document.getElementById('notificationBadge');
    const list = document.getElementById('notificationList');

    if (!bell) return;

    bell.addEventListener('click', function (e) {
        e.stopPropagation();
        dropdown.classList.toggle('show');
    });

    document.addEventListener('click', () => dropdown?.classList.remove('show'));

    fetch('/Notification/GetUnread')
        .then(r => r.json())
        .then(data => {
            if (badge) {
                badge.textContent = data.count;
                badge.style.display = data.count > 0 ? 'flex' : 'none';
            }
            if (list && data.items) {
                list.innerHTML = data.items.length === 0
                    ? '<div class="n-item"><div class="n-message">No new notifications</div></div>'
                    : data.items.map(n => `
                        <div class="n-item" onclick="markNotificationRead(${n.notificationId})">
                            <div class="n-title">${n.title}</div>
                            <div class="n-message">${n.message}</div>
                            <div class="n-time">${n.time}</div>
                        </div>`).join('');
            }
        })
        .catch(() => { });
}

function markNotificationRead(id) {
    fetch(`/Notification/MarkAsRead/${id}`, { method: 'POST' })
        .then(() => loadNotifications());
}

// Dynamic prescription rows
function addPrescriptionRow() {
    const container = document.getElementById('prescriptionItems');
    if (!container) return;
    const index = container.querySelectorAll('.prescription-row').length;
    const row = document.createElement('div');
    row.className = 'prescription-row row g-2 mb-2';
    row.innerHTML = `
        <div class="col-md-3"><input name="PrescriptionItems[${index}].MedicineName" class="form-control" placeholder="Medicine" required /></div>
        <div class="col-md-2"><input name="PrescriptionItems[${index}].Dosage" class="form-control" placeholder="Dosage" required /></div>
        <div class="col-md-2"><input name="PrescriptionItems[${index}].Frequency" class="form-control" placeholder="Frequency" required /></div>
        <div class="col-md-2"><input name="PrescriptionItems[${index}].Duration" class="form-control" placeholder="Duration" /></div>
        <div class="col-md-2"><input name="PrescriptionItems[${index}].Instructions" class="form-control" placeholder="Instructions" /></div>
        <div class="col-md-1"><button type="button" class="btn btn-danger btn-sm" onclick="this.closest('.prescription-row').remove()"><i class="bi bi-trash"></i></button></div>`;
    container.appendChild(row);
}

// Appointment booking wizard - AJAX helpers
function loadDoctors(specializationId) {
    const select = document.getElementById('doctorSelect');
    if (!select || !specializationId) return;
    select.innerHTML = '<option value="">Loading...</option>';
    fetch(`/Appointment/GetDoctorsBySpecialization?specializationId=${specializationId}`)
        .then(r => r.json())
        .then(doctors => {
            select.innerHTML = '<option value="">-- Select Doctor --</option>';
            doctors.forEach(d => {
                select.innerHTML += `<option value="${d.doctorId}">${d.name} ($${d.consultationFee})</option>`;
            });
        });
}

function loadSlots(doctorId, date) {
    const select = document.getElementById('slotSelect');
    if (!select || !doctorId || !date) return;
    select.innerHTML = '<option value="">Loading...</option>';
    fetch(`/Appointment/GetAvailableSlots?doctorId=${doctorId}&date=${date}`)
        .then(r => r.json())
        .then(slots => {
            select.innerHTML = '<option value="">-- Select Time --</option>';
            if (slots.length === 0) {
                select.innerHTML = '<option value="">No available slots</option>';
            } else {
                slots.forEach(s => {
                    select.innerHTML += `<option value="${s}">${s}</option>`;
                });
            }
        });
}

// Form loading state
document.querySelectorAll('form[data-loading]').forEach(form => {
    form.addEventListener('submit', function () {
        showLoading();
    });
});
