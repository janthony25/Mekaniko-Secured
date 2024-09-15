$(document).ready(function () {
    console.log("Document ready");

    // Get the URLs from the scriptData div
    var addInvoiceUrl = $('#scriptData').data('add-invoice-url');
    var getInvoiceDetailsUrl = $('#scriptData').data('get-invoice-details-url');
    var markAsPaidUrl = $('#scriptData').data('mark-as-paid-url');
    var viewPdfUrl = $('#scriptData').data('view-pdf-url');
    var sendInvoiceEmailUrl = $('#scriptData').data('send-invoice-email-url');
    var updateInvoiceNotesUrl = $('#scriptData').data('update-invoice-notes-url');
    var deleteInvoiceUrl = $('#scriptData').data('delete-invoice-url'); // Added for delete functionality

    // Get the CSRF token
    var csrfToken = $('input[name="__RequestVerificationToken"]').val();

    // Add Invoice Item functionality
    $('#addItemButton').click(function () {
        var newItem = $(`
            <div class="invoice-item d-flex justify-content-between align-items-center mb-2">
                <input type="text" class="form-control me-2 item-name" placeholder="Item Name">
                <input type="number" class="form-control me-2 item-quantity" placeholder="Quantity">
                <input type="number" class="form-control me-2 item-price" placeholder="Price">
                <input type="number" class="form-control item-total" placeholder="Total" readonly>
                <button type="button" class="btn btn-danger ms-2 remove-item">Remove</button>
            </div>
        `);
        $('#invoiceItems').append(newItem);
    });

    // Remove Invoice Item functionality
    $(document).on('click', '.remove-item', function () {
        $(this).closest('.invoice-item').remove();
        updateTotals();
    });

    // Update item total and invoice totals when quantity or price changes
    $(document).on('input', '.item-quantity, .item-price', function () {
        var item = $(this).closest('.invoice-item');
        var quantity = parseFloat(item.find('.item-quantity').val()) || 0;
        var price = parseFloat(item.find('.item-price').val()) || 0;
        var total = quantity * price;
        item.find('.item-total').val(total.toFixed(2));
        updateTotals();
    });

    // Update totals
    function updateTotals() {
        var subTotal = 0;
        $('.item-total').each(function () {
            subTotal += parseFloat($(this).val()) || 0;
        });

        var laborPrice = parseFloat($('#LaborPrice').val()) || 0;
        var discount = parseFloat($('#Discount').val()) || 0;
        var shippingFee = parseFloat($('#ShippingFee').val()) || 0;

        var totalAmount = subTotal + laborPrice - discount + shippingFee;

        $('#SubTotal').val(subTotal.toFixed(2));
        $('#TotalAmount').val(totalAmount.toFixed(2));

        // Update payment status
        var amountPaid = parseFloat($('#AmountPaid').val()) || 0;
        var paymentStatus = amountPaid >= totalAmount ? 'Paid' : 'Not Paid';
        $('#PaymentStatus').val(paymentStatus);
    }

    // Trigger totals update when labor price, discount, shipping fee, or amount paid changes
    $('#LaborPrice, #Discount, #ShippingFee, #AmountPaid').on('input', updateTotals);

    // Save Invoice functionality
    $('#saveInvoiceBtn').click(function () {
        var invoiceData = {
            CarId: $('#CarId').val(),
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
            IsPaid: $('#PaymentStatus').val() === 'Paid',
            InvoiceItems: []
        };

        $('.invoice-item').each(function () {
            var item = {
                ItemName: $(this).find('.item-name').val(),
                Quantity: parseFloat($(this).find('.item-quantity').val()) || 0,
                ItemPrice: parseFloat($(this).find('.item-price').val()) || 0,
                ItemTotal: parseFloat($(this).find('.item-total').val()) || 0
            };
            invoiceData.InvoiceItems.push(item);
        });

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
                    alert(response.message);
                    $('#addInvoiceModal').modal('hide');
                    location.reload();
                } else {
                    alert('Error: ' + response.message);
                }
            },
            error: function (xhr, status, error) {
                console.error('AJAX error:', error);
                alert('An error occurred while saving the invoice.');
            }
        });
    });

    // Mark as Paid functionality
    $('.mark-as-paid').click(function () {
        var invoiceId = $(this).data('invoice-id');
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
            error: function () {
                alert('An error occurred while marking the invoice as paid.');
            }
        });
    });

    // View PDF functionality
    $('.view-pdf').click(function () {
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
    $('.send-invoice-email').click(function () {
        var invoiceId = $(this).data('invoice-id');
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
            error: function () {
                alert('An error occurred while sending the email.');
            }
        });
    });

    // Handle click on Edit Invoice button
    $(document).on('click', '.edit-invoice', function () {
        var invoiceId = $(this).data('invoice-id');
        loadInvoiceDetails(invoiceId);
    });

    // Function to load invoice details
    function loadInvoiceDetails(invoiceId) {
        $.ajax({
            url: getInvoiceDetailsUrl,
            type: 'GET',
            data: { id: invoiceId },
            success: function (response) {
                if (response.success) {
                    populateEditModal(response.data);
                    $('#editInvoiceModal').modal('show');
                } else {
                    alert('Error loading invoice details: ' + response.message);
                }
            },
            error: function () {
                alert('An error occurred while fetching invoice details.');
            }
        });
    }

    // Function to populate the edit modal with invoice details
    function populateEditModal(invoice) {
        $('#EditInvoiceId').val(invoice.invoiceId);
        $('#EditCustomerName').val(invoice.customerName);
        $('#EditCustomerEmail').val(invoice.customerEmail);
        $('#EditCustomerNumber').val(invoice.customerNumber);
        $('#EditCarRego').val(invoice.carRego);
        $('#EditCarModel').val(invoice.carModel);
        $('#EditCarYear').val(invoice.carYear);
        $('#EditDateAdded').val(formatDate(invoice.dateAdded));
        $('#EditDueDate').val(formatDate(invoice.dueDate));
        $('#EditIssueName').val(invoice.issueName);
        $('#EditPaymentTerm').val(invoice.paymentTerm);
        $('#EditNotes').val(invoice.notes);
        $('#EditLaborPrice').val(invoice.laborPrice?.toFixed(2) || '0.00');
        $('#EditDiscount').val(invoice.discount?.toFixed(2) || '0.00');
        $('#EditShippingFee').val(invoice.shippingFee?.toFixed(2) || '0.00');
        $('#EditSubTotal').val(invoice.subTotal?.toFixed(2) || '0.00');
        $('#EditTotalAmount').val(invoice.totalAmount?.toFixed(2) || '0.00');
        $('#EditAmountPaid').val(invoice.amountPaid?.toFixed(2) || '0.00');
        $('#EditPaymentStatus').val(invoice.isPaid ? 'Paid' : 'Not Paid');

        // Populate invoice items
        var itemsContainer = $('#editInvoiceItems');
        itemsContainer.empty();
        invoice.invoiceItems.forEach(function (item) {
            var newItem = $(`
                <div class="invoice-item d-flex justify-content-between align-items-center mb-2">
                    <input type="text" class="form-control me-2 item-name" value="${item.itemName}" readonly>
                    <input type="number" class="form-control me-2 item-quantity" value="${item.quantity}" readonly>
                    <input type="number" class="form-control me-2 item-price" value="${item.itemPrice?.toFixed(2) || '0.00'}" readonly>
                    <input type="number" class="form-control item-total" value="${item.itemTotal?.toFixed(2) || '0.00'}" readonly>
                </div>
            `);
            itemsContainer.append(newItem);
        });
    }

    // Helper function to format date
    function formatDate(dateString) {
        if (!dateString) return '';
        var date = new Date(dateString);
        return date.toISOString().split('T')[0];
    }

    // Handle save changes button click
    $('#saveEditInvoiceBtn').click(function () {
        var invoiceId = $('#EditInvoiceId').val();
        var updatedNotes = $('#EditNotes').val();

        $.ajax({
            url: updateInvoiceNotesUrl,
            type: 'POST',
            data: {
                invoiceId: invoiceId,
                notes: updatedNotes
            },
            headers: {
                'RequestVerificationToken': csrfToken
            },
            success: function (response) {
                if (response.success) {
                    alert('Invoice notes updated successfully.');
                    $('#editInvoiceModal').modal('hide');
                    location.reload();
                } else {
                    alert('Error updating invoice notes: ' + response.message);
                }
            },
            error: function () {
                alert('An error occurred while updating invoice notes.');
            }
        });
    });

    // Clear form when modal is closed
    $('#addInvoiceModal').on('hidden.bs.modal', function () {
        clearInvoiceForm();
    });

    // Function to clear the invoice form
    function clearInvoiceForm() {
        $('#invoiceForm')[0].reset();
        $('#invoiceItems').empty();
        updateTotals();
    }

    // Delete Invoice functionality
    $(document).on('click', '.delete-invoice-btn', function (e) {
        e.preventDefault();
        var invoiceId = $(this).data('invoice-id');
        var invoiceIssue = $(this).data('invoice-issue');
        var totalAmount = $(this).data('invoice-total-amount');
        var status = $(this).data('invoice-status');
        var notes = $(this).data('invoice-notes');

        // Populate the delete modal with invoice details
        $('#deleteInvoiceId').text(invoiceId);
        $('#deleteInvoiceIssue').text(invoiceIssue);
        $('#deleteInvoiceTotalAmount').text(totalAmount);
        $('#deleteInvoiceStatus').text(status ? 'Paid' : 'Not Paid');
        $('#deleteInvoiceNotes').text(notes);
        $('#deleteInvoiceModal').modal('show');
    });

    // Confirm Delete Invoice
    $('#confirmDeleteInvoice').click(function () {
        var invoiceId = $('#deleteInvoiceId').text();
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
                console.error('AJAX error:', xhr.responseText);
                alert('An error occurred while deleting the invoice.');
            }
        });
        $('#deleteInvoiceModal').modal('hide');
    });
});
