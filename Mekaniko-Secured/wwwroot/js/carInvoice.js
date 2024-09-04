// Global variables
var csrfToken;
var addInvoiceUrl;
var markAsPaidUrl;
var viewPdfUrl;
var sendInvoiceEmailUrl;
var deleteInvoiceUrl; // Added for deletion

// Global variable to keep track of the number of invoice items
let itemIndex = 0;

$(document).ready(function () {
    console.log('Document ready');

    // Initialize variables
    csrfToken = $('input[name="__RequestVerificationToken"]').val();
    addInvoiceUrl = $('#scriptData').data('add-invoice-url');
    markAsPaidUrl = $('#scriptData').data('mark-as-paid-url');
    viewPdfUrl = $('#scriptData').data('view-pdf-url');
    sendInvoiceEmailUrl = $('#scriptData').data('send-invoice-email-url');
    deleteInvoiceUrl = $('#scriptData').data('delete-invoice-url'); // Initialize deleteInvoiceUrl

    // Event listener for the "Add Invoice" button
    $('.btn-warning[data-bs-target="#addInvoiceModal"]').click(function () {
        console.log('Add Invoice button clicked');
        $('#addInvoiceModal').modal('show');
        populateCustomerDetails();
    });

    // Event listener for the "Add Item" button
    $(document).on('click', '#addItemButton', function (e) {
        e.preventDefault();
        console.log('Add Item button clicked');
        addInvoiceItem();
    });

    // Event listener for the "Save Invoice" button
    $('#saveInvoiceBtn').click(function () {
        if (validateForm()) {
            submitInvoice();
        }
    });

    // Add event listeners for real-time calculation
    $('#invoiceForm').on('input', '.calc-input', calculateTotals);
    $('#LaborPrice, #Discount, #ShippingFee, #AmountPaid').on('input', calculateTotals);

    // Event listeners for PDF actions
    $(document).on('click', '.view-pdf', viewPdf);
    $(document).on('click', '.download-pdf', downloadPdf);

    // Event listener for sending emails
    $(document).on('click', '.send-invoice-email', sendInvoiceEmail);

    // Mark as Paid functionality
    $(document).on('click', '.mark-as-paid', function (e) {
        e.preventDefault();
        console.log("Mark as paid clicked");
        var invoiceId = $(this).data('invoice-id');
        if (confirm("Are you sure you want to mark this invoice as paid?")) {
            markInvoiceAsPaid(invoiceId);
        }
    });

    // Delete Invoice functionality
    $(document).on('click', '.delete-invoice-btn', function () {
        var invoiceId = $(this).data('invoice-id');
        var invoiceIssue = $(this).data('invoice-issue');

        $('#deleteInvoiceId').text(invoiceId);
        $('#deleteInvoiceIssue').text(invoiceIssue);
        $('#hiddenDeleteInvoiceId').val(invoiceId);

        $('#deleteInvoiceModal').modal('show');
    });

    $('#confirmDeleteInvoiceBtn').click(function () {
        var invoiceId = $('#hiddenDeleteInvoiceId').val().trim();

        $.ajax({
            url: deleteInvoiceUrl,
            type: 'POST',
            data: { invoiceId: invoiceId },  // Ensure this matches the controller action parameter name
            headers: {
                'RequestVerificationToken': csrfToken  // CSRF token
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
                alert('Error occurred while deleting invoice.');
                console.error('AJAX error:', status, error);
                console.error('Response text:', xhr.responseText);
            }
        });
    });
});

function populateCustomerDetails() {
    console.log('Populating customer details');
    var customerName = $('p:contains("Customer Name:")').find('.fw-bold').text().trim();
    var carRego = $('p:contains("Car Rego:")').find('.fw-bold').text().trim();
    $('#CustomerName').val(customerName);
    $('#CarRego').val(carRego);
    console.log('Customer details populated:', { name: customerName, rego: carRego });
}


function addInvoiceItem() {
    console.log('Adding new invoice item');
    const itemHtml = `
        <div class="invoice-item d-flex mb-3">
            <div class="form-group">
                <label for="InvoiceItems[${itemIndex}].ItemName">Item Name</label>
                <input name="InvoiceItems[${itemIndex}].ItemName" class="form-control input" required />
            </div>
            <div class="form-group ms-4">
                <label for="InvoiceItems[${itemIndex}].Quantity">Quantity</label>
                <input type="number" name="InvoiceItems[${itemIndex}].Quantity" class="form-control quantity-input calc-input" required />
            </div>
            <div class="form-group ms-4">
                <label for="InvoiceItems[${itemIndex}].ItemPrice">Item Price</label>
                <input type="number" name="InvoiceItems[${itemIndex}].ItemPrice" class="form-control price-input calc-input" step="0.01" required />
            </div>
            <div class="form-group ms-4">
                <label for="InvoiceItems[${itemIndex}].ItemTotal">Item Total</label>
                <input type="number" name="InvoiceItems[${itemIndex}].ItemTotal" class="form-control total-input" readonly />
            </div>
        </div>
    `;
    $('#invoiceItems').append(itemHtml);
    itemIndex++;
    calculateTotals();
}

