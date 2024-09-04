// invoiceSummary.js

// Wait for the DOM to be fully loaded before executing any JavaScript
$(document).ready(function () {
    console.log('InvoiceSummary page loaded');

    // Event listener for marking an invoice as paid
    $(document).on('click', '.mark-as-paid', function (e) {
        e.preventDefault();
        console.log("Mark as paid clicked");

        var invoiceId = $(this).data('invoice-id');
        console.log("Invoice ID:", invoiceId);

        if (confirm("Are you sure you want to mark this invoice as paid?")) {
            markInvoiceAsPaid(invoiceId);
        }
    });

    // Event listener for viewing PDF
    $(document).on('click', '.view-pdf', viewPdf);

    // Event listener for downloading PDF
    $(document).on('click', '.download-pdf', downloadPdf);

    // Event listener for sending email
    $(document).on('click', '.send-invoice-email', sendInvoiceEmail);

    // Event listener for deleting invoice
    $(document).on('click', '.delete-invoice', function (e) {
        e.preventDefault();
        var invoiceId = $(this).data('invoice-id');
        if (confirm("Are you sure you want to delete this invoice?")) {
            deleteInvoice(invoiceId);
        }
    });
});

// Function to handle marking an invoice as paid
function markInvoiceAsPaid(invoiceId) {
    console.log("Sending AJAX request to mark invoice as paid");

    $.ajax({
        url: markAsPaidUrl,
        type: 'POST',
        data: { invoiceId: invoiceId },
        headers: {
            'RequestVerificationToken': csrfToken
        },
        success: function (response) {
            console.log("AJAX response received:", response);
            if (response.success) {
                alert(response.message);
                location.reload();
            } else {
                alert('Error: ' + response.message);
            }
        },
        error: function (xhr, status, error) {
            console.error('AJAX error:', status, error);
            console.error('Response text:', xhr.responseText);
            alert('An error occurred while marking the invoice as paid.');
        }
    });
}

// Function to view PDF
function viewPdf(e) {
    e.preventDefault();
    console.log('View PDF button clicked');
    var invoiceId = $(this).data('invoice-id');
    console.log('Invoice ID:', invoiceId);
    if (!invoiceId) {
        console.error('No invoice ID found');
        return;
    }
    var url = viewPdfUrl + '?id=' + invoiceId;
    console.log('View URL:', url);
    $('#pdfViewerFrame').attr('src', url);
    $('#pdfViewerModal').modal('show');
}

// Function to download PDF
function downloadPdf(e) {
    e.preventDefault();
    console.log('Download PDF button clicked');
    var invoiceId = $(this).data('invoice-id');
    console.log('Invoice ID:', invoiceId);
    if (!invoiceId) {
        console.error('No invoice ID found');
        return;
    }
    var downloadUrl = viewPdfUrl + '?id=' + invoiceId + '&download=true';
    console.log('Download URL:', downloadUrl);
    window.location.href = downloadUrl;
}

// Function to send invoice email
function sendInvoiceEmail(e) {
    e.preventDefault();
    var invoiceId = $(this).data('invoice-id');
    console.log('Sending email for invoice ID:', invoiceId);

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

// Function to delete invoice
function deleteInvoice(invoiceId) {
    console.log("Deleting invoice:", invoiceId);

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