function validateInput() {
    var file_d = document.getElementById('uploadBox').value;
    var file_type = file_d.split(".").slice(-1)[0];
    var arr_types = ["png", "jpg", "jpeg", "webp", "bmp"];

    if (file_d == "") {
        document.getElementById('source-original-error').innerHTML = '(This field is required)';
        return false;
    }

    if (file_type == "") {
        document.getElementById('source-original-error').innerHTML = '(Incorrect type of image)';
        return false;
    }

    if (!arr_types.includes(file_type)) {
        document.getElementById('source-original-error').innerHTML = '(You can load file with format "png", "jpg", "jpeg", "webp", "bmp")';
        return false;
    }

    document.getElementById('source-original-error').innerHTML = '';
    return true;
}


// display loaded image in right section
function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('.image-display-editor').attr('src', e.target.result);
        }
        reader.readAsDataURL(input.files[0]); // convert to base64 string
    }
}

$("#uploadBox").change(function () {
    readURL(this);
});


function deleteAction(model, ids) {
    swal({
        title: "Are you sure you want to Delete?",
        text: "You will be not able to restore the data.",
        icon: "warning",
        buttons: true,
        dangetModel: true,
    }).then((willDelete) => {
        if (willDelete) {
            $.ajax({
                type: "DELETE",
                url: `/${model}/DeleteImage/${ids}`,
                success: function (data) {
                    if (data.success) {
                        toastr.success(data.message);
                        $(`#result-item_${ids}`).remove();
                    } else {
                        toastr.error(data.message);
                    }
                }
            })
        }
    })
}