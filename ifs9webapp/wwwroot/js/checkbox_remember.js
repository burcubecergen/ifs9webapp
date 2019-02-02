function myFunction() {
        var checkBox = document.getElementById("myCheck");
        var text = document.getElementById("text");
        if (checkBox.checked == true) {
            text.style.color = "green";
            text = "Hatırlandı!"
            document.getElementById("text").innerHTML= text
        } else {
            text.style.color = "black" 
            text = "Beni Hatırla"
            document.getElementById("text").innerHTML= text
        }
}