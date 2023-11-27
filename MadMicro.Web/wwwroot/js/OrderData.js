$(document).ready(function() {
    loadDataTable();
});


var dataTable;

function loadDataTable() {

    dataTable = $('#tblData').DataTable({
        "ajax": { url: '/order/getAll' },
        "columns": [
            { data: 'orderHeaderId', "width": "10%" },
            { data: 'email', "width": "20%" },
            { data: 'name', "width": "20%" },
            { data: 'phone', "width": "10%" },
            { data: 'status', "width": "10%" },
            { data: 'orderTotal', "width": "5%" },
            {
                data: 'orderHeaderId',
                "render": function (data) {
                    return `<div class="w-75 btn-group" role="group">
                      <a href="/order/orderDetail?orderId=${data}" class="btn btn-primary mx-2"><i class="bi bi-pencil-square"></i></a>
                    </div>`
                },
                "width":"10%"
            }
        ]
    })
}