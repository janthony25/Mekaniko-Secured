$(document).ready(function () {
    var csrfToken = $('input[name="__RequestVerificationToken"]').val();
    var markAsPaidUrl = $('#scriptData').data('mark-as-paid-url');
    var viewPdfUrl = $('#scriptData').data('view-pdf-url');
    var sendInvoiceEmailUrl = $('#scriptData').data('send-invoice-email-url');
    var deleteInvoiceUrl = $('#scriptData').data('delete-invoice-url');

    // Mark as Paid functionality
    $('.mark-as-paid').click(function (e) {
        e.preventDefault();
        var invoiceId = $(this).data('invoice-id');
        console.log("Marking as paid invoice ID:", invoiceId);
        $.ajax({
            url: markAsPaidUrl,
            type: 'POST',
            data: { invoiceId: invoiceId },
            headers: {
                'RequestVerificationToken': csrfToken
            },
            success: function (response) {
                console.log("Response:", response);
                if (response.success) {
                    alert(response.message);
                    location.reload();
                } else {
                    alert('Error: ' + response.message);
                }
            },
            error: function (xhr, status, error) {
                console.error('AJAX error:', xhr.responseText);
                alert('An error occurred while marking the invoice as paid.');
            }
        });
    });

    // View PDF functionality
    $('.view-pdf').click(function (e) {
        e.preventDefault();
        var invoiceId = $(this).data('invoice-id');
        var url = viewPdfUrl + '?id=' + invoiceId;
        $('#pdfViewerFrame').attr('src', url);
        $('#pdfViewerModal').modal('show');
    });

    // Download PDF functionality
    $('.download-pdf').click(function (e) {
        e.preventDefault();
        var invoiceId = $(this).data('invoice-id');
        var downloadUrl = viewPdfUrl + '?id=' + invoiceId + '&download=true';
        window.location.href = downloadUrl;
    });

    // Send Invoice Email functionality
    $('.send-invoice-email').click(function (e) {
        e.preventDefault();
        var invoiceId = $(this).data('invoice-id');
        console.log("Sending email for invoice ID:", invoiceId);
        $.ajax({
            url: sendInvoiceEmailUrl,
            type: 'POST',
            data: { invoiceId: invoiceId },
            headers: {
                'RequestVerificationToken': csrfToken
            },
            success: function (response) {
                console.log("Response:", response);
                if (response.success) {
                    alert(response.message);
                } else {
                    alert('Error: ' + response.message);
                }
            },
            error: function (xhr, status, error) {
                console.error('AJAX error:', xhr.responseText);
                alert('An error occurred while sending the email.');
            }
        });
    });

    // Delete Invoice functionality
    $('.delete-invoice').click(function (e) {
        e.preventDefault();
        var invoiceId = $(this).data('invoice-id');
        $('#deleteInvoiceId').text(invoiceId);
        $('#deleteInvoiceModal').modal('show');
    });

    $('#confirmDeleteInvoice').click(function () {
        var invoiceId = $('#deleteInvoiceId').text();
        $.ajax({
            url: deleteInvoiceUrl,
            type: 'POST',
            data: { invoiceId: invoiceId }, // Ensure this matches the controller parameter name
            headers: {
                'RequestVerificationToken': csrfToken
            },
            success: function (response) {
                console.log("Response:", response);
                if (response.success) {
                    alert(response.message);
                    location.reload();
                } else {
                    alert('Error: ' + response.message);
                }
            },
            error: function (xhr, status, error) {
                console.error('AJAX error:', xhr.responseText);
                alert('An error occurred while deleting the invoice.');
            }
        });
        $('#deleteInvoiceModal').modal('hide');
    });
});