// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
$(function () {
    var x = $('#p');

    $('button[data-toggle="ajax-modal"]').click(function (event) {

        var url = $(this).data('url');
        var decodedUrl = decodeURIComponent(url);

        $.get(decodedUrl).done(function (data) {
            x.html(data);
            x.find('.modal').modal('show');
        })

    })

    x.on('click', '[data-save="modal"]', function (event) {
        event.preventDefault();
        var form = $(this).parents(".modal").find('form');
        var actionUrl = form.attr('action');
        var sendData = form.serialize();
        $.post(actionUrl, sendData).done(function (data) {
            x.find('.modal').modal('hide');
        })
    })
})