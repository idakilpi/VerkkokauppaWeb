window.setTimeout(function () { //alert message pois tietyn ajan jälkeen
    $(".alert").fadeTo(500, 0).slideUp(500, function () {
        $(this).remove();
    });
}, 10000);