//$(document).ready(function () {
//    $('#save-btn').click(function () {
//        var dob = $("#dob").val();
//        var formData = $("#UserForm").serialize();
//        $.ajax({
//            type: 'POST',
//            url: '/Streamverse/RegisterPage',
//            data: dob,formData,
//            success: function () {
//                alert('Registered Successfully!');
//            },
//            error: function (error) {
//                alert('Error: '+error);
//            }
//        })
//    })
//})

$(document).ready(function () {
    $('#save-btn').click(function () {
        var formData = $("#UserForm").serialize();
        $.ajax({
            type: 'POST',
            url: '/Streamverse/RegisterPage',
            data: formData,
            success: function () {
                alert('Registered Successfully!');
            },
            error: function (error) {
                alert('Error: ' + error);
            }
        })
    })
})


//function verifyUser() {
//    var user_email = $("#email-box").val();
//    var user_password = $("#password").val();
//    $.ajax({
//        type: 'POST',
//        url: '/Streamverse/LoginPage',
//        data: user_email, user_password,
//        success: function (reponse) {
//            alert('Welcome to Streamverse: ' + response + '!');
//        },
//        error: function (error) {
//            alert('Error signing in: ' + error);
//        }
//    })
//}
$(document).ready(function () {
    $('#login-btn').click(function () {
        var user_email = $("#email-box").val();
        var user_password = $("#password").val();
        $.ajax({
            type: 'POST',
            url: '/Streamverse/LoginPage',
            data: { user_email, user_password },
            success: function (response) {
                alert('Welcome to Streamverse!:'+response.message);
            },
            error: function (error) {
                alert('Error signing in: ' + error);
            }
        })
    })
})
