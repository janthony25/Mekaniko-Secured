// Global variable to keep track of the number of invoice items
let itemIndex = 0;

$(document).ready(function () {
    console.log('Document ready');

    // Ensure these values are defined before you use them
    var csrfToken = $('input[name="__RequestVerificationToken"]').val();
    var markAsPaidUrl = $('#scriptData').data('mark-as-paid-url');

    // Event listener for the "Add Invoice" button
    $('.btn-warning').click(function () {
        console.log('Add Invoice button clicked');
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
        console.log("Invoice ID:", invoiceId);

        // Confirm and mark as paid
        if (confirm("Are you sure you want to mark this invoice as paid?")) {
            markInvoiceAsPaid(invoiceId);
        }
    });

    // Function to handle marking an invoice as paid via AJAX
    function markInvoiceAsPaid(invoiceId) {
        console.log("Sending AJAX request to mark invoice as paid");

        $.ajax({
            url: markAsPaidUrl,
            type: 'POST',
            data: { invoiceId: invoiceId }, // Pass as form data
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
});

// Function to populate customer details when adding an invoice
function populateCustomerDetails() {
    console.log('Populating customer details');
    var customerName = $('input[name="[0].CustomerName"]').val();
    var carRego = $('input[name="[0].CarRego"]').val();
    var carModel = $('input[name="[0].CarModel"]').val();
    var carYear = $('input[name="[0].CarYear"]').val();
    $('#CustomerName').val(customerName);
    $('#CarRego').val(carRego);
    $('#CarModel').val(carModel);
    $('#CarYear').val(carYear);
    console.log('Customer details populated:', { name: customerName, rego: carRego, model: carModel, year: carYear });
}

// Function to add a new invoice item to the form
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

// Function to calculate totals for the invoice based on input values
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

// Function to validate the form before submission
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

// Helper function to check if a date is valid
function isValidDate(dateString) {
    var regEx = /^\d{4}-\d{2}-\d{2}$/;
    if (!dateString.match(regEx)) return false;
    var d = new Date(dateString);
    var dNum = d.getTime();
    if (!dNum && dNum !== 0) return false;
    return d.toISOString().slice(0, 10) === dateString;
}

// Function to submit the invoice form data via AJAX
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

    // AJAX request to submit invoice data to the server
    $.ajax({
        url: addInvoiceUrl,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(invoiceData),
        headers: {
            'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
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

// Function to clear the invoice form after submission or cancellation
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

// Function to view a PDF of the invoice in a modal
function viewPdf(e) {
    e.preventDefault();
    console.log('View PDF button clicked');
    var invoiceId = $(this).data('invoice-id');
    console.log('Invoice ID:', invoiceId);
    if (!invoiceId) {
        console.error('No invoice ID found');
        return;
    }
    var viewUrl = '/Invoice/GeneratePdf?id=' + invoiceId;
    console.log('View URL:', viewUrl);
    $('#pdfViewerFrame').attr('src', viewUrl);
    console.log('iframe src set');
    $('#pdfViewerModal').modal('show');
    console.log('Modal should be shown now');
}

// Function to download a PDF of the invoice
function downloadPdf(e) {
    e.preventDefault();
    var invoiceId = $(this).data('invoice-id');
    var downloadUrl = '/Invoice/GeneratePdf?id=' + invoiceId + '&download=true';
    window.location.href = downloadUrl;
}

// Function to send an invoice email via AJAX
function sendInvoiceEmail(e) {
    e.preventDefault();
    var invoiceId = $(this).data('invoice-id');
    console.log('Sending email for invoice ID:', invoiceId);

    // AJAX request to send the invoice email
    $.ajax({
        url: '/Invoice/SendInvoiceEmail',
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

// Handle PDF download
$(document).on('click', '.download-pdf', function (e) {
    e.preventDefault();
    var quotationId = $(this).data('quotation-id');
    var downloadUrl = '/Quotation/GeneratePdf?id=' + quotationId + '&download=true';
    window.location.href = downloadUrl;
});