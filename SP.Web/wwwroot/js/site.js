// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function loadSelectOptions(url, control) {
    $.get(url,
        function (data) {
            debugger;
            control.empty();
            $.each(data, function (id, name) {
                control.append($('<option>',
                    {
                        value: id,
                        text: name
                    }));
            });
        });
}
