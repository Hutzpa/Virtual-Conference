//var $ = require('jquery');
//var socket = io("http://localhost:3000");



var socket = io("http://localhost:3000");


socket.emit("greetingToServ", document.getElementById('idEvent').value + document.getElementById('idRoom').value);

function CreateMes(idEvent, idRoom, userName, message) {
    return { room: idEvent + idRoom, name: userName, msg: message }
}

$('form').submit(e => {
    e.preventDefault();
    console.log(CreateMes(document.getElementById('idEvent').value,
        document.getElementById('idRoom').value, document.getElementById('name').value, document.getElementById('message').value));
    socket.emit('chatMessage', CreateMes(document.getElementById('idEvent').value,
        document.getElementById('idRoom').value, document.getElementById('name').value, document.getElementById('message').value));
  
    $('#all_mess').append(`<div class='alert'> <b> ${$('#name').val()} </b> : ${$('#message').val()} </div>`)
    $('#message').val('');
    return false;
});

socket.on('chat message', Message => {
    $('#all_mess').append(`<div class='alert'> <b>${Message.name} </b> : ${Message.msg} </div>`);
});
