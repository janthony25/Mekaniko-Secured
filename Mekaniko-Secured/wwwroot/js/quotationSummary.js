$(document).ready(function () {
    console.log('Quotation summary page loaded');

    // View PDF functionality
    $(document).on('click', '.view-pdf', function (e) {
        e.preventDefault();
        var quotationId = $(this).data('quotation-id');
        viewPdf(quotationId);
    });

    // Download PDF functionality
    $(document).on('click', '.download-pdf', function (e) {
        e.preventDefault();
        var quotationId = $(this).data('quotation-id');
        downloadPdf(quotationId);
    });

    // Send Email functionality
    $(document).on('click', '.send-quotation-email', function (e) {
        e.preventDefault();
        var quotationId = $(this).data('quotation-id');
        sendQuotationEmail(quotationId);
    });
});

function viewPdf(quotationId) {
    console.log('Viewing PDF for quotation ID:', quotationId);
    var url = viewPdfUrl + '?id=' + quotationId;
    $('#pdfViewerFrame').attr('src', url);
    $('#pdfViewerModal').modal('show');
}

function downloadPdf(quotationId) {
    console.log('Downloading PDF for quotation ID:', quotationId);
    var url = downloadPdfUrl + '?id=' + quotationId + '&download=true';

    // Create a temporary anchor element
    var link = document.createElement('a');
    link.href = url;
    link.target = '_blank';
    link.download = 'Quotation_' + quotationId + '.pdf';

    // Append to body, click programmatically, and remove
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
}

function sendQuotationEmail(quotationId) {
    console.log('Sending email for quotation ID:', quotationId);
    $.ajax({
        url: sendEmailUrl,
        type: 'POST',
        data: { quotationId: quotationId },
        headers: {
            'RequestVerificationToken': csrfToken
        },
        success: function (response) {
            if (response.success) {
                alert('Email sent successfully');
                location.reload();
            } else {
                alert('Error sending email: ' + response.message);
            }
        },
        error: function (xhr, status, error) {
            console.error('AJAX error:', status, error);
            alert('An error occurred while sending the email. Please try again.');
        }
    });
}