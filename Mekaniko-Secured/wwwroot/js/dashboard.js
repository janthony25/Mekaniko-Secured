$(document).ready(function () {
    var ctx = document.getElementById('paymentChart').getContext('2d');
    var paymentChart = new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: ['Total Invoiced Amount', 'Total Paid Amount', 'Remaining Balance'],
            datasets: [{
                data: [TotalInvoiceAmount, TotalPaidAmount, RemainingBalance],
                backgroundColor: [
                    'rgba(54, 162, 235, 0.6)', // Blue for Total Invoiced Amount
                    'rgba(75, 192, 192, 0.6)', // Green for Total Paid Amount
                    'rgba(255, 99, 132, 0.6)'  // Red for Remaining Balance
                ],
                borderColor: [
                    'rgba(54, 162, 235, 1)',
                    'rgba(75, 192, 192, 1)',
                    'rgba(255, 99, 132, 1)'
                ],
                borderWidth: 1
            }]
        },
        options: {
            responsive: true,
            maintainAspectRatio: false, // Disable aspect ratio to allow custom height/width
            plugins: {
                legend: {
                    display: true,
                    position: 'right',
                    labels: {
                        font: {
                            size: 16, // Adjust the font size as needed
                        },
                        generateLabels: function (chart) {
                            var data = chart.data;
                            return data.labels.map(function (label, i) {
                                var value = data.datasets[0].data[i];
                                return {
                                    text: label + ': $' + value.toLocaleString(),
                                    fillStyle: data.datasets[0].backgroundColor[i],
                                    hidden: isNaN(data.datasets[0].data[i]),
                                    strokeStyle: data.datasets[0].borderColor[i],
                                    datasetIndex: 0
                                };
                            });
                        }
                    }
                },
                tooltip: {
                    callbacks: {
                        label: function (tooltipItem) {
                            return tooltipItem.label + ': $' + tooltipItem.raw.toLocaleString();
                        }
                    }
                }
            }
        }
    });
});
