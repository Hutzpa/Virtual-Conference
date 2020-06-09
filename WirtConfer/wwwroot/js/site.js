// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.



showCreateEvent = (url) => {
    $.ajax({
        type: "GET",
        url: url,
        success: function (res) {
            $('#CreateEventModal .modal-body').html(res);
            $('#CreateEventModal').modal('show');
        }
    })
}


CreateEventSubmit = form => {
    try {
        $.ajax({
            type: "POST",
            url: form.action,
            data: new FormData(form),
            contentType: false,
            processData: false,
            success: function (res) {
                if (res.isValid) {
                    $('#CreateEventModal').modal('hide');
                }
                else {
                    $('#CreateEventModal .modal-body').html(res.html);
                }

            },
            error: function (res) {
                console.log(res);
            }
        })
    } catch (e) {
        console.log(e);
    }


    //to prevent submission
    return false;
}

$(function () {

    var PlaceHolderLogin = $('#PlaceHolderLogin');
    $('button[data-toggle="login-modal"]').click(function (event) {

        var url = $(this).data('url');
        $.get(url).done(function (data) {
            PlaceHolderLogin.html(data);
            PlaceHolderLogin.find('.modal').modal('show');
        })
    });

    PlaceHolderLogin.on('click', '[data-save="modal"]', function (event) {
        var form = $(this).parents('.modal').find('form');
        var actionUrl = form.attr('action');
        var sendData = form.serialize();
        $.post(actionUrl, sendData).done(function (data) {
            location.reload();
            PlaceHolderLogin.find(".modal").modal('hide');
        })
    })

})