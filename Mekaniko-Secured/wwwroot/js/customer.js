$(document).ready(function () {
    $('#saveCustomerBtn').click(function () {
        var customerName = $('#CustomerName').val();
        if (!customerName || customerName.trim() === '') {
            $('#CustomerNameError').text('Customer Name is required.');
            return;
        } else {
            $('#CustomerNameError').text('');
        }

        var customerData = {
            CustomerName: customerName,
            CustomerNumber: $('#CustomerNumber').val(),
            CustomerEmail: $('#CustomerEmail').val()
        };

        $.ajax({
            url: addCustomerUrl,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(customerData),
            headers: {
                'RequestVerificationToken': csrfToken  // Use the csrfToken defined in the view
            },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    $('#addCustomerModal').modal('hide');
                    location.reload();
                } else {
                    handleValidationErrors(response.errors);
                }
            },
            error: function (xhr, status, error) {
                alert('Error occurred while adding customer.');
                console.error(xhr.responseText);
            }
        });
    });

    function handleValidationErrors(errors) {
        $('#CustomerNameError').text('');
        $('#CustomerNumberError').text('');
        $('#CustomerEmailError').text('');

        if (errors) {
            if (errors['CustomerName']) {
                $('#CustomerNameError').text(errors['CustomerName']);
            }
            if (errors['CustomerNumber']) {
                $('#CustomerNumberError').text(errors['CustomerNumber']);
            }
            if (errors['CustomerEmail']) {
                $('#CustomerEmailError').text(errors['CustomerEmail']);
            }
        }
    }

    $('.delete-customer-btn').click(function () {
        var customerId = $(this).data('id');
        console.log("Customer ID for deletion: " + customerId); // Check this value
        var customerName = $(this).data('name');
        var customerEmail = $(this).data('email');
        var customerNumber = $(this).data('number');

        $('#deleteCustomerId').text(customerId);
        $('#deleteCustomerName').text(customerName);
        $('#deleteCustomerEmail').text(customerEmail);
        $('#deleteCustomerNumber').text(customerNumber);
        $('#hiddenDeleteCustomerId').val(customerId);  // This is crucial

        $('#deleteCustomerModal').modal('show');
    });


    $('#confirmDeleteCustomerBtn').click(function () {
        var customerId = $('#hiddenDeleteCustomerId').val().trim();

        $.ajax({
            url: deleteCustomerUrl,  // Use the URL defined in the view,
            type: 'POST',
            data: { id: customerId },  // Sending as form data
            headers: {
                'RequestVerificationToken': csrfToken  // Use the csrfToken defined in the view
            },
            success: function (response) {
                if (response.success) {
                    alert(response.message);
                    $('#deleteCustomerModal').modal('hide');
                    location.reload();
                } else {
                    alert('Error occurred while deleting customer: ' + response.message);
                    console.error('Server-side error:', response.message);
                }
            },
            error: function (xhr, status, error) {
                alert('Error occurred while deleting customer.');
                console.error('AJAX error:', error);
                console.error('Status:', status);
                console.error('Response:', xhr.responseText);
            }
        });
    });
});