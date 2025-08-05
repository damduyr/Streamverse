function showForm() {
    const div1 = document.querySelector('.profile-container');
    const div2 = document.querySelector("#add-profile-form");
    div1.style.display = "none";
    $("h2").css("display", "none");
    div2.style.display = "block";

}

function showProfiles() {
    const div1 = document.querySelector('.profile-container');
    const div2 = document.querySelector("#add-profile-form");
    div2.style.display = "none";
    $("h2").css("display", "block");
    div1.style.display = "grid";  
}

function saveProfile() {
    var profileName = $("#ProfileName").val();
    var profileAge = $("#Age").val();

    $.ajax({
        type: 'POST',
        url: '/Streamverse/SaveUserProfile',
        data: { profileName, profileAge },
        success: function (response) {
            //console.log("outside if");
            if (response.success == false) {
                alert(response.message);
                //console.log("inside if");
            }
            //console.log("inside false");
            alert('Wohooo! Profile Successfully Added');
           
        },
        error: function (error) {
            alert('Error: ' + error);
        }
    })
}