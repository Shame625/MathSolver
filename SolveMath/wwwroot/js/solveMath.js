"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/solveMathHub").build();

//Disable send button until connection is established
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (type, message) {
    var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    var encodedMsg = msg;
    var li = document.createElement("li");
    li.textContent = encodedMsg;
    switch (type) {
        case "MSG":
            document.getElementById("messagesList").appendChild(li);
            break;
        case "GAME_RESTARTING":
            document.getElementById("sendButton").disabled = true;
            document.getElementById("messagesList").innerHTML = "";
            document.getElementById("messagesList").appendChild(li);
            break;
        case "GAME":
            document.getElementById("messagesList").appendChild(li);
            break;
    }
});

connection.on("Equation", function (currentEquation) {
    document.getElementById("sendButton").disabled = false;
    document.getElementById("messagesList").innerHTML = "";
    $("#x").html(currentEquation.x);
    $("#y").html(currentEquation.y);
    $('#messageInput').val('');
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
    connection.invoke("GetCurrentGame");
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendAnswer", message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});