// Global variable to keep track of the number of invoice items
let itemIndex = 0;

$(document).ready(function () {
    console.log('Document ready');

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

    // Event listener for viewing invoice details
    $('.view-invoice').click(function (e) {
        e.preventDefault();
        var invoiceId = $(this).data('invoice-id');
        fetchInvoiceDetails(invoiceId);
    });
});

// Function to populate customer details in the "Add Invoice" modal
function populateCustomerDetails() {
    console.log('Populating customer details');

    // Get customer details from the main page
    var customerName = $('input[name="[0].CustomerName"]').val();
    var carRego = $('input[name="[0].CarRego"]').val();
    var carModel = $('input[name="[0].CarModel"]').val();
    var carYear = $('input[name="[0].CarYear"]').val();

    // Set the values in the modal fields
    $('#CustomerName').val(customerName);
    $('#CarRego').val(carRego);
    $('#CarModel').val(carModel);
    $('#CarYear').val(carYear);

    console.log('Customer details populated:',
        { name: customerName, rego: carRego, model: carModel, year: carYear });
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

// Function to calculate totals for the entire invoice
function calculateTotals() {
    let subTotal = 0;
    // Calculate totals for each invoice item
    $('.invoice-item').each(function () {
        const quantity = parseFloat($(this).find('.quantity-input').val()) || 0;
        const price = parseFloat($(this).find('.price-input').val()) || 0;
        const itemTotal = quantity * price;
        $(this).find('.total-input').val(itemTotal.toFixed(2));
        subTotal += itemTotal;
    });

    // Add labor price and shipping fee to subtotal
    const laborPrice = parseFloat($('#LaborPrice').val()) || 0;
    const shippingFee = parseFloat($('#ShippingFee').val()) || 0;
    subTotal += laborPrice + shippingFee;

    $('#SubTotal').val(subTotal.toFixed(2));

    // Calculate total amount after discount
    const discount = parseFloat($('#Discount').val()) || 0;
    const totalAmount = Math.max(subTotal - discount, 0);
    $('#TotalAmount').val(totalAmount.toFixed(2));

    // Determine if the invoice is paid
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

// Helper function to validate date format (YYYY-MM-DD)
function isValidDate(dateString) {
    var regEx = /^\d{4}-\d{2}-\d{2}$/;
    if (!dateString.match(regEx)) return false;
    var d = new Date(dateString);
    var dNum = d.getTime();
    if (!dNum && dNum !== 0) return false;
    return d.toISOString().slice(0, 10) === dateString;
}

// Function to submit the invoice data to the server
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
        url: addInvoiceUrl, // Use the URL variable defined in the view
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

// Function to fetch invoice details from the server
function fetchInvoiceDetails(invoiceId) {
    $.ajax({
        url: getInvoiceDetailsUrl + '?id=' + invoiceId, // Use the URL variable defined in the view
        type: 'GET',
        success: function (response) {
            if (response.success) {
                populateViewModal(response.data);
                $('#viewInvoiceModal').modal('show');
            } else {
                alert('Error: ' + response.message);
            }
        },
        error: function () {
            alert('An error occurred while fetching invoice details.');
        }
    });
}

// Function to populate the view modal with invoice details
function populateViewModal(invoice) {
    $('#ViewCustomerName').val(invoice.customerName);
    $('#ViewCarRego').val(invoice.carRego);
    $('#ViewDateAdded').val(invoice.dateAdded);
    $('#ViewDueDate').val(invoice.dueDate);
    $('#ViewIssueName').val(invoice.issueName);
    $('#ViewPaymentTerm').val(invoice.paymentTerm);
    $('#ViewNotes').val(invoice.notes);
    $('#ViewLaborPrice').val(invoice.laborPrice);
    $('#ViewDiscount').val(invoice.discount);
    $('#ViewShippingFee').val(invoice.shippingFee);
    $('#ViewSubTotal').val(invoice.subTotal);
    $('#ViewTotalAmount').val(invoice.totalAmount);
    $('#ViewAmountPaid').val(invoice.amountPaid);
    $('#ViewPaymentStatus').val(invoice.isPaid ? 'Paid' : 'Not Paid');

    $('#viewInvoiceItems').empty();
    invoice.invoiceItems.forEach(function (item) {
        var itemHtml = `
            <div class="invoice-item d-flex mb-3">
                <div class="form-group">
                    <label>Item Name</label>
                    <input type="text" class="form-control" value="${item.itemName}" readonly />
                </div>
                <div class="form-group ms-4">
                    <label>Quantity</label>
                    <input type="text" class="form-control" value="${item.quantity}" readonly />
                </div>
                <div class="form-group ms-4">
                    <label>Item Price</label>
                    <input type="text" class="form-control" value="${item.itemPrice}" readonly />
                </div>
                <div class="form-group ms-4">
                    <label>Item Total</label>
                    <input type="text" class="form-control" value="${item.itemTotal}" readonly />
                </div>
            </div>
        `;
        $('#viewInvoiceItems').append(itemHtml);
    });
}

// Function to clear the invoice form
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
    populateCustomerDetails(); // Re-populate customer details after clearing
}