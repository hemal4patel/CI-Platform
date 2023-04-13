
//Admin user table
var userTable = $('#userTable').DataTable({
    lengthChange: false,
    ordering: false,
    paging: true,
    searching: true,
    pageLength: 7,
    pagingType: "full_numbers",
    language: {
        paginate: {
            first: '<span class="image-class-first"><i class="bi bi-chevron-double-left"></i></span>',
            previous: '<span class="image-class-previous"><i class="bi bi-chevron-left"></i></span>',
            next: '<span class="image-class-next"><i class="bi bi-chevron-right"></i></span>',
            last: '<span class="image-class-last"><i class="bi bi-chevron-double-right"></i></span>'
        }
    },
    drawCallback: function (settings) {
        var api = this.api();
        var numRows = api.rows({ search: "applied" }).count();
        if (numRows === 0) {
            $(this).closest('.dataTables_wrapper').find('.dataTables_paginate').hide();
        } else {
            $(this).closest('.dataTables_wrapper').find('.dataTables_paginate').show();
        }
    }
});

$('#searchUser').on('keyup', function () {
    userTable.search($(this).val()).draw();
})

//Admin mission table
var missionTable = $('#missionTable').DataTable({
    lengthChange: false,
    ordering: false,
    paging: true,
    searching: true,
    pageLength: 7,
    pagingType: "full_numbers",
    language: {
        paginate: {
            first: '<span class="image-class-first"><i class="bi bi-chevron-double-left"></i></span>',
            previous: '<span class="image-class-previous"><i class="bi bi-chevron-left"></i></span>',
            next: '<span class="image-class-next"><i class="bi bi-chevron-right"></i></span>',
            last: '<span class="image-class-last"><i class="bi bi-chevron-double-right"></i></span>'
        }
    },
    drawCallback: function (settings) {
        var api = this.api();
        var numRows = api.rows({ search: "applied" }).count();
        if (numRows === 0) {
            $(this).closest('.dataTables_wrapper').find('.dataTables_paginate').hide();
        } else {
            $(this).closest('.dataTables_wrapper').find('.dataTables_paginate').show();
        }
    }
});

$('#searchMission').on('keyup', function () {
    missionTable.search($(this).val()).draw();
})


