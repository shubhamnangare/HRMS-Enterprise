// Initialize tooltips
var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
    return new bootstrap.Tooltip(tooltipTriggerEl)
})

// Initialize popovers
var popoverTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="popover"]'))
var popoverList = popoverTriggerList.map(function (popoverTriggerEl) {
    return new bootstrap.Popover(popoverTriggerEl)
})

// Initialize Select2
$(document).ready(function () {
    $('.select2').select2({
        theme: 'bootstrap-5',
        width: '100%'
    });

    // Initialize DataTables
    $('.dataTable').DataTable({
        language: {
            search: "_INPUT_",
            searchPlaceholder: "Search...",
            lengthMenu: "Show _MENU_ entries",
            info: "Showing _START_ to _END_ of _TOTAL_ entries",
            paginate: {
                first: "First",
                last: "Last",
                next: "Next",
                previous: "Previous"
            }
        },
        pageLength: 25,
        responsive: true
    });

    // Load notifications
    loadNotifications();
});

// Global notification function
function showNotification(message, type) {
    Swal.fire({
        title: message,
        icon: type,
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true
    });
}

// Load notifications
function loadNotifications() {
    $.get('/api/notifications/recent', function (data) {
        var count = data.length;
        $('#notificationCount').text(count);

        if (count > 0) {
            var html = '<span class="dropdown-header">' + count + ' Notifications</span>';
            data.forEach(function (notif) {
                html += '<div class="dropdown-divider"></div>';
                html += '<a href="' + notif.link + '" class="dropdown-item">';
                html += '<i class="fas fa-' + notif.icon + ' mr-2"></i> ' + notif.message;
                html += '<span class="float-right text-muted text-sm">' + moment(notif.timestamp).fromNow() + '</span>';
                html += '</a>';
            });
            $('#notificationDropdown').html(html);
        }
    }).fail(function () {
        console.log('Failed to load notifications');
    });
}

// Confirm delete function
function confirmDelete(formId, itemName) {
    Swal.fire({
        title: 'Are you sure?',
        text: 'You are about to delete ' + itemName + '. This action cannot be undone!',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#6c757d',
        confirmButtonText: 'Yes, delete it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $('#' + formId).submit();
        }
    });
    return false;
}

// Format currency
function formatCurrency(amount) {
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD',
        minimumFractionDigits: 0,
        maximumFractionDigits: 0
    }).format(amount);
}

// Export to Excel
function exportToExcel(url) {
    window.location.href = url;
}

// Print table
function printTable() {
    window.print();
}

// Toggle sidebar on mobile
$(document).on('click', '[data-widget="pushmenu"]', function (e) {
    e.preventDefault();
    $('body').toggleClass('sidebar-collapse');
});