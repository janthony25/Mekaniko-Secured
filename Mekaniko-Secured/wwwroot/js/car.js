$(document).ready(function () {
    console.log("Document ready");

    // Add Car functionality
    $('#saveCarBtn').click(function () {
        console.log("Save Car button clicked");

        // Gather car data from the form
        var carData = {
            CarRego: $('#CarRego').val().trim(),
            CarModel: $('#CarModel').val().trim(),
            CarYear: $('#CarYear').val().trim(),
            MakeId: $('#MakeId').val(),
            CustomerId: $('#CustomerId').val()
        };

        console.log("Car Data to be sent:", carData);

        // Send AJAX request to add the car
        $.ajax({
            url: addCarUrl, // This variable should be defined in the view
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(carData),
            headers: {
                'RequestVerificationToken': csrfToken // This variable should be defined in the view
            },
            success: function (response) {
                console.log("AJAX response received:", response);
                if (response.success) {
                    alert(response.message);
                    $('#addCarModal').modal('hide');
                    location.reload();
                } else {
                    console.log("Server returned errors:", response.errors);
                    handleValidationErrors(response.errors);
                }
            },
            error: function (xhr, status, error) {
                console.error("AJAX error:", error);
                console.error("Status:", status);
                console.error("Response:", xhr.responseText);
                alert('Error occurred while adding car: ' + error);
            }
        });
    });

    // Function to handle validation errors
    function handleValidationErrors(errors) {
        $('.text-danger').text(''); // Clear all previous error messages

        if (errors) {
            console.log("Handling server-side validation errors:", errors);
            $.each(errors, function (key, value) {
                $('#' + key + 'Error').text(value);
                console.log("Setting error for", key, ":", value);
            });
        }
    }

    // Delete Car functionality
    $(document).on('click', '.delete-car-btn', function (e) {
        e.preventDefault(); // Prevent default anchor behavior
        console.log("Delete car button clicked");

        // Populate the delete confirmation modal with car details
        var carId = $(this).data('id');
        var carRego = $(this).data('rego');
        var carModel = $(this).data('model');
        var carYear = $(this).data('year');

        $('#deleteCarId').text(carId);
        $('#deleteCarRego').text(carRego);
        $('#deleteCarModel').text(carModel);
        $('#deleteCarYear').text(carYear);
        $('#hiddenDeleteCarId').val(carId);

        $('#deleteCarModal').modal('show');
    });

    // Confirm Delete Car functionality
    $('#confirmDeleteCarBtn').click(function () {
        console.log("Confirm delete button clicked");
        var carId = $('#hiddenDeleteCarId').val();
        console.log("Car ID to delete:", carId);

        // Send AJAX request to delete the car
        $.ajax({
            url: deleteCarUrl, // This variable should be defined in the view
            type: 'POST',
            data: { id: carId },
            headers: {
                'RequestVerificationToken': csrfToken // This variable should be defined in the view
            },
            success: function (response) {
                console.log("AJAX success response:", response);
                if (response.success) {
                    alert(response.message);
                    $('#deleteCarModal').modal('hide');
                    location.reload();
                } else {
                    alert('Error occurred while deleting car: ' + response.message);
                    console.error('Server-side error:', response.message);
                }
            },
            error: function (xhr, status, error) {
                console.error("AJAX error:", error);
                console.error("Status:", status);
                console.error("Response:", xhr.responseText);
                alert('Error occurred while deleting car.');
            }
        });
    });
});