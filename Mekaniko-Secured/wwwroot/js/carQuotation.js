// js/carQuotation.js

// Declare variables that will be set in the view
var csrfToken;
var viewPdfUrl;

// Function to initialize variables
function initializeVariables(token, viewPdf) {
    csrfToken = token;
    viewPdfUrl = viewPdf;
}

// Global variable to keep track of the number of quotation items
let itemIndex = 0;

$(document).ready(function () {
    console.log('Document ready');

    // Event listener for the "Add Quotation" button
    $('.btn-warning[data-bs-target="#addQuotationModal"]').click(function () {
        console.log('Add Quotation button clicked');
        $('#addQuotationModal').modal('show');
        populateCustomerDetails();
    });

    // Event listener for the "Add Item" button
    $(document).on('click', '#addItemButton', function (e) {
        e.preventDefault();
        console.log('Add Item button clicked');
        addQuotationItem();
    });

    // Event listener for the "Save Quotation" button
    $('#saveQuotationBtn').click(function () {
        console.log('Save Quotation button clicked');
        if (validateForm()) {
            submitQuotation();
        }
    });

    // Event listener for viewing PDF
    $(document).on('click', '.view-pdf', viewPdf);

    // Add event listeners for real-time calculation
    $('#quotationForm').on('input', '.calc-input', calculateTotals);
    $('#LaborPrice, #Discount, #ShippingFee').on('input', calculateTotals);
});

// Function to populate customer details when adding a quotation
function populateCustomerDetails() {
    console.log('Populating customer details');
    var customerName = $('input[name="[0].CustomerName"]').val();
    var carRego = $('input[name="[0].CarRego"]').val();
    $('#CustomerName').val(customerName);
    $('#CarRego').val(carRego);
    console.log('Customer details populated:', { name: customerName, rego: carRego });
}

// Function to add a new quotation item to the form
function addQuotationItem() {
    console.log('Adding new quotation item');
    const itemHtml = `
        <div class="quotation-item d-flex mb-3">
            <div class="form-group">
                <label for="QuotationItems[${itemIndex}].ItemName">Item Name</label>
                <input name="QuotationItems[${itemIndex}].ItemName" class="form-control input" required />
            </div>
            <div class="form-group ms-4">
                <label for="QuotationItems[${itemIndex}].Quantity">Quantity</label>
                <input type="number" name="QuotationItems[${itemIndex}].Quantity" class="form-control quantity-input calc-input" required />
            </div>
            <div class="form-group ms-4">
                <label for="QuotationItems[${itemIndex}].ItemPrice">Item Price</label>
                <input type="number" name="QuotationItems[${itemIndex}].ItemPrice" class="form-control price-input calc-input" step="0.01" required />
            </div>
            <div class="form-group ms-4">
                <label for="QuotationItems[${itemIndex}].ItemTotal">Item Total</label>
                <input type="number" name="QuotationItems[${itemIndex}].ItemTotal" class="form-control total-input" readonly />
            </div>
        </div>
    `;
    $('#quotationItems').append(itemHtml);
    itemIndex++;
    calculateTotals();
}

// Function to calculate totals for the quotation based on input values
function calculateTotals() {
    console.log('Calculating totals');
    let subTotal = 0;
    $('.quotation-item').each(function () {
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
    console.log('Totals calculated:', { subTotal, totalAmount });
}

// Function to validate the form before submission
function validateForm() {
    console.log('Validating form');
    let isValid = true;
    $('.error-message').remove();
    $('#quotationForm [required]').each(function () {
        if ($(this).val().trim() === '') {
            isValid = false;
            $(this).after('<span class="error-message text-danger">This field is required.</span>');
        }
    });
    if (!isValidDate($('#DateAdded').val())) {
        isValid = false;
        $('#DateAdded').after('<span class="error-message text-danger">Please enter a valid date.</span>');
    }
    console.log('Form validation result:', isValid);
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

// Function to submit the quotation form data via AJAX
function submitQuotation() {
    console.log('Submitting quotation');
    const quotationData = {
        CarId: parseInt($('#CarId').val()),
        DateAdded: $('#DateAdded').val(),
        IssueName: $('#IssueName').val(),
        Notes: $('#Notes').val(),
        LaborPrice: parseFloat($('#LaborPrice').val()) || 0,
        Discount: parseFloat($('#Discount').val()) || 0,
        ShippingFee: parseFloat($('#ShippingFee').val()) || 0,
        SubTotal: parseFloat($('#SubTotal').val()) || 0,
        TotalAmount: parseFloat($('#TotalAmount').val()) || 0,
        QuotationItems: []
    };
    $('.quotation-item').each(function () {
        quotationData.QuotationItems.push({
            ItemName: $(this).find('input[name$=".ItemName"]').val(),
            Quantity: parseInt($(this).find('input[name$=".Quantity"]').val()) || 0,
            ItemPrice: parseFloat($(this).find('input[name$=".ItemPrice"]').val()) || 0,
            ItemTotal: parseFloat($(this).find('input[name$=".ItemTotal"]').val()) || 0
        });
    });
    console.log('Submitting quotation data:', quotationData);

    // AJAX request to submit quotation data to the server
    $.ajax({
        url: addQuotationUrl,
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(quotationData),
        headers: {
            'RequestVerificationToken': csrfToken
        },
        success: function (response) {
            console.log('AJAX success response:', response);
            if (response.success) {
                alert('Quotation added successfully');
                $('#addQuotationModal').modal('hide');
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
            alert('An error occurred while adding the quotation');
        }
    });
}

// Function to clear the quotation form after submission or cancellation
function clearQuotationForm() {
    console.log('Clearing quotation form');
    $('#quotationForm')[0].reset();
    $('#quotationItems').empty();
    itemIndex = 0;
    $('#SubTotal, #TotalAmount').val('');
    $('.error-message').remove();
    $('#DateAdded, #IssueName, #Notes, #LaborPrice, #Discount, #ShippingFee').val('');
    $('.quotation-item').remove();
    console.log('Quotation form cleared');
    populateCustomerDetails();
}

// Function to view the PDF in an iframe modal
function viewPdf(e) {
    e.preventDefault();
    console.log('View PDF button clicked');
    var quotationId = $(this).data('invoice-id');
    console.log('Quotation ID:', quotationId);
    if (!quotationId) {
        console.error('No quotation ID found');
        return;
    }
    var url = viewPdfUrl + '?id=' + quotationId;
    console.log('View URL:', url);

    // Set the iframe src and show the modal
    $('#pdfViewerFrame').attr('src', url);
    $('#pdfViewerModal').modal('show');
}
