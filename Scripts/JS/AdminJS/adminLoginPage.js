function signInAdmin() {
    var adminUsername = $("#admin-username").val();
    var adminPassword = $("#password").val();

    $.ajax({
        type: 'POST',
        url: '/AdminManage/VerifyAdmin',
        data: { adminUsername, adminPassword },
        success: function (response) {
            //console.log(response);
            //console.log("inside success");
            //if (response.success) {
            //    //console.log(reponse.success);
            //    alert(response.message);
            //    //var url = '@Url.Action("../Admin/SystemAdminPage")' + '?adminID=' + response.adminID;
            //    //var url = '/Admin/SystemAdminPage?adminID=' + response.adminID;
            //    //console.log(url);
            //    //var url = '@Url.Action("../Home/MyActionResult")' + '?Page=' + data + '&' + PostData;
            //    //window.location.href = url;
            //}
            //else {
            //    console.log("inside success fail");
            //    alert(response.message);
            //}
            console.log("inside success");
            var url = '@Url.Action("../Home/MyActionResult")' + '?Page=' + data + '&' + PostData;
            window.location.href = '/AdminManage/SystemAdminPage?adminID=' + response.adminID;
            console.log("1");
        },
        error: function (error) {
            console.log("inside error");
            alert('Error: Please Try Again!');
        }
    })
}