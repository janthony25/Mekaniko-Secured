$(document).ready(function () {
    // Mark as Paid functionality
    $('.mark-as-paid').click(function (e) {
        e.preventDefault();
        var invoiceId = $(this).data('invoice-id');
        markInvoiceAsPaid(invoiceId);
    });

    // View PDF functionality
    $('.view-pdf').click(function (e) {
        e.preventDefault();
        var invoiceId = $(this).data('invoice-id');
        viewPdf(invoiceId);
    });

    // Download PDF functionality
    $('.download-pdf').click(function (e) {
        e.preventDefault();
        var invoiceId = $(this).data('invoice-id');
        downloadPdf(invoiceId);
    });

    // Send Email functionality
    $('.send-invoice-email').click(function (e) {
        e.preventDefault();
        var invoiceId = $(this).data('invoice-id');
        sendInvoiceEmail(invoiceId);
    });

    // Delete Invoice functionality
    $('.delete-invoice').click(function (e) {
        e.preventDefault();
        var invoiceId = $(this).data('invoice-id');
        showDeleteConfirmation(invoiceId);
    });

    // Confirm Delete Invoice
    $('#confirmDeleteInvoice').click(function () {
        var invoiceId = $('#deleteInvoiceId').text();
        deleteInvoice(invoiceId);
    });
});

function markInvoiceAsPaid(invoiceId) {
    $.ajax({
        url: markAsPaidUrl,
        type: 'POST',
        data: { invoiceId: invoiceId },
        headers: {
            'RequestVerificationToken': csrfToken
        },
        success: function (response) {
            if (response.success) {
                alert(response.message);
                location.reload();
            } else {
                alert('Error: ' + response.message);
            }
        },
        error: function (xhr, status, error) {
            console.error('AJAX error:', status, error);
            alert('An error occurred while marking the invoice as paid.');
        }
    });
}

function viewPdf(invoiceId) {
    var url = viewPdfUrl + '?id=' + invoiceId;
    $('#pdfViewerFrame').attr('src', url);
    $('#pdfViewerModal').modal('show');
}

function downloadPdf(invoiceId) {
    var downloadUrl = viewPdfUrl + '?id=' + invoiceId + '&download=true';
    window.location.href = downloadUrl;
}

function sendInvoiceEmail(invoiceId) {
    $.ajax({
        url: sendInvoiceEmailUrl,
        type: 'POST',
        data: { invoiceId: invoiceId },
        headers: {
            'RequestVerificationToken': csrfToken
        },
        success: function (response) {
            if (response.success) {
                alert(response.message);
            } else {
                alert('Error: ' + response.message);
            }
        },
        error: function (xhr, status, error) {
            console.error('AJAX error:', status, error);
            alert('An error occurred while sending the email. Please try again.');
        }
    });
}

function showDeleteConfirmation(invoiceId) {
    $('#deleteInvoiceId').text(invoiceId);
    $('#deleteInvoiceModal').modal('show');
}

function deleteInvoice(invoiceId) {
    $.ajax({
        url: deleteInvoiceUrl,
        type: 'POST',
        data: { invoiceId: invoiceId },
        headers: {
            'RequestVerificationToken': csrfToken
        },
        success: function (response) {
            if (response.success) {
                alert(response.message);
                $('#deleteInvoiceModal').modal('hide');
                location.reload();
            } else {
                alert('Error: ' + response.message);
            }
        },
        error: function (xhr, status, error) {
            console.error('AJAX error:', status, error);
            alert('An error occurred while deleting the invoice.');
        }
    });
}