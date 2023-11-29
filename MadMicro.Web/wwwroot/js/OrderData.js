$(document).ready(function () {
    var Approved = "Approved";
    var ReadyToPickup = "ReadyToPickup";
    var Cancelled = "Cancelled";
    var All = "All";

    var url = window.location.search;

    switch (true) {
        case url.includes(Approved):
            loadDataTable(Approved);
            break;
        case url.includes(ReadyToPickup):
            loadDataTable(ReadyToPickup);
            break;
        case url.includes(Cancelled):
            loadDataTable(Cancelled);
            break;
        default:
            loadDataTable(All);
            break;
    }
});

var dataTable;

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        order: [[0, 'desc']],
        ajax: {
            url: `/order/getAll?status=${status}`
        },
        columns: [
            { data: 'orderHeaderId', width: '10%' },
            { data: 'email', width: '20%' },
            { data: 'name', width: '20%' },
            { data: 'phone', width: '10%' },
            { data: 'status', width: '10%' },
            { data: 'orderTotal', width: '5%' },
            {
                data: 'orderHeaderId',
                render: function (data) {
                    return `<div class="w-75 btn-group" role="group">
                        <a href="/order/orderDetail?orderId=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i></a>
                    </div>`;
                },
                width: '10%'
            }
        ]
    });
}