function calculateTotals() {
    let subTotal = 0;
    $('.invoice-item').each(function () {
        const quantity = parseFloat($(this).find('.quantity-input').val()) || 0;
        const price = parseFloat($(this).find('.price-input').val()) || 0;
        const itemTotal = quantity * price;
        $(this).find('.total-input').val(itemTotal.toFixed(2));
        subTotal += itemTotal;
    });
    const laborPrice = parseFloat($('#LaborPrice').val()) || 0;
    const shippingFee = parseFloat($('#ShippingFee').val()) || 0;
    subTotal += laborPrice + shippingFee;
    $('#SubTotal').val(subTotal.toFixed(2));
    const discount = parseFloat($('#Discount').val()) || 0;
    const totalAmount = Math.max(subTotal - discount, 0);
    $('#TotalAmount').val(totalAmount.toFixed(2));
    const amountPaid = parseFloat($('#AmountPaid').val()) || 0;
    const isPaid = amountPaid >= totalAmount;
    $('#PaymentStatus').val(isPaid ? "Paid" : "Not Paid");
    $('#IsPaid').val(isPaid);
}

function validateForm() {
    let isValid = true;
    $('.error-message').remove();
    $('#invoiceForm [required]').each(function () {
        if ($(this).val().trim() === '') {
            isValid = false;
            $(this).after('<span class="error-message text-danger">This field is required.</span>');
        }
    });
    if (!isValidDate($('#DateAdded').val())) {
        isValid = false;
        $('#DateAdded').after('<span class="error-message text-danger">Please enter a valid date.</span>');
    }
    if (!isValidDate($('#DueDate').val())) {
        isValid = false;
        $('#DueDate').after('<span class="error-message text-danger">Please enter a valid date.</span>');
    }
    return isValid;
}

function isValidDate(dateString) {
    var regEx = /^\d{4}-\d{2}-\d{2}$/;
    if (!dateString.match(regEx)) return false;
    var d = new Date(dateString);
    var dNum = d.getTime();
    if (!dNum && dNum !== 0) return false;
    return d.toISOString().slice(0, 10) === dateString;
}

function submitInvoice() {
    const invoiceData = {
        CarId: parseInt($('#CarId').val()),
        DateAdded: $('#DateAdded').val(),
        DueDate: $('#DueDate').val(),
        IssueName: $('#IssueName').val(),
        PaymentTerm: $('#PaymentTerm').val(),
        Notes: $('#Notes').val(),
        LaborPrice: parseFloat($('#LaborPrice').val()) || 0,
        Discount: parseFloat($('#Discount').val()) || 0,
        ShippingFee: parseFloat($('#ShippingFee').val()) || 0,
        SubTotal: parseFloat($('#SubTotal').val()) || 0,
        TotalAmount: parseFloat($('#TotalAmount').val()) || 0,
        AmountPaid: parseFloat($('#AmountPaid').val()) || 0,
        IsPaid: $('#PaymentStatus').val() === "Paid",
        InvoiceItems: []
    };
    $('.invoice-item').each(function () {
        invoiceData.InvoiceItems.push({
            ItemName: $(this).find('input[name$=".ItemName"]').val(),
            Quantity: parseInt($(this).find('input[name$=".Quantity"]').val()) || 0,
            ItemPrice: parseFloat($(this).find('input[name$=".ItemPrice"]').val()) || 0,
            ItemTotal: parseFloat($(this).find('input[name$=".ItemTotal"]').val()) || 0
        });
    });
    console.log('Submitting invoice data:', invoiceData);

    $.ajax({
        url: addInvoiceUrl,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(invoiceData),
        headers: {
            'RequestVerificationToken': csrfToken
        },
        success: function (response) {
            if (response.success) {
                alert('Invoice added successfully');
                $('#addInvoiceModal').modal('hide');
                location.reload();
            } else {
                console.error('Server response:', response);
                if (response.errors) {
                    let errorMessage = "Validation errors:\n";
                    for (let field in response.errors) {
                        errorMessage += `${field}: ${response.errors[field].join(', ')}\n`;
                    }
                    alert(errorMessage);
                } else {
                    alert('Error: ' + response.message);
                }
            }
        },
        error: function (xhr, status, error) {
            console.error('AJAX error:', status, error);
            console.error('Response:', xhr.responseText);
            alert('An error occurred while adding the invoice');
        }
    });
}

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

function clearInvoiceForm() {
    console.log('Clearing invoice form');
    $('#invoiceForm')[0].reset();
    $('#invoiceItems').empty();
    itemIndex = 0;
    $('#SubTotal, #TotalAmount, #PaymentStatus').val('');
    $('.error-message').remove();
    $('#DateAdded, #DueDate, #IssueName, #PaymentTerm, #Notes, #LaborPrice, #Discount, #ShippingFee, #AmountPaid').val('');
    $('.invoice-item').remove();
    console.log('Invoice form cleared');
    populateCustomerDetails();
}